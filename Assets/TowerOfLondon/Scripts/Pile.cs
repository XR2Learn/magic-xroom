using System;
using System.Collections.Generic;
using System.Linq;
using Common.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace TowerOfLondon.Scripts {
    [Serializable]
    public class PileEvent : UnityEvent<Pile, Placeable> { }

    [AddComponentMenu("XR2Learn/Tower of London/Pile")]
    [RequireComponent(typeof(Collider))]
    public class Pile : MonoBehaviour {
        public PileEvent placeableEntered;
        public PileEvent placeableExited;

        [SerializeField] private List<Placeable> _discs = new List<Placeable>();

        public List<Placeable> Discs => _discs.OrderByDescending(d => d.transform.localPosition.y).ToList();

        private void OnTriggerEnter(Collider other) {
            Placeable p;
            if (!(p = other.GetComponent<Placeable>())) return;
            _discs.Add(p);
            //Debug.Log($"[{gameObject.name}] {p.gameObject.name} entered");
            placeableEntered?.Invoke(this, p);
        }

        private void OnTriggerExit(Collider other) {
            Placeable p;
            if (!(p = other.GetComponent<Placeable>())) return;
            _discs.Remove(p);
            //Debug.Log($"[{gameObject.name}] {p.gameObject.name} exited");
            placeableExited?.Invoke(this, p);
        }
    }
}