using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public abstract class OnTeleportSteamVr : MonoBehaviour
{
    public abstract void OnTeleport(TeleportPoint teleportPoint);
    public abstract void OnTeleportAway();

}
