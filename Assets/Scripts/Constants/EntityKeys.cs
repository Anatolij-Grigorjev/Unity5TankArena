﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankArena.Constants
{
    class EntityKeys
    {
        //ALL
        public const string EK_NAME = "name";
        public const string EK_ID = "id";
        public const string EK_PRICE = "price";

        //PREFABBED
        public const string EK_ENTITY_PREFAB = "entity_prefab";

        //CHARACTER
        public const string EK_AVATAR_IMAGE = "avatar";
        public const string EK_BACKGROUND_IMAGE = "background";
        public const string EK_CHARACTER_MODEL_IMAGE = "char_model";
        public const string EK_CHARACTER_STARTER_HEALTH = "start_health";
        public const string EK_CHARACTER_STARTER_CASH = "start_cash";
        public const string EK_CHARACTER_STARTER_TANK = "start_tank";

        //TANK PARTS
        public const string EK_ON_TANK_POSITION = "on_tank_position";
        public const string EK_MASS = "mass";
        public const string EK_SHOP_ITEM_IMAGE = "shop_item";
        public const string EK_GARAGE_ITEM_IMAGE = "garage_item";
        public const string EK_ENTITY_SPRITESHEET = "spritesheet";
        public const string EK_ACTIVE_SPRITES = "active_sprites";
        public const string EK_COLLISION_BOX = "box_collider";

        //TANK TRACKS
        public const string EK_COUPLING = "coupling";
        public const string EK_TURN_SPEED = "turn_speed";
        public const string EK_LOWER_INTEGRITY = "lower_integrity";
        public const string EK_LEFT_TRACK_POSITION = "left_track_pos";
        public const string EK_RIGHT_TRACK_POSITION = "right_track_pos";

        //TANK ENGINE
        public const string EK_TOP_SPEED = "top_speed";
        public const string EK_TORQUE = "torque";
        public const string EK_ACCELERATION = "acceleration";
        public const string EK_DEACCELERATION = "deacceleration";
        public const string EK_IDLE_SOUND = "idle_sound";
        public const string EK_REVVING_SOUND = "revving_sound";

        //TANK TURRET
        public const string EK_HEAVY_WEAPON_SLOTS = "heavy_weapon_slots";
        public const string EK_LIGHT_WEAPON_SLOTS = "light_weapon_slots";
        public const string EK_WEAPONS_SHOP_IMAGE = "weapons_shop";
        public const string EK_TURRET_SPIN_SPEED = "spin_speed";
        public const string EK_TURRET_SPIN_SOUND = "spin_sound";

        //TANK CHASSIS
        public const string EK_INTEGRITY = "integrity";
        public const string EK_TURRET_PIVOT = "turret_pivot";

        //WEAPONS
        public const string EK_ON_TURRET_POSITION = "on_turret_position";
        public const string EK_WEAPON_TYPE = "wpn_type";
        public const string EK_DAMAGE = "damage";
        public const string EK_RELOAD_TIME = "reload";
        public const string EK_RATE_OF_FIRE = "rate_of_fire";
        public const string EK_RANGE = "range";
        public const string EK_CLIP_SIZE = "clip_size";
        public const string EK_WEAPON_BEHAVIOR_TYPE = "behavior_type";
        public const string EK_PROJECTILE_PREFAB = "projectile_prefab";
        public const string EK_SHOT_SOUND = "shot_sound";
        public const string EK_RELOAD_SOUND = "reload_sound";
        public const string EK_WEAPON_ANIMATION = "weapon_animations";



        //DISPLAY PROPS
        public static readonly Dictionary<string, string> ENTITY_KEYS_DISPLAY_MAP = new Dictionary<string, string>() 
        {
            { EK_NAME, "Name"},
            { EK_PRICE, "Price"},
            { EK_MASS, "Weight"},
            { EK_COUPLING, "Coupling" },
            { EK_TURN_SPEED, "Turn Speed" },
            { EK_LOWER_INTEGRITY, "Integrity" },
            { EK_TOP_SPEED, "Top Velocity"},
            { EK_TORQUE, "Power"},
            { EK_ACCELERATION, "Acceleration"},
            { EK_DEACCELERATION, "Breaking"},
            { EK_HEAVY_WEAPON_SLOTS, "Heavy Weapons"},
            { EK_LIGHT_WEAPON_SLOTS, "Light Weapons"},
            { EK_TURRET_SPIN_SPEED, "Turn Speed"},
            { EK_INTEGRITY, "Integrity"},
            { EK_DAMAGE, "Damage"},
            { EK_RELOAD_TIME, "Reload Time"},
            { EK_RATE_OF_FIRE, "Rate of Fire"},
            { EK_RANGE, "Range"},
            { EK_CLIP_SIZE, "Clip Size"}

        };
        
    }
}
