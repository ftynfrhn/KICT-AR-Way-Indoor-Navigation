using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// manage the compass and AR session adjustments before starting a navigation process in an AR application
/// functionality to initialize the compass, adjust the heading based on user input,
/// and ensure the phone's orientation is correct for accurate compass readings
/// </summary>
public class PreflightCompassAdjustment : MonoBehaviour
{
    [SerializeField] private TMP_Text compassText; // compass heading
    [SerializeField] private ARSession session;
    [SerializeField] private ARSessionOrigin sessionOrigin; // manage position and rotation of AR content
    
    // Adjust these values to control the smoothness
    [SerializeField] private float rotationSpeed = 5f;
    // [SerializeField] private float angleThreshold = 1f;
    
    // Phone level detection, indicate phone is properly leveled
    [SerializeField] private GameObject phoneLevelPanel;

    private bool _startTracking; // track compass has started
    private bool _startTrackingEditor;
    private float _compassValue; // store compass value from user input
    private bool _navigationStarted; // track if navigation has started

    // ensure that the AR content aligns correctly with the physical environment and provides an accurate and reliable user experience
    private const float BuildingHeading = 265f; // yeah sometimes not correct, need more calibration
    private float _correctedHeading; // stoores corrected heading value

    private void Start()
    {
        // to disable compass input from ArSession interfere with our compass input
        session.GetComponent<ARInputManager>().enabled = false;
        Input.compass.enabled = true; // enables device's compass and start location services
        // accurate location data is critical for aligning virtual content with the physical environment and providing users with accurate navigation guidance
        Input.location.Start(); // initiate location services on the device 
        StartCoroutine(InitializeCompass());
    }
    
    /// <summary>
    /// To be attached the Start Navigation Button
    /// </summary>
    public void StartNavigation()
    {
        _navigationStarted = true;
        session.Reset();

        // set the starting AR position and rotation to be the origin position
        sessionOrigin.transform.position = NavigationManager.OriginTarget.Position; // call NavigationManager.cs
        sessionOrigin.transform.rotation = Quaternion.Euler(0, _correctedHeading, 0);
        
        // Stop Input.Compass from handling the operation
        // Let AR things to the job
        // _startTracking = false;
        // _startTrackingEditor = false;
        session.GetComponent<ARInputManager>().enabled = true;
    }

    /// <summary>
    /// Coroutine that waits for a second before enabling compass tracking.
    /// </summary>
    /// <returns></returns>
    IEnumerator InitializeCompass()
    {
        yield return new WaitForSeconds(1f);
        _startTracking |= Input.compass.enabled; // Input.compass is a property in Unity used to access the device's compass.

        // for editor
        _startTrackingEditor = true;
    }

    /// <summary>
    /// Manual offset of the compass input value
    /// </summary>
    /// <param name="value">Slider value</param>
    public void SliderValueChanged(float value)
    {
        _compassValue = value;
    }

    /// <summary>
    /// Handles real-time updates for the compass heading and phone orientation.
    /// Updates the AR session's rotation and displays the heading on the UI.
    /// </summary>
    private void Update()
    {
        // Checks if _startTrackingEditor is true and the application is running in the editor
        if (_startTrackingEditor && Application.isEditor)
        {
            var targetHeading = BuildingHeading - _compassValue;
            targetHeading = (targetHeading + 360) % 360; // Ensure the target is within 0-360 range
    
            float smoothedHeading = Mathf.LerpAngle(sessionOrigin.transform.eulerAngles.y, targetHeading,
                rotationSpeed * Time.deltaTime);
            _correctedHeading = smoothedHeading;
    
            sessionOrigin.transform.rotation = Quaternion.Euler(0, smoothedHeading, 0);
            compassText.text = (int)smoothedHeading + "° " + DegreesToCardinalDetailed(smoothedHeading);
        }
        
        // Phone's screen pointing to the sky to get better, smoother & consistent compass reading
        if (_startTracking)
        {
            var targetHeading = Input.compass.trueHeading - BuildingHeading - _compassValue;
            targetHeading = (targetHeading + 360) % 360; // Ensure the target is within 0-360 range
    
            float smoothedHeading = Mathf.LerpAngle(sessionOrigin.transform.eulerAngles.y, targetHeading,
                rotationSpeed * Time.deltaTime);
            _correctedHeading = smoothedHeading;

            if (!_navigationStarted)
            {
                // only edit the ar pose before the navigation starts
                sessionOrigin.transform.rotation = Quaternion.Euler(0, smoothedHeading, 0);
            
                // check orientation
                CheckPhoneOrientation();
            }
            compassText.text = (int)smoothedHeading + "° " + DegreesToCardinalDetailed(smoothedHeading);
        }
    }

    /// <summary>
    /// Checks if the phone's screen is facing up to ensure accurate compass readings.
    /// </summary>
    private void CheckPhoneOrientation()
    {
        // Get the current acceleration vector from the device
        Vector3 acceleration = Input.acceleration;

        // Calculate the absolute value of the Z-axis acceleration
        float zAcceleration = Mathf.Abs(acceleration.z);

        // Define a threshold value to determine if the screen is pointing up
        float threshold = 0.7f;

        // Check if the Z-axis acceleration is greater than the threshold
        bool isScreenUp = zAcceleration > threshold;

        phoneLevelPanel.SetActive(!isScreenUp);
    }

    /// <summary>
    /// Converts a heading in degrees to a detailed cardinal direction (e.g., N, NE, E, etc.).
    /// </summary>
    /// <param name="degrees"></param>
    /// <returns></returns>
    private static string DegreesToCardinalDetailed(double degrees)
    {
        string[] cardinals = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
        return cardinals[(int)Math.Round((degrees * 10 % 3600) / 225)];
    }
}
