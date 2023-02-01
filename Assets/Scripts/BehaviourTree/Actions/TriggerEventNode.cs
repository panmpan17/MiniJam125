using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Set Fixed As Target")]
    public class TriggerEventNode : ActionNode
    {
        // public EventReference eventReference;
        public IntEventReference IntEventReference;
        public FloatEventReference FloatEventReference;
        public BoolEventReference BoolEventReference;
        public ValueWithEnable<float> carriedFloatValue;
        public ValueWithEnable<int> carriedIntValue;
        public ValueWithEnable<bool> carriedBoolValue;


        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (carriedFloatValue.Enable)
                FloatEventReference.Invoke(carriedFloatValue.Value);

            if (carriedIntValue.Enable)
                IntEventReference.Invoke(carriedIntValue.Value);

            if (carriedBoolValue.Enable)
                BoolEventReference.Invoke(carriedBoolValue.Value);

            return State.Success;
        }
    }
}