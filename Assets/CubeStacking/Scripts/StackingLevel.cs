using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;
using Valve.VR.InteractionSystem;

namespace StackingGame
{
    [Serializable]
    public class StackingLevelEvent : UnityEvent<StackingLevel>
    {
    }

    [Serializable]
    public class TimerEvent : UnityEvent<double>
    {
    }

    public class StackingLevel : MonoBehaviour
    {
        [SerializeField] private List<Stackable> stackables;

        [Tooltip(
            "If enabled, cubes will need to be stacked bottom to top in the same order as they're defined in the list.")]
        [SerializeField]
        private bool enforcedOrder;

        [SerializeField] private double timeLimit;
        [SerializeField] private LocalizedString uiHint;

        public UnityEvent stackCompleted;
        public UnityEvent stackCollapsed;
        public UnityEvent stackWaiting;
        public StackingLevelEvent levelCompleted;
        public StackingLevelEvent levelFailed;
        public TimerEvent timerUpdated;


        private bool _isActive;

        private bool _stackCompleted;

        public IEnumerable<Stackable> Stackables => stackables.AsReadOnly();
        public LocalizedString UIHint => uiHint;

        public async void StartLevel()
        {
            _stackCompleted = false;
            _isActive = true;
            await Spawn();
            StartCoroutine(TimerCoroutine());
        }

        private IEnumerator TimerCoroutine()
        {
            double start = Time.time;
            double elapsed;
            while ((elapsed = Time.time - start) < timeLimit && _isActive)
            {
                timerUpdated?.Invoke(timeLimit - elapsed);
                yield return new WaitForSeconds(0.1f);
            }

            if (!_isActive) yield break;

            _isActive = false;
            levelFailed?.Invoke(this);
        }

        private IEnumerator WaitingCoroutine()
        {
#if DEBUG
            Debug.Log($"[{GetType().Name} ({{gameObject.name}})] Waiting for stability...", this);
#endif
            stackWaiting?.Invoke();
            yield return new WaitForSeconds(2f);
            if (!_isActive || !_stackCompleted) yield break;
#if DEBUG
            Debug.Log($"[{GetType().Name} ({{gameObject.name}})] Tower stable, level complete!", this);
#endif
            Debug.Log("LEVEL COMPLETED _isActive = false");
            _isActive = false;
            levelCompleted?.Invoke(this);
        }

#pragma warning disable CS4014
        public async Task Spawn()
        {
            gameObject.SetActive(true);
            foreach (Stackable stackable in stackables)
            {
                stackable.DespawnImmediate();
                stackable.Reset();
            }

            foreach (Stackable stackable in stackables)
            {
                stackable.SpawnAnimated();
                await UniTask.Delay(TimeSpan.FromSeconds(.15f));
            }
        }

        public async void Despawn()
        {
            _isActive = false;
            foreach (Stackable stackable in stackables)
            {
                stackable.ObjectReleased -= OnLastStackableReleased;
                stackable.DespawnAnimated();
                await UniTask.Delay(TimeSpan.FromSeconds(.15f));
            }

            gameObject.SetActive(false);
        }
#pragma warning restore CS4014

        public void OnObjectStacked(Stackable bottom, Stackable top)
        {
            if (!_isActive) return;
            if (!CheckStackComplete()) return;

// #if DEBUG
//             Debug.Log($"[{GetType().Name} ({gameObject.name})] Stack completed...", this);
// #endif

            if (top.IsAttachedToHand())
            {
                top.ObjectReleased += OnLastStackableReleased;
            }
            else
            {
                StartCoroutine(WaitingCoroutine());
                _stackCompleted = true;
                stackCompleted?.Invoke();
            }
        }

        public void OnObjectUnstacked(Stackable bottom, Stackable top)
        {
            if (!_stackCompleted || !_isActive) return;
// #if DEBUG
//             Debug.Log($"[{GetType().Name} ({gameObject.name})] Tower collapsed.", this);
// #endif

            _stackCompleted = false;
            stackCollapsed?.Invoke();
        }

        private void OnLastStackableReleased(object _, Stackable stackable)
        {
            stackable.ObjectReleased -= OnLastStackableReleased;
#if DEBUG
            Debug.Log($"[{GetType().Name} ({gameObject.name})] Stackable released.", this);
#endif
            _stackCompleted = true;
            stackCompleted?.Invoke();
            StartCoroutine(WaitingCoroutine());
        }

        private bool CheckStackComplete()
        {
            if (enforcedOrder)
            {
                Stackable current = stackables[0];
                int i = 1;
                // first in the stack can't have any other stackables below it
                if (current.Below != null) return false;
                while (current.Above != null)
                {
                    if (current.Above != stackables[i]) return false;
                    current = current.Above;
                    i++;
                }

                return i == stackables.Count;
            }

            foreach (Stackable stackable in stackables)
            {
                if (stackable.Below != null) continue;
                Stackable current = stackable;
                int i = 1;
                while (current.Above != null)
                {
                    i++;
                    current = current.Above;
                }

                return i == stackables.Count;
            }

            return false;
        }
    }
}