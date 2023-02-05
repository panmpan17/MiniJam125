using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XnodeBehaviourTree
{
    [CreateNodeMenu("BehaviourTree/Action/Invoke Function")]
    [NodeTint("#AA68BE")]
    public class InvokeFunctionNode : ActionNode
    {
        public string FunctionName;

        private DefineFunctionNode _defineNode;

        protected override void OnStart() {}

        protected override void OnStop() {}

        protected override State OnUpdate()
        {
            context.runner.InvokeFunction(FunctionName);
            return State.Success;
        }
    }
}