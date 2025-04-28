using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryDataReader : MonoBehaviour
{
    // Reference to the LogDatabase that will store the parsed logs
    public LogDatabase logDatabase;

    private void Awake()
    {
        // Load data if logDatabase is assigned
        if (logDatabase != null)
        {
            LoadFromCsv();
        }
        else
        {
            Debug.LogError("LogDatabase reference is missing in StoryDataReader");
        }
    }

    public void LoadFromCsv()
    {
        // Read the CSV file
        List<Dictionary<string, object>> data = CSVReader.Read("Suit Up Data - Story");
        
        // Clear any existing logs
        logDatabase.allLogs.Clear();
        
        // Process each row in the CSV
        foreach (var row in data)
        {
            // Create a new LogEntry
            LogEntry log = new LogEntry();
            
            // Parse the ID and convert to string for the entry ID
            if (row.ContainsKey("ID") && row["ID"] != null)
            {
                log.id = row["ID"].ToString();
            }
            
            // Set the title
            if (row.ContainsKey("Title") && row["Title"] != null)
            {
                log.title = row["Title"].ToString();
            }
            
            // Set the content
            if (row.ContainsKey("Content") && row["Content"] != null)
            {
                log.content = row["Content"].ToString();
            }
            
            // Set the category
            if (row.ContainsKey("Category") && row["Category"] != null)
            {
                log.category = row["Category"].ToString();
            }
            
            // Add the log to the database
            logDatabase.allLogs.Add(log);
        }
        
        Debug.Log($"Loaded {logDatabase.allLogs.Count} story logs from CSV");
    }
}
