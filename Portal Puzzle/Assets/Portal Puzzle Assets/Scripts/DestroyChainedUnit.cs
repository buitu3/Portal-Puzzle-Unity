using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ScanUnit
{
    public int _value;
    public bool _isChained;

    public ScanUnit(int value)
    {
        _value = value;
    }
}

public class DestroyChainedUnit : MonoBehaviour {

    //==============================================
    // Constants
    //==============================================

    //==============================================
    // Fields
    //==============================================

    public static DestroyChainedUnit Instance;

    public ScanUnit[,] scanUnitARR;

    public Text _turnPointText;
    public Text _unitCounterText;
    public Text _unitTypeCounterText;
    public Text _scoreText;
    public Text _turnText;
    public Slider _scoreSlider;

    [HideInInspector]
    public int _1starPoint;
    [HideInInspector]
    public int _2starPoint;
    [HideInInspector]
    public int _3starPoint;
    public Image _1starImage;
    public Image _2starImage;
    public Image _3starImage;

    private int _unitCounter;
    private int _unitTypeCounter;
    private int _turnPoint;
    [HideInInspector]
    public  int _score;
    private List<int> _unitTypeCheckContainer;              // A list to check for the number of chained unit types 

    private GeneratingPuzzle puzzleGen;
    private GameStateController gameStateController;
    //private InputHandler inputHandler;

    private bool _noChainedUnit;

    //==============================================
    // Unity Methods
    //==============================================

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        puzzleGen = GeneratingPuzzle.Instance;
        gameStateController = GameStateController.Instance;
        //inputHandler = InputHandler.Instance;
        scanUnitARR = new ScanUnit[puzzleGen._columns, puzzleGen._rows];
        _unitCounter = 0;
        _unitTypeCheckContainer = new List<int>();
        _score = 0;
        _turnText.text = puzzleGen._turns.ToString();
        _1starPoint = puzzleGen._1starPoint;
        _2starPoint = puzzleGen._2starPoint;
        _3starPoint = puzzleGen._3starPoint;

