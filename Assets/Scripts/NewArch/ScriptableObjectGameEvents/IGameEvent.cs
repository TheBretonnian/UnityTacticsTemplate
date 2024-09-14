using UnityEngine;

public interface IGameEvent
{
    void Raise(object arg);
}