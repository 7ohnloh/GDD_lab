using UnityEngine;

// stand-in "no data" type, since GameEvent<T> always needs some T
public class Void { } // dummy class

[CreateAssetMenu(fileName = "SimpleGameEvent", menuName = "ScriptableObjects/SimpleGameEvent", order = 3)]
public class SimpleGameEvent : GameEvent<Void>
{
    // lets you call Raise() with nothing, e.g. from a Button's OnClick
    public void Raise() => Raise(new Void());
}
