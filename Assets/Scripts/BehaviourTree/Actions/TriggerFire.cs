using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Trigger Fire")]
    public class TriggerFire : ActionNode
    {
        public int TriggerGroupIndex;
        // public ValueWithEnable<int> TriggerGroupIndex;
        // public ValueWithEnable<int> CarriedParameter;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            context.runner.TriggerFireGroup(TriggerGroupIndex);
            // if (TriggerGroupIndex.Enable)
            // {
            //     if (CarriedParameter.Enable)
            //         context.runner.TriggerFireGroup(TriggerGroupIndex.Value, CarriedParameter.Value);
            //     else
            //         context.runner.TriggerFireGroup(TriggerGroupIndex.Value);
            // }
            // else
            // {
            //     if (CarriedParameter.Enable)
            //         context.runner.TriggerFire(CarriedParameter.Value);
            //     else
            //         context.runner.TriggerFire();
            // }
            return State.Success;
        }
    }
}