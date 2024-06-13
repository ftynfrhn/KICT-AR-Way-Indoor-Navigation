using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// manages the AR session and navigation within a scene
/// sets up the AR session, handles target destinations, updates UI elements,
/// and manages user interactions
/// </summary>
public class SessionManager : MonoBehaviour
{
    [SerializeField] private ARSession session;
    [SerializeField] private ARSessionOrigin sessionOrigin;

    [SerializeField] private GameObject environmentObject;

    //[SerializeField] private List<GameObject> allTargets;

    [SerializeField] private TMP_Text textTopPanel;

    [SerializeField] private GameObject locationPinPrefab; // prefab for display location pin
    [SerializeField] private float destinationPinOffsetMultiplier = 1;

    [SerializeField] private GameObject startPanel; // panel at start of navigation
    [SerializeField] private GameObject navigationCommand; // panel during navigation


    private Target _originTarget; // stores origion target location
    private Target _destinationTarget; // stores destination target location
    private List<Target> allTargets; // target game object used for navigation
    //private TargetHandler targetHandler;

    //private void Awake()
    //{
    //    TargetHandler targetHandler = FindObjectOfType<TargetHandler>();

    //    // Check if targetHandler is null after attempting to find it
    //    if (targetHandler == null)
    //    {
    //        Debug.LogError("TargetHandler not found in the scene!");
    //    }
    //}


    private void Start()
    {
        TargetHandler targetHandler = FindObjectOfType<TargetHandler>();
        allTargets = targetHandler.GetTargetList();

        // default value are assigned for debugging purposes
        // set origin destination
        _originTarget = GetDestinationTarget(MySceneManager.OriginLocationName ?? "staircase 6"); // call MySceneManager.cs class
        NavigationManager.OriginTarget = _originTarget; // set the target in NavigationManager.OriginTarget (another script file)
        Debug.Log("We'll start from " + _originTarget.Name + " at " + _originTarget.Position);

        // set destination target
        _destinationTarget = GetDestinationTarget(MySceneManager.DestinationLocationName ?? "KICT Multipurpose Hall"); // call MySceneManager.cs class
        NavigationManager.DestinationTarget = _destinationTarget; // set the target in NavigationManager.OriginTarget (another script file)
        Debug.Log("We'll go to " + _destinationTarget.Name + " at " + _destinationTarget.Position);

        MoveArSessionToUserOrigin(); // moving AR session to user's origin
        SetTopPanelInformation(); // updating top panel information
        SetLocationPinToDestination(); // set location pin to destination
    }

    /// <summary>
    /// Positions the AR session origin to the user's starting location.
    /// </summary>
    private void MoveArSessionToUserOrigin()
    {
        sessionOrigin.transform.position = _originTarget.Position;
        sessionOrigin.transform.rotation = new Quaternion(0f, 100f, 0f, 1f);
        // more reliable way to set rotations in Unity is to use helper methods that handle the underlying quaternion math for you
        // sessionOrigin.transform.rotation = Quaternion.Euler(0f, 100f, 0f);

    }

    /// <summary>
    /// Updates the top panel with text based on the destination target.
    /// </summary>
    private void SetTopPanelInformation()
    {
        // set the text
        textTopPanel.text = $"{_originTarget.Name} - {_destinationTarget.Name}";
    }

    /// <summary>
    /// Resets the AR session and repositions the session origin to the default position.
    /// </summary>
    public void ResetSession()
    {
        session.Reset();

        sessionOrigin.transform.position = Vector3.zero; // Vector3.zero returns a vector with all components set to 0
        sessionOrigin.transform.rotation = Quaternion.identity; // Quaternion.identity returns the identity quaternion, which represents no rotation (i.e., (0, 0, 0, 1)).
    }

    //public void ToggleArAbility()
    //{
    //    session.enabled = !session.enabled;
    //    arButtonText.text = session.enabled ? "Disable AR" : "Enable AR";
    //}

    /// <summary>
    /// Finds and returns a target by name from the allTargets list.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private Target GetDestinationTarget(string name)
    {
        // find the relevant target from the allTargets list
        // by comparing the current one (allTargets) with the string provided as a parameter here
        var target = allTargets.Find(x => x.Name == name);

        // Check if the target was found
        if (target != null)
        {
            return target;
        }
        else
        {
            Debug.LogError("Target not found: " + name);
            return null;
        }
    }

    /// <summary>
    /// Finds and returns a target by name from the allTargets list.
    /// </summary>
    private void SetLocationPinToDestination()
    {
        // instantiate prefab at destinationTarget position
        var offset = Vector3.down * destinationPinOffsetMultiplier; // moving it slightly downward from the destination target position
        // The locationPinPrefab is instantiated at the position of _destinationTarget, adjusted by subtracting the calculated offset
        Instantiate(locationPinPrefab, _destinationTarget.Position - offset, Quaternion.identity); // Quaternion.identity represents no rotation
    }

    /// <summary>
    /// Hide Start Button & show destination command panel
    /// </summary>
    public void StartNavigation()
    {
        startPanel.SetActive(false);
        navigationCommand.SetActive(true);
    }
}