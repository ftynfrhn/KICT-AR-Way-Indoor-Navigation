using TMPro;
using UnityEngine;

public class PlayerNavigation : MonoBehaviour
{
    [SerializeField] private TMP_Text directionCommandText;
    [SerializeField] private SetNavigationTarget navigationTarget;

    void Update()
    {
        // Get the path points
        Vector3[] pathPoints = navigationTarget.GetPathPoints();

        // Assuming you have at least one point in the path
        if (pathPoints.Length >= 1)
        {
            // Check if the player has arrived
            if (navigationTarget.IsArrived())
            {
                // Set the direction command text to "You have arrived"
                directionCommandText.text = "You have arrived";
            }
            else
            {
                // Set the direction command text to "Follow the line"
                directionCommandText.text = "Follow the line";
            }
        }
    }
}
