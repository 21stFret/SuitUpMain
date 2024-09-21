using UnityEngine;
using TMPro;
using DG.Tweening;


public class CashCollector : MonoBehaviour
{
    [SerializeField] public TMP_Text alienParts;

    public static CashCollector instance;
    public GameObject panel;
    private bool UIshown;
    public float timeToHide = 2f;
    private float _timeToHide = 2f;
    public float posX;
    public float savedPos;
    public GameObject crawlerPartParent;

    public TMP_Text artifactParts;
    public GameObject Artpanel;
    private bool ArtUIshown;
    public float timeToHideA = 2f;
    private float _timeToHideA = 2f;

    private PlayerProgressManager playerProgressManager;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateUI(0);
        UpdateArtUI(0);
        savedPos = panel.transform.localPosition.x;
        playerProgressManager = PlayerProgressManager.instance;
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
        _timeToHide = timeToHide;
        UIshown = true;
        panel.transform.DOLocalMoveX(posX, 0.5f);
    }

    public void HideUI()
    {
        if (!UIshown)
        {
            return;
        }
        UIshown = false;
        panel.transform.DOLocalMoveX(savedPos, 0.5f);
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
        Artpanel.transform.DOLocalMoveX(posX, 0.5f);
    }

    public void HideArtUI()
    {
        if (!ArtUIshown)
        {
            return;
        }
        ArtUIshown = false;
        Artpanel.transform.DOLocalMoveX(savedPos, 0.5f);
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