using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region SerializeField

    [SerializeField] private Text currentCarNoText;
    [SerializeField] private Text levelNoText;
    [SerializeField] private GameObject finishText;
    [SerializeField] private GameObject levelUpText;

    #endregion
    
    //Set car text
    public void SetCarText(int carNo, int totalCarNum)
    {
        currentCarNoText.text = carNo + "/" + totalCarNum;
    }

    //Set level text
    public void SetLevelText(int levelNo)
    {
        levelNoText.text = levelNo.ToString();
    }

    //Show finish text
    public void ShowFinishText()
    {
        finishText.gameObject.SetActive(true);
    }

    //Show level up text
    public void ShowLevelUp()
    {
        levelUpText.SetActive(true);
    }
    
}
