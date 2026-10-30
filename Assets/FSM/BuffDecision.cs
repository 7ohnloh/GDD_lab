using UnityEngine;

// true when Mario has just collected the given buff (e.g. StarMan)
[CreateAssetMenu(menuName = "PluggableSM/Decisions/Buff")]
public class BuffDecision : Decision
{
    public PowerupType buffType;

    public override bool Decide(StateController controller)
    {
        BuffStateController b = (BuffStateController)controller;
        return b.currentBuffType == buffType;
    }
}
