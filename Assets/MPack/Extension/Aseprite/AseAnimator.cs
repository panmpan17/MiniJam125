#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using System.Reflection;

namespace MPack.Aseprite {
    public class AseAnimator : MonoBehaviour
    {
        [property: SerializeField] public bool UseScaleTime { get; set; }

        [SerializeField]
        private AseAnimation[] animations;
        [SerializeField]
        private Light2D light2D;
        private FieldInfo _LightCookieSprite = typeof(Light2D).GetField("m_LightCookieSprite", BindingFlags.NonPublic | BindingFlags.Instance);
        private int animI = -1, animKeyI;
        private float timer;
        private bool stop;
        public bool IsStopped => stop;

        [SerializeField]
        private SpriteRenderer spriteRenderer;
        // private Image image;

        private void Awake()
        {
            if (!spriteRenderer)
                spriteRenderer = GetComponent<SpriteRenderer>();
            Play(0);
        }

        private void Update()
        {
            if (stop)
                return;

            timer += Time.deltaTime;
            if (timer > animations[animI].Points[animKeyI].Time) {
                timer = 0;
                animKeyI++;

                if (animKeyI >= animations[animI].Points.Length) {
                    if (animations[animI].Loop)
                        animKeyI = 0;
                    else {
                        stop = true;
                        spriteRenderer.sprite = null;
                        return;
                    }
                }

                Sprite sprite = animations[animI].Points[animKeyI].Sprite;
                spriteRenderer.sprite = sprite;
                if (light2D)
                    _LightCookieSprite.SetValue(light2D, sprite);
            }
        }

        public void Play(int index) {
            if (!stop && animI == index)
                return;

            animI = index;
            animKeyI = 0;

            stop = false;

            Sprite sprite = animations[animI].Points[animKeyI].Sprite;
            spriteRenderer.sprite = sprite;
            if (light2D)
            {
                light2D.enabled = false;
                _LightCookieSprite.SetValue(light2D, sprite);
            }
        }

        public void Stop()
        {
            stop = true;
            spriteRenderer.sprite = null;

            if (light2D)
                light2D.enabled = false;
        }

        public void PlayAnimation(string name)
        {
            int index = 0;
            bool found = false;

            for (; index < animations.Length; index++)
            {
                if (animations[index].Name == name)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
            #if UNITY_EDITOR
                Debug.LogWarningFormat("{0} animation not found", name);
            #endif
                return;
            }

            if (animI == index)
                return;

            animI = index;
            animKeyI = 0;

            stop = false;
            spriteRenderer.sprite = animations[animI].Points[animKeyI].Sprite;
        }

        public float GetAnimationDuration(int index) => animations[animI].Duration;
    }
}