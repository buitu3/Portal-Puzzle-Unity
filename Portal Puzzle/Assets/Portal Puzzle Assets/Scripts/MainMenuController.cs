using UnityEngine;
using MadLevelManager;
using System.Collections;

public class MainMenuController : MonoBehaviour {

    public GameObject title;

    void Start()
    {
        Application.targetFrameRate = 60;

        Vector3 desPos = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.8f, 0.0f));
        //print(desPos);

        //print(title.GetComponent<RectTransform>().rect.position);
        //title.GetComponent<RectTransform>().position = desPos;
        iTween.ValueTo(gameObject, iTween.Hash("from", title.GetComponent<RectTransform>().position, "to", desPos, "delay", 0.5f, "time", 0.8f, "easetype", iTween.EaseType.easeOutBack, "onUpdate", "moveGameTitle"));
    }

    public void startGame()
    {
        //Application.LoadLevel("_Level Select Scene");
        MadLevel.LoadLevelByName("Level Select");
    }

    public void exitGame()
    {
        Application.Quit();
    }

    private void moveGameTitle(Vector3 newPos)
    {
        title.GetComponent<RectTransform>().position = newPos;
    }
}
