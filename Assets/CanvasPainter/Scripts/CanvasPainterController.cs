using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CanvasPainter
{
    public class CanvasPainterController : ProgressableGame
    {
        [Header("Game logic components")] [SerializeField]
        private PaintableImage paintableImage;

        [FormerlySerializedAs("referenceImages")] [SerializeField]
        private List<ReferenceImage> levels;

        [Header("Screens")] [SerializeField] private CanvasGroup instructionsScreen;
        [SerializeField] private CanvasGroup intermissionScreen;
        [SerializeField] private CanvasGroup mainGameScreen;
        [SerializeField] private CanvasGroup slidersContainer;
        [SerializeField] private CanvasGroup timerContainer;
        [SerializeField] private CanvasGroup timeUpContainer;

        [Header("Game state UI")] [SerializeField]
        private Slider coverageSlider;

        [SerializeField] private Slider overflowSlider;
        [SerializeField] private Slider foulSlider;
        [SerializeField] private Timer timer;

        private int _level;
        private bool _isActive;

        private void Start()
        {
            instructionsScreen.gameObject.SetActive(true);
            intermissionScreen.gameObject.SetActive(false);
            mainGameScreen.gameObject.SetActive(false);
            slidersContainer.gameObject.SetActive(false);
            timerContainer.gameObject.SetActive(false);
            timeUpContainer.gameObject.SetActive(false);
        }

        public async void StartGame()
        {
            scenarioStarted?.Invoke("canvas_painter");
            _level = 0;
            paintableImage.SetImage(levels[_level]);
            await DoTransition(
                new List<CanvasGroup> { instructionsScreen },
                new List<CanvasGroup> { intermissionScreen }
            );
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            await DoTransition(new List<CanvasGroup> { intermissionScreen },
                new List<CanvasGroup> { mainGameScreen, slidersContainer, timerContainer });
            paintableImage.StartGame();
            _isActive = true;
            levelStarted?.Invoke(_level);
            StartCoroutine(TimerCoroutine(levels[_level].TimeLimit));
        }

        public async void StopGame()
        {
            scenarioEnded?.Invoke("canvas_painter");
            paintableImage.StopGame();
            await DoTransition(
                new List<CanvasGroup> { intermissionScreen, timerContainer, timeUpContainer, slidersContainer, mainGameScreen },
                new List<CanvasGroup> { instructionsScreen }
            );
            paintableImage.Cleanup();
            FeedbackBoard.Instance.Show(FeedbackBoard.FeedbackEventType.AFTER_SCENARIO);
        }

        private async void NextLevel(bool isOverFaultLimit)
        {
            if (isOverFaultLimit || paintableImage.Coverage * 100 < levels[_level].CoverageTarget)
                levelFailed?.Invoke(_level);
            else
                levelCompleted?.Invoke(_level);

            paintableImage.StopGame();
            _level++;
            timerContainer.gameObject.SetActive(false);
            timeUpContainer.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            paintableImage.Cleanup();
            
            if (_level == levels.Count)
            {
                await DoTransition(
                    new List<CanvasGroup> { timeUpContainer, slidersContainer, mainGameScreen },
                    new List<CanvasGroup> { instructionsScreen }
                );
                return;
            }

            paintableImage.SetImage(levels[_level]);
            await DoTransition(
                new List<CanvasGroup> { timeUpContainer, slidersContainer, mainGameScreen },
                new List<CanvasGroup> { intermissionScreen }
            );
            await UniTask.Delay(TimeSpan.FromSeconds(3));
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

            if (paintableImage.MissCount >= levels[_level].FoulLimit || paintableImage.Coverage * 100 >= levels[_level].CoverageTarget)
            {
                FeedbackBoard.Instance.Show(FeedbackBoard.FeedbackEventType.AFTER_LEVEL);
                _isActive = false;
                NextLevel(true);
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
            NextLevel(false);
        }
    }
}