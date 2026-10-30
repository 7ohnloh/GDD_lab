using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/RaiseEvent")]
public class RaiseEventAction : Action
{
    public SimpleGameEvent gameEvent;
    public override void Act(StateController controller)
    {
        gameEvent.Raise();
    }
}
