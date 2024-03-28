using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CursorSetting : MonoBehaviour
{
    public Texture2D plusCursor;

    private void OnMouseEnter()
    {
        if(tag=="Harvest")
            Cursor.SetCursor(plusCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
    }
}
