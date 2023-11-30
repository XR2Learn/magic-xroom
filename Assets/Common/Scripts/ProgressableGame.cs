using UnityEngine;
using UnityEngine.Events;

namespace Common
{
    public abstract class ProgressableGame : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] protected UnityEvent<int> levelFailed;
        [SerializeField] protected UnityEvent<int> levelCompleted;
        [SerializeField] protected UnityEvent<int> levelStarted;
        [SerializeField] protected UnityEvent<string> scenarioStarted;
        [SerializeField] protected UnityEvent<string> scenarioEnded;
    }
}
