using System.Collections.Generic;
using System.Linq; // LINQ is used for Select() method
using TMPro;
using UnityEngine;

public class OriginDropdownMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown targetDataDropdown; // Dropdown menu for debug

    private List<Target> targetList; // Change to List<Target>
    private string _currentSelectedLocation;

    // Start is called before the first frame update
    void Start()
    {
        // Instantiate TargetHandler
        TargetHandler targetHandler = FindObjectOfType<TargetHandler>();
        if (targetHandler != null)
        {
            // Access the targetList through TargetHandler
            targetList = targetHandler.GetTargetList();
            FillDropdownWithTargetItems();
            
        }
        else
        {
            Debug.LogError("TargetHandler not found in the scene.");
        }
    }

    private void FillDropdownWithTargetItems()
    {
        if (targetList == null || targetList.Count == 0)
        {
            Debug.LogError("TargetList is null or empty.");
            return;
        }

        // Check for null elements in the targetList
        foreach (var target in targetList)
        {
            if (target == null)
            {
                Debug.LogError("Found null element in targetList.");
                return;
            }
        }

        List<TMP_Dropdown.OptionData> targetOptionData =
          targetList.Select(x => new TMP_Dropdown.OptionData
          {
              text = $"{x.Name}"
          }).ToList();

        targetDataDropdown.ClearOptions();
        targetDataDropdown.AddOptions(targetOptionData);

        // Assign the onValueChanged listener
        targetDataDropdown.onValueChanged.RemoveAllListeners(); // Remove any existing listeners
        targetDataDropdown.onValueChanged.AddListener(OnDropdownItemSelected);
    }

    /// <summary>
    /// Handles the selection of an item from the dropdown.
    /// </summary>
    /// <param name="index">The index of the selected item.</param>
    private void OnDropdownItemSelected(int index)
    {
        if (index >= 0 && index < targetList.Count)
        {
            Target selectedTarget = targetList[index];
            _currentSelectedLocation = selectedTarget.Name;
            MySceneManager.OriginLocationName = _currentSelectedLocation; // Store destination location name
            MySceneManager.LoadScene("ARNavigation");
        }
    }
}
