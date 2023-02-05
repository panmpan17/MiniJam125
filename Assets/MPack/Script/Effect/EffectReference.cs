using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack.Aseprite;


namespace MPack
{
    [CreateAssetMenu(menuName = "MPack/Effect Reference", order = 0)]
    public class EffectReference : ScriptableObject
    {
        public bool UseParticle;
        public ParticleSystem ParticlePrefab;
        public AseAnimator AseAnimatorPrefab;

        public Stack<ParticleSystem> Pools;
        public Stack<AseAnimator> AseAnimatorPools;
        public Stack<EffectQueue> WaitingList;

        void OnEnable()
        {
            Pools = new Stack<ParticleSystem>();
            AseAnimatorPools = new Stack<AseAnimator>();
            WaitingList = new Stack<EffectQueue>();
        }

        public ParticleSystem GetFreshEffect()
        {
            while (Pools.Count > 0)
            {
                ParticleSystem particle = Pools.Pop();
                if (particle != null)
                    return particle;
            }

            return GameObject.Instantiate(ParticlePrefab);
        }

        public AseAnimator GetFreshAseAnimatorEffect()
        {
            while (AseAnimatorPools.Count > 0)
            {
                AseAnimator particle = AseAnimatorPools.Pop();
                if (particle != null)
                    return particle;
            }

            return GameObject.Instantiate(AseAnimatorPrefab);
        }

        public void Put(ParticleSystem effect)
        {
            effect.Stop();
            Pools.Push(effect);
        }
        public void Put(AseAnimator effect)
        {
            effect.Stop();
            AseAnimatorPools.Push(effect);
        }

        public void AddWaitingList(EffectQueue queue)
        {
            WaitingList.Push(queue);
        }

        public void AddWaitingList(Vector3 position, Quaternion rotation, bool useScaleTime=true)
        {
            WaitingList.Push(new EffectQueue {
                Position = position,
                Rotation = rotation,
                UseScaleTime = useScaleTime,
                Scale = Vector3.one,
            });
        }

        public void Clear()
        {
            Pools.Clear();
            WaitingList.Clear();
        }

        public struct EffectQueue
        {
            public Transform Parent;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;
            public bool UseScaleTime;
        }
    }
}