using UnityEngine;
using MadLevelManager;
using System.Collections;

public class MainMenuController : MonoBehaviour {

    public void startGame()
    {
        //Application.LoadLevel("_Level Select Scene");
        MadLevel.LoadLevelByName("Level Select");
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
