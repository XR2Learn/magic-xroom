using Common.Scripts;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Placeable))]
[CanEditMultipleObjects]
public class PlaceableInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Placeable placeable = serializedObject.targetObject as Placeable;
        if (!placeable) return;
        if (!placeable.Target && GUILayout.Button("Create target"))
        {
            GameObject g = new GameObject($"{placeable.gameObject.name}_Target");
            Transform t = placeable.transform;
            Transform tt = g.transform;
            tt.SetParent(t.parent);
            tt.localPosition = t.localPosition;
            tt.localRotation = t.localRotation;
            tt.localScale = t.localScale;
            Collider c = t.GetComponent<Collider>();
            Collider cc = c switch
            {
                BoxCollider box => g.AddComponent(box),
                SphereCollider sphere => g.AddComponent(sphere),
                CapsuleCollider capsule => g.AddComponent(capsule),
                MeshCollider mesh => g.AddComponent(mesh),
                _ => null
            };

            if (!cc)
            {
                DestroyImmediate(g);
                return;
            }
            cc.isTrigger = true;
            serializedObject.FindProperty("target").objectReferenceValue = g;
            if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
            Selection.activeGameObject = g;
        }
    }
}