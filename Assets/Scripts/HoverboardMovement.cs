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

    private bool _grounded;

    private void FixedUpdate()
    {
        _grounded = Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, _hoverHeight * 2, _layer, QueryTriggerInteraction.Ignore);

        if(_grounded)
        {
            var toRotation = Quaternion.FromToRotation(transform.up, hit.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, _rotationDamping);

            transform.position = Vector3.Lerp(transform.position, hit.point + hit.normal * _hoverHeight, _hoverDamping);
        }
        else
        {
            var toRotation = Quaternion.FromToRotation(transform.up, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, _rotationDamping);

            transform.position += Vector3.down * _fallSpeed;
        }

        transform.position += transform.forward * 0.1f;
    }
}
