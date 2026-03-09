using System;

public class GameEvents {
    private static GameEvents m_instance;

    public static GameEvents Instance {
        get {
            m_instance ??= new GameEvents();
            return m_instance;
        }
    }


    public event Action<int> OnAddScore;

    public void AddToScore(int amount) {
        OnAddScore?.Invoke(amount);
    }
}
