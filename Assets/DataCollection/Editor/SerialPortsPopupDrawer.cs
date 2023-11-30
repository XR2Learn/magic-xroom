using UnityEngine;
using XR2Learn_ShimmerAPI;

namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(ListToPopupAttribute))]
    public class SerialPortsPopupDrawer : PropertyDrawer
    {
        public int selectedIndex;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            selectedIndex = EditorGUI.Popup(position, ObjectNames.NicifyVariableName(property.name), selectedIndex, XR2Learn_SerialPortsManager.GetAvailableSerialPortsNames());
            property.stringValue = XR2Learn_SerialPortsManager.GetAvailableSerialPortsNames()[selectedIndex];
        }
    }
}
