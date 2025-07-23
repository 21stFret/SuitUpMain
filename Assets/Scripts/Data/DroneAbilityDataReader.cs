using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DroneAbilityDataReader : MonoBehaviour
{
    public void LoadFromExcel(DroneAbilityManager droneAbilityManager)
    {
        // Read the CSV file
        List<Dictionary<string, object>> data = CSVReader.Read("Suit Up Data - DroneAbilites");
        
        // The first column should be "Name" which contains row labels
        string nameColumn = "Name";
        
        // Get all ability columns (skip the Name column)
        List<string> abilityColumns = new List<string>(data[0].Keys);
        abilityColumns.Remove(nameColumn);

        // Extract information from the rows based on the "Name" column values
        // We need to find the specific rows for description, charge description, etc.

        // Process each ability column
        for (int abilityIndex = 0; abilityIndex < abilityColumns.Count; abilityIndex++)
        {
            string abilityColumn = abilityColumns[abilityIndex];
            DroneAbility ability = new DroneAbility();

            // Set ability name
            ability.abilityName = abilityColumn;
            ability.droneAbilityID = abilityIndex; // Assuming ID is the index
            ability.chargeInts = new int[3]; // Assuming there are 3 charge levels

            // For each row, check the "Name" column to determine what data it contains
            for (int rowIndex = 0; rowIndex < data.Count; rowIndex++)
            {
                string rowName = data[rowIndex][nameColumn].ToString();

                // Based on the row name, assign the appropriate property
                switch (rowName)
                {
                    case "description":
                        ability.abilityDescription = data[rowIndex][abilityColumn].ToString();
                        break;
                    case "chargeDescription":
                        ability.abilityChargeDescription = data[rowIndex][abilityColumn].ToString();
                        break;
                    case "charge1":
                        if (int.TryParse(data[rowIndex][abilityColumn].ToString(), out int charge1))
                        {
                            ability.chargeInts[0] = charge1;
                        }
                        break;
                    case "charge2":
                        if (int.TryParse(data[rowIndex][abilityColumn].ToString(), out int charge2))
                        {
                            ability.chargeInts[1] = charge2;
                        }
                        break;
                    case "charge3":
                        if (int.TryParse(data[rowIndex][abilityColumn].ToString(), out int charge3))
                        {
                            ability.chargeInts[2] = charge3;
                        }
                        break;
                    case "cost":
                        if (int.TryParse(data[rowIndex][abilityColumn].ToString(), out int cost))
                        {
                            ability.cost = cost;
                        }
                        break;
                        // Add more cases if there are other row types
                }
            }
            // Add the ability to the list in the DroneAbilityManager
            ability.unlocked = PlayerSavedData.instance._droneAb[abilityIndex]==0 ? true : false;
            droneAbilityManager._droneAbilities.Add(ability);
        }
    }
}