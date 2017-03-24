using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

namespace TankArena.Controllers
{

    public class DamageBitsController : MonoBehaviour
    {


        public SpriteRenderer bitsSpriteRenderer;
        public Sprite[] bitsAnimationSprites;
        public float timeBetweenFrames = 0.12f;
        private IEnumerator<float> bitsHandle;
        public GameObject bitsPrefab;
        int currentSprite;
        // Use this for initialization
        void Start()
        {
            ResetBits();
        }

        IEnumerator<float> _AnimateBits()
        {
            yield return Timing.WaitForSeconds(timeBetweenFrames);
			currentSprite++;
            //still got sprites to show
            if (currentSprite < bitsAnimationSprites.Length)
            {
                bitsSpriteRenderer.sprite = bitsAnimationSprites[currentSprite];
                bitsHandle = Timing.RunCoroutine(_AnimateBits());
            }
            else
            {
                //bits done
                FinishBits();
            }
        }

        void FinishBits()
        {
            if (bitsPrefab != null)
            {
                Instantiate(bitsPrefab, transform.position, transform.rotation);
            }
            //reset bits state
            ResetBits();
        }

        private void ResetBits()
        {
            currentSprite = 0;
            bitsSpriteRenderer.sprite = null;
        }

        public void StartBits()
        {
            bitsSpriteRenderer.sprite = bitsAnimationSprites[currentSprite];
            bitsHandle = Timing.RunCoroutine(_AnimateBits(), Segment.Update);
        }
    }
}
