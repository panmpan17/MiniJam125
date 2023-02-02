using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace XnodeBehaviourTree
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        [SerializeField]
        private BehaviourTreeGraph graph;

        private Blackboard _blackboard;

        public System.Action<int> OnTriggerFire;

        void Start()
        {
            Context context = Context.Create(gameObject);
            context.runner = this;
            _blackboard = new Blackboard();

            graph = (BehaviourTreeGraph) graph.Copy();
            graph.OnInitial(context, _blackboard);
        }

        void Update()
        {
            graph.Update();
        }

        public void SetStage(int stageIndex)
        {
            _blackboard.Stage = stageIndex;
        }

        public void TriggerFireGroup(int index)
        {
            OnTriggerFire?.Invoke(index);
        }
    }
}