using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] public List<Level> level;
    
    #region Public

    public int numberOfCars = 8;
    public int currentLevel = 1;
    public int maxLevel = 2;

    #endregion

    #region Private

    private UIController _uiController;

    #endregion
    
    #region Singleton

    public static GameController Instance;
    
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
    }

    #endregion
    
    private void Start()
    {
        //Access UIController
        _uiController = GetComponent<UIController>();

        //Read level
        LevelController.Instance.LoadLevel(ReadCurrentLevel());
        
        //Set level number text
        SetLevelNumberText();
    }

    private Level ReadCurrentLevel()
    {
        //Is key exist?
        if (!PlayerPrefs.HasKey(PrefKeys.UnlockLevel))
        {
            //Set by 1
            PlayerPrefs.SetInt(PrefKeys.UnlockLevel,1);
        }
        
        //Return level
        return level[currentLevel-1];
    }
    
    
    public void SetCarNumberText(int carNo)
    {
        //Set text
        _uiController.SetCarText(carNo,numberOfCars);
    }

    private void SetLevelNumberText()
    {
        //Set text
        _uiController.SetLevelText(currentLevel);
    }

    public void LoadNextLevel()
    {
        if (currentLevel + 1 <= maxLevel)
        {
            //Increase current level by 1
            currentLevel += 1;
            
            //Destroy all cars for new level
            LevelController.Instance.DestroyCars();
            
            //Set unlocked level
            PlayerPrefs.SetInt(PrefKeys.UnlockLevel, currentLevel);
            
            //UI stuff
            _uiController.SetLevelText(currentLevel);
            _uiController.SetCarText(1, numberOfCars);
            _uiController.ShowLevelUp();
            
            //Load next level
            LevelController.Instance.LoadLevel(ReadCurrentLevel());
        }
        else
        {
            //Show the finish text
            _uiController.ShowFinishText();
        }
    }

}