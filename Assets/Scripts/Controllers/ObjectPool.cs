using System.Collections.Generic;
using Cars;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region SerializeField

    [SerializeField] private GameObject carPrefab;
    [SerializeField] private GameObject startPointPrefab;
    [SerializeField] private GameObject targetPointPrefab;
    [SerializeField] private GameObject obstaclePrefab;
    
    [SerializeField] private Transform carParent;
    [SerializeField] private Transform obstacleParent;

    #endregion

    #region HideInInspector

    [HideInInspector] public List<GameObject> obstacles;

    #endregion

    #region Private

    private GameObject _currentEntrance;
    private GameObject _currentExit;
    private Level _level;

    #endregion
    
    public void LoadCarSpawnPoints(Level level)
    {
        //Is level null ? then execute 
        if (_level != null) ClearOldLevelData();
        
        //Set _level object with parameter
        _level = level;
        
        //Crete obstacle
        CreateObstacle();

        //Create for initialize
        InitialCreatePoint();
    }

    //Clear old obstacles , points
    private void ClearOldLevelData()
    {
        //Dont show old points
        CloseOldPoints();
        
        //Clear obstacle list
        obstacles.RemoveAll(o =>
        {
            Destroy(o);
            return true;
        });
        
        //Set level null
        _level = null;
    }

    //Creation of Car
    public GameObject CreateCar(int carId)
    {
        //Get index value for about car
        var index = carId - 1;
        
        //Is index equal or big then execute
        if (index - 1 >= 0) CloseOldPoints();
        
        //Get index and show new points
        ShowCurrentPoints(index);

        //Create car
        var car = Instantiate(carPrefab, _level.startPoints[index].position,
            _level.startPoints[index].rotation,carParent);
        
        //Set car info
        car.GetComponent<CarController>().SetCar(new Car(carId, CarType.Current));
        
        //Return car object
        return car;
    }

    //Creation of Obstacles
    private void CreateObstacle()
    {
        //According to certain level, each obstacle add to obstacle list and adjust position and rotation
        _level.obstacles.ForEach(pointData =>
        {
            obstacles.Add(Instantiate(obstaclePrefab, pointData.position, pointData.rotation,obstacleParent));
        });
    }
    
    //Show points by index  
    private void ShowCurrentPoints(int index)
    {
        //Set position and rotation by index
        _currentEntrance.transform.position = _level.startPoints[index].position;
        _currentEntrance.transform.rotation = _level.startPoints[index].rotation;
        _currentEntrance.SetActive(true);
        
        //Set position and rotation by index
        _currentExit.transform.position = _level.targetPoints[index].position;
        _currentExit.transform.rotation = _level.targetPoints[index].rotation;
        _currentExit.SetActive(true);
    }
    
    //Dont show old points
    private void CloseOldPoints()
    {
        _currentEntrance.SetActive(false);
        _currentExit.SetActive(false);
    }

    private void InitialCreatePoint()
    {
        //Crete but dont show
        _currentEntrance = Instantiate(startPointPrefab);
        _currentEntrance.SetActive(false);
        
        //Create but dont show
        _currentExit = Instantiate(targetPointPrefab);
        _currentExit.SetActive(false);
    }
    
}
