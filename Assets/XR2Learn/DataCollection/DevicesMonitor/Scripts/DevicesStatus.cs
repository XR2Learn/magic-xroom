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


using XR2Learn.DataCollection.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XR2Learn.DataCollection.DevicesMonitor
{
    public class DevicesStatus : MonoBehaviour
    {
        [SerializeField]
        private bool _enabled = true;
        [SerializeField]
        private MainDataCollectionManager _dataCollectionManager;
        [SerializeField]
        private TextMeshProUGUI _dataCollectionText;
        [SerializeField]
        private TextMeshProUGUI _shimmerStateText;

        private ShimmerGSRManager.ConnectionState _shimmerLastState;
        private ShimmerGSRManager _shimmer;

        [SerializeField]
        public Color _runningColor = Color.green;
        [SerializeField]
        public Color _stoppedColor = Color.red;

        [SerializeField]
        public Color _connectedColor = Color.green;
        [SerializeField]
        public Color _connectingColor = Color.yellow;
        [SerializeField]
        public Color _disconnectedColor = Color.red;
        [SerializeField]
        public Color _streamingColor = Color.blue;
        [SerializeField]
        public Color _inactiveColor = Color.gray;

        [SerializeField]
        public Button _startButton;
        [SerializeField]
        public Button _stopButton;

        private void Awake()
        {
            enabled = _enabled;
            gameObject.SetActive(enabled);

            _shimmer = ShimmerGSRManager.Instance;
        }

        private void LateUpdate()
        {
            // Data Collection
            if (_dataCollectionManager == null) return;

            if (_dataCollectionManager.IsRunning)
            {
                _dataCollectionText.text = "Running";
                _dataCollectionText.color = _runningColor;

                _startButton.interactable = false;
                _stopButton.interactable = true;
            }
            else
            {
                _dataCollectionText.text = "Stopped";
                _dataCollectionText.color = _stoppedColor;

                _startButton.interactable = true;
                _stopButton.interactable = false;
            }


            // Shimmer
            if (_shimmer.State == ShimmerGSRManager.ConnectionState.INACTIVE)
            {
                _shimmerStateText.text = "Inactive";
                _shimmerStateText.color = _inactiveColor;
                return;
            }

            if (_shimmerLastState == ShimmerGSRManager.ConnectionState.STREAMING && _shimmerLastState == _shimmer.State) return;
            _shimmerLastState = _shimmer.State;

            switch (_shimmer.State)
            {
                case ShimmerGSRManager.ConnectionState.CONNECTED:
                    _shimmerStateText.text = "Connected";
                    _shimmerStateText.color = _connectedColor;
                    break;

                case ShimmerGSRManager.ConnectionState.CONNECTING:
                    _shimmerStateText.text = "Connecting";
                    _shimmerStateText.color = _connectingColor;
                    break;

                case ShimmerGSRManager.ConnectionState.DISCONNECTED:
                    _shimmerStateText.text = "Disconnected";
                    _shimmerStateText.color = _disconnectedColor;
                    break;

                case ShimmerGSRManager.ConnectionState.STREAMING:
                    _shimmerStateText.text = "Streaming";
                    _shimmerStateText.color = _streamingColor;
                    break;

                case ShimmerGSRManager.ConnectionState.INACTIVE:
                    _shimmerStateText.text = "Inactive";
                    _shimmerStateText.color = _inactiveColor;
                    break;
            }
        }
    }
}
