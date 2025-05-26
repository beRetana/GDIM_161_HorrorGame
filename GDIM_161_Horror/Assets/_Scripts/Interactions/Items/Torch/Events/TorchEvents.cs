using UnityEngine;
using UnityEngine.Events;

public class TorchEvents : MonoBehaviour
{
    public UnityEvent OnIgnited;
    public UnityEvent OnExtinguished;
    public UnityEvent OnSmothered;
    public UnityEvent OnBurnComplete;

    public void FireIgnited() => OnIgnited.Invoke();
    public void FireExtinguished() => OnExtinguished.Invoke();
    public void FireSmother() => OnSmothered.Invoke();
    public void FireBurnedOut() => OnBurnComplete.Invoke();
}