using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Decisions/Transform")]
public class TransformDecision : Decision
{
    public StateTransformMap[] map;

    public override bool Decide(StateController controller)
    {
        MarioStateController m = (MarioStateController)controller;
        // the state asset's name must match a MarioState value, e.g. "SuperMario"
        MarioState toCompareState = EnumExtension.ParseEnum<MarioState>(m.currentState.name);

        for (int i = 0; i < map.Length; i++)
        {
            if (toCompareState == map[i].fromState && m.currentPowerupType == map[i].powerupCollected)
            {
                return true;
            }
        }
        return false;
    }
}

[System.Serializable]
public struct StateTransformMap
{
    public MarioState fromState;
    public PowerupType powerupCollected;
}
