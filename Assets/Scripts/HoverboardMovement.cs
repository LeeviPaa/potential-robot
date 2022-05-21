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
    private float _downhillAcceleration = 1;
    [SerializeField]
    private float _hoverHeight = 1;
    [SerializeField]
    private float _hoverDamping = 0.25f;
    [SerializeField]
    private float _rotationDamping = 0.25f;
    [SerializeField]
    private LayerMask _layer;
    [SerializeField]
    private float _gravity;
    
    [Header("Debug")]
    [SerializeField]
    private Vector3[] _boardRayPoints = new Vector3[4];
    [SerializeField]
    private Transform _debugGroundAverage;

    private bool _grounded;
    private Vector2 _input;
    private Quaternion _targetRotation;
    private Vector3 _velocity;

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

    private void AerialMovement()
    {
        _targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;

        _velocity += Vector3.down * _gravity * Time.fixedDeltaTime;
    }

    private void GroundedMovement(Vector3 avgPoint, Vector3 avgNormal)
    {
        SlopeAcceleration();

        _targetRotation = Quaternion.FromToRotation(transform.up, avgNormal) * transform.rotation;

        transform.position = Vector3.Lerp(transform.position, avgPoint + avgNormal * _hoverHeight, _hoverDamping);
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
        transform.Rotate(new Vector3(0, _input.x * _rotationSpeed, 0));
        _velocity += transform.forward * _acceleration * _input.y;
    }

    private void VelocityDrag()
    {
        float speed = _velocity.magnitude;
    }

    private void BoardTurnAcceleration()
    {
        // when board turns, change the velocity to that direction
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
