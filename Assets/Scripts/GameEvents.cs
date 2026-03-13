using System;

public class GameEvents {
    private static GameEvents m_instance;

    public static GameEvents Instance {
        get {
            m_instance ??= new GameEvents();
            return m_instance;
        }
    }


    public event Action<int, bool> OnAddScore;
    public event Action OnPlayerDie;
    public event Action OnGameOver;
    public event Action OnRetry;

    public void AddToScore(int amount, bool isCollectable = false) {
        OnAddScore?.Invoke(amount, isCollectable);
    }

    public void PlayerDies() {
        OnPlayerDie?.Invoke();
    }

    public void GameOver() {
        OnGameOver?.Invoke();
    }

    public void Retry() {
        OnRetry?.Invoke();
    }
}
