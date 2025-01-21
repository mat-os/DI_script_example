using UnityEngine;
using UnityEngine.Events;

public class StartEventTrigger : MonoBehaviour 
{
    public UnityEvent OnStartEvent;

    void Awake() 
    {
        OnStartEvent.Invoke();
    }
}