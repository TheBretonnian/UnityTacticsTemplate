using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/GameEvent")]
public class GameEvent<T> : ScriptableObject, IGameEvent
{
    public event Action<T> gameEvent;

    public void Raise(T arg)
    {
        gameEvent?.Invoke(arg);
    }

    // Implement the Raise method from IGameEvent (overloading Raise for flexibility)
    public void Raise(object arg)
    {
        if (arg is T tArg)
        {
            Raise(tArg);
        }
        else
        {
            Debug.LogWarning($"Invalid argument type: expected {typeof(T)}, got {arg.GetType()}");
        }
    }
}
