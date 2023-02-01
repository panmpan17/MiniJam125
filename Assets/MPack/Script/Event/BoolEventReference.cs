using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MPack
{
    [CreateAssetMenu(menuName="MPack/Event/bool", order=1)]
    public class BoolEventReference : AbstractEventRefernece
    {
        private event System.Action<bool> triggerEvent;

        public void Invoke(bool parameter)
        {
            for (int i = eventDispatchers.Count - 1; i >= 0; i--)
                eventDispatchers[i].DispatchEventWithBool(parameter);

            triggerEvent?.Invoke(parameter);
        }

        public void RegisterEvent(System.Action<bool> callback) => triggerEvent += callback;
        public void UnregisterEvent(System.Action<bool> callback) => triggerEvent -= callback;
    }
}