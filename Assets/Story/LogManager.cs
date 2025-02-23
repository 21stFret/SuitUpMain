using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manager class to handle log discovery and viewing
public class LogManager : MonoBehaviour
{
    public LogDatabase logDatabase;
    public GameObject logPrefab;
    public SequenceInputController sequenceInputController;
    public LogUIManager logUIManager;
    public LogEntry currentLog;
    public DoTweenFade doTweenFade;

    // Set of discovered log IDs (Serialized to save progress)
    [SerializeField]
    private HashSet<string> discoveredLogs = new HashSet<string>();

    // Dictionary to cache logs by category for quick access
    private Dictionary<string, List<LogEntry>> logsByCategory = new Dictionary<string, List<LogEntry>>();

    private void Awake()
    {
        // Initialize category cache
        foreach (var log in logDatabase.allLogs)
        {
            if (!logsByCategory.ContainsKey(log.category))
            {
                logsByCategory[log.category] = new List<LogEntry>();
            }
            logsByCategory[log.category].Add(log);
        }

        sequenceInputController.OnSequenceComplete +=  () => DiscoverLog(currentLog.id);
        sequenceInputController.OnSequenceFailed += () => HideSequencer();

        LoadDiscoveredLogs();
        //Testing
        GenerateGOLog();
    }

    // Create a new log GameObject in the scene
    public void GenerateGOLog()
    {
        logPrefab.SetActive(true);
        currentLog = GetRandomUndiscoverdEntry();
        // logPrefab.entry = GetRandomUndiscoverdEntry();
    }

    public void ShowSequencer()
    {
        doTweenFade.FadeIn();
        sequenceInputController.enabled = true;
        BattleMech.instance.playerInput.SwitchCurrentActionMap("UI");
        sequenceInputController.gameObject.SetActive(true);
        sequenceInputController.StartNewSequence();
    }

    public void HideSequencer()
    {
        sequenceInputController.enabled = false;
        doTweenFade.FadeOut();
    }


    // Mark a log as discovered
    public void DiscoverLog(string logId)
    {
        if (!discoveredLogs.Contains(logId))
        {
            discoveredLogs.Add(logId);
            // You might want to trigger events here for UI updates
            OnLogDiscovered?.Invoke(logId);
            print("Log Discovered: " + logId);
            SaveDiscoveredLogs();
            BattleMech.instance.myCharacterController.ToggleCanMove(true);
            BattleMech.instance.playerInput.SwitchCurrentActionMap("Gameplay");
            logPrefab.SetActive(false);
            HideSequencer();
        }
    }

    // Check if a log has been discovered
    public bool IsLogDiscovered(string logId)
    {
        return discoveredLogs.Contains(logId);
    }

    // Get all discovered logs in a specific category
    public List<LogEntry> GetDiscoveredLogsByCategory(string category)
    {
        if (!logsByCategory.ContainsKey(category))
            return new List<LogEntry>();

        return logsByCategory[category].FindAll(log => discoveredLogs.Contains(log.id));
    }

    public LogEntry GetRandomUndiscoverdEntry()
    {
        List<LogEntry> undiscoveredLogs = new List<LogEntry>();
        foreach (var log in logDatabase.allLogs)
        {
            if (!discoveredLogs.Contains(log.id))
            {
                undiscoveredLogs.Add(log);
            }
        }

        if (undiscoveredLogs.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, undiscoveredLogs.Count);
            return undiscoveredLogs[randomIndex];
        }

        return null;
    }

    // Get all categories that have at least one discovered log
    public List<string> GetActiveCategories()
    {
        var activeCategories = new List<string>();
        foreach (var category in logsByCategory.Keys)
        {
            if (GetDiscoveredLogsByCategory(category).Count > 0)
            {
                activeCategories.Add(category);
            }
        }
        return activeCategories;
    }

    // Event for when a new log is discovered
    public event Action<string> OnLogDiscovered;

    public void SaveDiscoveredLogs()
    {
        string logsString = string.Join(",", discoveredLogs);
        PlayerPrefs.SetString("DiscoveredLogs", logsString);
        PlayerPrefs.Save();
    }

    // Load discovered logs
    private void LoadDiscoveredLogs()
    {
        string logsString = PlayerPrefs.GetString("DiscoveredLogs", "");
        if (!string.IsNullOrEmpty(logsString))
        {
            string[] logIds = logsString.Split(',');
            discoveredLogs = new HashSet<string>(logIds);
        }
    }
}
