using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour {
    private UIDocument m_menuDocument;

    [SerializeField] private SoundEffectHandler m_focusSound;
    [SerializeField] private SoundEffectHandler m_submitSound;
    private VisualElement m_playButton;
    private VisualElement m_exitButton;


    private void OnEnable() {
        m_menuDocument = gameObject.GetComponent<UIDocument>();
        VisualElement root = m_menuDocument.rootVisualElement;

        m_playButton = root.Q<VisualElement>("PlayButton");
        m_exitButton = root.Q<VisualElement>("ExitButton");

        Clickable playClickable = new Clickable(() => HandlePlayButton());
        m_playButton.AddManipulator(playClickable);
        m_playButton.AddManipulator(new UiSoundManipulator(m_focusSound, m_submitSound));


        Clickable exitClickable = new Clickable(() => HandleExitButton());
        m_exitButton.AddManipulator(exitClickable);
        m_exitButton.AddManipulator(new UiSoundManipulator(m_focusSound, m_submitSound));
    }

    private void HandlePlayButton() {
        SceneManager.LoadScene("GameScene");
    }

    private void HandleExitButton() {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }

}


