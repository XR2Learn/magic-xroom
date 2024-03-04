//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using System;
using UnityEngine;

namespace Valve.VR.Extras
{
    public class SteamVR_LaserPointer : MonoBehaviour
    {
        public SteamVR_Behaviour_Pose pose;

        public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

        public bool active = true;
        public Color color;
        public float thickness = 0.002f;
        public float maxDistance = 50f;
        public Color clickColor = Color.green;
        public GameObject pointerAnchor;
        public bool addRigidBody = false;
        public Transform reference;
        public LayerMask raycastLayers;
        
        public event PointerEventHandler PointerIn;
        public event PointerEventHandler PointerOut;
        public event PointerEventHandler PointerClick;
        public event PointerEventHandler PointerUp;
        public event PointerEventHandler PointerDown;
        public event PointerEventHandler PointerMove;
        
        private GameObject _pointer;
        private Transform _previousContact;
        private Transform _pointerDownContact;
        private Renderer _pointerRenderer;

        public bool Active
        {
            get => active;
            set => active = value;
        }

        public void Toggle(bool status) => active = status ^ active;

        private void Start()
        {
            if (pose == null)
                pose = GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.LogError("No SteamVR_Behaviour_Pose component found on this object", this);

            if (interactWithUI == null)
                Debug.LogError("No ui interaction action has been set on this component.", this);

            if (pointerAnchor == null)
            {
                pointerAnchor = new GameObject("PointerAnchor");
                pointerAnchor.transform.parent = transform;
                pointerAnchor.transform.localPosition = Vector3.zero;
                pointerAnchor.transform.localRotation = Quaternion.identity;
                pointerAnchor.transform.localScale = Vector3.one;
            }
            
            CreatePointer();
        }

        protected virtual void CreatePointer()
        {
            _pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _pointer.transform.parent = pointerAnchor.transform;
            _pointer.transform.localScale = new Vector3(thickness, thickness, 100.0f);
            _pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
            _pointer.transform.localRotation = Quaternion.identity;
            BoxCollider c = _pointer.GetComponent<BoxCollider>();

            if (addRigidBody)
            {
                if (c)
                {
                    c.isTrigger = true;
                }

                Rigidbody rigidBody = _pointer.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
            }
            else
            {
                if (c)
                {
                    Destroy(c);
                }
            }

            Material newMaterial = new Material(Shader.Find("Unlit/Color"));
            newMaterial.SetColor("_Color", color);
            _pointerRenderer = _pointer.GetComponent<MeshRenderer>();
            _pointerRenderer.material = newMaterial;
        }
        
        public virtual void OnPointerClick(PointerEventArgs e) => PointerClick?.Invoke(this, e);

        public virtual void OnPointerIn(PointerEventArgs e) => PointerIn?.Invoke(this, e);

        public virtual void OnPointerOut(PointerEventArgs e) => PointerOut?.Invoke(this, e);

        public virtual void OnPointerUp(PointerEventArgs e) => PointerUp?.Invoke(this, e);

        public virtual void OnPointerDown(PointerEventArgs e) => PointerDown?.Invoke(this, e);

        public virtual void OnPointerMove(PointerEventArgs e) => PointerMove?.Invoke(this, e);

        private void LateUpdate()
        {
            _pointer.SetActive(active);

            if (!active) return;

            float dist = maxDistance;

            Ray raycast = new Ray(transform.position, _pointer.transform.forward);
            RaycastHit hit;

            bool bHit = Physics.Raycast(raycast, out hit, maxDistance, raycastLayers);

            if (_previousContact && _previousContact != hit.transform)
            {
                PointerEventArgs args = new PointerEventArgs();
                args.fromInputSource = pose.inputSource;
                args.point = hit.point;
                args.distance = 0f;
                args.flags = 0;
                args.target = _previousContact;
                OnPointerOut(args);
                _previousContact = null;
            }

            if (bHit && _previousContact != hit.transform)
            {
                PointerEventArgs argsIn = new PointerEventArgs();
                argsIn.fromInputSource = pose.inputSource;
                argsIn.point = hit.point;
                argsIn.distance = hit.distance;
                argsIn.flags = 0;
                argsIn.target = hit.transform;
                OnPointerIn(argsIn);
                _previousContact = hit.transform;
            }

            if (!bHit)
            {
                _previousContact = null;
            }

            if (bHit && hit.distance < 100f)
            {
                dist = hit.distance;
            }

            bool isUp = interactWithUI.GetStateUp(pose.inputSource);
            bool isDown = interactWithUI.GetStateDown(pose.inputSource);
            bool isHeld = interactWithUI.GetState(pose.inputSource);
            
            if (isUp)
            {
                PointerEventArgs argsUp = new PointerEventArgs();
                argsUp.fromInputSource = pose.inputSource;
                argsUp.flags = 0;
                if (bHit)
                {
                    argsUp.distance = hit.distance;
                    argsUp.point = hit.point;
                    argsUp.target = hit.transform;
                    if (_pointerDownContact && _pointerDownContact == hit.transform)
                    {
                        OnPointerClick(argsUp);
                    }
                    OnPointerUp(argsUp);
                }
                else
                {
                    argsUp.distance = hit.distance;
                    argsUp.point = Vector3.positiveInfinity;
                    argsUp.target = hit.transform;
                    OnPointerUp(argsUp);
                }
            }

            if (isDown)
            {
                PointerEventArgs argsDown = new PointerEventArgs();
                argsDown.fromInputSource = pose.inputSource;
                argsDown.target = null;
                argsDown.distance = float.PositiveInfinity;
                argsDown.flags = 0;
                if (bHit)
                {
                    _pointerDownContact = hit.transform;
                    argsDown.distance = hit.distance;
                    argsDown.point = hit.point;
                    argsDown.target = hit.transform;
                }

                OnPointerDown(argsDown);
            }

            if (bHit && isHeld && !isDown)
            {
                PointerEventArgs argsMove = new PointerEventArgs();
                argsMove.fromInputSource = pose.inputSource;
                argsMove.target = hit.transform;
                argsMove.point = hit.point;
                argsMove.distance = hit.distance;
                argsMove.flags = 0;
                OnPointerMove(argsMove);
            }
            
            if (interactWithUI != null && isHeld)
            {
                _pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, dist);
                _pointerRenderer.material.color = clickColor;
            }
            else
            {
                _pointer.transform.localScale = new Vector3(thickness, thickness, dist);
                _pointerRenderer.material.color = color;
            }

            _pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);
        }
    }


    public struct PointerEventArgs
    {
        public SteamVR_Input_Sources fromInputSource;
        public uint flags;
        public float distance;
        public Transform target;
        public Vector3 point;
        public GameObject gameObject;
    }

    public delegate void PointerEventHandler(object sender, PointerEventArgs e);
}