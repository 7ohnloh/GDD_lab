using UnityEngine;

public abstract class StateController : MonoBehaviour
{
    public State startState;
    public State previousState;
    public State currentState;
    public State remainState;
    public bool transitionStateChanged = false;
    [HideInInspector] public float stateTimeElapsed;

    public bool isActive = true;

    public virtual void Start()
    {
        OnSetupState(); // setup when game starts
    }

    public virtual void OnSetupState()
    {
        if (currentState)
            currentState.DoSetupActions(this);
    }

    public virtual void OnExitState()
    {
        // reset time in this state
        stateTimeElapsed = 0;
        if (currentState)
            currentState.DoExitActions(this);
    }

    // draws a coloured circle in the Scene view showing the current state
    public virtual void OnDrawGizmos()
    {
        if (currentState != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(this.transform.position, 1.0f);
        }
    }

    // no action should be done here, strictly for transition
    public void TransitionToState(State nextState)
    {
        if (nextState == remainState) return;

        OnExitState(); // cast exit action if any

        previousState = currentState;
        currentState = nextState;
        transitionStateChanged = true;

        OnSetupState(); // cast entry action if any
    }

    // true once we've been in this state for `duration` seconds; call once per frame
    public bool CheckIfCountDownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        return stateTimeElapsed >= duration;
    }

    public void Update()
    {
        if (!isActive) return; // separate from gameObject active

        currentState.UpdateState(this);
    }
}
