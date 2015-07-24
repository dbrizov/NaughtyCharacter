using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Character), true)]
public class CharacterEditor : Editor
{
    private SerializedProperty orientRotationToMovement;
    private SerializedProperty useControlRotation;

    protected virtual void OnEnable()
    {
        this.orientRotationToMovement = this.serializedObject.FindProperty("orientRotationToMovement");
        this.useControlRotation = this.serializedObject.FindProperty("useControlRotation");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        this.serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(this.orientRotationToMovement);
        if (EditorGUI.EndChangeCheck())
        {
            this.useControlRotation.boolValue = !this.orientRotationToMovement.boolValue;
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(this.useControlRotation);
        if (EditorGUI.EndChangeCheck())
        {
            this.orientRotationToMovement.boolValue = !this.useControlRotation.boolValue;
        }

        this.serializedObject.ApplyModifiedProperties();
    }
}
