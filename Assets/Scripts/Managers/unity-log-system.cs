using UnityEngine;
using System;
using System.Collections.Generic;

// ScriptableObject to store all available logs
[CreateAssetMenu(fileName = "LogDatabase", menuName = "Game/Log Database")]
public class LogDatabase : ScriptableObject
{
    public List<LogEntry> allLogs = new List<LogEntry>();

    // Helper method to add a new log
    public void AddLog(string id, string title, string content, string category)
    {
        var log = new LogEntry
        {
            id = id,
            title = title,
            content = content,
            category = category
        };
        allLogs.Add(log);
    }

    // Helper method to get logs by category
    public List<LogEntry> GetLogsByCategory(string category)
    {
        return allLogs.FindAll(log => log.category == category);
    }
}