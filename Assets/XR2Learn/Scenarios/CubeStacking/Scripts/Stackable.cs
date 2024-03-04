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
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace XR2Learn.Scenarios.CubeStacking
{
    [Serializable]
    public class StackEvent : UnityEvent<Stackable, Stackable>
    {
    }

    [RequireComponent(typeof(Interactable))]
    public class Stackable : MonoBehaviour
    {
        [SerializeField] private string objectTag;
        [SerializeField] private Transform spawnHeight;

        public StackEvent objectStacked;
        public StackEvent objectUnstacked;
        public event EventHandler<Stackable> ObjectReleased;
        
        public Stackable Above { get; private set; }

        public Stackable Below { get; private set; }

        public Vector3 OriginalPosition { get; private set; }
        public Vector3 OriginalScale { get; private set; }

        private void Awake()
        {
            GetComponent<Interactable>().onDetachedFromHand += _ => ObjectReleased?.Invoke(this, this);
            OriginalPosition = transform.position;
            OriginalScale = transform.localScale;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag(objectTag)) return;

            Vector3 distance = transform.position - other.transform.position;
            float angle = Vector3.Angle(Vector3.down, distance);
            switch (angle)
            {
                case < 45:
                    Above = other.gameObject.GetComponent<Stackable>();
                    objectStacked?.Invoke(this, other.gameObject.GetComponent<Stackable>());
                    break;
                case > 135:
                    Below = other.gameObject.GetComponent<Stackable>();
                    break;
            }
// #if DEBUG
//             Debug.Log(
//                 $"[{GetType().Name}({gameObject.name})] Stacked: {(Below ? Below.gameObject.name : "None")} -> {gameObject.name} -> {(Above ? Above.gameObject.name : "None")}");
// #endif
        }

        private void OnCollisionExit(Collision other)
        {
            if (!other.gameObject.CompareTag(objectTag)) return;

            if (Above != null && other.gameObject == Above.gameObject)
            {
                Above = null;
                objectUnstacked?.Invoke(this, other.gameObject.GetComponent<Stackable>());
            }

            if (Below != null && other.gameObject == Below.gameObject)
            {
                Below = null;
            }
            // #if DEBUG
            //             Debug.Log(
            //                 $"[{GetType().Name}({gameObject.name})] Unstacked: {(Below ? Below.gameObject.name : "None")} -> {gameObject.name} -> {(Above ? Above.gameObject.name : "None")}");
            // #endif
        }

        private void OnCollisionStay(Collision other)
        {
            if (!other.gameObject.CompareTag(objectTag)) return;

            Vector3 distance = transform.position - other.transform.position;
            float angle = Vector3.Angle(Vector3.down, distance);
            switch (angle)
            {
                case < 45:
                    Above = other.gameObject.GetComponent<Stackable>();
                    objectStacked?.Invoke(this, other.gameObject.GetComponent<Stackable>());
                    break;
                case > 135:
                    Below = other.gameObject.GetComponent<Stackable>();
                    break;
            }
            // #if DEBUG
            //             Debug.Log(
            //                 $"[{GetType().Name}({gameObject.name})] Stacked: {(Below ? Below.gameObject.name : "None")} -> {gameObject.name} -> {(Above ? Above.gameObject.name : "None")}");
            // #endif
        }

        public void Reset()
        {
            if (isActiveAndEnabled)
            {
#if DEBUG
                Debug.LogWarning($"[{GetType().Name} ({gameObject.name})] Stackable must be disabled before calling Reset().");
#endif
                return;
            }

            Above = null;
            Below = null;
            
            Transform t = transform;
            Vector3 pos = new Vector3(OriginalPosition.x, spawnHeight.position.y + OriginalScale.y, OriginalPosition.z);
            
            t.position = pos;
            t.localRotation = Quaternion.identity;
        }
        
        public async void RespawnAnimated()
        {
            await DespawnAnimated();

            Reset();

            await SpawnAnimated();
        }

        public async Task DespawnAnimated()
        {
            
            Collider c = GetComponent<Collider>();
            Rigidbody rb = GetComponent<Rigidbody>();
            Interactable i = GetComponent<Interactable>();

            c.enabled = false;
            rb.isKinematic = true;
            if (i.attachedToHand)
                i.attachedToHand.DetachObject(gameObject);


            await transform.DOScale(Vector3.zero, 0.3f).AsyncWaitForCompletion();

            gameObject.SetActive(false);
        }

        public void DespawnImmediate()
        {
            Interactable i = GetComponent<Interactable>();
            if (i.attachedToHand)
                i.attachedToHand.DetachObject(gameObject);

            gameObject.SetActive(false);
        }

        public bool IsAttachedToHand()
        {
            Hand hand = GetComponent<Interactable>().attachedToHand;
            return hand != null;
        }
        
        public async Task SpawnAnimated()
        {
            Collider c = GetComponent<Collider>();
            Rigidbody rb = GetComponent<Rigidbody>();
            
            gameObject.SetActive(false);
            
            Reset();
            
            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
            await transform.DOScale(OriginalScale, 0.3f).AsyncWaitForCompletion();

            c.enabled = true;
            rb.isKinematic = false;
        }

        public void SpawnImmediate()
        {
            transform.localScale = OriginalScale;
            gameObject.SetActive(true);
        }
    }
}