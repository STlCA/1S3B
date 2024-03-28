#if COM_UNITY_2D_SPRITE
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;
using System.Runtime.CompilerServices;

namespace Voltrig.VoltSpriter
{
    public partial class VSData : ScriptableObject
    {
        public enum WindowShowMode 
        {
            All,
            Selected
        }

        private List<VSSpriteRect> selectionsList = new List<VSSpriteRect>();
        private List<VSSpriteRect> highlightsList = new List<VSSpriteRect>();
        private List<VSSpriteRect> filteredList = new List<VSSpriteRect>();
        private List<VSSpriteRect> sprites = new List<VSSpriteRect>();
        private List<VSSpriteRect> copiedSprites = new List<VSSpriteRect>();
        private Vector2 copiedSpritesCenterPos = Vector2.zero;

        private WindowShowMode _showMode = WindowShowMode.All;
        private string lastFilter = string.Empty;
        public bool windowMultiedit = true;
        public bool scrollToSelection = false;

        public ISpriteEditor spriteEditor;

        public SpriteRect [] spriteRects 
        {
            get 
            {
                SpriteRect [] rects = new SpriteRect[sprites.Count];

                for(int i = 0; i < sprites.Count; i++) 
                {
                    rects[i] = sprites[i].sr;
                }

                return rects;
            }
        }

        public int spriteAmount => sprites.Count;

        public int selectionAmount => selectionsList.Count;

        public int highlightAmount => highlightsList.Count;

        public int filteredAmount => filteredList.Count;

        public int selectAmount => selectionsList.Count;

        [field:SerializeField]
        public bool isFiltered { get; private set; }

        public WindowShowMode showMode 
        {
            get => _showMode;
            set 
            {
                if (_showMode == value)
                {
                    return;
                }

                _showMode = value;
                OnConfigChangeE?.Invoke();
            }
        }

