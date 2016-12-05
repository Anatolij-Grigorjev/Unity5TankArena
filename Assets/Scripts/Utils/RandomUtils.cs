using UnityEngine;

namespace TankArena.Utils
{
    public class RandomUtils
    {
        private RandomUtils() {}

        public static Vector2 RandomVector2D(float maxX, float maxY, float minX = 0.0f, float minY = 0.0f) 
        {
            return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        } 

        public static Quaternion RandomQuaternion2D()
        {
            return Quaternion.Euler(0.0f, 0.0f, Random.value * 360.0f);
        }
    }
}