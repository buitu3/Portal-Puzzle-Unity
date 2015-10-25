﻿using UnityEngine;
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

    // Called when button finish is clicked
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
            for (int i = 0; i < puzzleGen._unitPrefabsContainer.Count; i++)
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
        //scanUSevenChained();
        scanLFiveChained();
        scanCrossFiveChained();
        
        /*
        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {
                if (isBlockNineChained(XIndex, YIndex))
                {
                    scanUnitARR[XIndex - 1, YIndex - 1]._isChained = true;
                    scanUnitARR[XIndex, YIndex - 1]._isChained = true;
                    scanUnitARR[XIndex + 1, YIndex - 1]._isChained = true;
                    scanUnitARR[XIndex - 1, YIndex]._isChained = true;
                    scanUnitARR[XIndex, YIndex]._isChained = true;
                    scanUnitARR[XIndex + 1, YIndex]._isChained = true;
                    scanUnitARR[XIndex - 1, YIndex + 1]._isChained = true;
                    scanUnitARR[XIndex, YIndex + 1]._isChained = true;
                    scanUnitARR[XIndex + 1, YIndex + 1]._isChained = true;

                    print("Block9 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                    _noChainedUnit = false;
                    _unitCounter += 9;
                    if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                    {
                        _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                    }
                }
            }
        }
        */
        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {
                if (isHorizontalTreeChained(XIndex, YIndex))
                {
                    scanUnitARR[XIndex - 1, YIndex]._isChained = true;
                    scanUnitARR[XIndex, YIndex]._isChained = true;
                    scanUnitARR[XIndex + 1, YIndex]._isChained = true;

                    print("Horizontal3 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                    _noChainedUnit = false;
                    _unitCounter += 3;
                    if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                    {
                        _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                    }
                }
            }
        }

        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {
                if (isVerticalTreeChained(XIndex, YIndex))
                {
                    scanUnitARR[XIndex, YIndex - 1]._isChained = true;
                    scanUnitARR[XIndex, YIndex]._isChained = true;
                    scanUnitARR[XIndex, YIndex + 1]._isChained = true;

                    print("Vertical3 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                    _noChainedUnit = false;
                    _unitCounter += 3;
                    if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                    {
                        _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                    }
                }
            }
        }
        
        #endregion

        // Call Destroy Chained Unit method if there are chained Units
        if (!_noChainedUnit)
        {
            StartCoroutine(destroyChainedUnits());
            // Update game info text
            _turnPointText.text = (_turnPoint.ToString());
            _unitCounterText.text = (_unitCounter.ToString());
            _unitTypeCounter = puzzleGen._unitPrefabsContainer.Count - _unitTypeCheckContainer.Count;           
            _unitTypeCounterText.text = (_unitTypeCounter.ToString());         
        }
        else
        {
            StartCoroutine(updateScore());
            GameStateController.currentState = GameStateController.gameState.idle;
            if (puzzleGen._turns < 1)
            {
                //print("over");
                StartCoroutine(gameStateController.endGame());
            }
        }
    }

    // Shrink chained Units before destroy
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
                shrinkUnitsContainer[i].transform.localScale = Vector3.MoveTowards(shrinkUnitsContainer[i].transform.localScale, 
                                                                                   Vector3.zero, 
                                                                                   0.15f);
                shrinkUnitsContainer[i].transform.position = Vector3.MoveTowards(shrinkUnitsContainer[i].transform.position,
                                                                                shrinkUnitsContainerDesPos[i],
                                                                                0.05f);
            }
            yield return new WaitForEndOfFrame();
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
        yield return (StartCoroutine(playDestroyUnitsAnimation(shrinkUnitsList)));

        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {
                if (scanUnitARR[XIndex, YIndex]._isChained)
                {
                    Destroy(puzzleGen._unitARR[XIndex, YIndex]);
                }
            }
        }
        yield return new WaitForEndOfFrame();
        puzzleGen.organizePuzzleAfterDestroy();
        //if (inputHandler._unitHighLight.activeInHierarchy)
        //{
        //    inputHandler._unitHighLight.SetActive(false);
        //}
    }

    IEnumerator updateScore()
    {
        int increaseAmount = (_unitCounter * _unitTypeCounter);
        int newScore = _score + increaseAmount;
        int smallIncrease = increaseAmount / 30;
        //int smallIncrease = 1;
        //print(smallIncrease);
        while (_score < newScore)
        {
            if (newScore - _score >= smallIncrease + 1)
            {
                _score += smallIncrease + 1;
            }
            else
            {
                _score += (newScore - _score);
            }

            _scoreText.text = (_score.ToString());


            if ((_score >= _1starPoint) && (_scoreSlider.maxValue == _1starPoint))
            {
                _scoreSlider.minValue = _1starPoint;
                _scoreSlider.maxValue = _2starPoint;

                if (_1starImage.enabled == false)
                {
                    _1starImage.enabled = true;
                }
            }
            if ((_score >= _2starPoint) && (_scoreSlider.maxValue == _2starPoint))
            {
                _scoreSlider.minValue = _2starPoint;
                _scoreSlider.maxValue = _3starPoint;

                if (_2starImage.enabled == false)
                {
                    _2starImage.enabled = true;
                }
            }
            if ((_score >= _3starPoint) && (_3starImage.enabled == false))
            {
                _3starImage.enabled = true;

            }
            _scoreSlider.value = _score;

            //yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(3/60);
        }
    }

    #region Specific chained Units types checking methods
    /*
    bool isBlockNineChained(int unitXIndex, int unitYIndex)
    {
        bool isChained = false;
        int scanUnitValue = scanUnitARR[unitXIndex, unitYIndex]._value;
        if (isHorizontalTreeChained(unitXIndex, unitYIndex)
            && isVerticalTreeChained(unitXIndex, unitYIndex)
            && !scanUnitARR[unitXIndex, unitYIndex]._isChained)
        {
            if (scanUnitARR[unitXIndex - 1, unitYIndex - 1]._value == scanUnitValue
                && scanUnitARR[unitXIndex + 1, unitYIndex - 1]._value == scanUnitValue
                && scanUnitARR[unitXIndex - 1, unitYIndex + 1]._value == scanUnitValue
                && scanUnitARR[unitXIndex + 1, unitYIndex + 1]._value == scanUnitValue
                && !scanUnitARR[unitXIndex - 1, unitYIndex - 1]._isChained
                && !scanUnitARR[unitXIndex + 1, unitYIndex - 1]._isChained
                && !scanUnitARR[unitXIndex - 1, unitYIndex + 1]._isChained
                && !scanUnitARR[unitXIndex + 1, unitYIndex + 1]._isChained)
            {
                isChained = true;
            }
        }
        return isChained;
    }
    */
    private void scanBlockNineChained()
    {
        int point = 9;
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
                        _unitCounter += 9;
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
                        _unitCounter += 9;
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
        int point = 5;
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
                        _unitCounter += 5;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
                /*               
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

                        print("U7 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += 5;
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

                        print("U7 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += 5;
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

                        print("U7 chained at" + XIndex + " : " + YIndex + "value :" + scanUnitARR[XIndex, YIndex]._value);
                        _noChainedUnit = false;
                        _unitCounter += 5;
                        _turnPoint += point;

                        if (_unitTypeCheckContainer.Contains(scanUnitARR[XIndex, YIndex]._value))
                        {
                            _unitTypeCheckContainer.Remove(scanUnitARR[XIndex, YIndex]._value);
                        }
                    }
                }
                */
            }
        }
    }

    private void scanLFiveChained()
    {
        int point = 5;
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
                        _unitCounter += 5;
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
                        _unitCounter += 5;
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
                        _unitCounter += 5;
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
                        _unitCounter += 5;
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
                        _unitCounter += 5;
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



    bool isHorizontalTreeChained(int unitXIndex, int unitYIndex)
    {
        bool isChained = false;
        int scanUnitValue = scanUnitARR[unitXIndex, unitYIndex]._value;
        if (unitXIndex > 0 && unitXIndex < puzzleGen._columns - 1 
            && !scanUnitARR[unitXIndex, unitYIndex]._isChained)
        {
            if (scanUnitARR[unitXIndex - 1, unitYIndex]._value == scanUnitValue
                && scanUnitARR[unitXIndex + 1, unitYIndex]._value == scanUnitValue
                && !scanUnitARR[unitXIndex - 1, unitYIndex]._isChained
                && !scanUnitARR[unitXIndex + 1, unitYIndex]._isChained)
            {
                isChained = true;
            }
        }
        return isChained;
    }

    bool isVerticalTreeChained(int unitXIndex, int unitYIndex)
    {
        bool isChained = false;
        int scanUnitValue = scanUnitARR[unitXIndex, unitYIndex]._value;
        if (unitYIndex > 0 && unitYIndex < puzzleGen._rows - 1 
            && !scanUnitARR[unitXIndex, unitYIndex]._isChained)
        {
            if (scanUnitARR[unitXIndex, unitYIndex - 1]._value == scanUnitValue
                && scanUnitARR[unitXIndex, unitYIndex + 1]._value == scanUnitValue
                && !scanUnitARR[unitXIndex, unitYIndex - 1]._isChained
                && !scanUnitARR[unitXIndex, unitYIndex + 1]._isChained)
            {
                isChained = true;
            }
        }
        return isChained;
    }

    #endregion
}
