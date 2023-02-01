using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MPack
{
    public class EventDispatcher : MonoBehaviour
    {
        [SerializeField]
        private AbstractEventRefernece eventReference;
        [HideInInspector, SerializeField]
        private ParameterMode parameterMode;

        public UnityEvent Event;
        public BoolUnityEvent BoolEvent;
        public IntUnityEvent IntEvent;
        public FloatUnityEvent FloatEvent;
        public StringUnityEvent StringEvent;

        void OnEnable()
        {
            eventReference.RegisterEvent(this);
        }
        void OnDisable()
        {
            eventReference.UnregisterEvent(this);
        }

        public void DispatchEvent() => Event.Invoke();
        public void DispatchEventWithBool(bool parameter) => BoolEvent.Invoke(parameter);
        public void DispatchEventWithInt(int parameter) => IntEvent.Invoke(parameter);
        public void DispatchEventWithFloat(float parameter) => FloatEvent.Invoke(parameter);
        public void DispatchEventWithString(string parameter) => StringEvent.Invoke(parameter);

        public enum ParameterMode {
            NotAssigned,
            None,
            Bool,
            Int,
            Float,
            String
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(EventDispatcher))]
        public class _Editor : Editor
        {
            SerializedProperty eventProperty;
            ParameterMode mode = ParameterMode.NotAssigned;
            EventDispatcher dispatcher;

            void OnEnable()
            {
                dispatcher = (EventDispatcher) target;
                eventProperty = serializedObject.FindProperty("eventReference");

                AbstractEventRefernece _event = (AbstractEventRefernece)eventProperty.objectReferenceValue;
                mode = ChangeEventDisplayMode(_event);
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(eventProperty);

                if (EditorGUI.EndChangeCheck())
                {
                    AbstractEventRefernece _event = (AbstractEventRefernece)eventProperty.objectReferenceValue;
                    mode = ChangeEventDisplayMode(_event);
                }

                switch (mode)
                {
                    case ParameterMode.None:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("Event"));
                        break;
                    case ParameterMode.Bool:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("BoolEvent"));
                        break;
                    case ParameterMode.Int:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("IntEvent"));
                        break;
                    case ParameterMode.Float:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("FloatEvent"));
                        break;
                    case ParameterMode.String:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("StringEvent"));
                        break;
                }

                serializedObject.ApplyModifiedProperties();
            }

            ParameterMode ChangeEventDisplayMode(AbstractEventRefernece abstractEventRefernece)
            {
                if (abstractEventRefernece == null)
                {
                    return ParameterMode.NotAssigned;
                }

                if (abstractEventRefernece is EventReference)
                    return ParameterMode.None;
                if (abstractEventRefernece is BoolEventReference)
                    return ParameterMode.Bool;
                if (abstractEventRefernece is IntEventReference)
                    return ParameterMode.Int;
                if (abstractEventRefernece is FloatEventReference)
                    return ParameterMode.Float;
                if (abstractEventRefernece is StringEventReference)
                    return ParameterMode.String;

                return ParameterMode.NotAssigned;
            }
        }
#endif
    }

    [System.Serializable]
    public class BoolUnityEvent : UnityEvent<bool> { }
    [System.Serializable]
    public class IntUnityEvent : UnityEvent<int> { }
    [System.Serializable]
    public class FloatUnityEvent : UnityEvent<float> { }
    [System.Serializable]
    public class StringUnityEvent : UnityEvent<string> { }
}