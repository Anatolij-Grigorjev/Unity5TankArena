namespace TankArena.Constants
{
    public class UIShopButtonTexts
    {
        private UIShopButtonTexts() {}
        
        public static string SHOP_WEAPONS_HEADER_TEXT = "GO TO GARAGE";
        public static string SHOP_GARAGE_HEADER_TEXT = "GO TO WEAPONS";
        public static string SHOP_NOT_ENOUGH_MSG_BOX = "You don't have enough money to buy this.\n" + 
            "Try spending more time fighting in the arena!";
        public static string SHOP_ALREADY_HAVE_PART_MSG_BOX = "You already have this part installed on the tank!";
        public static string SHOP_CHOSE_WEAPON_SLOT = "Chose the destination weapon slot...";
        public static string SHOP_NO_APPROPRIATE_SLOTS = "You dont have these types of slots on your turret!";
        public static string SHOP_SOLD_N_WEAPONS_FOR_M(int N, float M)
        {
            return string.Format("Along with the turret, you have recieved {0} dollars for selling {1} weapons!", M, N);
        }

    }
}