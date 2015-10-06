using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour {

    public void startGame()
    {
        Application.LoadLevel("_Level Select Scene");
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
