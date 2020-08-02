using System.Collections.Generic;
using Cars;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    #region Enum

    public enum LevelState
    {
        Preparing,
        Playing,
        Fail,
        Success,
        WaitInput,
        Completed
    }

    #endregion

    #region Public
    
    public float carForwardSpeed = 100;
    public float carRotateSpeed = 1;

    #endregion

    #region Property

    public LevelState LevelStateProp { get; private set; }

    #endregion

    #region Private

    private GameObject _currentCar;
    private ObjectPool _objectPool;
    private CarController _currentCarController;

    private List<GameObject> _oldOnes;
    
    private int _currentCarNumber;

    #endregion
    
    #region Singleton

    public static LevelController Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
        
        //Access objectSpawer
        _objectPool = GetComponent<ObjectPool>();
    }

    #endregion
    
    private void OnValidate()
    {
        //Is it null?
        if (_currentCarController == null) return;
        
        //Set speed
        _currentCarController.speed = carForwardSpeed;
        //Set rotation
        _currentCarController.rotationSpeed = carRotateSpeed;
    }
    
    public void LoadLevel(Level level)
    {
        //Is parameter null or equal 0?
        if (level == null || level.startPoints.Count == 0) return;

        //Set level state
        SetLevelState(LevelState.Preparing);
        
        _objectPool.LoadCarSpawnPoints(level);
        
        //Set first car number
        _currentCarNumber = 1; 
        
        //Create list for trace car steps
        _oldOnes = new List<GameObject>();
        
        //Made playable
        PrepareCar();
    }
    
    private void Update()
    {
        //Which level state ?
        switch (LevelStateProp)
        {
            case LevelState.WaitInput:
                
                //Execute any input
                if (Input.anyKey)
                {
                    //Set level state by playing
                    SetLevelState(LevelState.Playing);
                }
                break;

            case LevelState.Success:
                //Set state
                SetLevelState(LevelState.Preparing);
                
                //Set car state
                _currentCarController.UpdateCarState(CarState.Success);
                
                //Get new playable car
                GetNextCar();
                
                //Locate old cars
                RelocateOldOnes();
                break;

            case LevelState.Fail:
                //Set state
                SetLevelState(LevelState.Preparing);
                
                //Set car state
                _currentCarController.UpdateCarState(CarState.Fail);
                
                //Reload same car
                ReloadCurrentCar();
                
                //Reload same old cars
                RelocateOldOnes();
                break;
        }
    }

    //Car preparation
    private void PrepareCar()
    {
        //Set car and create
        SetCurrentCar(_objectPool.CreateCar(_currentCarNumber));
        
        //Set car number text
        GameController.Instance.SetCarNumberText(_currentCarNumber);
    
        //Set level state
        SetLevelState(LevelState.WaitInput);
    }

    //Get new car
    private void GetNextCar()
    {
        if (!IsLevelCompleted())
        {
            //Make old one
            _currentCarController.MakeOldOne();
            //Add to oldOnes list
            _oldOnes.Add(_currentCar);
            //Car number ++
            _currentCarNumber += 1;
            //Set new car and create
            SetCurrentCar(_objectPool.CreateCar(_currentCarNumber));
            //Set text for the new car
            GameController.Instance.SetCarNumberText(_currentCarNumber);
            //Set level state
            SetLevelState(LevelState.WaitInput);
        }
        else
        {
            //Set level state
            SetLevelState(LevelState.Completed);
            //Execute next level
            GameController.Instance.LoadNextLevel();
        }
    }

    //Reload car
    private void ReloadCurrentCar()
    {
        _currentCarController.Replay();
        
        SetLevelState(LevelState.WaitInput);
    }

    //Located the correct position old cars 
    private void RelocateOldOnes()
    {
        foreach (var car in _oldOnes)
        {
            car.GetComponent<CarController>().Relocate();
        }
    }
    
    //Destroy cars for the next level
    public void DestroyCars()
    {
        foreach (var car in _oldOnes)
        {
            Destroy(car);
        }
        Destroy(_currentCar);
    }

    
    private bool IsLevelCompleted()
    {
        //if currentCarNumber equal to numberOfCars
        return _currentCarNumber == GameController.Instance.numberOfCars;
    }

    //Set current car 
    private void SetCurrentCar(GameObject car)
    {
        //Set car and access CarController
        _currentCar = car;
        _currentCarController = _currentCar.GetComponent<CarController>();
    }

    //For direction 
    public void StartDriveRotating(int direction)
    {
        _currentCarController.CarRotateDirection = direction;
    }

    //Set level state
    public void SetLevelState(LevelState state)
    {
        LevelStateProp = state;
    }
    
}