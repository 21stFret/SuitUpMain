using UnityEngine;
using TMPro;
using DG.Tweening;


public class CashCollector : MonoBehaviour
{
    [SerializeField] public TMP_Text alienParts;
    public static CashCollector Instance;
    public GameObject panel;
    private bool UIshown;
    public float timeToHide = 2f;
    private float _timeToHide = 2f;
    public float posX;
    public float savedPos;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI(0);
        savedPos = panel.transform.localPosition.x;
    }

    private void Update()
    {
        if(UIshown)
        {
            _timeToHide -= Time.deltaTime;
            if(_timeToHide <= 0)
            {
                HideUI();
            }
        }
    }

    public void AddCash(int amount)
    {
        GameManager.instance.cashCount += amount;
    }

    public void AddCrawlerPart()
    {
        GameManager.instance.crawlerParts += 1;
        ShowUI();
        UpdateUI(GameManager.instance.crawlerParts);
    }

    private void ShowUI()
    {
        if (UIshown)
        {
            _timeToHide = timeToHide;
            return;
        }
        _timeToHide = timeToHide;
        UIshown = true;
        panel.transform.DOLocalMoveX(posX, 0.5f);
    }

    private void HideUI()
    {
        if (!UIshown)
        {
            return;
        }
        UIshown = false;
        panel.transform.DOLocalMoveX(savedPos, 0.5f);
    }

    private void UpdateUI(int parts)
    {
        alienParts.text = parts.ToString();
        if(alienParts.transform.localScale.x > 2)
        {
            return;
        }
        alienParts.color = Color.green;
        alienParts.transform.DOPunchScale(new Vector3(1, 1, 1), 0.2f,5, 1).OnComplete(ResetScale);
    }

    private void ResetScale()
    {
        alienParts.color = Color.white;
        alienParts.transform.localScale = new Vector3(2,2,2);
    }
}