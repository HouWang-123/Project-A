﻿public static class EventConstName
{
    // 玩家受到怪物的近战伤害
    public const string PLAYER_HURTED_BY_DROWNED_ONES_MELEE = "player.hurtedByDrownedOnesMelee";
    public const string PLAYER_HURTED_BY_DROWNED_ONES_RANGED = "player.hurtedByDrownedOnesRanged";
    public const string PLAYER_HURTED_BY_HOUND_TINDALOS_MELEE = "player.hurtedByHoundTindalosMelee";
    

    //  游戏设置
    public const string GAME_SETTINGS_TRANSLATION_CHANGES = "gamesettings.translationChanges";


    //玩家手中的物品变更事件
    public const string PlayerHandItem = "PlayerHandItem";

    public const string OnChangeRoom = "Scene.OnChangeRoom";

    //玩家收到伤害动画事件
    public const string PlayerHurtAnimation = "PlayerHurtAnimation";
    //玩家死亡时动画事件
    public const string PlayerOnDeadAnimation = "PlayerOnDeadAnimation";
    // 玩家进入安全屋事件
    public const string PlayerEnterSafeHouseEvent = "PlayerEnterSafeHouseEvent";
    // 玩家离开安全屋事件，用来触发时间不变
    public const string PlayerLeaveSafeHouseEvent = "PlayerLeaveSafeHouseEvent";

    public const string PlayerFinishInteraction = "PlayerFinishInteraction";
    // 鼠标聚焦的物品位置发生改变
    public const string OnMouseFocusItemChanges = "OnMouseFocusItemChanges";
}

