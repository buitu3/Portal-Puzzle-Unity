using UnityEngine;
using System.Collections;
using MadLevelManager;

public class LevelSelectController : MonoBehaviour {

    public void backToMainMenu()
    {
        MadLevel.LoadLevelByName("Main Menu");
    }
}
