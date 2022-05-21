using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverboardMovement : MonoBehaviour
{
    [SerializeField]
    private float _hoverHeight = 1;
    [SerializeField]
    private float _hoverDamping = 0.25f;
    [SerializeField]
    private float _rotationDamping = 0.25f;
    [SerializeField]
    private LayerMask _layer;
    [SerializeField]
    private float _fallSpeed;
    [Header("Debug")]
    [SerializeField]
    private Vector3[] _boardRayPoints = new Vector3[4];
    [SerializeField]
    private Transform _debugGroundAverage;

    private bool _grounded;
    private Vector2 _input;
    private Quaternion _targetRotation;

    private void Update()
    {
        _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        _grounded = RaycastGroundAverage(out Vector3 avgPoint, out Vector3 avgNormal);

        if(_debugGroundAverage)
        {
            _debugGroundAverage.position = avgPoint;
            _debugGroundAverage.up = avgNormal;
        }

        transform.Rotate(new Vector3(0, _input.x * 5, 0));
        transform.position += transform.forward * 0.25f * _input.y;

        if(_grounded)
        {
            // accelerate along the ground angle
            float forwardGroundAngle = Vector3.Dot(transform.forward, -Vector3.up);

            Debug.Log($"{forwardGroundAngle * 90} degrees");

            _targetRotation = Quaternion.FromToRotation(transform.up, avgNormal) * transform.rotation;

            transform.position = Vector3.Lerp(transform.position, avgPoint + avgNormal * _hoverHeight, _hoverDamping);
        }
        else
        {
            _targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;

            transform.position += Vector3.down * _fallSpeed;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _rotationDamping);
    }

    private bool RaycastGroundAverage(out Vector3 avgPoint, out Vector3 avgNormal)
    {
        int hitCount = 0;
        avgPoint = Vector3.zero;
        avgNormal = Vector3.up;

        foreach(Vector3 point in _boardRayPoints)
        {
            Vector3 pointInWorld = transform.TransformPoint(point);

            if(Physics.Raycast(pointInWorld, -transform.up, out RaycastHit hit, _hoverHeight * 2, _layer, QueryTriggerInteraction.Ignore))
            {
                avgNormal += hit.normal;
                avgPoint += hit.point;
                hitCount++;
            }
        }

        if(hitCount > 0)
        {
            avgNormal /= hitCount;
            avgNormal.Normalize();
            avgPoint /= hitCount;
        }

        return hitCount > 0;
    }

    private void OnDrawGizmos()
    {
        foreach(Vector3 point in _boardRayPoints)
        {
            Vector3 pointInWorld = transform.TransformPoint(point);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(pointInWorld, 0.2f);
        }
    }
}
