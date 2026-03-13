using UnityEngine;
using UnityEngine.UIElements;

public class HUD : MonoBehaviour {

    private UIDocument m_uiDocument;

    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private SoundEffectHandler m_focusSound;
    [SerializeField] private SoundEffectHandler m_submitSound;
    private VisualElement m_gameOverScreen;
    private VisualElement m_retryButton;
    private VisualElement m_quitButton;


    private void OnEnable() {
        m_uiDocument = gameObject.GetComponent<UIDocument>();

        VisualElement root = m_uiDocument.rootVisualElement;
        VisualElement topBar = root.Q<VisualElement>("TopBar");
        topBar.dataSource = scoreManager;

        m_gameOverScreen = root.Q<VisualElement>("GameOver");
        m_retryButton = m_gameOverScreen.Q<VisualElement>("RetryButton");
        m_quitButton = m_gameOverScreen.Q<VisualElement>("ExitButton");

        Clickable retryClickable = new Clickable(() => HandleRetryEvent());
        Clickable quitClickable = new Clickable(() => HandleQuitEvent());

        m_retryButton.AddManipulator(retryClickable);
        m_retryButton.AddManipulator(new UiSoundManipulator(m_focusSound, m_submitSound));
        m_quitButton.AddManipulator(quitClickable);
        m_quitButton.AddManipulator(new UiSoundManipulator(m_focusSound, m_submitSound));


        GameEvents.Instance.OnGameOver += OnGaneOver;
    }

    private void OnDisable() {
        GameEvents.Instance.OnGameOver -= OnGaneOver;
    }

    private void OnGaneOver() {
        m_gameOverScreen.RemoveFromClassList("hidden");
    }

    private void HandleRetryEvent() {
        GameEvents.Instance.Retry();
        m_gameOverScreen.AddToClassList("hidden");
    }

    private void HandleQuitEvent() {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.EnterPlaymode();
#endif
    }
}
