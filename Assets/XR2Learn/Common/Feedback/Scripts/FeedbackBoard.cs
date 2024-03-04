/*********** Copyright © 2024 University of Applied Sciences of Southern Switzerland (SUPSI) ***********\
 
 Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
 associated documentation files (the "Software"), to deal in the Software without restriction,
 including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 subject to the following conditions:

 The above copyright notice and this permission notice shall be included in all copies or substantial
 portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
 LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

\*******************************************************************************************************/


using DG.Tweening;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using XR2Learn.Common.UserConfig;

namespace XR2Learn.Common.Feedback
{ 
    public class FeedbackBoard : MonoBehaviour
    {
        public enum FeedbackEventType
        {
            AFTER_LEVEL, AFTER_SCENARIO, MANUAL
        }

        [SerializeField] private bool _enabled = true;
        [SerializeField] private Player player;
        [SerializeField] private float distance;

        private bool _isActive = false;
        private const int _inputDelay = 1000; //ms

        public UnityEvent<string, long> feedbackReceived;

        private Stopwatch _stopwatch;

        private bool _afterLevel;
        private bool _afterScenario;
        public bool AfterLevel { get => _afterLevel; private set => _afterLevel = value; }
        public bool AfterScenario { get => _afterScenario; private set => _afterScenario = value; }

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

        private void OnEnable()
        {
            Teleport.Player.Listen(OnTeleport);
        }

        private void OnDisable()
        {
            Teleport.Player.Remove(OnTeleport);
        }

        private void OnTeleport(TeleportMarkerBase _)
        {
            if(enabled && _isActive) {
                OnFeedback("skip");
                Hide();
            }
        }

        public void Show(FeedbackEventType eventType)
        {
            if (!_enabled || _isActive) return;

            _isActive = true;

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
            if (!_enabled || !_isActive) return;

            _isActive = false;

            transform.DOScale(0, .3f).SetEase(Ease.InCubic).OnComplete(() => gameObject.SetActive(false)).Play();
            _stopwatch.Reset();
        }
    }
}