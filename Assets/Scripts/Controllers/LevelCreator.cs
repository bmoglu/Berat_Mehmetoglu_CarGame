using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{

    #region Public

    public GameObject targetPointPrefab;
    public GameObject startPointPrefab;
    public GameObject obstaclePrefab;

    public List<GameObject> targetGameObjects;
    public List<GameObject> startGameObjects;
    public List<GameObject> obstaclesGameObjects;

    public Level creatingLevel;
    
    public bool isEditingLevel;    // If a level is loaded and changed, the level will be updated.
    public bool isCreatingNewLevel; 

    #endregion

    #region Private

    private int _existsLevelNumber;
    private int _pointNum;

    #endregion
    
    private void OnValidate()
    {
        if (PlayerPrefs.HasKey(PrefKeys.ExistsLevelNumber))
        {
            _existsLevelNumber = PlayerPrefs.GetInt(PrefKeys.ExistsLevelNumber);
        }
        else
        {
            _existsLevelNumber = 2;    // Assume that the game developer delivered at least 2 levels.
        }
    }

    public void CreateNewLevel()
    {
        isEditingLevel = false;
        var lvlNo = GetNewLevelNumber();
        if (lvlNo != 0)
        {
            creatingLevel = ScriptableObject.CreateInstance<Level>();
            creatingLevel.levelNumber = lvlNo;
            creatingLevel.name = "Level_" + lvlNo;
            _pointNum = 0;
            
        }
    }

    private int GetNewLevelNumber()
    {
        var attemptNumber = 0;
        
        while (true)
        {
            var tempNo = _existsLevelNumber + 1;
            
            #if UNITY_EDITOR
            
            
            if (AssetDatabase.LoadAssetAtPath($"{PrefKeys.AssetsPath}Level_{tempNo}.asset", typeof(Level)) != null) //Level already exists.
            {
                _existsLevelNumber += 1;
                attemptNumber =+ 1;
            }
            else
            {
                PlayerPrefs.SetInt(PrefKeys.ExistsLevelNumber, tempNo);
                return tempNo;
            }
            
            #endif

            if (attemptNumber > 1000)
            {
                Debug.LogError("An error occurred creating a level, please contact a developer.");
                return 0;
            }
        }
    }


    public void SaveLevel()
    {
        if (!isEditingLevel)
        {
            startGameObjects.ForEach(go =>
                creatingLevel.startPoints.Add(new PointData(go.transform.position, go.transform.rotation)));

            targetGameObjects.ForEach(go =>
                creatingLevel.targetPoints.Add(new PointData(go.transform.position, go.transform.rotation)));

            obstaclesGameObjects.ForEach(go =>
                creatingLevel.obstacles.Add(new PointData(go.transform.position, go.transform.rotation)));

            #if UNITY_EDITOR
            AssetDatabase.CreateAsset(creatingLevel, $"{PrefKeys.AssetsPath}/{creatingLevel.name}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            PlayerPrefs.SetInt(PrefKeys.ExistsLevelNumber, creatingLevel.levelNumber);
            Debug.Log($"Level saved! File name: {creatingLevel.name} ");
            #endif
            creatingLevel = null;
        }
    }

    public void UpdateLevel()
    {
        if (isEditingLevel)
        {
            startGameObjects.ForEach(go =>
                creatingLevel.startPoints.Add(new PointData(go.transform.position, go.transform.rotation)));

            targetGameObjects.ForEach(go =>
                creatingLevel.targetPoints.Add(new PointData(go.transform.position, go.transform.rotation)));

            obstaclesGameObjects.ForEach(go =>
                creatingLevel.obstacles.Add(new PointData(go.transform.position, go.transform.rotation)));

#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Level updated! File name: {creatingLevel.name} ");
#endif
            creatingLevel = null;
            isEditingLevel = false;
        }
    }

    public void CreatePoints()
    {
        // Entrance Point
        var start = Instantiate(startPointPrefab, new Vector3(0,1,0), Quaternion.identity);
        start.name = $"Entrance_{_pointNum}";
        startGameObjects.Add(start);

        // Exit Point
        var target = Instantiate(targetPointPrefab, new Vector3(0,1,0), Quaternion.identity);
        target.name = $"Target_{_pointNum}";
        targetGameObjects.Add(target);

        _pointNum++;
    }
    
    public void CreateObstacles()
    {
        obstaclesGameObjects.Add(Instantiate(obstaclePrefab, new Vector3(0,1.5f,0f), Quaternion.identity));
    }

    public void LoadLevelObject()
    {
        isEditingLevel = true;
        
        // A level can be drag and drop to creating level. It means that the level will be update.
        if (creatingLevel != null)
        {
            Debug.Log("Loading...");
            var i = 0;
            // Load and Create Objects
            creatingLevel.obstacles.ForEach((pointData =>
            {
                GameObject obj = Instantiate(obstaclePrefab, pointData.position, pointData.rotation);
                obj.name = $"Obstacle_{i}";
                i++;
                obstaclesGameObjects.Add(obj);
            }));
            
            i = 0;
            creatingLevel.startPoints.ForEach((pointData =>
            {
                GameObject obj = Instantiate(startPointPrefab, pointData.position, pointData.rotation);
                obj.name = $"Entrance_{i}";
                i++;
                startGameObjects.Add(obj);
            }));

            i = 0;
            creatingLevel.targetPoints.ForEach((pointData =>
            {
                GameObject obj = Instantiate(targetPointPrefab, pointData.position, pointData.rotation);
                obj.name = $"Exit_{i}";
                i++;
                targetGameObjects.Add(obj);
            }));

            Debug.Log("Loaded.");

            //Clear the list of the loaded levels to avoid confusion when an attempt to update level
            creatingLevel.obstacles.Clear();
            creatingLevel.startPoints.Clear();
            creatingLevel.targetPoints.Clear();
        }
    }

    public void Clear()
    {
        targetGameObjects.RemoveAll(o => { DestroyImmediate(o); return true; });
        startGameObjects.RemoveAll(o => { DestroyImmediate(o); return true; });
        obstaclesGameObjects.RemoveAll(o => { DestroyImmediate(o); return true; });

        isEditingLevel = false;
        creatingLevel = null; 
        _pointNum = 0;
    }


}