        public event System.Action OnSelectionChangeE;
        public event System.Action OnHighlightChangeE;
        public event System.Action OnFilteredChangeE;
        public event System.Action OnConfigChangeE;
        public event System.Action OnAutoindiceE;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IVSSpriteRect GetSprite(int id) 
        {
            return sprites[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IVSSpriteRect GetSelection(int id) 
        {
             return selectionsList[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IVSSpriteRect GetHighlight(int id) 
        {
            return highlightsList[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IVSSpriteRect GetFiltered(int id) 
        {
            return filteredList[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Select(IVSSpriteRect sprite, bool value) 
        {
            if (sprites[sprite.ID].isSelected == value)
            {
                return;
            }

            sprites[sprite.ID].isSelected = value;
            UpdateSelectionList();
        }

        public void Highlight(IVSSpriteRect sprite, bool value)
        {
            if (sprites[sprite.ID].isHighlighted == value)
            {
                return;
            }

            sprites[sprite.ID].isHighlighted = value;
            UpdateHighlightList();
        }

        public void Filter(string value) 
        {
            lastFilter = value.ToLower();
            if(string.IsNullOrEmpty(value)) 
            {
                isFiltered = false;
            } 
            else 
            {
                isFiltered = true;
            }

            UpdateFilterList();
        }

        public void ReplaceFilteredTo(string value) 
        {
            for(int i = 0; i < filteredList.Count; i++) 
            {
                VSSpriteRect spr = filteredList[i];
               
                spr.sr.name = spr.sr.name.Replace(lastFilter, value, System.StringComparison.CurrentCultureIgnoreCase);
            }

            lastFilter = value.ToLower();

            UpdateFilterList();
        }

        public void SetSprites(SpriteRect [] spriteRects) 
        {
            sprites.Clear();
            ClearAll();

            VSSpriteRect spr;
            for(int i = 0; i < spriteRects.Length; i++) 
            {
                spr = new VSSpriteRect(spriteRects[i]);
                sprites.Add(spr);
                spr.id = i;
            }
        }

        public void DeleteSelection() 
        {
            if (selectAmount == 0)
            {
                return;
            }

            List<VSSpriteRect> _newSprites = new List<VSSpriteRect>();
            for(int i = 0; i < sprites.Count; i++) 
            {
                if (!sprites[i].isSelected)
                {
                    _newSprites.Add(sprites[i]);
                }
            }

            sprites = _newSprites;

            UpdateAll();
        }

        public void DeleteAllSprites()
        {
            sprites.Clear();
            UpdateAll();
        }

        public void DeleteSprites(IEnumerable<IVSSpriteRect> spritesForDeletion)
        {
            HashSet<IVSSpriteRect> spritesHashSet = new HashSet<IVSSpriteRect>(spritesForDeletion);

            if(spritesHashSet.Count == 0)
            {
                return;
            }

            List<VSSpriteRect> _newSprites = new List<VSSpriteRect>();
            for (int i = 0; i < sprites.Count; i++)
            {
                if (!spritesHashSet.Contains(sprites[i]))
                {
                    _newSprites.Add(sprites[i]);
                }
            }

            sprites = _newSprites;

            UpdateAll();
        }

        public IVSSpriteRect GetExistingOverlappingSprite(Rect rect)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i].sr.rect.Overlaps(rect))
                {
                    return sprites[i];
                }
            }
            return null;
        }

        public IVSSpriteRect CreateSprite(Rect rect) 
        {
            SpriteRect sr = new SpriteRect();
            sr.rect = rect;
            sr.name = $"sprite{sprites.Count}";

            VSSpriteRect spr = new VSSpriteRect(sr);
            spr.id = sprites.Count;
            sprites.Add(spr);

            return spr;
        }

        public int CreateSprite(Rect rect, SpriteAlignment alignment, Vector2 pivot, string name, Vector4 border)
        {
            SpriteRect spriteRect = new SpriteRect();
            spriteRect.rect = rect;
            spriteRect.alignment = alignment;
            spriteRect.pivot = pivot;
            spriteRect.name = name;
            spriteRect.border = border;

            VSSpriteRect vsSpriteRect = new VSSpriteRect(spriteRect);
            vsSpriteRect.id = sprites.Count;
            sprites.Add(vsSpriteRect);

            return sprites.Count - 1;
        }

        private bool HasName(string name)
        {
            for(int i = 0; i < sprites.Count; i++)
            {
                if(sprites[i].Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        private void ClearAll() 
        {
            selectionsList.Clear();
            highlightsList.Clear();
            filteredList.Clear();
        }

        public void ClearSelection() 
        {
            if(selectionsList.Count == 0)
            {
                return;
            }

            for(int i = 0; i < selectionsList.Count; i++) 
            {
                selectionsList[i].isSelected = false;
            }

            UpdateSelectionList();
        }

        public void ClearHighlights() 
        {
            if(highlightsList.Count == 0)
            {
                return;
            }

            for(int i = 0; i < highlightsList.Count; i++) 
            {
                highlightsList[i].isHighlighted = false;
            }

            UpdateHighlightList();
        }

        public void ClearFilter() 
        {
            for(int i = 0; i < filteredList.Count; i++) 
            {
                filteredList[i].isFiltered = false;
            }
            filteredList.Clear();

            isFiltered = false;
        }

        public void CopySelection() 
        {
            copiedSprites.Clear();
            if (selectionsList.Count == 0)
            {
                return;
            }

            VSSpriteRect sprBuf;

            copiedSpritesCenterPos = Vector2.zero;
            for(int i = 0; i < selectionsList.Count; i++) 
            {
                sprBuf = new VSSpriteRect(selectionsList[i]);
                copiedSprites.Add(sprBuf);
                copiedSpritesCenterPos += sprBuf.sr.rect.position;
            }

            copiedSpritesCenterPos = copiedSpritesCenterPos / selectionsList.Count;
        }

        public void PasteSelection(Vector2 pastePos) 
        {
            if (copiedSprites.Count == 0)
            {
                return;
            }

            VSSpriteRect sprBuf;
            Rect rectBuf;
            Vector2 offset = pastePos - copiedSpritesCenterPos;
            offset.x = Mathf.Round(offset.x);
            offset.y = Mathf.Round(offset.y);

            ClearSelection();

            for(int i = 0; i < copiedSprites.Count; i++) 
            {
                sprBuf = new VSSpriteRect(copiedSprites[i]);
                sprBuf.sr.name = sprBuf.sr.name + "_Copy";
                sprBuf.id = sprites.Count;
                rectBuf = sprBuf.sr.rect;
                rectBuf.position += offset;
                sprBuf.sr.rect = rectBuf;

                sprites.Add(sprBuf);
                Select(sprBuf, true);
            }
        }

        public void PasteSelection() 
        {
            if (copiedSprites.Count == 0)
            {
                return;
            }

            VSSpriteRect sprBuf;
            Rect rectBuf;

            ClearSelection();
            for(int i = 0; i < copiedSprites.Count; i++) 
            {
                sprBuf = new VSSpriteRect(copiedSprites[i]);
                sprBuf.sr.name = sprBuf.sr.name + "_Copy";
                sprBuf.id = sprites.Count;
                rectBuf = sprBuf.sr.rect;
                rectBuf.position += Vector2.one * 4.0f;
                sprBuf.sr.rect = rectBuf;

                sprites.Add(sprBuf);
                Select(sprBuf, true);
            }
        }

        public void FlipSelection(bool vertical) 
        {
            if (selectionsList.Count <= 1)
            {
                return;
            }

            Vector2 minPos = selectionsList[0].sr.rect.min;
            Vector2 maxPos = selectionsList[0].sr.rect.max;
            Vector2 pos;

            for(int i = 1; i < selectionsList.Count; i++) 
            {
                pos = selectionsList[i].sr.rect.min;
                if(pos.x < minPos.x)
                {
                    minPos.x = pos.x;
                }

                if(pos.y < minPos.y)
                {
                    minPos.y = pos.y;
                }

                pos = selectionsList[i].sr.rect.max;

                if(maxPos.x < pos.x)
                {
                    maxPos.x = pos.x;
                }

                if(maxPos.y < pos.y)
                {
                    maxPos.y = pos.y;
                }
            }

            Vector2 flipPos = minPos + (maxPos - minPos) * 0.5f;

            flipPos.x = Mathf.Round(flipPos.x);
            flipPos.y = Mathf.Round(flipPos.y);

            Vector2 offset;
            Rect rBuf;
            VSSpriteRect srBuf;

            for(int i = 0; i < selectionsList.Count; i++) 
            {
                srBuf = selectionsList[i];
                rBuf = srBuf.sr.rect;
                offset = flipPos - rBuf.center;
                if (vertical)
                {
                    rBuf.y += offset.y * 2.0f;
                }
                else
                {
                    rBuf.x += offset.x * 2.0f;
                }
                srBuf.sr.rect = rBuf;
            }
        }

        public void RotateSelection() 
        {
            if (selectionsList.Count < 1)
            {
                return;
            }

            Vector2 pos;
            Vector2 minPos = selectionsList[0].sr.rect.min;
            Vector2 maxPos = selectionsList[0].sr.rect.max;

            for (int i = 1; i < selectionsList.Count; i++)
            {
                pos = selectionsList[i].sr.rect.min;
                if (pos.x < minPos.x)
                {
                    minPos.x = pos.x;
                }

                if (pos.y < minPos.y)
                {
                    minPos.y = pos.y;
                }

                pos = selectionsList[i].sr.rect.max;

                if (maxPos.x < pos.x)
                {
                    maxPos.x = pos.x;
                }

                if (maxPos.y < pos.y)
                {
                    maxPos.y = pos.y;
                }
            }

            Vector2 flipPos = minPos + (maxPos - minPos) * 0.5f;
            flipPos.x = Mathf.Round(flipPos.x);
            flipPos.y = Mathf.Round(flipPos.y);

            Vector2 offset;
            Rect rBuf;
            VSSpriteRect srBuf;
            float fBuf;

            for(int i = 0; i < selectionsList.Count; i++) 
            {
                srBuf = selectionsList[i];
                rBuf = srBuf.sr.rect;
                offset = rBuf.center - flipPos;

                float width = rBuf.width;
                rBuf.width = rBuf.height;
                rBuf.height = width;

                fBuf = offset.x;
                offset.x = offset.y;
                offset.y = -fBuf;
                rBuf.center = flipPos + offset;

                srBuf.sr.rect = rBuf;
            }
        }

        public void MoveSelection(Vector2 amount)
        {
            if (selectionsList.Count == 0)
            {
                return;
            }

            Rect rectBuf;

            for (int i = 0; i < selectionsList.Count; i++)
            {
                rectBuf = selectionsList[i].sr.rect;
                rectBuf.x += amount.x;
                rectBuf.y += amount.y;
                selectionsList[i].sr.rect = rectBuf;
            }
        }

        public void MoveSelection(Vector2Int amount) 
        {
            if (selectionsList.Count == 0)
            {
                return;
            }

            if (amount.x == 0 && amount.y == 0)
            {
                return;
            }

            Rect rectBuf;

            for(int i = 0; i < selectionsList.Count; i++) 
            {
                rectBuf = selectionsList[i].sr.rect;
                rectBuf.x += amount.x;
                rectBuf.y += amount.y;
                selectionsList[i].sr.rect = rectBuf;
            }
        }

        public void RoundSelectionRects()
        {
            if (selectionsList.Count == 0)
            {
                return;
            }

            Rect rectBuf;

            for (int i = 0; i < selectionsList.Count; i++)
            {
                rectBuf = selectionsList[i].sr.rect;
                rectBuf.x = Mathf.Round(rectBuf.x);
                rectBuf.y = Mathf.Round(rectBuf.y);
                selectionsList[i].sr.rect = rectBuf;
            }
        }

        public void AddIndicesToSelectionByXThenY()
        {
            AddIndicesToSelection((sr1, sr2) =>
            {
                Rect r1 = sr1.sr.rect;
                Rect r2 = sr2.sr.rect;
                return (int)((r2.y - r1.y) * 10000 + r1.x - r2.x);
            });
        }

        public void AddIndicesToSelectionByYThenX()
        {
            AddIndicesToSelection((sr1, sr2) =>
            {
                Rect r1 = sr1.sr.rect;
                Rect r2 = sr2.sr.rect;
                return (int)((r1.x - r2.x) * 10000 + r2.y - r1.y);
            });
        }

        /// <summary>
        /// Returns TRUE if there is any sprite with the same name.
        /// </summary>
        /// <returns></returns>
        public bool CheckDuplicatedNames(bool debugDisplay)
        {
            HashSet<string> namesHash = new HashSet<string>();
            List<VSSpriteRect> invalidSprites = new List<VSSpriteRect>();

            for (int i = 0; i < sprites.Count; i++)
            {
                MarkIfNameColliding(sprites[i]);
            }

            void MarkIfNameColliding(VSSpriteRect vssr)
            {
                if (!namesHash.Add(vssr.sr.name))
                {
                    invalidSprites.Add(vssr);
                }
            }

            if (invalidSprites.Count > 0)
            {
                for (int i = 0; i < invalidSprites.Count; i++)
                {
                    invalidSprites[i].duplicateName = true;
                }
                if (debugDisplay)
                {
                    Debug.LogWarning($"[VoltSpriter] Sprite names must be unique, but there were {invalidSprites.Count} names colliding.");
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void FixDuplicatedNames()
        {
            int maxAttempts = 100;

            HashSet<string> namesHash = new HashSet<string>();
            List<SpriteRect> invalidSpritesL1 = new List<SpriteRect>();
            List<SpriteRect> invalidSpritesL2 = new List<SpriteRect>();
            List<SpriteRect> invalidSpritesBuf;

            Dictionary<SpriteRect, string> origNamesD = new Dictionary<SpriteRect, string>();
            SpriteRect sr;

            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i].duplicateName)
                {
                    sr = sprites[i].sr;
                    invalidSpritesL1.Add(sr);
                    origNamesD[sr] = sr.name;
                }
                namesHash.Add(sprites[i].sr.name);
            }

            int id = 0;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                for (int i = 0; i < invalidSpritesL1.Count; i++)
                {
                    sr = invalidSpritesL1[i];

                    sr.name = origNamesD[sr] + "_DUPE" + i;
                    if (namesHash.Add(sr.name))
                    {
                        invalidSpritesL2.Add(sr);
                    }
                    id++;
                }
                invalidSpritesL1.Clear();
                invalidSpritesBuf = invalidSpritesL2;
                invalidSpritesL2 = invalidSpritesL1;

                if (invalidSpritesL1.Count == 0)
                {
                    break;
                }
            }
        }

        private void UpdateAll()
        {
            UpdateSpriteIndices();
            UpdateSelectionList();
            UpdateHighlightList();
            UpdateFilterList();
        }

        private void UpdateSpriteIndices() 
        {
            for(int i = 0; i < sprites.Count; i++) 
            {
                sprites[i].id = i;
            }
        }

        private void UpdateSelectionList() 
        {
            selectionsList.Clear();
            for(int i = 0; i < sprites.Count; i++) 
            {
                if (sprites[i].isSelected)
                {
                    selectionsList.Add(sprites[i]);
                }
            }

            OnSelectionChangeE?.Invoke();
        }

        private void UpdateHighlightList() 
        {
            highlightsList.Clear();
            for(int i = 0; i < sprites.Count; i++) 
            {
                if (sprites[i].isHighlighted)
                {
                    highlightsList.Add(sprites[i]);
                }
            }

            OnHighlightChangeE?.Invoke();
        }

        private void UpdateFilterList() 
        {
            if(!isFiltered) 
            {
                OnFilteredChangeE?.Invoke();
                return;
            }

            VSSpriteRect spr;

            if (showMode == WindowShowMode.Selected)
            {
                for (int i = 0; i < selectionsList.Count; i++)
                {
                    spr = selectionsList[i];

                    if (spr.isFiltered == true)
                    {
                        continue;
                    }

                    if (spr.sr.name.ToLower().Contains(lastFilter))
                    {
                        spr.isFiltered = true;
                        filteredList.Add(spr);
                    }
                }
            }
            else
            {
                for (int i = 0; i < sprites.Count; i++)
                {
                    spr = sprites[i];

                    if (spr.isFiltered == true)
                    {
                        continue;
                    }

                    if (spr.sr.name.ToLower().Contains(lastFilter))
                    {
                        spr.isFiltered = true;
                        filteredList.Add(spr);
                    }
                }
            }

            OnFilteredChangeE?.Invoke();
        }

        private void AddIndicesToSelection(System.Comparison<VSSpriteRect> compMethod)
        {
            List<VSSpriteRect> list = new List<VSSpriteRect>(selectionsList);
            list.Sort(compMethod);

            for (int i = 0; i < list.Count; i++)
            {
                list[i].sr.name = list[i].sr.name + $"_{i}";
            }

            OnAutoindiceE?.Invoke();
        }
    }
}
#endif

