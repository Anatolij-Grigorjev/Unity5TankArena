﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankArena.Constants
{
    class Tags
    {
        private Tags() { }


        public const String TAG_LEFT_TRACK = "left_track";
        public const String TAG_RIGHT_TRACK = "right_track";
        public const String TAG_SIMPLE_BOOM = "SimpleBoom";
        public const String TAG_CANNON_PROJECTILE = "CannonShell";
        public const String TAG_GATLING_BULLET = "GatlingBullet";
        public const String TAG_UI_CANVAS = "UICanvas";
        public const String TAG_UI_STATS_BG = "StatsBG";
        public const String TAG_UI_SHOP_ITEM_TEXT_PARENT = "sold_item_text_parent";
        public const String TAG_UI_SHOP_ITEM_IMAGE = "sold_item_image";
        public const String TAG_MAP_COLLISION = "MapCollision";
        public const String TAG_SPAWNER_MARKER = "BySpawn";
        public const String TAG_SPAWNER = "Spawner";
        public const String TAG_BACK_TO_ITEMS_BTN = "back_to_items_button";
        public const String TAG_MSG_BOX_TEXT = "msg_box_text";
        public const String TAG_SHOP_HEAVY_SLOT = "shop_heavy_slot";
        public const String TAG_SHOP_LIGHT_SLOT = "shop_light_slot";
        public const String TAG_TANK_CHASSIS_GO = "TankChassis";
        public const String TAG_TURRET_ROTATOR = "TurretRotator";
        public const String TAG_CHASSIS_ROTATOR = "ChassisRotator";
        public static String TAG_CHARACTER_AVATAR(int rowIndex, int columnIndex)
        {
            return String.Format("CharAvatar_{0}{1}", rowIndex, columnIndex);
        }
        public const String TAG_ENEMY_LOCK = "EnemyLock";
        public const String TAG_PLAYER = "Player";
        public const String TAG_ENEMY = "Enemy";
        public const String TAG_CURSOR = "Cursor";
        public const String TAG_TRIFECTA = "Trifecta";
        public const String TAG_ACTOR_LEFT = "ActorLeft";
        public const String TAG_ACTOR_RIGHT = "ActorRight";
        public const String TAG_DIALOGUE_CONTAINER = "DialogueBoxContainer";
    }
}
