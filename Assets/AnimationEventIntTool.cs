using UnityEngine;
using UnityEngine.Events;

public class AnimationEventIntTool : MonoBehaviour
{
    public int parameter;
    public UnityEvent<int> useInt;

    public void TriggerIntEvent()
    {
        useInt.Invoke(parameter); // safe to invoke even without callbacks
    }
}