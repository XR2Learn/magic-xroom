using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StackingGame;
using UnityEngine;

[RequireComponent(typeof(StackingLevel))]
public class WindArea : MonoBehaviour {
    [SerializeField] private float force;
    [SerializeField] private Vector3 direction;

    private List<Rigidbody> _rigidbodies;

    private void Awake()
    {
        _rigidbodies = GetComponent<StackingLevel>().Stackables.Select((s) => s.GetComponent<Rigidbody>()).ToList();
    }

    private void FixedUpdate() {
        foreach (Rigidbody r in _rigidbodies) {
            r.AddForce(direction * force);
        }
    }

    private void OnDrawGizmosSelected() {
        DrawArrow.ForGizmo(transform.position, direction.normalized, Color.green);
    }
}