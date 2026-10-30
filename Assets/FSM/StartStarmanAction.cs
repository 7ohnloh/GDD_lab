using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/StartStarman")]
public class StartStarmanAction : Action
{
    public override void Act(StateController controller)
    {
        BuffStateController b = (BuffStateController)controller;
        b.StartStarman();
    }
}
