using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataLogListUI : MonoBehaviour
{
    public TMP_Text logTitleText;
    public TMP_Text logDescriptionText;
    private string _description;
    public GameObject foundTick;
    public GameObject readTick;
    private Button _button;
    public Image icon;
    public LogManager _logManager;
    public string logID;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    public void SetInfo(string title, string description, bool isFound, bool beenRead, string _logID)
    {
        _description = description;
        logTitleText.text = title;
        foundTick.SetActive(isFound);
        readTick.SetActive(false);
        if (isFound)
        {
            readTick.SetActive(!beenRead);
        }
        logID = _logID;
    }

    public void OnClick()
    {
        if (string.IsNullOrEmpty(_description))
        {
            return;
        }
        if(foundTick.activeSelf == false)
        {
            return;
        }
        string fullText = logTitleText.text + "\n\n" + _description;

        logDescriptionText.text = fullText;
        icon.gameObject.SetActive(false);
        readTick.SetActive(false);
        _logManager.MarkLogAsRead(logID);
        if(icon.sprite != null)
        {
            icon.gameObject.SetActive(true);
        }
    }
}
