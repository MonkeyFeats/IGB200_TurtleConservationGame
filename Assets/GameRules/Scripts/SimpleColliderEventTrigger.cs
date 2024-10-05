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
}
