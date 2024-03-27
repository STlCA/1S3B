//#define VS_DEBUG_MODE

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Debug = UnityEngine.Debug;

namespace Voltrig.VoltSpriter
{
    public class VSScriptPatcher
    {
        private FileInfo fileInfo;

        private const string className = "class SpriteEditorWindow";
        private const string internalsUsingInjection = "using System.Runtime.CompilerServices;";
        private const string internalsInjection = "[assembly: InternalsVisibleTo(\"VoltSpriter\")]";
        private const string usingDirective = "using";
        private const string openBracket = "{";

        private const string fieldInjection = "public bool m_shouldDrawSpriteBoxes = true;";
        
        private const string conditionalLineCode = "var selectedRect = selectedSpriteRect != null ? selectedSpriteRect.spriteID : new GUID();";
        private const string conditionalInjection = "if(m_shouldDrawSpriteBoxes)";

        public bool IsPatched { get; private set; }

        public VSScriptPatcher(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
        }

        public bool CheckOrApplyPatch()
        {
            if (!fileInfo.Exists)
            {
                Debug.LogError($"File {fileInfo.FullName} doesn't exist.");
                return false;
            }

            List<string> scriptFile;
            bool requireFileSave = false;

            using (StreamReader sr = new StreamReader(fileInfo.FullName))
            {
                char[] newlines = new char[] { '\n' };
                scriptFile = new List<string>(sr.ReadToEnd().Split(newlines));

                int line = 0;

                if (!InjectField(scriptFile, ref requireFileSave, ref line))
                {
                    Debug.LogError("Failed to inject the field.");
                    return false;
                }

                if (!InjectConditional(scriptFile, ref requireFileSave, ref line))
                {
                    VSConsole.LogError(this, "Failed to inject the conditional.");
                    return false;
                }

                //VSConsole.Log(this, scriptFile);
            }

            if (requireFileSave)
            {
                //Patch is required.
                using (StreamWriter sw = new StreamWriter(fileInfo.FullName, false))
                {
                    for(int i = 0; i < scriptFile.Count; i++)
                    {
                        sw.WriteLine(scriptFile[i]);
                    }

                    VSConsole.Log(this, $"Patched file {fileInfo.FullName}");
                }
            }
            else
            {
                VSConsole.Log(this, $"No patch required for {fileInfo.FullName}.");
            }

            return true;
        }

        private bool InjectConditional(List<string> scriptFile, ref bool requireFileSave, ref int line)
        {
            for (; line < scriptFile.Count; line++)
            {
                if (!scriptFile[line].Contains(conditionalLineCode))
                {
                    continue;
                }

                //We've found the conditional line code.

                int index = line + 1;

                if (index >= scriptFile.Count)
                {
                    break;
                }

                if (scriptFile[index].Contains(conditionalInjection))
                {
                    VSConsole.Log(this, "Conditional injection found!");
                    return true;
                }
                else
                {
                    VSConsole.Log(this, "Conditional injection not found. Inserting");
                    scriptFile.Insert(index, conditionalInjection);
                    requireFileSave = true;
                    return true;
                }
            }

            return false;
        }

        private bool InjectField(List<string> scriptFile, ref bool requireFileSave, ref int line)
        {
            bool stepSuccessful;
            int classNameLine = 0;

            //Find SpriteEditorWindow class.
            stepSuccessful = false;
            for (; line < scriptFile.Count; line++)
            {
                if (scriptFile[line].Contains(className))
                {
                    classNameLine = line;
                    line++;
                    stepSuccessful = true;
                    break;
                }
            }

            if (!stepSuccessful)
            {
                //Failed to find SpriteEditorWindow class.
                return false;
            }

            //Check old spot for injection
            int index = classNameLine + 39; // 39 is the offset where the bool will be put at.

            if (index < scriptFile.Count
                && scriptFile[index].Contains(fieldInjection))
            {
                VSConsole.Log(this, "Field injection found!");
                return true;
            }

            //Find first open bracket.
            stepSuccessful = false;
            for (; line < scriptFile.Count; line++)
            {
                if (scriptFile[line].Contains(openBracket))
                {
                    line++;
                    stepSuccessful = true;
                    break;
                }
            }

            if (!stepSuccessful)
            {
                //Failed to find open bracket.
                return false;
            }

            //Find or apply injection. Max 100 steps.
            int maxSteps = 100;
            for (int step = 0; step < maxSteps; step++)
            {
                if (scriptFile[line].Contains(fieldInjection))
                {
                    VSConsole.Log(this, "Field injection found!");
                    return true;
                }

                if (string.IsNullOrEmpty(scriptFile[line]))
                {
                    scriptFile.Insert(line, fieldInjection);
                    VSConsole.Log(this, "Field injection not found. Inserting");
                    requireFileSave = true;
                    return true;
                }

                line++;

                if(line >= scriptFile.Count)
                {
                    return false;
                }
            }

            return false;
        }
    }
}