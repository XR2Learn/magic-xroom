using UnityEngine;
using UnityEngine.Events;

public class EasedMapping : MonoBehaviour {
    
    public Vector2 inputRange;
    
    [SerializeField] protected EasingFunction.Ease easing;
    
    private EasingFunction.Function func;

    public UnityEvent<float> onChange;

    public EasingFunction.Ease Easing {
        get => easing;
        set {
            easing = value;
            func = null;
        }
    }
    
    public float Value {
        set {
            func ??= EasingFunction.GetEasingFunction(easing);
            onChange?.Invoke(func.Invoke(inputRange.x, inputRange.y, value));
        }
    }
}
