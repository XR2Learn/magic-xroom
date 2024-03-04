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
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using XR2Learn.Common;
using XR2Learn.Common.Feedback;

namespace XR2Learn.Scenarios.TowerOfLondon
{
    [Serializable]
    public class TimerEvent : UnityEvent<double, double> { }

    [Serializable]
    public class LevelEvent : UnityEvent<int, Level> { }

    [AddComponentMenu("XR2Learn/Tower of London/GameController")]
    public class TowerOfHanoiController : ProgressableGame {
        private class PlaceableHandleData {
            public Pile originalPile;
            public Pile pile;
        }

        [SerializeField] private Piles piles;
        [SerializeField] private Discs discs;
        [SerializeField] private SolutionDiscs solutionDiscs;
        [SerializeField] private int maxMoves;
        [SerializeField] private int waitTime;

        [SerializeField] private List<Level> levels = new List<Level>();

        public LevelEvent levelLoaded;
        public UnityEvent testCompleted;
        public UnityEvent gameStopped;
        public TimerEvent timerUpdate;

        private readonly Dictionary<Placeable, PlaceableHandleData> _pileMap = new();
        private int _moves;
        private int _currentLevel;
        private Coroutine _countdownCoroutine;
        private bool _started;
        private bool _active;

        public Discs Discs => discs;
        public Piles Piles => piles;
        public int MaxMoves => maxMoves;

        private void Awake() {
            foreach (Placeable disc in discs) _pileMap[disc] = new PlaceableHandleData();
        }

        public void OnDiscEnteredPile(Pile pile, Placeable disc) {
            _pileMap[disc].pile = pile;
        }

        public void OnDiscExitedPile(Pile pile, Placeable disc) {
            _pileMap[disc].pile = null;
        }

        public void OnDiscEnteredTarget(Placeable disc) { }

        public void OnDiscExitedTarget(Placeable disc) { }

        public void OnDiscPickedUp(Placeable disc, Hand hand)
        {
            if(!_active) return;
            _pileMap[disc].originalPile = _pileMap[disc].pile;
            Placeable first = _pileMap[disc].pile.Discs.First();
            if (first == null || first != disc) return;
            disc.Rigidbody.constraints = RigidbodyConstraints.None;
        }

        public async void OnDiscReleased(Placeable disc, Hand hand)
        {
            if (!_active) return;
            if (_pileMap[disc].pile != null && _pileMap[disc].pile.Discs.First() != disc) return;

            Vector3 pilePos;
            Vector3 snapPos;
            Pile p;
            if (_pileMap[disc].pile == null) {
                p = _pileMap[disc].originalPile;
                pilePos = p.transform.localPosition;
                snapPos = new Vector3(pilePos.x, .24f + p.Discs.Count * .16f, pilePos.z);
                await disc.MoveTo(snapPos, .15f, EasingFunction.Ease.EaseOutQuart);
                disc.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                return;
            }

            p = _pileMap[disc].pile;
            pilePos = p.transform.localPosition;
            snapPos = new Vector3(pilePos.x, .24f + (p.Discs.Count - 1) * .16f, pilePos.z);
            await disc.MoveTo(snapPos, .15f, EasingFunction.Ease.EaseOutQuart);
            disc.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            if (_pileMap[disc].pile == _pileMap[disc].originalPile)
                return;

            _moves += 1;

            if (discs.Count((x) => x.IsPlaced) == 3) {
                Debug.Log($"[{GetType().Name}] Level {_currentLevel} completed.");
                EndLevel(true);
            }
            else if (_moves >= maxMoves) {
                Debug.Log($"[{GetType().Name}] Level {_currentLevel} failed.");
                RestartLevel();
            }
        }

        public async override void StartGame()
        {
            scenarioStarted?.Invoke("tower_of_hanoi");
            if (_started) return;
            _started = true;
            _currentLevel = 0;
            await LoadLevel(levels[_currentLevel]);
        }

        public override void StopGame()
        {
            if (!_started) return;
            scenarioEnded?.Invoke("tower_of_hanoi");
            StopAllCoroutines();
            _started = false;
            _active = false;
            if(_countdownCoroutine != null) StopCoroutine(_countdownCoroutine);
            gameStopped?.Invoke();
            FeedbackBoard.Instance.Show(FeedbackBoard.FeedbackEventType.AFTER_SCENARIO);
        }

