using UnityEngine;
using TMPro;
using DG.Tweening;


public class CashCollector : MonoBehaviour
{
    [SerializeField] private int currentCash;
    public int cash { get { return currentCash; } }
    [SerializeField] public TMP_Text cashText;
    public static CashCollector Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddCash(int amount)
    {
        currentCash += amount;
        UpdateUI();
        SaveCash();
    }

    public void ResetCash()
    {
        currentCash = 0;
        UpdateUI();
        SaveCash();
    }

    public void SaveCash()
    {         
        PlayerSavedData.instance._playerCash = currentCash;
    }

    private void UpdateUI()
    {
        cashText.text = "$" + currentCash.ToString();
        if(cashText.transform.localScale.x > 2)
        {
            return;
        }
        cashText.transform.DOPunchScale(new Vector3(1, 1, 1), 0.2f,5, 1).OnComplete(ResetScale);
    }

    private void ResetScale()
    {
        cashText.transform.localScale = new Vector3(2,2,2);
    }
}