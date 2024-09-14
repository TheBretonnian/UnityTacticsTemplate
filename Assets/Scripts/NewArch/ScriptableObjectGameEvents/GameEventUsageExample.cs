// Event Publisher
public class Enemy : MonoBehaviour
{
    public GameEvent<int> scoreEvent;

    public void Defeat()
    {
        scoreEvent.Raise(10);
    }
}

// Event Subscriber
public class ScoreDisplay : MonoBehaviour
{
    public GameEvent<int> scoreEvent;

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
