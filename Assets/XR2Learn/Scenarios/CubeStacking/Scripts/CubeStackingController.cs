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


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using XR2Learn.Common;
using XR2Learn.Common.Feedback;

namespace XR2Learn.Scenarios.CubeStacking
{
    public class CubeStackingController : ProgressableGame
    {
        [Header("Configuration")]
        [SerializeField]
        private List<StackingLevel> levels;

        [SerializeField] private int timeBetweenLevels = 3;

        [Header("UI")][SerializeField] private CanvasGroup introScreen;
        [SerializeField] private CanvasGroup levelScreen;
        [SerializeField] private CanvasGroup intermissionScreen;
        [SerializeField] private CanvasGroup retryScreen;
        [SerializeField] private CanvasGroup waitingScreen;
        [SerializeField] private CanvasGroup finishScreen;
        [SerializeField] private LocalizeStringEvent hintText;
        [SerializeField] private LocalizeStringEvent timerText;

        private CanvasGroup _activeCanvasGroup;

        private int _currentLevel = 0;
        private int _attempts = 0;
        private bool _started = false;

        private void Awake()
        {
            foreach (StackingLevel level in levels) level.gameObject.SetActive(false);
            introScreen.gameObject.SetActive(true);
            introScreen.alpha = 1f;

            _activeCanvasGroup = introScreen;
        }

        private async Task ScreenTransition(CanvasGroup to)
        {
            await DOTween.Sequence()
                .Insert(0, _activeCanvasGroup.DOFade(0, .15f))
                .Insert(0, _activeCanvasGroup.transform.DOScale(0, .15f))
                .AsyncWaitForCompletion();
            _activeCanvasGroup.gameObject.SetActive(false);
            to.transform.localScale = Vector3.zero;
            to.gameObject.SetActive(true);
            await DOTween.Sequence()
                .Insert(0, to.DOFade(1, .15f))
                .Insert(0, to.transform.DOScale(1, .15f))
                .AsyncWaitForCompletion();
            _activeCanvasGroup = to;
        }

        public void OnStackComplete()
        {
        }

        public async void OnStackCollapsed()
        {
            if (waitingScreen.isActiveAndEnabled)
                await ScreenTransition(levelScreen);
        }

        public async void OnStackWaiting()
        {
            await ScreenTransition(waitingScreen);
        }

        public void OnTimerUpdated(double time)
        {
            DoubleVariable timeVar = null;
            if (!timerText.StringReference.TryGetValue("time", out IVariable variable))
            {
                timeVar = new DoubleVariable();
                timerText.StringReference.Add("time", timeVar);
            }
            else
            {
                timeVar = variable as DoubleVariable;
            }

            timeVar!.Value = time;
        }

        public async void OnLevelComplete()
        {
            FeedbackBoard.Instance.Show(FeedbackBoard.FeedbackEventType.AFTER_LEVEL);
            levelCompleted?.Invoke(_currentLevel);

            if (_currentLevel >= levels.Count - 1)
            {
                levels[_currentLevel].Despawn();
                await ScreenTransition(finishScreen);
                return;
            }

            await ScreenTransition(intermissionScreen);

            levels[_currentLevel].Despawn();

            await UniTask.Delay(TimeSpan.FromSeconds(timeBetweenLevels));

            _attempts = 0;
            _currentLevel++;

            hintText.StringReference = levels[_currentLevel].UIHint;
            await ScreenTransition(levelScreen);

            levels[_currentLevel].StartLevel();
            levelStarted?.Invoke(_currentLevel);
        }

        public async void OnLevelFailed()
        {
            levelFailed?.Invoke(_currentLevel);
            await ScreenTransition(retryScreen);
            levels[_currentLevel].Despawn();
            await UniTask.Delay(TimeSpan.FromSeconds(timeBetweenLevels));
            _attempts++;
            await ScreenTransition(levelScreen);
            levels[_currentLevel].StartLevel();
            levelStarted?.Invoke(_currentLevel);
        }

        public async override void StartGame()
        {
            scenarioStarted?.Invoke("cube_stacking");
            if (_started) return;
            hintText.StringReference = levels[_currentLevel].UIHint;
            await ScreenTransition(levelScreen);
            _started = true;
            _currentLevel = 0;
            _attempts = 0;
            levels[_currentLevel].StartLevel();
            levelStarted?.Invoke(_currentLevel);
        }

        public async override void StopGame()
        {
            if (!_started) return;

            scenarioEnded?.Invoke("cube_stacking");

            StopAllCoroutines();
            levels[_currentLevel].StopAllCoroutines();

            await ScreenTransition(introScreen);
            levels[_currentLevel].Despawn();

            if (_started)
            {
                FeedbackBoard.Instance.Show(FeedbackBoard.FeedbackEventType.AFTER_SCENARIO);
            }

            _started = false;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("cube") || !other.GetComponent<Stackable>()) return;

            other.GetComponent<Stackable>().RespawnAnimated();
        }
    }
}