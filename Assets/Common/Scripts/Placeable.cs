using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

namespace Common.Scripts {
    [Serializable]
    public class PlaceableEvent : UnityEvent<Placeable> { }

    [Serializable]
    public class PlaceableHandEvent : UnityEvent<Placeable, Hand> { }

    [AddComponentMenu("XR2Learn/Common/Placeable")]
    [RequireComponent(typeof(Throwable))]
    public class Placeable : MonoBehaviour {
        [SerializeField] private GameObject target;
        [SerializeField] private Color gizmoColor;

        public PlaceableEvent targetEntered;
        public PlaceableEvent targetExited;
        public PlaceableHandEvent pickedUp;
        public PlaceableHandEvent released;

        private Throwable _throwable;
        private Collider _collider;
        private Rigidbody _rigidbody;
        private Hand _hand;
        private Vector3 _pickUpLocation;

        public bool IsPlaced { get; private set; }

        public Throwable Throwable {
            get {
                if (!_throwable)
                    _throwable = GetComponent<Throwable>();
                return _throwable;
            }
        }

        public Rigidbody Rigidbody {
            get {
                if (!_rigidbody)
                    _rigidbody = GetComponent<Rigidbody>();
                return _rigidbody;
            }
        }

        public Collider Collider {
            get {
                if (!_collider)
                    _collider = GetComponent<Collider>();
                return _collider;
            }
        }

        public GameObject Target {
            get => target;
            set => target = value;
        }

        private void OnEnable() {
            Throwable.onPickUp?.AddListener(OnPickUp);
            Throwable.onDetachFromHand?.AddListener(OnDetachFromHand);
        }

        private void OnDisable() {
            Throwable.onPickUp?.RemoveListener(OnPickUp);
            Throwable.onDetachFromHand?.RemoveListener(OnDetachFromHand);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject != target) return;
            IsPlaced = true;
            targetEntered?.Invoke(this);
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject != target) return;
            IsPlaced = false;
            targetExited?.Invoke(this);
        }

        private void OnPickUp(Hand hand) {
            pickedUp?.Invoke(this, hand);
        }

        private void OnDetachFromHand(Hand hand) {
            released?.Invoke(this, hand);
        }

        public void MoveTo(Vector3 position) {
            transform.localPosition = position;
            transform.localRotation = Quaternion.identity;
        }

        public async UniTask MoveTo(Vector3 position, float duration, EasingFunction.Ease easing) {
            Rigidbody.isKinematic = true;
            float start = Time.time;
            float end = Time.time + duration;
            Vector3 startPos = transform.localPosition;
            Quaternion startRot = transform.localRotation;
            EasingFunction.Function func = EasingFunction.GetEasingFunction(easing);
            do {
                float progress = (Time.time - start) / (duration);
                float scaledProgress = func(0, 1, progress);
                transform.localPosition = Vector3.Lerp(startPos, position, scaledProgress);
                transform.localRotation = Quaternion.Lerp(startRot, Quaternion.identity, scaledProgress);
                await UniTask.NextFrame();
            } while (Time.time < end);

            Rigidbody.isKinematic = false;
        }

        public void MoveTargetTo(Vector3 position) {
            target.transform.localPosition = position;
        }

        private void OnDrawGizmosSelected() {
            if (!target || !target.GetComponent<Collider>()) return;
            Gizmos.color = gizmoColor;
            switch (target.GetComponent<Collider>()) {
                case BoxCollider box:
                    Gizmos.DrawWireCube(box.center, box.size);
                    break;
                case SphereCollider sphere:
                    Gizmos.DrawWireSphere(sphere.center, sphere.radius);
                    break;
                case MeshCollider mesh:
                    Transform t = mesh.transform;
                    Gizmos.DrawWireMesh(mesh.sharedMesh, t.position, t.rotation, t.lossyScale);
                    break;
                default: return;
            }
        }
    }
}