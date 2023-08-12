
public class GameManager : Singleton<GameManager>
{
    public ScoreController scoreController;
    public HealthController healthController;
    public GameOverController gameOverController;
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }
}
