using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

namespace TowerOfLondon.Scripts {
    public class UIController : MonoBehaviour {
        [Header("Intro UI")] [SerializeField] private GameObject introUI;

        [Header("In-Game UI")] [SerializeField]
        private GameObject inGameUI;

        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI movesText;
        [SerializeField] private List<Image> difficultyIcons;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Image timerFill;
        [SerializeField] private AudioSource timerTickSound;
        [SerializeField] private float timerUpdateStep;
        [SerializeField] private float timerFastTickThreshold;

        [Header("Before level UI")] [SerializeField]
        private GameObject beforeLevelUI;

        [SerializeField] private TextMeshProUGUI countdownText;

        [Header("It's over")] [SerializeField] private GameObject itsOver;

        private LocalizedString _levelStringRef;
        private LocalizedString _movesStringRef;

        private bool _inGame;
        private double _lastTimerUpdate;
        private double _lastTimerTick;

        private void Awake() {
            _levelStringRef = levelText.GetComponent<LocalizeStringEvent>().StringReference;
            _movesStringRef = movesText.GetComponent<LocalizeStringEvent>().StringReference;

            introUI.SetActive(true);
            beforeLevelUI.SetActive(false);
            inGameUI.SetActive(false);
            itsOver.SetActive(false);
        }

        public void OnTimerUpdate(double elapsed, double total) {
            
            if (_inGame) {
                double time = Time.time;
                if (time - _lastTimerUpdate > timerUpdateStep) {
                    float remaining = (float)(total - elapsed);
                    float progress = (float)(remaining / total);
                    timerText.text = $"{remaining:F0}";
                    timerFill.fillAmount = progress;
                    timerFill.color = progress > .33f
                        ? Color.Lerp(new Color(.9f, .9f, .0f), new Color(.0f, .702f, .0f), (progress - .33f) * 3)
                        : Color.Lerp(new Color(.702f, .0f, .0f), new Color(.9f, .9f, .0f), progress * 3);
                    _lastTimerUpdate = time;
                }

                if (total - elapsed > timerFastTickThreshold) {
                    if (time - _lastTimerTick > timerUpdateStep) {
                        timerTickSound.Play();
                        _lastTimerTick = time;
                    }
                }
                else {
                    if (time - _lastTimerTick > timerUpdateStep / 2) {
                        timerTickSound.Play();
                        _lastTimerTick = time;
                    }
                }
            }
            else {
                float remaining = (float)(total - elapsed);
                countdownText.text = $"{remaining:F0}";
            }
        }

        public void OnLevelLoaded(int number, Level level) {
            _inGame = false;
            introUI.SetActive(false);
            beforeLevelUI.SetActive(true);
            inGameUI.SetActive(false);
            itsOver.SetActive(false);

            if (_levelStringRef.TryGetValue("level", out IVariable levelVar)) {
                if (levelVar is IntVariable levelNum)
                    levelNum.Value = number;
            }

            if (_movesStringRef.TryGetValue("moves", out IVariable movesVar)) {
                if (movesVar is IntVariable moves)
                    moves.Value = level.maxMoves;
            }

            for (int i = 0; i < difficultyIcons.Count; i++)
                difficultyIcons[i].gameObject.SetActive(i < level.difficulty);

            timerFill.fillAmount = 1.0f;
            timerFill.color = new Color(.0f, .702f, .0f);
        }

        public void OnLevelStarted() {
            _inGame = true;
            introUI.SetActive(false);
            beforeLevelUI.SetActive(false);
            inGameUI.SetActive(true);
            itsOver.SetActive(false);
        }

        public void OnTestCompleted() {
            _inGame = false;
            introUI.SetActive(false);
            beforeLevelUI.SetActive(false);
            inGameUI.SetActive(false);
            itsOver.SetActive(true);
        }

        public void OnGameStopped()
        {
            introUI.SetActive(true);
            beforeLevelUI.SetActive(false);
            inGameUI.SetActive(false);
            itsOver.SetActive(false);
        }
    }
}