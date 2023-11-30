using DataCollection.Managers;
using TMPro;
using UnityEngine;

public class ShimmerStatus : MonoBehaviour
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
    public Color _disabledColor = Color.gray;

    private void Awake()
    {
        enabled = _enabled;
        gameObject.SetActive(enabled);

        _shimmer = ShimmerGSRManager.Instance;
    }

    private void LateUpdate()
    {
        if (_dataCollectionManager == null) return;

        if (_dataCollectionManager.IsRunning)
        {
            _dataCollectionText.text = "Running";
            _dataCollectionText.color = _runningColor;
        }
        else
        {
            _dataCollectionText.text = "Stopped";
            _dataCollectionText.color = _stoppedColor;
        }

        if (_shimmerLastState != ShimmerGSRManager.ConnectionState.STREAMING && _shimmerLastState == _shimmer.State) return;
        _shimmerLastState = _shimmer.State;

        switch (_shimmer.State)
        {
            case ShimmerGSRManager.ConnectionState.CONNECTED:
                //ShimmerStatusColorMaterial.color = ConnectedColor;
                //ShimmerStatusColorMaterial.SetColor("_EmissionColor", ConnectedColor);
                _shimmerStateText.text = "Connected";
                _shimmerStateText.color = _connectedColor;
            break;

            case ShimmerGSRManager.ConnectionState.CONNECTING:
                //ShimmerStatusColorMaterial.color = ConnectingColor;
                //ShimmerStatusColorMaterial.SetColor("_EmissionColor", ConnectingColor);
                _shimmerStateText.text = "Connecting";
                _shimmerStateText.color = _connectingColor;
                break;

            case ShimmerGSRManager.ConnectionState.DISCONNECTED:
                //ShimmerStatusColorMaterial.color = DisconnectedColor;
                //ShimmerStatusColorMaterial.SetColor("_EmissionColor", DisconnectedColor);
                _shimmerStateText.text = "Disconnected";
                _shimmerStateText.color = _disconnectedColor;
                break;

            case ShimmerGSRManager.ConnectionState.STREAMING:
                //ShimmerStatusColorMaterial.color = StreamingColor;
                //ShimmerStatusColorMaterial.SetColor("_EmissionColor", StreamingColor);
                _shimmerStateText.text = "Streaming";
                _shimmerStateText.color = _streamingColor;
                break;

            case ShimmerGSRManager.ConnectionState.INACTIVE:
                //ShimmerStatusColorMaterial.color = StreamingColor;
                //ShimmerStatusColorMaterial.SetColor("_EmissionColor", StreamingColor);
                _shimmerStateText.text = "Inactive";
                _shimmerStateText.color = _disabledColor;
                break;
        }
    }
}
