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
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using XR2Learn.Common;
using XR2Learn.Common.Feedback;

namespace XR2Learn.Scenarios.CanvasPainter
{
    public class CanvasPainterController : ProgressableGame
    {
        [Header("Game logic components")]
        [SerializeField]
        private PaintableImage paintableImage;

        [FormerlySerializedAs("referenceImages")]
        [SerializeField]
        private List<ReferenceImage> levels;

        [Header("Screens")][SerializeField] private CanvasGroup instructionsScreen;
        [SerializeField] private CanvasGroup intermissionScreen;
        [SerializeField] private CanvasGroup mainGameScreen;
        [SerializeField] private CanvasGroup slidersContainer;
        [SerializeField] private CanvasGroup timerContainer;
        [SerializeField] private CanvasGroup timeUpContainer;

        [Header("Game state UI")]
        [SerializeField]
        private Slider coverageSlider;

        [SerializeField] private Slider overflowSlider;
        [SerializeField] private Slider foulSlider;
        [SerializeField] private Timer timer;

        private int _level;
        private bool _isActive;
        private bool _isStopping;

        private void Start()
        {
            instructionsScreen.gameObject.SetActive(true);
            intermissionScreen.gameObject.SetActive(false);
            mainGameScreen.gameObject.SetActive(false);
            slidersContainer.gameObject.SetActive(false);
            timerContainer.gameObject.SetActive(false);
            timeUpContainer.gameObject.SetActive(false);
        }

        public override async void StartGame()
        {
            _isStopping = false;
            scenarioStarted?.Invoke("canvas_painter");
            _level = 0;
            paintableImage.SetImage(levels[_level]);
            await DoTransition(
                new List<CanvasGroup> { instructionsScreen },
                new List<CanvasGroup> { intermissionScreen }
            );
            await UniTask.Delay(TimeSpan.FromSeconds(3));

            if (_isStopping) return;
            
            await DoTransition(new List<CanvasGroup> { intermissionScreen },
                new List<CanvasGroup> { mainGameScreen, slidersContainer, timerContainer });
            
            paintableImage.StartGame();
            _isActive = true;
            levelStarted?.Invoke(_level);
            StartCoroutine(TimerCoroutine(levels[_level].TimeLimit));
        }

        public override async void StopGame()
        {
            if (!_isActive) return;

            scenarioEnded?.Invoke("canvas_painter");
            StopAllCoroutines();
            _isStopping = true;
            paintableImage.StopGame();

            timeUpContainer.GetComponentInChildren<TextMeshProUGUI>().text = "GAME OVER";
            await DoTransition(
                new List<CanvasGroup> { intermissionScreen, timerContainer, timeUpContainer, slidersContainer, mainGameScreen },
                new List<CanvasGroup> { instructionsScreen }
            );
            paintableImage.Cleanup();
        }

        private async void NextLevel(bool isOverFaultLimit)
        {
            if (isOverFaultLimit || paintableImage.Coverage * 100 < levels[_level].CoverageTarget)
            {
                levelFailed?.Invoke(_level);
                timeUpContainer.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
            else
            {
                timeUpContainer.GetComponentInChildren<TextMeshProUGUI>().text = "GOOD JOB!";
                levelCompleted?.Invoke(_level);
            }

            paintableImage.StopGame();
            _level++;

            if (_level == levels.Count)
            {
                FeedbackBoard.Instance.Show(FeedbackBoard.FeedbackEventType.AFTER_SCENARIO);
            }
            else
            {
                FeedbackBoard.Instance.Show(FeedbackBoard.FeedbackEventType.AFTER_LEVEL);
            }

            timerContainer.gameObject.SetActive(false);
            timeUpContainer.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            paintableImage.Cleanup();

            if (_isStopping) return;

            if (_level == levels.Count)
            {
                timeUpContainer.GetComponentInChildren<TextMeshProUGUI>().text = "GAME OVER";
                await DoTransition(
                    new List<CanvasGroup> { timeUpContainer, slidersContainer, mainGameScreen },
                    new List<CanvasGroup> { instructionsScreen }
                );
                scenarioEnded?.Invoke("canvas_painter");

                return;
            }

            paintableImage.SetImage(levels[_level]);
            await DoTransition(
                new List<CanvasGroup> { timeUpContainer, slidersContainer, mainGameScreen },
                new List<CanvasGroup> { intermissionScreen }
            );

            await UniTask.Delay(TimeSpan.FromSeconds(3));
            if (_isStopping) return;

            await DoTransition(new List<CanvasGroup> { intermissionScreen },
                new List<CanvasGroup> { mainGameScreen, slidersContainer, timerContainer });

            paintableImage.StartGame();
            _isActive = true;
            levelStarted?.Invoke(_level);
            StartCoroutine(TimerCoroutine(levels[_level].TimeLimit));
        }

        private static async Task DoTransition(List<CanvasGroup> from, List<CanvasGroup> to)
        {
            Sequence fromSequence = DOTween.Sequence();
            foreach (CanvasGroup f in from)
            {
                f.alpha = 1;
                f.transform.localScale = Vector3.one;
                fromSequence
                    .Insert(0, f.transform.DOScale(0, .3f).SetEase(Ease.OutQuart))
                    .Insert(0, f.DOFade(0, .3f).SetEase(Ease.OutQuart));
            }

            await fromSequence.Play().AsyncWaitForCompletion();
            foreach (CanvasGroup f in from) f.gameObject.SetActive(false);

            Sequence toSequence = DOTween.Sequence();
            foreach (CanvasGroup t in to)
            {
                t.gameObject.SetActive(true);
                t.alpha = 0;
                t.transform.localScale = Vector3.zero;
                toSequence
                    .Insert(0, t.transform.DOScale(1, .3f).SetEase(Ease.OutQuart))
                    .Insert(0, t.DOFade(1, .3f).SetEase(Ease.OutQuart));
            }

            await toSequence.Play().AsyncWaitForCompletion();
        }

        private void LateUpdate()
        {
            if (!_isActive) return;
            coverageSlider.value = paintableImage.Coverage;
            overflowSlider.value = paintableImage.Overflow;
            foulSlider.value = 1.0f - 1.0f * paintableImage.MissCount / levels[_level].FoulLimit;

            if (paintableImage.MissCount >= levels[_level].FoulLimit)
            {
                _isActive = false;
                NextLevel(true);
            }
            else if (paintableImage.Coverage * 100 >= levels[_level].CoverageTarget)
            {
                _isActive = false;
                NextLevel(false);
            }
        }

        private IEnumerator TimerCoroutine(double time)
        {
            double start = Time.time;
            double elapsed;
            timer.MaxValue = time;
            while ((elapsed = Time.time - start) < time && _isActive)
            {
                timer.CurrentValue = time - elapsed;
                yield return new WaitForSeconds(.5f);
            }

            if (!_isActive) yield break;
            _isActive = false;
            NextLevel(true);
        }
    }
}