namespace TankArena.Utils
{
    public static class ExtensionMethods 
    {
        public static void Fill<T>(this T[] originalArray, T with, int startIndex = 0) {
            for(int i = startIndex; i < originalArray.Length; i++){
                originalArray[i] = with;
            }
        }
    }
}