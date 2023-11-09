﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util;

public class Define
{
    public static readonly Dictionary<Type, Array> _enumDict = new Dictionary<Type, Array>();
   
    #region Enum

    public enum EAnimationState
    {
        Attack,
        End,
    }
    
    public enum EJoystickState
    {
        PointDown,
        Dragging,
        PointUp
    }

    public enum ESkillType
    {
        None,
        MeleeAttack
    }
    public enum ECreatureState
    {
        Idle,
        Attack,
        Moving,
        Gathering,
        OnDamaged,
        Dead
    }
    
    public enum EObjectType
    {
        Player = 0,
        Hero = 1,
        Monster = 2,
        EliteMonster = 3,
        Projectile = 4,
        GR_Mine = 101,
        GR_Tree = 102,
        Meat = 400000,
        Firewood = 400001,
        Mineral = 400002,
    }
    public enum EJoystickType
    {
        Fixed,
        Flexible
    }
    public enum EScene
    {
        Unknown,
        TitleScene,
        LobbyScene,
        GameScene,
    }

    public enum ESound
    {
        Bgm,
        SubBgm,
        Effect,
        Max,
    }

    public enum UIEvent
    {
        Click,
        Preseed,
        PointerDown,
        PointerUp,
        BeginDrag,
        Drag,
        EndDrag,
    }
    #endregion

}
public static class EquipmentUIColors
{
    #region 장비 이름 색상
    public static readonly Color CommonNameColor = HexToColor("A2A2A2");
    public static readonly Color UncommonNameColor = HexToColor("57FF0B");
    public static readonly Color RareNameColor = HexToColor("2471E0");
    public static readonly Color EpicNameColor = HexToColor("9F37F2");
    public static readonly Color LegendaryNameColor = HexToColor("F67B09");
    public static readonly Color MythNameColor = HexToColor("F1331A");
    #endregion
    #region 테두리 색상
    public static readonly Color Common = HexToColor("AC9B83");
    public static readonly Color Uncommon = HexToColor("73EC4E");
    public static readonly Color Rare = HexToColor("0F84FF");
    public static readonly Color Epic = HexToColor("B740EA");
    public static readonly Color Legendary = HexToColor("F19B02");
    public static readonly Color Myth = HexToColor("FC2302");
    #endregion
    #region 배경색상
    public static readonly Color EpicBg = HexToColor("D094FF");
    public static readonly Color LegendaryBg = HexToColor("F8BE56");
    public static readonly Color MythBg = HexToColor("FF7F6E");
    #endregion
}
