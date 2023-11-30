using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class SteamVR_BooleanActionEvent : MonoBehaviour {
    [SerializeField] private SteamVR_Action_Boolean action;
    [SerializeField] private SteamVR_Behaviour_Pose pose;

    public UnityEvent<bool> change;

    private void Awake() {
        action.AddOnChangeListener(OnChange, pose.inputSource);
    }

    private void OnChange(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState) {
        change?.Invoke(newState);
    }
}