        private async void RestartLevel()
        {
            Level level = levels[_currentLevel];
            _moves = 0;

            await (
                discs.Red.MoveTo(level.redStart, .3f, EasingFunction.Ease.EaseOutQuart),
                discs.Green.MoveTo(level.greenStart, .3f, EasingFunction.Ease.EaseOutQuart),
                discs.Blue.MoveTo(level.blueStart, .3f, EasingFunction.Ease.EaseOutQuart),
                MoveSolutionDisc(solutionDiscs.Red, level.redTarget, .3f, EasingFunction.Ease.EaseOutQuart),
                MoveSolutionDisc(solutionDiscs.Green, level.greenTarget, .3f, EasingFunction.Ease.EaseOutQuart),
                MoveSolutionDisc(solutionDiscs.Blue, level.blueTarget, .3f, EasingFunction.Ease.EaseOutQuart)
            );

            foreach (Placeable disc in Discs) {
                disc.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        private async void EndLevel(bool completed)
        {
            FeedbackBoard.Instance.Show(FeedbackBoard.FeedbackEventType.AFTER_LEVEL);
            _active = false;
            
            if (_countdownCoroutine != null)
                StopCoroutine(_countdownCoroutine);

            if (completed)
            {
                levelCompleted?.Invoke(_currentLevel);
            }
            else
            {
                levelFailed?.Invoke(_currentLevel);
            }
            _currentLevel += 1;
            
            if(_currentLevel >= levels.Count)
                testCompleted?.Invoke();
            else
                await LoadLevel(levels[_currentLevel]);
        }

        public async Task LoadLevel(Level level)
        {
            discs.Red.MoveTargetTo(level.redTarget);
            discs.Green.MoveTargetTo(level.greenTarget);
            discs.Blue.MoveTargetTo(level.blueTarget);
            maxMoves = level.maxMoves;
            _moves = 0;

            levelLoaded?.Invoke(_currentLevel + 1, level);

            await CountdownCoroutine(waitTime).ToUniTask();
            if(_started == false) {
                return;
            }
            
            await (
                discs.Red.MoveTo(level.redStart, .3f, EasingFunction.Ease.EaseOutQuart),
                discs.Green.MoveTo(level.greenStart, .3f, EasingFunction.Ease.EaseOutQuart),
                discs.Blue.MoveTo(level.blueStart, .3f, EasingFunction.Ease.EaseOutQuart),
                MoveSolutionDisc(solutionDiscs.Red, level.redTarget, .3f, EasingFunction.Ease.EaseOutQuart),
                MoveSolutionDisc(solutionDiscs.Green, level.greenTarget, .3f, EasingFunction.Ease.EaseOutQuart),
                MoveSolutionDisc(solutionDiscs.Blue, level.blueTarget, .3f, EasingFunction.Ease.EaseOutQuart)
            );

            foreach (Placeable disc in Discs) {
                disc.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }

            _countdownCoroutine = StartCoroutine(LevelTimerCoroutine(level.time));
            _active = true;
            levelStarted?.Invoke(_currentLevel);
        }

        private async UniTask MoveSolutionDisc(Transform disc, Vector3 position, float duration, EasingFunction.Ease easing)
            {
            float start = Time.time;
            float end = Time.time + duration;
            Vector3 startPos = disc.localPosition;
            Quaternion startRot = disc.localRotation;
            EasingFunction.Function func = EasingFunction.GetEasingFunction(easing);
            do {
                float progress = (Time.time - start) / (duration);
                float scaledProgress = func(0, 1, progress);
                disc.localPosition = Vector3.Lerp(startPos, position, scaledProgress);
                disc.localRotation = Quaternion.Lerp(startRot, Quaternion.identity, scaledProgress);
                await UniTask.NextFrame();
            } while (Time.time < end);
        }

        private IEnumerator LevelTimerCoroutine(double time)
        {
            yield return CountdownCoroutine(time);
            _countdownCoroutine = null;
            EndLevel(false);
        }

        private IEnumerator CountdownCoroutine(double time)
        {
            double start = Time.time;
            double elapsed;
            while ((elapsed = Time.time - start) < time) {
                timerUpdate?.Invoke(elapsed, time);
                yield return new WaitForSeconds(0.1f);
            }

            timerUpdate?.Invoke(elapsed, time);
        }
    }
}