using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.Extras;

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
        Vector2 pixelPos =  new Vector2 (localPos.x + size.x * .5f, size.y * .5f + localPos.y);
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