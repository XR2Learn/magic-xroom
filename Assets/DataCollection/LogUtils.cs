
using UnityEngine;

public class LogUtils
{
    private static void Log(string message, string color)
    {
        Debug.Log("<color=#" + color + ">" + message + "</color>");
    }

    public static void LogShimmer(string message)
    {
        Log(message, "FF99CC");
    }

    public static void LogVR(string message)
    {
        Log(message, "66FFFF");
    }
}
