using UnityEditor;
using UnityEngine;

namespace NaughtyCharacter
{
    [CustomEditor(typeof(PlayerController))]
    public class PlayerControllerEditor : Editor
    {
        private SerializedProperty _rotationSettings;
        private SerializedProperty _controlRotationSensitivity;
        private SerializedProperty _minPitchAngle;
        private SerializedProperty _maxPitchAngle;
        private SerializedProperty _useControlRotation;
        private SerializedProperty _orientRotationToMovement;
        private SerializedProperty _minRotationSpeed;
        private SerializedProperty _maxRotationSpeed;

        private bool _showRotationSettings = true;

        protected virtual void OnEnable()
        {
            _rotationSettings = serializedObject.FindProperty("RotationSettings");
            _controlRotationSensitivity = _rotationSettings.FindPropertyRelative("ControlRotationSensitivity");
            _minPitchAngle = _rotationSettings.FindPropertyRelative("MinPitchAngle");
            _maxPitchAngle = _rotationSettings.FindPropertyRelative("MaxPitchAngle");
            _useControlRotation = _rotationSettings.FindPropertyRelative("_useControlRotation");
            _orientRotationToMovement = _rotationSettings.FindPropertyRelative("_orientRotationToMovement");
            _minRotationSpeed = _rotationSettings.FindPropertyRelative("MinRotationSpeed");
            _maxRotationSpeed = _rotationSettings.FindPropertyRelative("MaxRotationSpeed");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            _showRotationSettings = EditorGUILayout.Foldout(_showRotationSettings, _rotationSettings.displayName);
            if (_showRotationSettings)
            {
                EditorGUI.indentLevel += 1;

                EditorGUILayout.PropertyField(_controlRotationSensitivity);
                EditorGUILayout.PropertyField(_minPitchAngle);
                EditorGUILayout.PropertyField(_maxPitchAngle);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_useControlRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    _orientRotationToMovement.boolValue = !_useControlRotation.boolValue;
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_orientRotationToMovement);
                if (EditorGUI.EndChangeCheck())
                {
                    _useControlRotation.boolValue = !_orientRotationToMovement.boolValue;
                }

                GUI.enabled = _orientRotationToMovement.boolValue;
                EditorGUILayout.PropertyField(_minRotationSpeed);
                EditorGUILayout.PropertyField(_maxRotationSpeed);
                GUI.enabled = true;

                EditorGUI.indentLevel -= 1;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
