using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Example UI Manager for displaying logs
public class LogUIManager : MonoBehaviour
{
    public LogManager logManager;
    public List<DataLogListUI> logListEntries = new List<DataLogListUI>();
    public Transform logListContainer;

    public void PopulateLogList(List<LogEntry> logs)
    {
        logListEntries.Clear();

        foreach (DataLogListUI child in logListContainer.GetComponentsInChildren<DataLogListUI>())
        {
            logListEntries.Add(child);
        }

        for (int i = 0; i < logs.Count; i++)
        {
            if (i < logListEntries.Count)
            {
                logListEntries[i]._logManager = logManager; // Assign the log manager to each entry
                logListEntries[i].gameObject.SetActive(true);
                logListEntries[i].SetInfo(logs[i].title, logs[i].content, logManager.IsLogDiscovered(logs[i].id), logManager.IsLogRead(logs[i].id), logs[i].id);
            }
        }
    }
}
