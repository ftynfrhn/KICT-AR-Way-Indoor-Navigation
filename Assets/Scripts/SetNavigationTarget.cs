using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetNavigationTarget : MonoBehaviour
{
    [SerializeField] private GameObject indicatorSphere; // player indicator
    [SerializeField] private GameObject topPanel;

    private NavMeshPath _path; // stores current calculated path
    private LineRenderer _line; // line renderer for path
    private Vector3 _targetPosition = Vector3.zero; // current target position

    private bool _lineToggle; // on/off LineRenderer
    private bool _isPathCalculated;

    [SerializeField] private float lineYPos = -.5f; // reduce this value to get lower line (control vertical position)

    private void Start()
    {
        _path = new NavMeshPath();
        _line = GetComponent<LineRenderer>();
        _line.enabled = _lineToggle;
    }

    private void Update()
    {
        if (_lineToggle && _targetPosition != Vector3.zero)
        {
            if (!_isPathCalculated)
            {
                NavMeshPathWalkthrough();
                _isPathCalculated = true;

                // clear rendered path
                _line.positionCount = 0;
            }

            RenderNavigationPath();

            if (IsCloseToDestination())
                topPanel.SetActive(false);

            Debug.DrawRay(indicatorSphere.transform.position, _targetPosition - indicatorSphere.transform.position, Color.red);
        }
    }

    private bool IsCloseToDestination()
    {
        var distance = GetDistanceFromPlayerToDestination();
        return distance > 5 && distance < 10f;
    }

    public bool IsArrived()
    {
        var distance = GetDistanceFromPlayerToDestination();
        return distance < 5;
    }

    private float GetDistanceFromPlayerToDestination()
    {
        return Vector3.Distance(indicatorSphere.transform.position, _targetPosition);
    }

    private void NavMeshPathWalkthrough()
    {
        var res = NavMesh.CalculatePath(indicatorSphere.transform.position, _targetPosition, NavMesh.AllAreas, _path);
        if (res && _path.status == NavMeshPathStatus.PathComplete)
        {
            _line.positionCount = _path.corners.Length;
            _line.SetPositions(_path.corners);

            for (int i = 0; i < _path.corners.Length - 1; i++)
            {
                Debug.DrawLine(_path.corners[i], _path.corners[i + 1], Color.green, 2.0f);
            }
        }
        else
        {
            Debug.LogError("Path calculation failed or path is incomplete");
        }
    }

    private void RenderNavigationPath()
    {
        List<Vector3> pathPoints = new List<Vector3>(_path.corners);

        for (int i = 0; i < pathPoints.Count; i++)
        {
            Vector3 point = pathPoints[i];
            point.y = lineYPos;
            pathPoints[i] = point;
        }

        // Remove redundant points to make path straight
        pathPoints = RemoveRedundantPoints(pathPoints);

        _line.positionCount = pathPoints.Count;
        _line.SetPositions(pathPoints.ToArray());
    }

    private List<Vector3> RemoveRedundantPoints(List<Vector3> pathPoints)
    {
        if (pathPoints.Count < 3)
            return pathPoints;

        List<Vector3> optimizedPath = new List<Vector3> { pathPoints[0] };

        for (int i = 1; i < pathPoints.Count - 1; i++)
        {
            if (!IsPointRedundant(optimizedPath[optimizedPath.Count - 1], pathPoints[i], pathPoints[i + 1]))
            {
                optimizedPath.Add(pathPoints[i]);
            }
        }

        optimizedPath.Add(pathPoints[pathPoints.Count - 1]);

        return optimizedPath;
    }

    private bool IsPointRedundant(Vector3 prev, Vector3 current, Vector3 next)
    {
        Vector3 directionToCurrent = (current - prev).normalized;
        Vector3 directionToNext = (next - current).normalized;

        float dotProduct = Vector3.Dot(directionToCurrent, directionToNext);
        return Mathf.Approximately(dotProduct, 1f);
    }

    public void ToggleVisibility()
    {
        _lineToggle = !_lineToggle;
        _line.enabled = _lineToggle;

        Target currentTarget = NavigationManager.DestinationTarget;

        if (_line.enabled)
        {
            if (currentTarget != null)
            {
                Debug.Log("Set to " + currentTarget.Name);
                _targetPosition = currentTarget.Position;
            }
        }
    }

    public void AdjustLineHeight(float value)
    {
        lineYPos = value;
    }

    public Vector3[] GetPathPoints()
    {
        return _path.corners;
    }
}
