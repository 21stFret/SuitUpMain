using UnityEngine;
using TMPro;
using DG.Tweening;


public class CashCollector : MonoBehaviour
{
    [SerializeField] public TMP_Text alienParts;

    public static CashCollector instance;
    public RectTransform panelTrans;
    private bool UIshown;
    public float timeToHide = 2f;
    private float _timeToHide = 2f;
    public float posX;
    public float savedPos;
    public GameObject crawlerPartParent;

    public TMP_Text artifactParts;
    public RectTransform Artpanel;
    private bool ArtUIshown;
    public float timeToHideA = 2f;
    private float _timeToHideA = 2f;

    private PlayerProgressManager playerProgressManager;

    private bool autoHide;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateUI(0);
        UpdateArtUI(0);
        savedPos = panelTrans.anchoredPosition.x;
        playerProgressManager = PlayerProgressManager.instance;
    }

    private void Update()
    {

        if(UIshown)
        {
            if (autoHide)
            {
                _timeToHide -= Time.deltaTime;
                if (_timeToHide <= 0)
                {
                    HideUI();
                }
            }
        }

        if(ArtUIshown)
        {
            _timeToHideA -= Time.deltaTime;
            if(_timeToHideA <= 0)
            {
                HideArtUI();
            }
        }
    }

    public void AddCash(int amount)
    {
        playerProgressManager.cashCount += amount;
    }

    public void AddCrawlerPart(int amount)
    {
        playerProgressManager.crawlerParts += amount;
        PlayerSavedData.instance._stats.totalParts += amount;
        ShowUI();
        UpdateUI(PlayerProgressManager.instance.crawlerParts);
    }

    public void AddArtifact(int amount)
    {
        playerProgressManager.artifactCount += amount;
        ShowArtUI();
        UpdateArtUI(playerProgressManager.artifactCount);
    }

    public void DestroyParts()
    {
        foreach (Transform child in crawlerPartParent.transform)
        {
            Destroy(child.gameObject);
        }
    }


    public void ShowUI()
    {
        if (UIshown)
        {
            _timeToHide = timeToHide;
            return;
        }
        autoHide = true;
        _timeToHide = timeToHide;
        UIshown = true;
        DOVirtual.Float(panelTrans.anchoredPosition.x, posX, 0.5f, (float value) => panelTrans.anchoredPosition = new Vector2(value, panelTrans.anchoredPosition.y));
    }

    public void SetUI()
    {
        autoHide = false;
        UIshown = true;
        panelTrans.anchoredPosition = new Vector2(posX, panelTrans.anchoredPosition.y);
    }

    public void HideUI()
    {
        if (!UIshown)
        {
            return;
        }
        UIshown = false;
        DOVirtual.Float(panelTrans.anchoredPosition.x, savedPos, 0.5f, (float value) => panelTrans.anchoredPosition = new Vector2(value, panelTrans.anchoredPosition.y));
    }

    public void ShowArtUI()
    {
        if (ArtUIshown)
        {
            _timeToHideA = timeToHideA;
            return;
        }
        _timeToHideA = timeToHideA;
        ArtUIshown = true;
        DOVirtual.Float(Artpanel.anchoredPosition.x, posX, 0.5f, (float value) => Artpanel.anchoredPosition = new Vector2(value, Artpanel.anchoredPosition.y));
        Artpanel.GetComponentInChildren<ParticleSystem>().Play();
    }

    public void HideArtUI()
    {
        if (!ArtUIshown)
        {
            return;
        }
        ArtUIshown = false;
        DOVirtual.Float(Artpanel.anchoredPosition.x, savedPos, 0.5f, (float value) => Artpanel.anchoredPosition = new Vector2(value, Artpanel.anchoredPosition.y));
        Artpanel.GetComponentInChildren<ParticleSystem>().Stop();
    }

    private void UpdateUI(int parts)
    {
        alienParts.text = parts.ToString();
        if(alienParts.transform.localScale.x > 2)
        {
            return;
        }
        alienParts.color = Color.green;
        alienParts.transform.DOPunchScale(new Vector3(2, 2, 2), 0.2f,5, 1).OnComplete(ResetScale);
    }

    private void UpdateArtUI(int parts)
    {
        artifactParts.text = parts.ToString();
    }

    private void ResetScale()
    {
        alienParts.color = Color.white;
        alienParts.transform.localScale = new Vector3(1,1,1);
    }
}