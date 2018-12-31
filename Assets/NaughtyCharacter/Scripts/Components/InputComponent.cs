using System;
using System.Collections.Generic;
using UnityEngine;

namespace NaughtyCharacter
{
    public class InputComponent : MonoBehaviour
    {
        public enum InputEvent
        {
            Pressed,
            Released
        }

        private Dictionary<string, List<Action<float>>> _boundFunctionsByAxis = new Dictionary<string, List<Action<float>>>();
        private Dictionary<string, List<Action>> _boundFunctionsByPressedAction = new Dictionary<string, List<Action>>();
        private Dictionary<string, List<Action>> _boundFunctionsByReleasedAction = new Dictionary<string, List<Action>>();

        private void Update()
        {
            foreach (var kvp in _boundFunctionsByAxis)
            {
                string axisName = kvp.Key;
                float axisValue = Input.GetAxis(axisName);

                foreach (var func in kvp.Value)
                {
                    func.Invoke(axisValue);
                }
            }

            foreach (var kvp in _boundFunctionsByPressedAction)
            {
                string actionName = kvp.Key;

                if (Input.GetButtonDown(actionName))
                {
                    foreach (var func in kvp.Value)
                    {
                        func.Invoke();
                    }
                }
            }

            foreach (var kvp in _boundFunctionsByReleasedAction)
            {
                string actionName = kvp.Key;

                if (Input.GetButtonUp(actionName))
                {
                    foreach (var func in kvp.Value)
                    {
                        func.Invoke();
                    }
                }
            }
        }

        public void BindAxis(string axisName, Action<float> func)
        {
            if (!_boundFunctionsByAxis.ContainsKey(axisName))
            {
                _boundFunctionsByAxis[axisName] = new List<Action<float>>();
            }

            _boundFunctionsByAxis[axisName].Add(func);
        }

        public void UnbindAxis(string axisName, Action<float> func)
        {
            _boundFunctionsByAxis[axisName].Remove(func);
        }

        public void BindAction(string actionName, InputEvent inputEvent, Action func)
        {
            if (inputEvent == InputEvent.Pressed)
            {
                if (!_boundFunctionsByPressedAction.ContainsKey(actionName))
                {
                    _boundFunctionsByPressedAction[actionName] = new List<Action>();
                }

                _boundFunctionsByPressedAction[actionName].Add(func);
            }
            else if (inputEvent == InputEvent.Released)
            {
                if (!_boundFunctionsByReleasedAction.ContainsKey(actionName))
                {
                    _boundFunctionsByReleasedAction[actionName] = new List<Action>();
                }

                _boundFunctionsByReleasedAction[actionName].Add(func);
            }
        }

        public void UnbindAction(string actionName, InputEvent inputEvent, Action func)
        {
            if (inputEvent == InputEvent.Pressed)
            {
                _boundFunctionsByPressedAction[actionName].Remove(func);
            }
            else if (inputEvent == InputEvent.Released)
            {
                _boundFunctionsByReleasedAction[actionName].Remove(func);
            }
        }

        public void ClearBindings()
        {
            this._boundFunctionsByAxis.Clear();
            this._boundFunctionsByPressedAction.Clear();
            this._boundFunctionsByReleasedAction.Clear();
        }
    }
}
