using UnityEngine;
using UnityEngine.Events;

public class RestartButtonController : MonoBehaviour
{
    public UnityEvent gameRestart;

    // set as the Button's OnClick
    public void ButtonClick()
    {
        gameRestart.Invoke();
    }
}
