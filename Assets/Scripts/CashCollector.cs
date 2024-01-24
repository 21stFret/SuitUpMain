using UnityEngine;
using TMPro;
using DG.Tweening;


public class CashCollector : MonoBehaviour
{
    [SerializeField] private float currentCash;
    public float cash { get { return currentCash; } }
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

    public void AddCash(float amount)
    {
        currentCash += amount;
        UpdateUI();
    }

    public void ResetCash()
    {
        currentCash = 0f;
        UpdateUI();
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