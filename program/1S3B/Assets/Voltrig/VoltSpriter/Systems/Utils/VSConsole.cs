#define VS_DEBUG_MODE

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;

namespace Voltrig.VoltSpriter
{
    /// <summary>
    /// An adapter for printing the debug info to console.
    /// </summary>
    internal static class VSConsole
    {
        public static void LogError<T>(T obj, string message)
        {
            Debug.LogError($"[{typeof(T).Name}] {message}");
        }

        [Conditional("VS_DEBUG_MODE")]
        public static void LogWarning<T>(T obj, string message)
        {
            Debug.LogWarning($"[{typeof(T).Name}] {message}");
        }

        [Conditional("VS_DEBUG_MODE")]
        public static void Log<T>(T obj, string message)
        {
            Debug.Log($"[{typeof(T).Name}] {message}");
        }

        [Conditional("VS_DEBUG_MODE")]
        public static void Log<T>(T obj, List<string> stringList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[{typeof(T).Name}]:");

            for (int i = 0; i < stringList.Count; i++)
            {
                sb.AppendLine($"{i}-{stringList[i]}");
            }

            Debug.Log(sb.ToString());
        }
    }
}