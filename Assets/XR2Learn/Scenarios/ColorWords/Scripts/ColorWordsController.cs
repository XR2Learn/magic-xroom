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


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using Valve.VR.InteractionSystem;
using XR2Learn.Common;
using XR2Learn.Common.Feedback;
using Random = UnityEngine.Random;

namespace XR2Learn.Scenarios.ColorChoose
{
    public class ColorWordsController : ProgressableGame
    {
        [SerializeField] private List<MeshRenderer> meshRenderers;
        [SerializeField] private List<Interactable> interactables;
        [SerializeField] private TextMeshProUGUI colorText;
        [SerializeField] private LocalizeStringEvent timerText;
        [SerializeField] private List<ColorEntry> colorMap;
        [SerializeField] public int startingTimeLimit;
        [SerializeField] public int winStreakTimePenalty;
        [SerializeField] public int maxWins;
        [SerializeField] public GameObject tableHeight;

        private readonly List<Vector3> _initPosition = new List<Vector3>();
        private readonly List<Quaternion> _initRotation = new List<Quaternion>();

        private MeshRenderer _chosenCube;
        private int _winCount;
        private int _currentlyActive;
        private Color _initColor;
        private LocalizeStringEvent _colorLocalized;

        private double _startTime;
        private double _timeLimit;

        private void Awake()
        {
            _colorLocalized = colorText.GetComponent<LocalizeStringEvent>();

            // Save all positions that later will be resetted
            foreach (MeshRenderer t in meshRenderers)
            {
                _initPosition.Add(t.transform.position);
                _initRotation.Add(t.transform.rotation);
            }

            _initColor = meshRenderers.First().material.color;
            enabled = false;
        }

        public override void StartGame()
        {
            if (enabled) return;

            scenarioStarted?.Invoke("color_words");
            foreach (Interactable interactable in interactables)
            {
                interactable.onAttachedToHand += Attach;
            }
            SetupGame();
            enabled = true;
            _startTime = Time.time;
            _timeLimit = startingTimeLimit;
            levelStarted?.Invoke(_currentlyActive);
            StartCoroutine(TimerCoroutine());
        }

        public override void StopGame()
        {
            if (!enabled) return;

            scenarioEnded?.Invoke("color_words");
            ResetCubePositions();

            foreach (Interactable interactable in interactables)
            {
                interactable.onAttachedToHand -= Attach;
            }
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.material.color = _initColor;
            }

            if (enabled)
            {
                FeedbackBoard.Instance.Show(FeedbackBoard.FeedbackEventType.AFTER_SCENARIO);
                FeedbackBoard.Instance.Show(FeedbackBoard.FeedbackEventType.AFTER_LEVEL);
            }

            enabled = false;
        }

        private void Attach(Hand hand)
        {
            if (!enabled) return;

            if (IsCubeColorCorrect(hand.currentAttachedObject))
            {
                // Check if we reach the maximum wins to pass to the next level
                if (_winCount % maxWins == 0)
                {
                    if (_currentlyActive < meshRenderers.Count)
                    {
                        // Activate next cube
                        meshRenderers[_currentlyActive].enabled = true;
                        _currentlyActive += 1;
                    }
                    else
                    {
                        _timeLimit -= winStreakTimePenalty;
                    }
                }

                RandomizeCubeColors();

                // Choose new winning cube
                _chosenCube = meshRenderers[Random.Range(0, _currentlyActive)];
                _colorLocalized.StringReference = colorMap.Find(c => c.Color == _chosenCube.material.color).LocalizedString;
                // Randomize next text color
                colorText.color = meshRenderers[Random.Range(0, _currentlyActive)].material.color;
                _winCount += 1;
                _startTime = Time.time;
                levelCompleted?.Invoke(_currentlyActive);
            }
            else
            {
                levelFailed?.Invoke(_currentlyActive);
                StopGame();
            }
        }

        private void Detach(Hand hand)
        {
            ResetCubePositions();
        }

        private bool IsCubeColorCorrect(GameObject attached)
        {
            return attached.GetComponent<MeshRenderer>().material.color == _chosenCube.material.color;
        }

        private void ResetCubePositions()
        {
            // Reset positions
            for (int i = 0; i < meshRenderers.Count; i++)
            {
                _initPosition[i] = new Vector3(_initPosition[i].x, tableHeight.transform.position.y, _initPosition[i].z);
                meshRenderers[i].gameObject.transform.position = _initPosition[i];
                meshRenderers[i].gameObject.transform.rotation = _initRotation[i];
            }
        }

        private void RandomizeCubeColors()
        {
            // Randomize all color cubes
            foreach (MeshRenderer t in meshRenderers.Where(t => t.enabled))
                t.material.color = colorMap[Random.Range(0, colorMap.Count)].Color;
        }

        private void SetupGame()
        {
            // Choose a random color for the cubes
            foreach (MeshRenderer t in meshRenderers)
            {
                t.material.color = colorMap[Random.Range(0, colorMap.Count)].Color;
            }

            // Disable all meshes avoiding the first 2
            for (int i = 2; i < meshRenderers.Count; i++)
            {
                meshRenderers[i].enabled = false;
            }

            // Choose from the currently active cubes the right one
            _currentlyActive = 2;
            _chosenCube = meshRenderers[Random.Range(0, _currentlyActive)];
            _colorLocalized.StringReference = colorMap.Find(c => c.Color == _chosenCube.material.color).LocalizedString;
            // Choose a random text color between active cube colors
            colorText.color = meshRenderers[Random.Range(0, _currentlyActive)].material.color;
        }

        private IEnumerator TimerCoroutine()
        {
            double elapsed;
            while ((elapsed = Time.time - _startTime) < _timeLimit && enabled)
            {
                UpdateTimer(_timeLimit - elapsed);
                yield return new WaitForSeconds(0.1f);
            }

            UpdateTimer(0f);
            levelFailed?.Invoke(_currentlyActive);

            if (enabled)
                StopGame();
        }

        private void UpdateTimer(double value)
        {
            DoubleVariable time;
            // Show the remaining time
            if (!timerText.StringReference.TryGetValue("time", out IVariable timeVar))
            {
                time = new DoubleVariable();
                timerText.StringReference.Add("time", time);
            }
            else
            {
                time = timeVar as DoubleVariable;
            }

            time!.Value = value;
        }

        private void Update()
        {
            ResetCubePositions();
        }
    }
}