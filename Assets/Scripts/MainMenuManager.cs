using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject listItemPrefab;
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private GameObject infoPanel;

    private List<Target> targetList;
    private string _currentSelectedLocation;

    void Start()
    {
        StartCoroutine(InitializeTargets());
    }

    private IEnumerator InitializeTargets()
    {
        // Find and instantiate TargetHandler
        TargetHandler targetHandler = FindObjectOfType<TargetHandler>();
        if (targetHandler != null)
        {
            // Wait a frame to ensure TargetHandler's Start method has run
            yield return null;

            // Access the target list through TargetHandler
            targetList = targetHandler.GetTargetList();
            while (targetList.Count == 0)
            {
                yield return new WaitForSeconds(0.1f); // Add a small delay to allow loading
                targetList = targetHandler.GetTargetList();
            }

            Debug.Log($"Number of elements in targetList is: {targetList.Count}");
            PopulateScrollView();
        }
        else
        {
            Debug.LogError("TargetHandler not found in the scene.");
        }
    }

    /// <summary>
    /// Populates the scroll view with list items based on the target list.
    /// </summary>
    void PopulateScrollView()
    {
        float itemHeight = 40f; // Height of each list item ori:50
        float xPosition = 90f; // Desired x position for each button

        // Start the loop from index 1 to skip the first item
        for (int i = 1; i < targetList.Count; i++)
        {
            Target target = targetList[i];
            GameObject listItem = Instantiate(listItemPrefab, scrollViewContent);
            listItem.GetComponentInChildren<TMP_Text>().text = target.Name;
            float yPosition = -(i - 1) * itemHeight;
            // Calculate the Y position based on index, adjusted for skipping the first item

            // Set the position of the list item
            listItem.transform.localPosition = new Vector3(xPosition, yPosition, 0);

            Debug.Log($"Instantiated list item: {target.Name}, Position: {listItem.transform.localPosition}");

            // Add a listener to the button component of the list item
            Button button = listItem.GetComponent<Button>();
            button.onClick.AddListener(() => OnLocationSelected(target));
        }
    }

    /// <summary>
    /// Handles the selection of a location from the list.
    /// </summary>
    /// <param name="target">The selected target.</param>
    public void OnLocationSelected(Target target)
    {
        _currentSelectedLocation = target.Name;
        MySceneManager.DestinationLocationName = _currentSelectedLocation; // Store destination location name
        MySceneManager.LoadScene("QRScanner");
    }

    public void OpenInfoPanel()
    {
        infoPanel.SetActive(true);
    }

    public void CloseInfoPanel()
    {
        infoPanel.SetActive(false);
    }
}
