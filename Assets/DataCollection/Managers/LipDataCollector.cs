using UnityEngine;
using ViveSR.anipal.Lip;

public class LipDataCollector : MonoBehaviour
{
    public static ViveSR.anipal.Lip.LipData _lipData;

    private void Awake()
    {
        if (!SRanipal_Lip_Framework.Instance.EnableLip)
        {
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (SRanipal_Lip_Framework.Status != SRanipal_Lip_Framework.FrameworkStatus.WORKING) return;
        SRanipal_Lip_API.GetLipData(ref _lipData);
    }
}
