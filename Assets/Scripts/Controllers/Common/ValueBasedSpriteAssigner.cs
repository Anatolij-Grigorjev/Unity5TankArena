using UnityEngine;
using TankArena.Controllers;

namespace TankArena.Utils
{
    public class ValueBasedSpriteAssigner: MonoBehaviour
    {
        
        public float maxValue;
        public float minValue = 0.0f;
        public Sprite[] sprites;
        private int currentIndex = 0;
        private float valuesPerSprite;
        public DamageBitsController damageController;
        public void Awake() 
        {
             UpdateVPS();
        }

        ///<summary>
        ///To be called after changing values for script but before new value updates!
        ///</summary>
        public void UpdateVPS()
        {
            valuesPerSprite = (maxValue - minValue) / sprites.Length;
        }

        public void UpdateSprite(SpriteRenderer renderer, float currValue)
        {
            int bucketNum = Mathf.RoundToInt(currValue / valuesPerSprite);
            int index = Mathf.Clamp(sprites.Length - bucketNum, 0, sprites.Length - 1);
            //only fire bits when getting damage, not for regen
            if (damageController != null && currentIndex < index) 
            {
                DBG.Log("index changed from {0} to {1}, firing off bits!", currentIndex, index);
                //new damage index! firing the bits!
                damageController.StartBits();
            }
            currentIndex = index;
            renderer.sprite = sprites[index];
        }

    }
}