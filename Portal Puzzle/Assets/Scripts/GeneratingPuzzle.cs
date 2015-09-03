using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BaseUnit
{
    public List<int> _valueList = new List<int>();           // A list of values object that can be choosen
    public int _value;                                       // A variable which is choosen randomly from valueList

    public BaseUnit(int unitTypes)
    {
        // add all value into the valueList with range from 0 to number of unit types-1
        for (int i = 0; i < unitTypes; i++)
        {
            _valueList.Add(i);
        }
    }
}

public class GeneratingPuzzle : MonoBehaviour {

    //==============================================
    // Constants
    //==============================================

    //==============================================
    // Fields
    //==============================================

    public static GeneratingPuzzle Instance;

    private DestroyChainedUnit destroyUnit;

    [HideInInspector]
    public Transform _unitsHolder;                             // Unit Clone Container in Hierachy
    public  List<GameObject> _unitPrefabsContainer;            // List contain Units Prefab

    public GameObject _portalGatePrefab;                       // Puzzle border prefab
    public GameObject _portalCornerPrefab;                     // Puzzle border corner prefab
    public Transform _borderHolder;                            // Puzzle border Container in Hierachy

    private BaseUnit[,] _valueARR;                             // Array of number used for generating puzzle
    public Vector3[,] _unitPosARR;                             // Array of Units position
    public  GameObject[,] _unitARR;                            // Array of unit gameobject

    public int _unitTypesCount;                                // Number of unit types in puzzle
    public int _rows;                                          // Number of unit rows in puzzle
    public int _columns;                                       // Number of unit collumns in puzzle

    public int _turns;                                         // Number of player's turns

    public int _unitFallingSpd;                                 // Unit falling down speed

    //[HideInInspector]
    //public float _unitWidth = 0.8f;                            // The width of an unit

    //[HideInInspector]
    //public float _unitHeight = 0.8f;                          // The height of an unit
    //private float _XOffset = 0.6f;                            // Distance between the first collumn and left side of the screen
    //private float _YOffset = 1.7f;                            // Distance between the first row and bottom side of the screen

    [HideInInspector]
    public float _unitWidth;                                    // The width of an unit

    [HideInInspector]
    public float _unitHeight;                                   // The height of an unit
    private float _XOffset;                              // Distance between the first collumn and left side of the screen
    private float _YOffset;                              // Distance between the first row and bottom side of the screen


