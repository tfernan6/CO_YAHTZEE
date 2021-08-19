using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionButtonController : MonoBehaviour
{
    public void openInstructionsOnClick() {
            Application.OpenURL("https://www.ultraboardgames.com/yahtzee/game-rules.php");
    }
}
