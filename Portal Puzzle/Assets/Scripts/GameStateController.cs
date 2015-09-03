using UnityEngine;
using System.Collections;

public class GameStateController : MonoBehaviour {

    //public static GameStateController Instance;
    public static gameState currentState;

    public enum gameState
    {
        idle = 0,
        movingUnit,
        destroyingUnit,
        regeneratingUnit,
        _statesCount
    }

    //public delegate void gameStateHandler(GameStateController.gameState newState);
    //public static event gameStateHandler onStateChange;

    public void Awake()
    {
        //Instance = this;
        currentState = gameState.idle;
    }
}
