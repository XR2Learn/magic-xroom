using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;


public class LongPressButton : HoverButton {
    [Header("Long press parameters")] [SerializeField] [Min(0.01f)]
    private float pressDelay;
    [SerializeField] [Min(0.01f)] private float decayMultiplier;

    public UnityEvent<float> onLongPressProgress = new UnityEvent<float>();
    public UnityEvent onLongPressComplete = new UnityEvent();

    private float buttonDownTime;
    private float buttonUpTime;
    private bool doProgressUpdate;

    private float progress;
    private float progressAtBtnDown;

    private const float Tolerance = .01f;
    
    public float Progress => progress;

    private void Awake() {
        onButtonDown.AddListener(OnButtonDown);
        onButtonUp.AddListener(OnButtonUp);
    }

    private void OnDestroy() {
        onButtonDown.RemoveListener(OnButtonDown);
        onButtonUp.RemoveListener(OnButtonUp);
    }

    private void Update() {
        if (!doProgressUpdate) return;

        
        float pressTime = Time.time - buttonDownTime;
        float depressTime = Time.time - buttonUpTime;
        if (engaged) {
            float delay = (1 - progressAtBtnDown) * pressDelay;
            progress = Mathf.Clamp01(progressAtBtnDown + pressTime / delay);
        } else {
            progress = Mathf.Clamp01(progress - Time.deltaTime * decayMultiplier / pressDelay);
        }

        onLongPressProgress?.Invoke(progress);
        
        
        switch (engaged) {
            case true when pressTime > (1 - progressAtBtnDown) * pressDelay:
                onLongPressComplete?.Invoke();
                doProgressUpdate = false;
                break;
            case false when depressTime > decayMultiplier / pressDelay:
                doProgressUpdate = false;
                break;
        }
    }

    private void OnButtonDown(Hand _) {
        buttonDownTime = Time.time;
        progressAtBtnDown = progress;
        doProgressUpdate = true;
    }

    private void OnButtonUp(Hand _) {
        buttonUpTime = Time.time;
        doProgressUpdate = true;
    }
}