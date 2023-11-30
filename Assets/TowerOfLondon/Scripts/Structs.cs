using System;
using System.Collections;
using System.Collections.Generic;
using Common.Scripts;
using UnityEngine;

namespace TowerOfLondon.Scripts {
    [Serializable]
    public struct SolutionDiscs {
        [SerializeField] private Transform red;
        [SerializeField] private Transform green;
        [SerializeField] private Transform blue;

        public Transform Red => red;
        public Transform Green => green;
        public Transform Blue => blue;
    }

    [Serializable]
    public struct Discs : IEnumerable<Placeable> {
        [SerializeField] private Placeable red;
        [SerializeField] private Placeable green;
        [SerializeField] private Placeable blue;

        public Placeable Red => red;
        public Placeable Green => green;
        public Placeable Blue => blue;

        public IEnumerator<Placeable> GetEnumerator() {
            yield return red;
            yield return green;
            yield return blue;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    [Serializable]
    public struct Piles : IEnumerable<Pile> {
        [SerializeField] private Pile onePile;
        [SerializeField] private Pile twoPile;
        [SerializeField] private Pile threePile;

        public Pile OnePile => onePile;
        public Pile TwoPile => twoPile;
        public Pile ThreePile => threePile;

        public IEnumerator<Pile> GetEnumerator() {
            yield return onePile;
            yield return twoPile;
            yield return threePile;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}