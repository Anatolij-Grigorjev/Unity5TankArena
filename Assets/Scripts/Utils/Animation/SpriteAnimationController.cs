using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TankArena.Utils
{
    public class SpriteAnimationController : MonoBehaviour
    {

        private string currentState;
        public string State
        {
            get
            {
                return currentState;
            }
            set
            {
                currentState = value;
                currentAnimation = statesToAnimations[currentState];
                SetFrameIdx(0);
                currentAnimationOver = false;
            }
        }

        public Dictionary<string, SpriteAnimation> statesToAnimations;

        public int currentFrameIdx;
        public float currentFrameTime;
        private SpriteAnimation currentAnimation;
        private bool currentAnimationOver;
        private Sprite currentSprite;
        public SpriteRenderer targetRenderer;
        public Sprite[] targetSprites;

        // Use this for initialization
        void Start()
        {
            currentFrameIdx = 0;
            currentFrameTime = 1000.0f;
            currentState = "";
            currentAnimation = null;
            currentAnimationOver = true;
        }

        // Update is called once per frame
        void Update()
        {

            if (!currentAnimationOver)
            {
                currentFrameTime -= Time.deltaTime;
                if (currentFrameTime <= 0.0f)
                {
                    var result = SetFrameIdx(currentFrameIdx + 1);
                    if (!result)
                    {
                        //animation out of bounds, move on
                        currentAnimationOver = !currentAnimation.loops;
                        if (currentAnimation.loops)
                        {
                            SetFrameIdx(0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// set the current animation fram index
        /// return true if setting it also altered the animation, false if out of bounds
        /// </summary>
        /// <param name="frameIdx"></param>
        /// <returns></returns>
        bool SetFrameIdx(int frameIdx)
        {
            currentFrameIdx = frameIdx;
            int size = currentAnimation != null? currentAnimation.spriteIdx.Length : 0;
            if (currentFrameIdx < size)
            {
                currentFrameTime = currentAnimation.spriteDuration[currentFrameIdx];
                currentSprite = targetSprites[currentAnimation.spriteIdx[currentFrameIdx]];
                targetRenderer.sprite = currentSprite;
                return true;
            }

            return false;
        }
    }
}
