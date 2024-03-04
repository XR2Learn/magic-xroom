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


using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.Extras;

namespace XR2Learn.Common
{
    [RequireComponent(typeof(SteamVR_LaserPointer))]
    public class SteamVR_UILaserPointer : MonoBehaviour
    {
        [SerializeField] private LayerMask raycastLayers;

        private SteamVR_LaserPointer _steamVrLaserPointer;
        private LayerMask _savedLayers;

        private RectTransform _target;
        private IPointerMoveHandler _moveHandler;

        private bool _isActive;

        private void Awake()
        {
            _steamVrLaserPointer = gameObject.GetComponent<SteamVR_LaserPointer>();
            _isActive = false;
        }

        private void OnEnable()
        {
            _steamVrLaserPointer.PointerIn += OnPointerIn;
            _steamVrLaserPointer.PointerOut += OnPointerOut;
            _steamVrLaserPointer.PointerDown += OnPointerDown;
            _steamVrLaserPointer.PointerUp += OnPointerUp;
            _steamVrLaserPointer.PointerClick += OnPointerClick;
            _steamVrLaserPointer.PointerMove += OnPointerMove;
        }

        private void OnDisable()
        {
            _steamVrLaserPointer.PointerIn -= OnPointerIn;
            _steamVrLaserPointer.PointerOut -= OnPointerOut;
            _steamVrLaserPointer.PointerDown -= OnPointerDown;
            _steamVrLaserPointer.PointerUp -= OnPointerUp;
            _steamVrLaserPointer.PointerClick -= OnPointerClick;
            _steamVrLaserPointer.PointerMove -= OnPointerMove;
            _isActive = false;
        }

        public void ToggleLayerMask(bool status)
        {
            _isActive = status ^ _isActive;
            if (_isActive)
            {
                _savedLayers = _steamVrLaserPointer.raycastLayers;
                _steamVrLaserPointer.raycastLayers = raycastLayers;
            }
            else
            {
                _steamVrLaserPointer.raycastLayers = _savedLayers;
            }
        }

        private PointerEventData CreatePointerEventData(PointerEventArgs e)
        {
            PointerEventData d = new PointerEventData(EventSystem.current);
            Vector2 localPos = _target.InverseTransformPoint(e.point);
            Vector2 size = _target.rect.size;
            Vector2 pixelPos = new Vector2(localPos.x + size.x * .5f, size.y * .5f + localPos.y);
            d.position = pixelPos;
            return d;
        }

        private void OnPointerIn(object sender, PointerEventArgs e)
        {
            if (!e.target) return;

            _target = e.target.GetComponent<RectTransform>();

            if (!_target) return;

            _moveHandler = e.target.GetComponent<IPointerMoveHandler>();
            e.target.GetComponent<IPointerEnterHandler>()?.OnPointerEnter(CreatePointerEventData(e));
        }

        private void OnPointerOut(object sender, PointerEventArgs e)
        {
            if (!_target) return;

            _target.GetComponent<IPointerExitHandler>()?.OnPointerExit(CreatePointerEventData(e));
            _target = null;
            _moveHandler = null;
        }

        private void OnPointerDown(object sender, PointerEventArgs e)
        {
            if (!_target) return;

            e.target.GetComponent<IPointerDownHandler>()?.OnPointerDown(CreatePointerEventData(e));
        }

        private void OnPointerClick(object sender, PointerEventArgs e)
        {
            if (!e.target) return;

            e.target.GetComponent<IPointerClickHandler>()?.OnPointerClick(CreatePointerEventData(e));
        }

        private void OnPointerUp(object sender, PointerEventArgs e)
        {
            if (!_target) return;

            e.target.GetComponent<IPointerUpHandler>()?.OnPointerUp(CreatePointerEventData(e));
        }

        private void OnPointerMove(object sender, PointerEventArgs e)
        {
            if (!_target) return;

            _moveHandler?.OnPointerMove(CreatePointerEventData(e));
        }
    }
}