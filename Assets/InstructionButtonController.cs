using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionButtonController : MonoBehaviour
{
    //if you want it private do:
    Texture2D cursor;
    public void openInstructionsOnClick() {
            Application.OpenURL("https://www.ultraboardgames.com/yahtzee/game-rules.php");
    }

    public void OnMouseEnter()
    {
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
    }

    public void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
