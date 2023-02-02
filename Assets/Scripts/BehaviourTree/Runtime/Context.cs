using UnityEngine;

namespace XnodeBehaviourTree
{
    public class Context
    {
        public GameObject gameObject;
        public Transform transform;
        public Animator animator;
        public BehaviourTreeRunner runner;

        public Rigidbody rigidbody => _rigidobdy == null ? gameObject.GetComponent<Rigidbody>() : _rigidobdy;
        private Rigidbody _rigidobdy;

        public static Context Create(GameObject gameObject)
        {
            // Fetch all commonly used components
            Context context = new Context
            {
                gameObject = gameObject,
                transform = gameObject.transform,
                animator = gameObject.GetComponentInChildren<Animator>(),
            };

            return context;
        }
    }
}