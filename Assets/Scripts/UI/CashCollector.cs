using UnityEngine;
using TMPro;
using DG.Tweening;


public class CashCollector : MonoBehaviour
{
    [SerializeField] public TMP_Text cashText;
    public static CashCollector Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI(PlayerSavedData.instance._Cash);
    }

    public void AddCash(int amount)
    {
        GameManager.instance.cashCount += amount;

        UpdateUI(GameManager.instance.cashCount);
    }


    private void UpdateUI(int cash)
    {
        cashText.text = "$" + cash.ToString();
        if(cashText.transform.localScale.x > 2)
        {
            return;
        }
        cashText.color = Color.green;
        cashText.transform.DOPunchScale(new Vector3(1, 1, 1), 0.2f,5, 1).OnComplete(ResetScale);
    }

    private void ResetScale()
    {
        cashText.color = Color.white;
        cashText.transform.localScale = new Vector3(2,2,2);
    }
}