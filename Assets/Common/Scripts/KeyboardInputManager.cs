using DataCollection.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardInputManager : MonoBehaviour
{
    [SerializeField]
    private bool _enabled;
    [SerializeField]
    private FeedbackBoard _feedbackBoard;
    [SerializeField]
    private MainDataCollectionManager _mainDataCollectionManager;

    public void Awake()
    {
        UserSettingsLoader.Load(UserSettingsLoader.userSettings.Keyboard_EnableShortcuts, out _enabled, false);
        gameObject.SetActive(_enabled);
    }

    public void Update()
    {
        if (Keyboard.current.fKey.wasReleasedThisFrame)
        {
            _feedbackBoard?.Show(FeedbackBoard.FeedbackEventType.MANUAL);
        }
        if (Keyboard.current.dKey.wasReleasedThisFrame)
        {
            _mainDataCollectionManager?.ToggleDataCollection();
        }
    }
}
