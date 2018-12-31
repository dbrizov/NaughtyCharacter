using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Character), true)]
public class CharacterEditor : Editor
{
    private SerializedProperty rotationSettings;
    private SerializedProperty rotationSmoothing;
    private SerializedProperty orientRotationToMovement;
    private SerializedProperty useControlRotation;
    
    private bool showRotationSettings = true;

    protected virtual void OnEnable()
    {
        this.rotationSettings = this.serializedObject.FindProperty("rotationSettings");
        this.rotationSmoothing = this.rotationSettings.FindPropertyRelative("rotationSmoothing");
        this.orientRotationToMovement = this.rotationSettings.FindPropertyRelative("orientRotationToMovement");
        this.useControlRotation = this.rotationSettings.FindPropertyRelative("useControlRotation");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        this.serializedObject.Update();

        this.showRotationSettings = EditorGUILayout.Foldout(this.showRotationSettings, "Rotation Settings");
        if (this.showRotationSettings)
        {
            EditorGUI.indentLevel += 1;

            EditorGUILayout.PropertyField(this.rotationSmoothing);

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
        }

        EditorGUI.indentLevel -= 1;

        this.serializedObject.ApplyModifiedProperties();
    }
}