    //==============================================
    // Unity Methods
    //==============================================

    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
    }

	void Start () {
        destroyUnit = DestroyChainedUnit.Instance;
        
        _unitsHolder = new GameObject("Units Holder").transform;
        _borderHolder = new GameObject("Borders Holder").transform;

        // Identify some information of the Puzzle
        _unitHeight = _unitPrefabsContainer[0].GetComponent<SpriteRenderer>().bounds.size.y;
        _unitWidth = _unitPrefabsContainer[0].GetComponent<SpriteRenderer>().bounds.size.x;
        float cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * Camera.main.aspect;
        _YOffset = (cameraHeight - _unitHeight * _rows) / 2;
        _XOffset = (cameraWidth - _unitWidth * _columns) / 2;

        // Generating value matrix
        _valueARR = generateValueMatrix();

        // Init Unit position Array
        _unitPosARR = new Vector3[_columns, _rows];

        // Init Unit gameobject Array
        _unitARR = new GameObject[_columns, _rows];

        // Identify each Unit infomation and Instantiate It
        for (int YIndex = 0; YIndex < _rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < _columns; XIndex++)
            {
                // Identify position for each Unit
                _unitPosARR[XIndex, YIndex] = new Vector3(_XOffset + _unitWidth * XIndex, 
                                                        _YOffset + _unitHeight * YIndex, 
                                                        _unitPrefabsContainer[0].transform.position.z);
                // Instantiate Unit
                _unitARR[XIndex, YIndex] = Instantiate(_unitPrefabsContainer[_valueARR[XIndex, YIndex]._value], 
                                                    _unitPosARR[XIndex, YIndex], 
                                                    Quaternion.identity) as GameObject;
                _unitARR[XIndex, YIndex].transform.SetParent(_unitsHolder);
                // Set information for each Unit
                _unitARR[XIndex, YIndex].GetComponent<UnitInfo>()._value = _valueARR[XIndex, YIndex]._value;
                _unitARR[XIndex, YIndex].GetComponent<UnitInfo>()._XIndex = XIndex;
                _unitARR[XIndex, YIndex].GetComponent<UnitInfo>()._YIndex = YIndex;
            }
        }

        #region Insantiate Puzzle border

        // Left side border
        for (int Yindex = 0; Yindex < _rows; Yindex++)
        {
            GameObject portalGate = Instantiate(_portalGatePrefab,
                                                new Vector2(_unitPosARR[0, Yindex].x, _unitPosARR[0, Yindex].y + _unitHeight),
                                                Quaternion.Euler(0, 0, 180)) as GameObject;
            portalGate.transform.SetParent(_borderHolder);
        }
        // Right side border
        for (int Yindex = 0; Yindex < _rows; Yindex++)
        {
            GameObject portalGate = Instantiate(_portalGatePrefab,
                                                new Vector2(_unitPosARR[_columns - 1, Yindex].x + _unitWidth, _unitPosARR[_columns - 1, Yindex].y),
                                                Quaternion.identity) as GameObject;
            portalGate.transform.SetParent(_borderHolder);
        }
        // Lower side border
        for (int Xindex = 0; Xindex < _columns; Xindex++)
        {
            GameObject portalGate = Instantiate(_portalGatePrefab,
                                                new Vector2(_unitPosARR[Xindex, 0].x, _unitPosARR[Xindex, 0].y),
                                                Quaternion.Euler(0, 0, -90)) as GameObject;
            portalGate.transform.SetParent(_borderHolder);
        }
        // Upper side border
        for (int Xindex = 0; Xindex < _columns; Xindex++)
        {
            GameObject portalGate = Instantiate(_portalGatePrefab,
                                                new Vector2(_unitPosARR[Xindex, _rows - 1].x + _unitWidth, _unitPosARR[Xindex, _rows - 1].y + _unitHeight),
                                                Quaternion.Euler(0, 0, 90)) as GameObject;
            portalGate.transform.SetParent(_borderHolder);
        }
        // Four border corner
        GameObject portalCorner = Instantiate(_portalCornerPrefab,
                                                new Vector2(_unitPosARR[0, 0].x - _unitWidth, _unitPosARR[0, 0].y - _unitHeight),
                                                Quaternion.identity) as GameObject;
        portalCorner.transform.SetParent(_borderHolder);
        portalCorner = Instantiate(_portalCornerPrefab,
                                   new Vector2(_unitPosARR[_columns - 1, 0].x + _unitWidth, _unitPosARR[_columns - 1, 0].y - _unitHeight),
                                   Quaternion.identity) as GameObject;
        portalCorner.transform.SetParent(_borderHolder);
        portalCorner = Instantiate(_portalCornerPrefab,
                                   new Vector2(_unitPosARR[0, _rows - 1].x - _unitWidth, _unitPosARR[0, _rows - 1].y + _unitHeight),
                                   Quaternion.identity) as GameObject;
        portalCorner.transform.SetParent(_borderHolder);
        portalCorner = Instantiate(_portalCornerPrefab,
                                   new Vector2(_unitPosARR[_columns - 1, _rows - 1].x + _unitWidth, _unitPosARR[_columns - 1, _rows - 1].y + _unitHeight),
                                   Quaternion.identity) as GameObject;
        portalCorner.transform.SetParent(_borderHolder);
        #endregion
    }

    //==============================================
    // Methods
    //==============================================

    // Make Units fall down and regenerate Units
    public void organizePuzzleAfterDestroy()
    {
        if (GameStateController.currentState == GameStateController.gameState.destroyingUnit)
        {
            GameStateController.currentState = GameStateController.gameState.regeneratingUnit;

            List<GameObject> regenUnitContainer = new List<GameObject>();
            List<Vector3> regenUnitTargetPosContainer = new List<Vector3>();
            
            // Make Units fall down after destroy state
            for (int XIndex = 0; XIndex < _columns; XIndex++)
            {
                int nullObjectCount = 0;
                for (int YIndex = 0; YIndex < _rows; YIndex++)
                {
                    if (_unitARR[XIndex, YIndex] == null)
                    {
                        nullObjectCount += 1;
                    }
                    else
                    {
                        // Make Unit fall down if there are empty space below
                        if (nullObjectCount > 0)
                        {
                            Vector3 targetPos = _unitPosARR[XIndex, YIndex - nullObjectCount];
                            StartCoroutine(moveUnitToOrganizedPos(_unitARR[XIndex, YIndex], targetPos));

                            _unitARR[XIndex, YIndex - nullObjectCount] = _unitARR[XIndex, YIndex];
                            _unitARR[XIndex, YIndex - nullObjectCount].GetComponent<UnitInfo>()._YIndex -= nullObjectCount;
                        }
                    }
                }

                // Regenerate Unit after destroy
                if (nullObjectCount > 0)
                {
                    int regenUnitCounter = 0;
                    for (int regenUnitYIndex = _rows - nullObjectCount; regenUnitYIndex < _rows; regenUnitYIndex++)
                    {
                        regenUnitCounter++;
                        Vector3 regenUnitSpawnPos = new Vector3(_unitPosARR[XIndex, 0].x,
                                                            _unitPosARR[XIndex, _rows - 1].y + _unitHeight * (regenUnitCounter),
                                                            _unitPosARR[XIndex, 0].z);
                        int regenUnitValue = Random.Range(0, _unitTypesCount);
                        GameObject regenPrefab = _unitPrefabsContainer[regenUnitValue];
                        GameObject regenUnit = Instantiate(regenPrefab, regenUnitSpawnPos, Quaternion.identity) as GameObject;
                        regenUnit.transform.SetParent(_unitsHolder);
                        Vector3 regenUnitTargetPos = _unitPosARR[XIndex, regenUnitYIndex];

                        _unitARR[XIndex, regenUnitYIndex] = regenUnit;
                        regenUnit.GetComponent<UnitInfo>()._XIndex = XIndex;
                        regenUnit.GetComponent<UnitInfo>()._YIndex = regenUnitYIndex;
                        regenUnit.GetComponent<UnitInfo>()._value = regenUnitValue;

                        regenUnitContainer.Add(regenUnit);
                        regenUnitTargetPosContainer.Add(regenUnitTargetPos);

                        //StartCoroutine(moveRegenUnitToOrganizedPos(regenUnit, regenUnitTargetPos));
                    }
                }
            }
            StartCoroutine(moveRegenUnitsToOrganizedPos(regenUnitContainer, regenUnitTargetPosContainer));
        }       
    }

    
    IEnumerator moveUnitToOrganizedPos(GameObject unit, Vector3 targetPos)
    {
        yield return new WaitForSeconds(0.2f);
        while (unit.transform.position != targetPos)
        {
            unit.transform.position = Vector3.MoveTowards(unit.transform.position, targetPos, Time.deltaTime * _unitFallingSpd);
            yield return new WaitForEndOfFrame();
        }
    }

    // Move all regenerated units into destinated position
    IEnumerator moveRegenUnitsToOrganizedPos(List<GameObject> unitContainer, List<Vector3> targetPosContainer)
    {
        yield return new WaitForSeconds(0.2f);
        while (unitContainer.Count > 0)
        {
            for (int i = 0; i < unitContainer.Count; i++)
            {
                unitContainer[i].transform.position = Vector3.MoveTowards(unitContainer[i].transform.position,
                                                                        targetPosContainer[i],
                                                                        Time.deltaTime * _unitFallingSpd);

                if (unitContainer[i].transform.position == targetPosContainer[i])
                {
                    unitContainer.Remove(unitContainer[i]);
                    targetPosContainer.Remove(targetPosContainer[i]);
                    i -= 1;
                }
            }
            yield return new WaitForEndOfFrame();
        }
        // Check for chained Units again
        GameStateController.currentState = GameStateController.gameState.idle;
        destroyUnit.markChainedUnits();
    }


    // Generate ValueMatrix for the puzzle

    private BaseUnit[,] generateValueMatrix()
    {
        BaseUnit[,] valueMatrix = new BaseUnit[_columns, _rows];

        // Init base UnitARR
        for (int YIndex = 0; YIndex < _rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < _columns; XIndex++)
            {
                valueMatrix[XIndex, YIndex] = new BaseUnit(_unitTypesCount);
            }
        }

        // Setting value to all baseUnits and make sure there are no chained Units
        for (int YIndex = 0; YIndex < _rows; YIndex++)
        {
            for (int XIndex = 0; XIndex < _columns; XIndex++)
            {
                // Scan current Unit's relatives to remove value that shouldn't be choosen
                if (XIndex > 1)
                {
                    if (valueMatrix[XIndex - 1, YIndex]._value == valueMatrix[XIndex - 2, YIndex]._value)
                    {
                        valueMatrix[XIndex, YIndex]._valueList.Remove(valueMatrix[XIndex - 1, YIndex]._value);
                    }
                }
                if (YIndex > 1)
                {
                    if (valueMatrix[XIndex, YIndex - 1]._value == valueMatrix[XIndex, YIndex - 2]._value)
                    {
                        valueMatrix[XIndex, YIndex]._valueList.Remove(valueMatrix[XIndex, YIndex - 1]._value);
                    }
                }

                // Select a random object from valueList

                if (valueMatrix[XIndex, YIndex]._valueList.Count != 0)
                {
                    valueMatrix[XIndex, YIndex]._value = valueMatrix[XIndex, YIndex]._valueList[Random.Range(0, valueMatrix[XIndex, YIndex]._valueList.Count)];
                    valueMatrix[XIndex, YIndex]._valueList.Remove(valueMatrix[XIndex, YIndex]._value);
                }
                else
                {
                    valueMatrix[XIndex, YIndex] = new BaseUnit(_unitTypesCount);
                    if (XIndex == 0)
                    {
                        YIndex -= 1;
                        XIndex = _columns - 1;
                    }
                    else
                    {
                        XIndex -= 1;
                    }
                    continue;
                }

            }
        }
        return valueMatrix;
    }
	
}
