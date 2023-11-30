using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(TeleportMarkerBase))]
public class TeleportEventWrapper : MonoBehaviour
{
    [SerializeField]
    public string Scenario;

    private TeleportMarkerBase _target;
    private bool _isPlayerInArea;

    public UnityEvent<string> playerTeleportedIn;
    public UnityEvent<string> playerTeleportedOut;

    private void Awake()
    {
        _target = GetComponent<TeleportMarkerBase>();
        playerTeleportedIn ??= new UnityEvent<string>();
        playerTeleportedOut ??= new UnityEvent<string>();
    }

    private void Start()
    {
        Teleport.Player.Listen(OnTeleport);
    }

    private void OnTeleport(TeleportMarkerBase destination)
    {
        if (destination.gameObject == _target.gameObject)
        {
            if (_isPlayerInArea) return;
            playerTeleportedIn.Invoke(Scenario);
            _isPlayerInArea = true;
            
#if DEBUG
            Debug.Log($"Player teleported to {transform.parent.name}");
#endif
        }
        else
        {
            if (!_isPlayerInArea) return;
            playerTeleportedOut.Invoke(Scenario);
            _isPlayerInArea = false;

#if DEBUG
            Debug.Log($"Player teleported out of {transform.parent.name}");
#endif
        }
    }
}