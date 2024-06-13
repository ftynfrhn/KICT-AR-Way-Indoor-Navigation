using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spin the target destination object around its Y-axis
/// </summary>
public class Spin : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0f, 50f * Time.deltaTime, 0f, Space.Self);
    }
}
