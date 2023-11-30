using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class AdjustableDesk : MonoBehaviour
{

    [SerializeField] private ArticulationBody upperDesk;
    [SerializeField] private ArticulationBody monitorStandBase;

    private List<ArticulationBody> _monitorStandHierarchy;

    private void Start() {
        upperDesk.jointFriction = float.PositiveInfinity;

        _monitorStandHierarchy = monitorStandBase.GetComponentsInChildren<ArticulationBody>().ToList();

        foreach (ArticulationBody body in _monitorStandHierarchy) {
            body.jointFriction = float.PositiveInfinity;
        }
    }
    
    public void OnHandleGrabbed() {
        upperDesk.jointFriction = 0;
    }

    public void OnHandleReleased() {
        upperDesk.jointFriction = float.PositiveInfinity;
    }

    public void OnMonitorScreenGrabbed() {
        foreach (ArticulationBody body in _monitorStandHierarchy) {
            body.jointFriction = 0;
        }
    }

    public void OnMonitorScreenReleased() {
        foreach (ArticulationBody body in _monitorStandHierarchy) {
            body.jointFriction = float.PositiveInfinity;
        }
    }
}