        _scoreSlider.minValue = 0;
        _scoreSlider.maxValue = _1starPoint;
        _scoreSlider.value = 0;

    }

    //==============================================
    // Methods
    //==============================================


    public void onBtnFinishClicked()
    {
        if (GameStateController.currentState == GameStateController.gameState.idle)
        {
            if (puzzleGen._turns > 1)
            {
                puzzleGen._turns -= 1;
                _turnText.text = puzzleGen._turns.ToString();
            }
            else
            {
                puzzleGen._turns -= 1;
                _turnText.text = ("Game Over");
            }

            //StartCoroutine(destroyChainedUnits());
            _unitCounter = 0;
            _turnPoint = 0;
            _unitTypeCheckContainer = new List<int>();
            for (int i = 0; i < puzzleGen._unitPrefabsContainer.Length; i++)
            {
                _unitTypeCheckContainer.Add(i);
            }
            markChainedUnits();
        }
    }

    // Mark chained Units then call destroy Units method
    public void markChainedUnits()
    {
        GameStateController.currentState = GameStateController.gameState.destroyingUnit;

        // Create a puzzle value matrix to scan for chained units
        _noChainedUnit = true;
        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {
                scanUnitARR[XIndex, YIndex] = new ScanUnit(puzzleGen._unitARR[XIndex, YIndex].GetComponent<UnitInfo>()._value);
            }
        }

        #region Scan and Mark chained Units

        scanBlockNineChained();
        scanCrossNineChained();
        scanTSevenChained();
        scanUSevenChained();
        scanLineFiveChained();
        scanLFiveChained();
        scanCrossFiveChained();
        scanLineFourChained();
        scanLineThreeChained();

        #endregion

        // Call Destroy Chained Unit method if there are chained Units
        if (!_noChainedUnit)
        {
            StartCoroutine(destroyChainedUnits());
            // Update game info texts
            _turnPointText.text = (_turnPoint.ToString());
            _unitCounterText.text = (_unitCounter.ToString());
            _unitTypeCounter = puzzleGen._unitPrefabsContainer.Length - _unitTypeCheckContainer.Count;           
            _unitTypeCounterText.text = (_unitTypeCounter.ToString());         
        }
        else
        {
            // Update total score if there are no more chained units
            //StartCoroutine(updateScore());
            //iTween.Stop(gameObject);
            iTween.ValueTo(gameObject, iTween.Hash("from", _score, "to", _score + (_turnPoint * _unitTypeCounter), "time", 0.6, "onUpdate", "updateScore", "ignoretimescale", true));
            _score += (_turnPoint * _unitTypeCounter);

            GameStateController.currentState = GameStateController.gameState.idle;
            // End game if out of turns
            if (puzzleGen._turns < 1)
            {
                StartCoroutine(gameStateController.endGame());
            }
        }
    }

    // Destroy Unit that is marked as chained
    IEnumerator destroyChainedUnits()
    {
        // Add chained Units into shrinking List for destroy animation
        List<GameObject> shrinkUnitsList = new List<GameObject>();
        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {
                if (scanUnitARR[XIndex, YIndex]._isChained)
                {
                    shrinkUnitsList.Add(puzzleGen._unitARR[XIndex, YIndex]);
                }
            }
        }

        //yield return (StartCoroutine(playDestroyUnitsAnimation(shrinkUnitsList)));
        StartCoroutine(playDestroyUnitsAnimation(shrinkUnitsList));

        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {
                if (scanUnitARR[XIndex, YIndex]._isChained)
                {
                    Destroy(puzzleGen._unitARR[XIndex, YIndex], 3.0f);
                }
            }
        }
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        puzzleGen.organizePuzzleAfterDestroy();
        //if (inputHandler._unitHighLight.activeInHierarchy)
        //{
        //    inputHandler._unitHighLight.SetActive(false);
        //}
    }

    // Shrink chained Units Image before destroy
    IEnumerator playDestroyUnitsAnimation(List<GameObject> shrinkUnitsContainer)
    {
        List<Vector3> shrinkUnitsContainerDesPos = new List<Vector3>();
        for (int i = 0; i < shrinkUnitsContainer.Count; i++)
        {
            shrinkUnitsContainerDesPos.Add(new Vector3 (shrinkUnitsContainer[i].transform.position.x + puzzleGen._unitWidth/2,
                                                            shrinkUnitsContainer[i].transform.position.y + puzzleGen._unitHeight/2,
                                                            shrinkUnitsContainer[i].transform.position.z));

        }
        while (shrinkUnitsContainer[0].transform.localScale != Vector3.zero)
        {
            for (int i = 0; i < shrinkUnitsContainer.Count; i++)
            {
                //shrinkUnitsContainer[i].transform.localScale = Vector3.MoveTowards(shrinkUnitsContainer[i].transform.localScale, 
                //                                                                   Vector3.zero, 
                //                                                                   0.15f);
                //shrinkUnitsContainer[i].transform.position = Vector3.MoveTowards(shrinkUnitsContainer[i].transform.position,
                //                                                                shrinkUnitsContainerDesPos[i],
                //                                                                0.05f);

                // Shrink chained Unit into zero size
                shrinkUnitsContainer[i].transform.localScale = Vector3.MoveTowards(shrinkUnitsContainer[i].transform.localScale,
                                                                                   Vector3.zero,
                                                                                   0.085f);
                shrinkUnitsContainer[i].transform.position = Vector3.MoveTowards(shrinkUnitsContainer[i].transform.position,
                                                                                shrinkUnitsContainerDesPos[i],
                                                                                0.039f);
                // Make chained Unit fall down
                Vector3 dropDesPos = new Vector3(shrinkUnitsContainer[i].transform.position.x,
                                                    shrinkUnitsContainer[i].transform.position.y - puzzleGen._unitHeight / 2,
                                                    shrinkUnitsContainer[i].transform.position.z);
                shrinkUnitsContainer[i].transform.position = Vector3.MoveTowards(shrinkUnitsContainer[i].transform.position,
                                                                                dropDesPos,
                                                                                0.02f);

                
            }
            yield return new WaitForEndOfFrame();
        }
    }

    //IEnumerator updateScore()
    //{
    //    int increaseAmount = (_unitCounter * _unitTypeCounter);
    //    int newScore = _score + increaseAmount;
    //    int smallIncrease = increaseAmount / 30;
    //    //int smallIncrease = 1;
    //    //print(smallIncrease);
    //    while (_score < newScore)
    //    {
    //        if (newScore - _score >= smallIncrease + 1)
    //        {
    //            _score += smallIncrease + 1;
    //        }
    //        else
    //        {
    //            _score += (newScore - _score);
    //        }

    //        _scoreText.text = (_score.ToString());


    //        if ((_score >= _1starPoint) && (_scoreSlider.maxValue == _1starPoint))
    //        {
    //            _scoreSlider.minValue = _1starPoint;
    //            _scoreSlider.maxValue = _2starPoint;

    //            if (_1starImage.enabled == false)
    //            {
    //                _1starImage.enabled = true;
    //            }
    //        }
    //        if ((_score >= _2starPoint) && (_scoreSlider.maxValue == _2starPoint))
    //        {
    //            _scoreSlider.minValue = _2starPoint;
    //            _scoreSlider.maxValue = _3starPoint;

    //            if (_2starImage.enabled == false)
    //            {
    //                _2starImage.enabled = true;
    //            }
    //        }
    //        if ((_score >= _3starPoint) && (_3starImage.enabled == false))
    //        {
    //            _3starImage.enabled = true;

    //        }
    //        _scoreSlider.value = _score;

    //        //yield return new WaitForEndOfFrame();
    //        yield return new WaitForSeconds(3 / 60);
    //    }
    //}

    void updateScore(int newScore)
    {
        _scoreText.text = newScore.ToString();

        if ((newScore >= _1starPoint) && (_scoreSlider.maxValue == _1starPoint))
        {
            _scoreSlider.minValue = _1starPoint;
            _scoreSlider.maxValue = _2starPoint;

            if (_1starImage.enabled == false)
            {
                _1starImage.enabled = true;
            }
        }
        if ((newScore >= _2starPoint) && (_scoreSlider.maxValue == _2starPoint))
        {
            _scoreSlider.minValue = _2starPoint;
            _scoreSlider.maxValue = _3starPoint;

            if (_2starImage.enabled == false)
            {
                _2starImage.enabled = true;
            }
        }
        if ((newScore >= _3starPoint) && (_3starImage.enabled == false))
        {
            _3starImage.enabled = true;

        }
        _scoreSlider.value = newScore;          

    }


    #region Specific chained Units types checking methods

    private void scanBlockNineChained()
    {
        int point = 9;
        int unitCount = 9;
        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {
                if (XIndex > 0 && XIndex < puzzleGen._columns - 1 
                    && YIndex > 0 && YIndex < puzzleGen._rows - 1
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex - 1, YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex + 1]._value == scanUnitValue
                    && !scanUnitARR[XIndex - 1, YIndex - 1]._isChained
                    && !scanUnitARR[XIndex    , YIndex - 1]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex - 1]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex + 1]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 1]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex + 1]._isChained)
                    {
                        scanUnitARR[XIndex - 1, YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex    , YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex    ]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex + 1]._isChained = true;

                        print("Block9 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                } 
            }
        }
    }

    private void scanCrossNineChained()
    {
        int point = 9;
        int unitCount = 9;
        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {
                if (XIndex > 1 && XIndex < puzzleGen._columns - 2
                    && YIndex > 1 && YIndex < puzzleGen._rows - 2
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex   , YIndex - 2]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex - 2, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex + 2, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 2]._value == scanUnitValue
                    && !scanUnitARR[XIndex    , YIndex - 2]._isChained
                    && !scanUnitARR[XIndex    , YIndex - 1]._isChained
                    && !scanUnitARR[XIndex - 2, YIndex    ]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex + 2, YIndex    ]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 1]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 2]._isChained)
                    {
                        scanUnitARR[XIndex    , YIndex - 2]._isChained = true;
                        scanUnitARR[XIndex    , YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex - 2, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex    ]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex + 2, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 2]._isChained = true;

                        print("Cross9 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
            }
        }
    }

    private void scanTSevenChained()
    {
        int point = 7;
        int unitCount = 7;
        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {
                
                // Check Down T chained
                if (XIndex > 1 && XIndex < puzzleGen._columns - 2 && YIndex > 1
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex    , YIndex -2]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex - 2, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex + 2, YIndex    ]._value == scanUnitValue
                    && !scanUnitARR[XIndex    , YIndex - 2]._isChained
                    && !scanUnitARR[XIndex    , YIndex - 1]._isChained
                    && !scanUnitARR[XIndex - 2, YIndex    ]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex + 2, YIndex    ]._isChained)
                    {
                        scanUnitARR[XIndex    , YIndex - 2]._isChained = true;
                        scanUnitARR[XIndex    , YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex - 2, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex    ]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex + 2, YIndex    ]._isChained = true;

                        print("T7 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
                               
                // Check Up T chained
                if (XIndex > 1 && XIndex < puzzleGen._columns - 2 && YIndex < puzzleGen._rows - 2
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex - 2, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex + 2, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 2]._value == scanUnitValue
                    && !scanUnitARR[XIndex - 2, YIndex    ]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex + 2, YIndex    ]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 1]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 2]._isChained)
                    {
                        scanUnitARR[XIndex - 2, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex    ]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex + 2, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 2]._isChained = true;

                        print("T7 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }               
                
                // Check Left T chained
                if (XIndex > 1 && YIndex > 1 && YIndex < puzzleGen._rows - 2
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex    , YIndex - 2]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex - 2, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 2]._value == scanUnitValue
                    && !scanUnitARR[XIndex    , YIndex - 2]._isChained
                    && !scanUnitARR[XIndex    , YIndex - 1]._isChained
                    && !scanUnitARR[XIndex - 2, YIndex    ]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 1]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 2]._isChained)
                    {
                        scanUnitARR[XIndex    , YIndex - 2]._isChained = true;
                        scanUnitARR[XIndex    , YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex - 2, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 2]._isChained = true;

                        print("T7 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
                
                // Check Right T chained
                if (XIndex < puzzleGen._columns - 2 && YIndex > 1 && YIndex < puzzleGen._rows - 2
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex    , YIndex - 2]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex + 2, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 2]._value == scanUnitValue
                    && !scanUnitARR[XIndex    , YIndex - 2]._isChained
                    && !scanUnitARR[XIndex    , YIndex - 1]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex + 2, YIndex    ]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 1]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 2]._isChained)
                    {
                        scanUnitARR[XIndex    , YIndex - 2]._isChained = true;
                        scanUnitARR[XIndex    , YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex    , YIndex    ]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex + 2, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 2]._isChained = true;

                        print("T7 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
                
            }
        }
    }

    private void scanUSevenChained()
    {
        int point = 7;
        int unitCount = 7;
        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {
                
                // Check Down U chained
                if (XIndex > 0 && XIndex < puzzleGen._columns - 1 && YIndex > 1
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex - 1, YIndex -2]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex - 2]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex    ]._value == scanUnitValue
                    && !scanUnitARR[XIndex - 1, YIndex - 2]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex - 2]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex - 1]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex - 1]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex    ]._isChained)
                    {
                        scanUnitARR[XIndex - 1, YIndex - 2]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex - 2]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex    ]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex    ]._isChained = true;

                        print("U7 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
                               
                // Check Up U chained
                if (XIndex > 0 && XIndex < puzzleGen._columns - 1 && YIndex < puzzleGen._rows - 2
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex - 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex + 2]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex + 2]._value == scanUnitValue
                    && !scanUnitARR[XIndex - 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex + 1]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex + 1]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex + 2]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex + 2]._isChained)
                    {
                        scanUnitARR[XIndex - 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex    ]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex + 2]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex + 2]._isChained = true;

                        print("U7 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }               
                
                // Check Left U chained
                if (XIndex > 1 && YIndex > 0 && YIndex < puzzleGen._rows - 1
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex - 2, YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex - 2, YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 1]._value == scanUnitValue
                    && !scanUnitARR[XIndex - 2, YIndex - 1]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex - 1]._isChained
                    && !scanUnitARR[XIndex    , YIndex - 1]._isChained
                    && !scanUnitARR[XIndex - 2, YIndex + 1]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex + 1]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 1]._isChained)
                    {
                        scanUnitARR[XIndex - 2, YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex    , YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex    , YIndex    ]._isChained = true;
                        scanUnitARR[XIndex - 2, YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 1]._isChained = true;

                        print("U7 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
                
                // Check Right U chained
                if (XIndex < puzzleGen._columns - 2 && YIndex > 0 && YIndex < puzzleGen._rows - 1
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex    , YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex + 2, YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex + 2, YIndex + 1]._value == scanUnitValue
                    && !scanUnitARR[XIndex    , YIndex - 1]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex - 1]._isChained
                    && !scanUnitARR[XIndex + 2, YIndex - 1]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 1]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex + 1]._isChained
                    && !scanUnitARR[XIndex + 2, YIndex + 1]._isChained)
                    {
                        scanUnitARR[XIndex    , YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex + 2, YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex    , YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex + 2, YIndex + 1]._isChained = true;

                        print("U7 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
                
            }
        }
    }

    private void scanLFiveChained()
    {
        int point = 5;
        int unitCount = 5;
        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {
                
                // Check Left Down L chained
                if (XIndex > 1 && YIndex > 1
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex   , YIndex -2]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex - 2, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex    ]._value == scanUnitValue
                    && !scanUnitARR[XIndex    , YIndex - 2]._isChained
                    && !scanUnitARR[XIndex    , YIndex - 1]._isChained
                    && !scanUnitARR[XIndex - 2, YIndex    ]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex    ]._isChained)
                    {
                        scanUnitARR[XIndex    , YIndex - 2]._isChained = true;
                        scanUnitARR[XIndex    , YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex    , YIndex    ]._isChained = true;
                        scanUnitARR[XIndex - 2, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex    ]._isChained = true;

                        print("L5 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
                               
                // Check Right Down L chained
                if (XIndex < puzzleGen._columns - 2 && YIndex > 1
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex   , YIndex - 2]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex + 2, YIndex    ]._value == scanUnitValue
                    && !scanUnitARR[XIndex    , YIndex - 2]._isChained
                    && !scanUnitARR[XIndex    , YIndex - 1]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex + 2, YIndex    ]._isChained)
                    {
                        scanUnitARR[XIndex    , YIndex - 2]._isChained = true;
                        scanUnitARR[XIndex    , YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex    , YIndex    ]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex + 2, YIndex    ]._isChained = true;

                        print("L5 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }               
                
                // Check Up Left L chained
                if (XIndex > 1 && YIndex < puzzleGen._rows - 2
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex - 2, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 2]._value == scanUnitValue
                    && !scanUnitARR[XIndex - 2, YIndex    ]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 1]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 2]._isChained)
                    {
                        scanUnitARR[XIndex - 2, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 2]._isChained = true;

                        print("L5 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
                
                // Check Up Right L chained
                if (XIndex < puzzleGen._columns - 2 && YIndex < puzzleGen._rows - 2
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex + 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex + 2, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 2]._value == scanUnitValue
                    && !scanUnitARR[XIndex + 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex + 2, YIndex    ]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 1]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 2]._isChained)
                    {
                        scanUnitARR[XIndex + 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex + 2, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 2]._isChained = true;

                        print("L5 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
            }
        }
    }

    private void scanCrossFiveChained()
    {
        int point = 5;
        int unitCount = 5;
        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {
                if (XIndex > 0 && XIndex < puzzleGen._columns - 1
                    && YIndex > 0 && YIndex < puzzleGen._rows - 1
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex   , YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex    ]._value == scanUnitValue
                    && scanUnitARR[XIndex    , YIndex + 1]._value == scanUnitValue
                    && !scanUnitARR[XIndex    , YIndex - 1]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex    ]._isChained
                    && !scanUnitARR[XIndex    , YIndex + 1]._isChained)
                    {
                        scanUnitARR[XIndex    , YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex    ]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex    , YIndex + 1]._isChained = true;

                        print("Cross5 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
            }
        }
    }

    private void scanLineFiveChained()
    {
        int point = 5;
        int unitCount = 5;
        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {

                // Check Horizontal line chained
                if (XIndex > 1 && XIndex < puzzleGen._columns - 2
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex - 2, YIndex]._value == scanUnitValue
                    && scanUnitARR[XIndex - 1, YIndex]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex]._value == scanUnitValue
                    && scanUnitARR[XIndex + 2, YIndex]._value == scanUnitValue
                    && !scanUnitARR[XIndex - 2, YIndex]._isChained
                    && !scanUnitARR[XIndex - 1, YIndex]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex]._isChained
                    && !scanUnitARR[XIndex + 2, YIndex]._isChained)
                    {
                        scanUnitARR[XIndex - 2, YIndex]._isChained = true;
                        scanUnitARR[XIndex - 1, YIndex]._isChained = true;
                        scanUnitARR[XIndex    , YIndex]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex]._isChained = true;
                        scanUnitARR[XIndex + 2, YIndex]._isChained = true;

                        print("Horizontal5 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }

                // Check Vertical line chained
                if (YIndex > 1 && YIndex < puzzleGen._rows - 2
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex, YIndex - 2]._value == scanUnitValue
                    && scanUnitARR[XIndex, YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex, YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex, YIndex + 2]._value == scanUnitValue
                    && !scanUnitARR[XIndex, YIndex - 2]._isChained
                    && !scanUnitARR[XIndex, YIndex - 1]._isChained
                    && !scanUnitARR[XIndex, YIndex + 1]._isChained
                    && !scanUnitARR[XIndex, YIndex + 2]._isChained)
                    {
                        scanUnitARR[XIndex, YIndex - 2]._isChained = true;
                        scanUnitARR[XIndex, YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex, YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex, YIndex + 2]._isChained = true;

                        print("Vertical5 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
            }
        }
    }

    private void scanLineFourChained()
    {
        int point = 4;
        int unitCount = 4;
        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {

                // Check Horizontal line chained
                if (XIndex > 0 && XIndex < puzzleGen._columns - 2
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex - 1, YIndex]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex]._value == scanUnitValue
                    && scanUnitARR[XIndex + 2, YIndex]._value == scanUnitValue
                    && !scanUnitARR[XIndex - 1, YIndex]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex]._isChained
                    && !scanUnitARR[XIndex + 2, YIndex]._isChained)
                    {
                        scanUnitARR[XIndex - 1, YIndex]._isChained = true;
                        scanUnitARR[XIndex    , YIndex]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex]._isChained = true;
                        scanUnitARR[XIndex + 2, YIndex]._isChained = true;

                        print("Horizontal4 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }

                // Check Vertical line chained
                if (YIndex > 0 && YIndex < puzzleGen._rows - 2
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex, YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex, YIndex + 1]._value == scanUnitValue
                    && scanUnitARR[XIndex, YIndex + 2]._value == scanUnitValue
                    && !scanUnitARR[XIndex, YIndex - 1]._isChained
                    && !scanUnitARR[XIndex, YIndex + 1]._isChained
                    && !scanUnitARR[XIndex, YIndex + 2]._isChained)
                    {
                        scanUnitARR[XIndex, YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex, YIndex + 1]._isChained = true;
                        scanUnitARR[XIndex, YIndex + 2]._isChained = true;

                        print("Vertical4 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
            }
        }
    }

    private void scanLineThreeChained()
    {
        int point = 1;
        int unitCount = 3;
        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {

                // Check Horizontal line chained
                if (XIndex > 0 && XIndex < puzzleGen._columns - 1
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex - 1, YIndex]._value == scanUnitValue
                    && scanUnitARR[XIndex + 1, YIndex]._value == scanUnitValue
                    && !scanUnitARR[XIndex - 1, YIndex]._isChained
                    && !scanUnitARR[XIndex + 1, YIndex]._isChained)
                    {
                        scanUnitARR[XIndex - 1, YIndex]._isChained = true;
                        scanUnitARR[XIndex    , YIndex]._isChained = true;
                        scanUnitARR[XIndex + 1, YIndex]._isChained = true;

                        print("Horizontal3 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }

                // Check Vertical line chained
                if (YIndex > 0 && YIndex < puzzleGen._rows - 1
                    && !scanUnitARR[XIndex, YIndex]._isChained)
                {
                    int scanUnitValue = scanUnitARR[XIndex, YIndex]._value;
                    if (scanUnitARR[XIndex, YIndex - 1]._value == scanUnitValue
                    && scanUnitARR[XIndex, YIndex + 1]._value == scanUnitValue
                    && !scanUnitARR[XIndex, YIndex - 1]._isChained
                    && !scanUnitARR[XIndex, YIndex + 1]._isChained)
                    {
                        scanUnitARR[XIndex, YIndex - 1]._isChained = true;
                        scanUnitARR[XIndex, YIndex    ]._isChained = true;
                        scanUnitARR[XIndex, YIndex + 1]._isChained = true;

                        print("Vertical3 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += unitCount;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
            }
        }
    }
   
    #endregion
}
