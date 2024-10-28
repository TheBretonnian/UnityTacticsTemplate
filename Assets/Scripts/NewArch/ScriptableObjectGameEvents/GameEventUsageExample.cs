// Event Publisher
using UnityEngine;
public class Enemy : MonoBehaviour
{
    public IntGameEvent scoreEvent;

    public void Defeat()
    {
        scoreEvent.Raise(10);
    }
}

// Event Subscriber
public class ScoreDisplay : MonoBehaviour
{
    public IntGameEvent scoreEvent;

    private void OnEnable()
    {
        scoreEvent.gameEvent += OnScoreChanged;
    }

    private void OnDisable()
    {
        scoreEvent.gameEvent -= OnScoreChanged;
    }

    private void OnScoreChanged(int newScore)
    {
        Debug.Log("Score: " + newScore);
    }
}
