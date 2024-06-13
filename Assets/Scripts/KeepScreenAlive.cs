using UnityEngine;

public class KeepScreenAlive : MonoBehaviour
{
    void Start()
    {
        // disable screen dimming
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

}
