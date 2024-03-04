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
using UnityEngine;
using XR2Learn.Common;

namespace XR2Learn.Scenarios.TowerOfLondon
{
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