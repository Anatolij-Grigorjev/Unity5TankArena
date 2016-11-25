using UnityEngine;

namespace TankArena.Utils
{
    public class ValueBasedSpriteAssigner: MonoBehaviour
    {
        
        public float maxValue;
        public float minValue = 0.0f;
        public Sprite[] sprites;
        private int currentIndex = 0;
        private float valuesPerSprite;
        public void Awake() 
        {
            valuesPerSprite = (maxValue - minValue) / sprites.Length; 
        }

        public void UpdateSprite(SpriteRenderer renderer, float currValue)
        {
            int bucketNum = Mathf.RoundToInt(currValue / valuesPerSprite);
            int index = Mathf.Clamp(sprites.Length - bucketNum, 0, sprites.Length - 1);
            renderer.sprite = sprites[index];
        }

    }
}