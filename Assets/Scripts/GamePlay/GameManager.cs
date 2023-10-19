
using System;

public class GameManager : Singleton<GameManager>
{
    public GameOverController gameOverController;
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }
}
