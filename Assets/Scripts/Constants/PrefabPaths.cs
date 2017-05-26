using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankArena.Constants
{
    public class PrefabPaths
    {
        private PrefabPaths() { }


        public const String PREFAB_AMMO_COUNTER = @"Weapons\UI\AmmoCounter\AmmoCounter";
        public const String PREFAB_WEAPON_RELOAD = @"Weapons\ReloadSound";
        public const String PREFAB_PROJECTILE = @"Weapons\Projectiles\Projectile";
        public const String PREFAB_SAVING_TEXT = @"Common\SavingText";
        public const String AUDIO_CLIP_SAVE_SLOT_MUSIC = @"Music\Menu\save_slot_select";
        public const String AUDIO_CLIP_MENU_MUSIC = @"Music\Menu\main_menu_loop";
        public const String AUDIO_CLIP_SHOP_MUSIC = @"Music\Menu\shop_menu_loop";
        public static String AUDIO_CLIP_ARENA_MUSIC(int trackNum) {
            return @"Music\Arena\arena_active_battle_" + trackNum;
        } 


    }
}
