using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : MonoBehaviour
{
    // List of tags to check for
    public List<string> triggerTags;
    // UnityEvent allows for setting any number of functions from the Unity inspector
    public UnityEvent onTriggerEnterEvents;
    public UnityEvent onTriggerExitEvents;

    // Limit for how many times the events can be triggered (-1 for infinite)
    public int maxTriggers = -1;
    private int currentTriggerCount = 0;

    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerTags.Contains(other.tag) && (maxTriggers == -1 || currentTriggerCount < maxTriggers))
        {
            if (onTriggerEnterEvents != null)
            {
                onTriggerEnterEvents.Invoke();
            }
            currentTriggerCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerTags.Contains(other.tag) && (maxTriggers == -1 || currentTriggerCount < maxTriggers))
        {
            if (onTriggerExitEvents != null)
            {
                onTriggerExitEvents.Invoke();
            }
            currentTriggerCount++;
        }
    }

    // This will draw the collider shape in the scene view.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;  // You can set any color you prefer

        if (_collider == null)
            _collider = GetComponent<Collider>();  // Ensure the collider is assigned

        if (_collider is BoxCollider box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.center, box.size);
        }
        else if (_collider is SphereCollider sphere)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(sphere.center, sphere.radius);
        }
        else if (_collider is CapsuleCollider capsule)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(capsule.center, capsule.radius);  // Capsule is more complex, this is a basic visualization
        }
    }
}
