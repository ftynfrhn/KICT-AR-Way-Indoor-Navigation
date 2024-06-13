using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetHandler : MonoBehaviour
{
    private List<Target> currentTargetItems = new List<Target>();

    void Start()
    {
        // Load the data initially in Start
        currentTargetItems = LoadTargetData().ToList();
        Debug.Log($"Number of elements in currentTargetItems after Start(): {currentTargetItems.Count}");
    }

    /// <summary>
    /// Loads target data from a JSON file in the Resources folder.
    /// </summary>
    /// <returns>An array of Target objects.</returns>
    private Target[] LoadTargetData()
    {
        TextAsset targetModelData = Resources.Load<TextAsset>("TargetData"); // Load JSON from Resources folder

        if (targetModelData != null)
        {
            Debug.Log("TargetData found in TargetHandler and loaded successfully.");
            TargetWrapper targetWrapper = JsonUtility.FromJson<TargetWrapper>(targetModelData.text);
            return targetWrapper.TargetList.ToArray(); // Convert List<Target> to array
        }
        else
        {
            Debug.LogError("TargetData not found in Resources.");
            return new Target[0]; // Return an empty array to avoid null reference issues
        }
    }

    /// <summary>
    /// Gets the list of current target items.
    /// </summary>
    /// <returns>A list of Target objects.</returns>
    public List<Target> GetTargetList()
    {
        // Check if currentTargetItems is empty and reload if necessary
        //if (currentTargetItems == null || currentTargetItems.Count == 0)
        //{
        //    currentTargetItems = LoadTargetData().ToList();
        //}

        Debug.Log($"Number of elements in currentTargetItems in GetTargetList(): {currentTargetItems.Count}");
        return currentTargetItems;
    }

    /// <summary>
    /// Finds and returns the target corresponding to the provided target name.
    /// </summary>
    /// <param name="targetText">The name of the target.</param>
    /// <returns>The Target object with the matching name, or null if not found.</returns>
    public Target GetCurrentTargetByTargetText(string targetText)
    {
        Debug.Log($"Looking for target with name: {targetText}");
        return currentTargetItems.Find(x => x.Name.ToLower().Equals(targetText.ToLower()));
    }
}