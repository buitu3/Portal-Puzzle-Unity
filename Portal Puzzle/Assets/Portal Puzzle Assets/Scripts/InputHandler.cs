using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InputHandler : MonoBehaviour,IPointerDownHandler,IPointerUpHandler  {

    //==============================================
    // Constants
    //==============================================

    //==============================================
    // Fields
    //==============================================

    public static InputHandler Instance;

    public float _shiftingSpeed;                            // Units moving speed

    public GameObject _unitHighLight;                       // Units highlighters
    public LineRenderer _aboveLine;
    public LineRenderer _belowLine;
    public LineRenderer _rightLine;
    public LineRenderer _leftLine;

    private float _horizontalHighlightBeginPos;           // Units highlight start and end position
    private float _horizontalHighlightEndPos;
    private float _verticalHighlightBeginPos;
    private float _verticalHighlightEndPos;

    private GeneratingPuzzle puzzleGen;
    private DestroyChainedUnit destroyUnit;
    private LayerMask _unitLayer;
    private GameObject _unit;                               // The focused unit

    [HideInInspector]
    public bool _isMoving = false;
    private bool _UnitSelected = false;
    private Transform _unitsHolder;

    private Vector2 _pointerOriginPos;
    private Vector2 _pointerCurrentPos;
    private Vector2 _direction;

    int counter = 0;

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
        destroyUnit = DestroyChainedUnit.Instance;
        _unitLayer = LayerMask.GetMask("Unit");
        _unitsHolder = GeneratingPuzzle.Instance._unitsHolder;

        _unitHighLight.SetActive(false);
        _horizontalHighlightBeginPos = puzzleGen._unitPosARR[0, 0].x;
        _horizontalHighlightEndPos = puzzleGen._unitPosARR[puzzleGen._columns - 1, 0].x + puzzleGen._unitWidth;
        _verticalHighlightBeginPos = puzzleGen._unitPosARR[0, 0].y;
        _verticalHighlightEndPos = puzzleGen._unitPosARR[0, puzzleGen._rows - 1].y + puzzleGen._unitHeight;

        StartCoroutine(controlMovingUnits());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (GameStateController.currentState == GameStateController.gameState.idle && _unit != null)
            {
                StartCoroutine(shiftRight(_unit, 1));
            }            
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (GameStateController.currentState == GameStateController.gameState.idle && _unit != null)
            {
                StartCoroutine(shiftLeft(_unit, 1));
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (GameStateController.currentState == GameStateController.gameState.idle && _unit != null)
            {
                StartCoroutine(shiftUp(_unit, 1));
            }
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (GameStateController.currentState == GameStateController.gameState.idle && _unit != null)
            {
                StartCoroutine(shiftDown(_unit, 1));
            }
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (GameStateController.currentState == GameStateController.gameState.idle)
        {
            // Cast a ray from camera into pointer position
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D unitHit;
            unitHit = Physics2D.Raycast(camRay.origin, camRay.direction, Mathf.Infinity, _unitLayer);

            if (unitHit.collider != null)
            {
                _UnitSelected = true;
                // Get the Unit if camray hit
                _unit = unitHit.collider.gameObject;
                print(_unit.GetComponent<UnitInfo>()._XIndex + " : "
                    + _unit.GetComponent<UnitInfo>()._YIndex + " : "
                    + _unit.GetComponent<UnitInfo>()._value);
                // Highlight the selected Unit
                highLightUnit(_unit);
            }       
        }
        _pointerOriginPos = data.position;
    }

    //public void OnDrag(PointerEventData data)
    //{
    //    if (_unit != null)
    //    {
    //        Vector2 pointerPosition = Camera.main.ScreenToWorldPoint(data.position);
    //        //_unit.transform.position = new Vector3(pointerPosition.x - puzzleGen._unitWidth/2, 
    //        //                                    _unit.transform.position.y,
    //        //                                    _unit.transform.position.z);
    //        if (pointerPosition.x > _unit.transform.position.x + puzzleGen._unitWidth
    //            && pointerPosition.y > _unit.transform.position.y
    //            && pointerPosition.y < _unit.transform.position.y + puzzleGen._unitHeight
    //            && GameStateController.currentState == GameStateController.gameState.idle
    //            && _unit.GetComponent<UnitInfo>()._XIndex < puzzleGen._columns - 1)
    //        {
    //            int shiftingSpeedMultiplier = (int)(Mathf.Abs(pointerPosition.x - _unit.transform.position.x) / puzzleGen._unitWidth);
    //            //print(shiftingSpeedMultiplier);
    //            StartCoroutine(shiftRight(_unit, shiftingSpeedMultiplier));
    //        }
    //        else if (pointerPosition.x < _unit.transform.position.x
    //            && pointerPosition.y > _unit.transform.position.y
    //            && pointerPosition.y < _unit.transform.position.y + puzzleGen._unitHeight
    //            && GameStateController.currentState == GameStateController.gameState.idle
    //            && _unit.GetComponent<UnitInfo>()._XIndex > 0)
    //        {
    //            StartCoroutine(shiftLeft(_unit));
    //        }
    //        else if (pointerPosition.y > _unit.transform.position.y + puzzleGen._unitHeight
    //            && pointerPosition.x > _unit.transform.position.x
    //            && pointerPosition.x < _unit.transform.position.x + puzzleGen._unitWidth
    //            && GameStateController.currentState == GameStateController.gameState.idle
    //            && _unit.GetComponent<UnitInfo>()._YIndex < puzzleGen._rows - 1)
    //        {
    //            StartCoroutine(shiftUp(_unit));
    //        }
    //        else if (pointerPosition.y < _unit.transform.position.y
    //            && pointerPosition.x > _unit.transform.position.x
    //            && pointerPosition.x < _unit.transform.position.x + puzzleGen._unitWidth
    //            && GameStateController.currentState == GameStateController.gameState.idle
    //            && _unit.GetComponent<UnitInfo>()._YIndex > 0)
    //        {
    //            StartCoroutine(shiftDown(_unit));
    //        }
    //        //else
    //        //{
    //        //    print("whoops!");
    //        //}
    //    }
    //}

    public void OnPointerUp(PointerEventData data)
    {
        if (_UnitSelected)
        {
            _UnitSelected = false;
            _unit = null;
            if (GameStateController.currentState == GameStateController.gameState.movingUnit)
            {
                StartCoroutine(waitForEndOfMovingUnit());
            }
            else
            {
                _unitHighLight.SetActive(false);
                destroyUnit.onBtnFinishClicked();   
            }
        }
    }

    // Check Pointer position frequently and Move Units based on pointer position
    IEnumerator controlMovingUnits()
    {
        while (true)
        {
            // Only shifting if there is Unit selected
            if (_UnitSelected)
            {
                Vector2 pointerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                
                if (pointerPosition.x > _unit.transform.position.x + puzzleGen._unitWidth
                    && pointerPosition.y > _unit.transform.position.y
                    && pointerPosition.y < _unit.transform.position.y + puzzleGen._unitHeight
                    && GameStateController.currentState == GameStateController.gameState.idle
                    && _unit.GetComponent<UnitInfo>()._XIndex < puzzleGen._columns - 1)
                {
                    // The further pointer position away from Selected Unit current position,the faster shifting speed is.
                    int shiftingSpeedMultiplier = (int)(Mathf.Abs(pointerPosition.x - _unit.transform.position.x) / puzzleGen._unitWidth);
                    //print(shiftingSpeedMultiplier);
                    StartCoroutine(shiftRight(_unit, shiftingSpeedMultiplier));
                }
                else if (pointerPosition.x < _unit.transform.position.x
                    && pointerPosition.y > _unit.transform.position.y
                    && pointerPosition.y < _unit.transform.position.y + puzzleGen._unitHeight
                    && GameStateController.currentState == GameStateController.gameState.idle
                    && _unit.GetComponent<UnitInfo>()._XIndex > 0)
                {
                    int shiftingSpeedMultiplier = (int)((Mathf.Abs(pointerPosition.x - _unit.transform.position.x) + 1) / puzzleGen._unitWidth);
                    StartCoroutine(shiftLeft(_unit, shiftingSpeedMultiplier));
                }
                else if (pointerPosition.y > _unit.transform.position.y + puzzleGen._unitHeight
                    && pointerPosition.x > _unit.transform.position.x
                    && pointerPosition.x < _unit.transform.position.x + puzzleGen._unitWidth
                    && GameStateController.currentState == GameStateController.gameState.idle
                    && _unit.GetComponent<UnitInfo>()._YIndex < puzzleGen._rows - 1)
                {
                    int shiftingSpeedMultiplier = (int)(Mathf.Abs(pointerPosition.y - _unit.transform.position.y) / puzzleGen._unitHeight);
                    StartCoroutine(shiftUp(_unit, shiftingSpeedMultiplier));
                }
                else if (pointerPosition.y < _unit.transform.position.y
                    && pointerPosition.x > _unit.transform.position.x
                    && pointerPosition.x < _unit.transform.position.x + puzzleGen._unitWidth
                    && GameStateController.currentState == GameStateController.gameState.idle
                    && _unit.GetComponent<UnitInfo>()._YIndex > 0)
                {
                    int shiftingSpeedMultiplier = (int)((Mathf.Abs(pointerPosition.y - _unit.transform.position.y) + 1)/ puzzleGen._unitHeight);
                    StartCoroutine(shiftDown(_unit, shiftingSpeedMultiplier));
                }
                //else
                //{
                //    print("whoops!");
                //}
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    //==============================================
    // Methods
    //==============================================

    IEnumerator waitForEndOfMovingUnit()
    {
        while (GameStateController.currentState == GameStateController.gameState.movingUnit)
        {
            yield return new WaitForEndOfFrame();
        }
        _unitHighLight.SetActive(false);
        destroyUnit.onBtnFinishClicked();
    }

    void highLightUnit(GameObject unit)
    {
        if (unit != null)
        {
            if (!_unitHighLight.activeInHierarchy)
            {
                _unitHighLight.SetActive(true);
            }

            //_aboveLine.SetPosition(0, new Vector3(0, unit.transform.position.y + puzzleGen._unitHeight, -1));
            //_aboveLine.SetPosition(1, new Vector3(6, unit.transform.position.y + puzzleGen._unitHeight, -1));

            //_belowLine.SetPosition(0, new Vector3(0, unit.transform.position.y, -1));
            //_belowLine.SetPosition(1, new Vector3(6, unit.transform.position.y, -1));

            //_rightLine.SetPosition(0, new Vector3(unit.transform.position.x + puzzleGen._unitWidth, 0, -1));
            //_rightLine.SetPosition(1, new Vector3(unit.transform.position.x + puzzleGen._unitWidth, 10, -1));

            //_leftLine.SetPosition(0, new Vector3(unit.transform.position.x, 0, -1));
            //_leftLine.SetPosition(1, new Vector3(unit.transform.position.x, 10, -1));

            _aboveLine.SetPosition(0, new Vector3(_horizontalHighlightBeginPos, unit.transform.position.y + puzzleGen._unitHeight, -1));
            _aboveLine.SetPosition(1, new Vector3(_horizontalHighlightEndPos, unit.transform.position.y + puzzleGen._unitHeight, -1));

            _belowLine.SetPosition(0, new Vector3(_horizontalHighlightBeginPos, unit.transform.position.y, -1));
            _belowLine.SetPosition(1, new Vector3(_horizontalHighlightEndPos, unit.transform.position.y, -1));

            _rightLine.SetPosition(0, new Vector3(unit.transform.position.x + puzzleGen._unitWidth, _verticalHighlightBeginPos, -1));
            _rightLine.SetPosition(1, new Vector3(unit.transform.position.x + puzzleGen._unitWidth, _verticalHighlightEndPos, -1));

            _leftLine.SetPosition(0, new Vector3(unit.transform.position.x, _verticalHighlightBeginPos, -1));
            _leftLine.SetPosition(1, new Vector3(unit.transform.position.x, _verticalHighlightEndPos, -1));
        }
    }

    #region Shifting Units Methods

    // Shift all units in the same row as the choosen unit 1 distance to the right 
    IEnumerator shiftRight(GameObject unit, int shiftingSpeedMultiplier)
    {
        GameStateController.currentState = GameStateController.gameState.movingUnit;

        // Position of each units after shifting
        Vector3[] targetPos = new Vector3[puzzleGen._columns];

        // The index of shifting row
        int YIndex = unit.GetComponent<UnitInfo>()._YIndex;

        // Spawn Position of the unit that go through potar before shifting
        Vector3 portalUnitSpawnPos = new Vector3(puzzleGen._unitARR[0, YIndex].transform.position.x - puzzleGen._unitWidth, 
                                                puzzleGen._unitARR[0, YIndex].transform.position.y, 
                                                puzzleGen._unitARR[0, YIndex].transform.position.z);

        // Position of the portal unit after shifting
        Vector3 portalUnitTargetPos = puzzleGen._unitARR[0, YIndex].transform.position;

        // Instantiate the Unit that go through portal
        GameObject portalUnit = Instantiate(puzzleGen._unitPrefabsContainer[puzzleGen._unitARR[puzzleGen._columns - 1, YIndex].GetComponent<UnitInfo>()._value], 
                                            portalUnitSpawnPos, 
                                            Quaternion.identity) as GameObject;
        // Set value for the Unit that go through portal
        portalUnit.GetComponent<UnitInfo>()._XIndex = puzzleGen._unitARR[0, YIndex].GetComponent<UnitInfo>()._XIndex;
        portalUnit.GetComponent<UnitInfo>()._YIndex = puzzleGen._unitARR[0, YIndex].GetComponent<UnitInfo>()._YIndex;
        portalUnit.GetComponent<UnitInfo>()._value = puzzleGen._unitARR[puzzleGen._columns - 1, YIndex].GetComponent<UnitInfo>()._value;
        portalUnit.transform.SetParent(_unitsHolder);

        // Identify destination for each Unit
        for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
        {
            targetPos[XIndex] = new Vector3(puzzleGen._unitARR[XIndex, YIndex].transform.position.x + puzzleGen._unitWidth,
                                            puzzleGen._unitARR[XIndex, YIndex].transform.position.y,
                                            puzzleGen._unitARR[XIndex, YIndex].transform.position.z);
        }

        if (unit.GetComponent<UnitInfo>()._XIndex == puzzleGen._columns - 1)
        {
            _unit = portalUnit;
        }

        while (unit.transform.position != targetPos[unit.GetComponent<UnitInfo>()._XIndex])
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {
                puzzleGen._unitARR[XIndex, YIndex].transform.position = Vector3.MoveTowards(puzzleGen._unitARR[XIndex, YIndex].transform.position,
                                                                                           targetPos[XIndex],
                                                                                           Time.deltaTime * _shiftingSpeed * shiftingSpeedMultiplier);
            }
            portalUnit.transform.position = Vector3.MoveTowards(portalUnit.transform.position,
                                                                portalUnitTargetPos, 
                                                                Time.deltaTime * _shiftingSpeed * shiftingSpeedMultiplier);
            //yield return null;
            highLightUnit(_unit);

            yield return new WaitForEndOfFrame();
        }

        Destroy(puzzleGen._unitARR[puzzleGen._columns - 1, YIndex]);
        
        for (int XIndex = puzzleGen._columns - 1; XIndex > 0; XIndex--)
        {
            puzzleGen._unitARR[XIndex, YIndex] = puzzleGen._unitARR[XIndex - 1, YIndex];
            puzzleGen._unitARR[XIndex, YIndex].GetComponent<UnitInfo>()._XIndex += 1;
        }
        puzzleGen._unitARR[0, YIndex] = portalUnit;

        //puzzleGen._unitARR[0, YIndex].GetComponent<UnitInfo>()._XIndex = portalUnit.GetComponent<UnitInfo>()._XIndex;
        //puzzleGen._unitARR[0, YIndex].GetComponent<UnitInfo>()._value = portalUnit.GetComponent<UnitInfo>()._value;

        //_isMoving = false;
        GameStateController.currentState = GameStateController.gameState.idle;
    }


    // Shift all units in the same row as the choosen unit 1 distance to the left 
    IEnumerator shiftLeft(GameObject unit, int shiftingSpeedMultiplier)
    {
        GameStateController.currentState = GameStateController.gameState.movingUnit;

        // Position of each units after shifting
        Vector3[] targetPos = new Vector3[puzzleGen._columns];

        // The index of shifting row
        int YIndex = unit.GetComponent<UnitInfo>()._YIndex;

        // Spawn Position of the unit that go through potar before shifting
        Vector3 portalUnitSpawnPos = new Vector3(puzzleGen._unitARR[puzzleGen._columns - 1, YIndex].transform.position.x + puzzleGen._unitWidth,
                                                puzzleGen._unitARR[puzzleGen._columns - 1, YIndex].transform.position.y,
                                                puzzleGen._unitARR[puzzleGen._columns - 1, YIndex].transform.position.z);

        // Position of the portal unit after shifting
        Vector3 portalUnitTargetPos = puzzleGen._unitARR[puzzleGen._columns - 1, YIndex].transform.position;

        // Instantiate the Unit that go through portal
        GameObject portalUnit = Instantiate(puzzleGen._unitPrefabsContainer[puzzleGen._unitARR[0, YIndex].GetComponent<UnitInfo>()._value],
                                            portalUnitSpawnPos,
                                            Quaternion.identity) as GameObject;
        // Set value for the Unit that go through portal
        portalUnit.GetComponent<UnitInfo>()._XIndex = puzzleGen._unitARR[puzzleGen._columns - 1, YIndex].GetComponent<UnitInfo>()._XIndex;
        portalUnit.GetComponent<UnitInfo>()._YIndex = puzzleGen._unitARR[puzzleGen._columns - 1, YIndex].GetComponent<UnitInfo>()._YIndex;
        portalUnit.GetComponent<UnitInfo>()._value = puzzleGen._unitARR[0, YIndex].GetComponent<UnitInfo>()._value;
        portalUnit.transform.SetParent(_unitsHolder);

        // Identify destination for each Unit
        for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
        {
            targetPos[XIndex] = new Vector3(puzzleGen._unitARR[XIndex, YIndex].transform.position.x - puzzleGen._unitWidth,
                                            puzzleGen._unitARR[XIndex, YIndex].transform.position.y,
                                            puzzleGen._unitARR[XIndex, YIndex].transform.position.z);
        }

        if (unit.GetComponent<UnitInfo>()._XIndex == 0)
        {
            _unit = portalUnit;
        }

        while (unit.transform.position != targetPos[unit.GetComponent<UnitInfo>()._XIndex])
        {
            for (int XIndex = 0; XIndex < puzzleGen._columns; XIndex++)
            {
                puzzleGen._unitARR[XIndex, YIndex].transform.position = Vector3.MoveTowards(puzzleGen._unitARR[XIndex, YIndex].transform.position,
                                                                                           targetPos[XIndex],
                                                                                           Time.deltaTime * _shiftingSpeed * shiftingSpeedMultiplier);
            }
            portalUnit.transform.position = Vector3.MoveTowards(portalUnit.transform.position, 
                                                                portalUnitTargetPos, 
                                                                Time.deltaTime * _shiftingSpeed * shiftingSpeedMultiplier);

            highLightUnit(_unit);

            yield return new WaitForEndOfFrame();
        }

        Destroy(puzzleGen._unitARR[0, YIndex]);

        for (int XIndex = 0; XIndex < puzzleGen._columns - 1; XIndex++)
        {
            puzzleGen._unitARR[XIndex, YIndex] = puzzleGen._unitARR[XIndex + 1, YIndex];
            puzzleGen._unitARR[XIndex, YIndex].GetComponent<UnitInfo>()._XIndex -= 1;
        }
        puzzleGen._unitARR[puzzleGen._columns - 1, YIndex] = portalUnit;
        //puzzleGen._unitARR[puzzleGen._columns - 1, YIndex].GetComponent<UnitInfo>()._XIndex = portalUnit.GetComponent<UnitInfo>()._XIndex;
        //puzzleGen._unitARR[puzzleGen._columns - 1, YIndex].GetComponent<UnitInfo>()._value = portalUnit.GetComponent<UnitInfo>()._value;

        //_isMoving = false;
        GameStateController.currentState = GameStateController.gameState.idle;
    }


    // Shift all units in the same column as the choosen unit 1 distance up 
    IEnumerator shiftUp(GameObject unit, int shiftingSpeedMultiplier)
    {
        GameStateController.currentState = GameStateController.gameState.movingUnit;

        // Position of each units after shifting
        Vector3[] targetPos = new Vector3[puzzleGen._rows];

        // The index of shifting column
        int XIndex = unit.GetComponent<UnitInfo>()._XIndex;

        // Spawn Position of the unit that go through potar before shifting
        Vector3 portalUnitSpawnPos = new Vector3(puzzleGen._unitARR[XIndex, 0].transform.position.x,
                                                puzzleGen._unitARR[XIndex, 0].transform.position.y - puzzleGen._unitHeight,
                                                puzzleGen._unitARR[XIndex, 0].transform.position.z);

        // Position of the portal unit after shifting
        Vector3 portalUnitTargetPos = puzzleGen._unitARR[XIndex, 0].transform.position;

        // Instantiate the Unit that go through portal
        GameObject portalUnit = Instantiate(puzzleGen._unitPrefabsContainer[puzzleGen._unitARR[XIndex, puzzleGen._rows - 1].GetComponent<UnitInfo>()._value],
                                            portalUnitSpawnPos,
                                            Quaternion.identity) as GameObject;
        // Set value for the Unit that go through portal
        portalUnit.GetComponent<UnitInfo>()._XIndex = puzzleGen._unitARR[XIndex, 0].GetComponent<UnitInfo>()._XIndex;
        portalUnit.GetComponent<UnitInfo>()._YIndex = puzzleGen._unitARR[XIndex, 0].GetComponent<UnitInfo>()._YIndex;
        portalUnit.GetComponent<UnitInfo>()._value = puzzleGen._unitARR[XIndex, puzzleGen._rows - 1].GetComponent<UnitInfo>()._value;
        portalUnit.transform.SetParent(_unitsHolder);

        if (unit.GetComponent<UnitInfo>()._YIndex == puzzleGen._rows - 1)
        {
            _unit = portalUnit;
        }

        // Identify destination for each Unit
        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            targetPos[YIndex] = new Vector3(puzzleGen._unitARR[XIndex, YIndex].transform.position.x ,
                                            puzzleGen._unitARR[XIndex, YIndex].transform.position.y + puzzleGen._unitHeight,
                                            puzzleGen._unitARR[XIndex, YIndex].transform.position.z);
        }

        while (unit.transform.position != targetPos[unit.GetComponent<UnitInfo>()._YIndex])
        {
            for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
            {
                puzzleGen._unitARR[XIndex, YIndex].transform.position = Vector3.MoveTowards(puzzleGen._unitARR[XIndex, YIndex].transform.position,
                                                                                           targetPos[YIndex],
                                                                                           Time.deltaTime * _shiftingSpeed * shiftingSpeedMultiplier);
            }
            portalUnit.transform.position = Vector3.MoveTowards(portalUnit.transform.position,
                                                                portalUnitTargetPos, 
                                                                Time.deltaTime * _shiftingSpeed * shiftingSpeedMultiplier);

            highLightUnit(_unit);

            yield return new WaitForEndOfFrame();
        }

        Destroy(puzzleGen._unitARR[XIndex, puzzleGen._rows - 1]);

        for (int YIndex = puzzleGen._rows - 1; YIndex > 0; YIndex--)
        {
            puzzleGen._unitARR[XIndex, YIndex] = puzzleGen._unitARR[XIndex, YIndex - 1];
            puzzleGen._unitARR[XIndex, YIndex].GetComponent<UnitInfo>()._YIndex += 1;
        }
        puzzleGen._unitARR[XIndex, 0] = portalUnit;
        //puzzleGen._unitARR[XIndex, 0].GetComponent<UnitInfo>()._YIndex = portalUnit.GetComponent<UnitInfo>()._YIndex;
        //puzzleGen._unitARR[XIndex, 0].GetComponent<UnitInfo>()._value = portalUnit.GetComponent<UnitInfo>()._value;

        //_isMoving = false;
        GameStateController.currentState = GameStateController.gameState.idle;
    }

    // Shift all units in the same column as the choosen unit 1 distance down 
    IEnumerator shiftDown(GameObject unit, int shiftingSpeedMultiplier)
    {
        GameStateController.currentState = GameStateController.gameState.movingUnit;

        // Position of each units after shifting
        Vector3[] targetPos = new Vector3[puzzleGen._rows];

        // The index of shifting column
        int XIndex = unit.GetComponent<UnitInfo>()._XIndex;

        // Spawn Position of the unit that go through potar before shifting
        Vector3 portalUnitSpawnPos = new Vector3(puzzleGen._unitARR[XIndex, puzzleGen._rows - 1].transform.position.x,
                                                puzzleGen._unitARR[XIndex, puzzleGen._rows - 1].transform.position.y + puzzleGen._unitHeight,
                                                puzzleGen._unitARR[XIndex, puzzleGen._rows - 1].transform.position.z);

        // Position of the portal unit after shifting
        Vector3 portalUnitTargetPos = puzzleGen._unitARR[XIndex, puzzleGen._rows - 1].transform.position;

        // Instantiate the Unit that go through portal
        GameObject portalUnit = Instantiate(puzzleGen._unitPrefabsContainer[puzzleGen._unitARR[XIndex, 0].GetComponent<UnitInfo>()._value],
                                            portalUnitSpawnPos,
                                            Quaternion.identity) as GameObject;
        // Set value for the Unit that go through portal
        portalUnit.GetComponent<UnitInfo>()._XIndex = puzzleGen._unitARR[XIndex, puzzleGen._rows - 1].GetComponent<UnitInfo>()._XIndex;
        portalUnit.GetComponent<UnitInfo>()._YIndex = puzzleGen._unitARR[XIndex, puzzleGen._rows - 1].GetComponent<UnitInfo>()._YIndex;
        portalUnit.GetComponent<UnitInfo>()._value = puzzleGen._unitARR[XIndex, 0].GetComponent<UnitInfo>()._value;
        portalUnit.transform.SetParent(_unitsHolder);

        // Identify destination for each Unit
        for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
        {
            targetPos[YIndex] = new Vector3(puzzleGen._unitARR[XIndex, YIndex].transform.position.x,
                                            puzzleGen._unitARR[XIndex, YIndex].transform.position.y - puzzleGen._unitHeight,
                                            puzzleGen._unitARR[XIndex, YIndex].transform.position.z);
        }

        if (unit.GetComponent<UnitInfo>()._YIndex == 0)
        {
            _unit = portalUnit;
        }

        while (unit.transform.position != targetPos[unit.GetComponent<UnitInfo>()._YIndex])
        {
            for (int YIndex = 0; YIndex < puzzleGen._rows; YIndex++)
            {
                puzzleGen._unitARR[XIndex, YIndex].transform.position = Vector3.MoveTowards(puzzleGen._unitARR[XIndex, YIndex].transform.position,
                                                                                           targetPos[YIndex],
                                                                                           Time.deltaTime * _shiftingSpeed * shiftingSpeedMultiplier);
            }
            portalUnit.transform.position = Vector3.MoveTowards(portalUnit.transform.position,
                                                            portalUnitTargetPos,
                                                            Time.deltaTime * _shiftingSpeed * shiftingSpeedMultiplier);

            highLightUnit(_unit);

            yield return new WaitForEndOfFrame();
        }

        Destroy(puzzleGen._unitARR[XIndex, 0]);

        for (int YIndex = 0; YIndex < puzzleGen._rows - 1; YIndex++)
        {
            puzzleGen._unitARR[XIndex, YIndex] = puzzleGen._unitARR[XIndex, YIndex + 1];
            puzzleGen._unitARR[XIndex, YIndex].GetComponent<UnitInfo>()._YIndex -= 1;
        }
        puzzleGen._unitARR[XIndex, puzzleGen._rows - 1] = portalUnit;
        //puzzleGen._unitARR[XIndex, puzzleGen._rows - 1].GetComponent<UnitInfo>()._YIndex = portalUnit.GetComponent<UnitInfo>()._YIndex;
        //puzzleGen._unitARR[XIndex, puzzleGen._rows - 1].GetComponent<UnitInfo>()._value = portalUnit.GetComponent<UnitInfo>()._value;

        //_isMoving = false;
        GameStateController.currentState = GameStateController.gameState.idle;
    }

    #endregion
}
