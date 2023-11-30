using DG.Tweening;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class FeedbackBoard : MonoBehaviour
{
    public enum FeedbackEventType
    {
        AFTER_LEVEL, AFTER_SCENARIO, MANUAL
    }

    [SerializeField] private bool _enabled = true;
    [SerializeField] private Player player;
    [SerializeField] private float distance;

    private const int _inputDelay = 1000; //ms

    public UnityEvent<string, long> feedbackReceived;

    private Stopwatch _stopwatch;

    private bool _afterLevel;
    private bool _afterScenario;

    public static FeedbackBoard Instance { get; private set; }

    public void OnFeedback(string value)
    {
        long ms = _stopwatch.ElapsedMilliseconds;
        if (ms < _inputDelay) return;

        feedbackReceived?.Invoke(value, ms);
        Hide();
    }

    private void Awake() 
    {
        UserSettingsLoader.Load(UserSettingsLoader.userSettings.Feedback_Enabled, out _enabled, true);
        UserSettingsLoader.Load(UserSettingsLoader.userSettings.Feedback_AfterLevel, out _afterLevel, true);
        UserSettingsLoader.Load(UserSettingsLoader.userSettings.Feedback_AfterScenario, out _afterScenario, true);

        Instance = this;

        _stopwatch = new();
        gameObject.SetActive(false);
    }

    public void Show(FeedbackEventType eventType)
    {
        if (!_enabled) return;

        if (eventType == FeedbackEventType.AFTER_LEVEL && !_afterLevel) return;
        if (eventType == FeedbackEventType.AFTER_SCENARIO && !_afterScenario) return;

        Vector3 dir = Vector3.ProjectOnPlane(player.hmdTransform.forward, Vector3.up).normalized * distance;
        Vector3 rootPos = player.feetPositionGuess + dir;
        transform.position = rootPos;
        transform.localScale = Vector3.zero;

        Transform offset = transform.GetChild(0);
        offset.localPosition = Vector3.up * player.eyeHeight;
        offset.LookAt(player.hmdTransform);
        
        gameObject.SetActive(true);

        transform.DOScale(1, .3f).SetEase(Ease.OutCubic).OnUpdate(() => offset.LookAt(player.hmdTransform)).Play();
        _stopwatch.Start();
    }

    public void Hide()
    {
        if (!_enabled) return;

        transform.DOScale(0, .3f).SetEase(Ease.InCubic).OnComplete(() => gameObject.SetActive(false)).Play();
        _stopwatch.Reset();
    }
}