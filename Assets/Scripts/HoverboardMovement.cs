using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverboardMovement : MonoBehaviour
{
    [Header("Board stats")]
    [SerializeField]
    private float _acceleration = 0.25f;
    [SerializeField]
    private float _maxSpeed;
    [SerializeField]
    private float _rotationSpeed = 4;
    
    [Header("General board behaviour")]
    [SerializeField]
    private float _maxVelocity = 100;
    [SerializeField]
    private float _velocityDamping = 1;
    [SerializeField]
    private float _terminalVelocity = 50;
    [SerializeField]
    private float _downhillAcceleration = 1;
    [SerializeField]
    private float _hoverHeight = 1;
    [SerializeField]
    private float _hoverDamping = 0.25f;
    [SerializeField]
    private float _hoverForce = 0.25f;
    [SerializeField]
    private float _rotationDamping = 0.25f;
    [SerializeField]
    private LayerMask _layer;
    [SerializeField]
    private float _gravity = 9.81f;
    
    [Header("Debug")]
    [SerializeField]
    private Vector3[] _boardRayPoints = new Vector3[4];
    [SerializeField]
    private Transform _debugGroundAverage;

    private bool _grounded;
    private Vector2 _input;
    private Quaternion _targetRotation;
    private Vector3 _velocity;
    private bool _directionForward;

    private void Update()
    {
        _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        _grounded = RaycastGroundAverage(out Vector3 avgPoint, out Vector3 avgNormal);

        if (_debugGroundAverage)
        {
            _debugGroundAverage.position = avgPoint;
            _debugGroundAverage.up = avgNormal;
        }

        _directionForward = CalculateForwardSign() >= 0;
        InputMovement();

        if (_grounded)
        {
            GroundedMovement(avgPoint, avgNormal);
        }
        else
        {
            AerialMovement();
        }

        VelocityDrag();

        transform.position += _velocity;
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _rotationDamping);
    }

    private float CalculateForwardSign()
    {
        Vector3 forwardXZ = new Vector3(transform.forward.x, 0, transform.forward.z);

        Vector3 velocityXZ = new Vector3(_velocity.x, 0, _velocity.z).normalized;
        return Vector3.Dot(forwardXZ, velocityXZ);
    }

    private void AerialMovement()
    {
        _targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;

        ApplyGravity();
    }

    private void GroundedMovement(Vector3 avgPoint, Vector3 avgNormal)
    {
        SlopeAcceleration();

        Vector3 forwardXZ = new Vector3(transform.forward.x, 0, transform.forward.z);
        Vector3 velocityXZ = new Vector3(_velocity.x, 0, _velocity.z);

        if(CalculateForwardSign() < 0)
            velocityXZ *= -1;

        _targetRotation = 
            Quaternion.FromToRotation(transform.up, avgNormal)
            * Quaternion.FromToRotation(forwardXZ, velocityXZ.normalized)
            * transform.rotation;

        float distFromGroundCapped = Mathf.Clamp(Vector3.Distance(avgPoint, transform.position), 0, _hoverHeight * 2);

        float deltaHeight = _hoverHeight - distFromGroundCapped;

        if(deltaHeight < 0)
        {
            ApplyGravity(Mathf.Abs(deltaHeight));
        }
        else
        {
            _velocity.y = deltaHeight;
        }
    }

    private void ApplyGravity(float maxDelta = float.MaxValue)
    {
        _velocity.y -= _gravity * Time.fixedDeltaTime;

        _velocity.y = Mathf.Clamp(_velocity.y, -_terminalVelocity, _terminalVelocity);
        _velocity.y = Mathf.Clamp(_velocity.y, -maxDelta, maxDelta);
    }

    private void SlopeAcceleration()
    {
        // accelerate along the slope angle
        //  +1 is down & -1 is up
        float downSlope = Vector3.Dot(transform.forward, -Vector3.up);

        _velocity += transform.forward * _downhillAcceleration * downSlope;
    }

    private void InputMovement()
    {
        int sign = _directionForward ? 1 : -1;
        _velocity += transform.right * _input.x * _rotationSpeed * _velocity.magnitude * sign;

        _velocity += transform.forward * _acceleration * _input.y;
    }

    private void VelocityDrag()
    {
        float speed = _velocity.magnitude;
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

        Gizmos.DrawLine(transform.position, transform.position - transform.up * _hoverHeight);
    }
}
