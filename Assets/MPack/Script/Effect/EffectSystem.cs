using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack.Aseprite;

namespace MPack
{
    public class EffectSystem : MonoBehaviour
    {
        [SerializeField]
        private EffectReference[] listenEffects;

        void LateUpdate()
        {
            for (int i = 0; i < listenEffects.Length; i++)
            {
                EffectReference effectReference = listenEffects[i];

                while (effectReference.WaitingList.Count > 0)
                {
                    EffectReference.EffectQueue effectQueue = effectReference.WaitingList.Pop();

                    if (effectReference.UseParticle)
                        SpawnParticle(effectReference, effectQueue);
                    else
                        SpawnAseAnimator(effectReference, effectQueue);
                }
            }
        }

        private void SpawnParticle(EffectReference effectReference, EffectReference.EffectQueue effectQueue)
        {
            ParticleSystem newEffect = effectReference.GetFreshEffect();

            Transform effectTransform = newEffect.transform;
            effectTransform.SetParent(effectQueue.Parent);
            effectTransform.SetPositionAndRotation(effectQueue.Position, effectQueue.Rotation);

            ParticleSystem.MainModule main = newEffect.main;
            main.useUnscaledTime = !effectQueue.UseScaleTime;
            newEffect.Play();

            StartCoroutine(WaitToCollectEffect(effectReference, newEffect, effectQueue.UseScaleTime, newEffect.main.duration));
        }

        private void SpawnAseAnimator(EffectReference effectReference, EffectReference.EffectQueue effectQueue)
        {
            AseAnimator animator = effectReference.GetFreshAseAnimatorEffect();

            Transform effectTransform = animator.transform;
            effectTransform.SetParent(effectQueue.Parent);
            effectTransform.SetPositionAndRotation(effectQueue.Position, effectQueue.Rotation);

            animator.UseScaleTime = effectQueue.UseScaleTime;

            StartCoroutine(WaitToCollectEffect(effectReference, animator));
        }

        IEnumerator WaitToCollectEffect(EffectReference effectReference, ParticleSystem effect, bool useScaleTime, float duration)
        {
            if (useScaleTime)
                yield return new WaitForSeconds(duration);
            else
                yield return new WaitForSecondsRealtime(duration);

            effectReference.Put(effect);
        }

        IEnumerator WaitToCollectEffect(EffectReference effectReference, AseAnimator animator)
        {
            animator.Play(0);
            float duration = animator.GetAnimationDuration(0);

            if (animator.UseScaleTime)
                yield return new WaitForSeconds(duration);
            else
                yield return new WaitForSecondsRealtime(duration);

            effectReference.Put(animator);
        }

        void OnDestroy()
        {
            for (int i = 0; i < listenEffects.Length; i++)
            {
                listenEffects[i].Clear();
            }
        }
    }
}