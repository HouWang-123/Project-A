using System;
using System.Collections.Generic;
using cfg.interact;
using UnityEngine;

public interface IInteractHandler
{
    /// 获得交互焦点
    void OnPlayerFocus();   
    /// 失去交互焦点
    void OnPlayerDefocus();      
    /// 必须实现，否则功能无法使用
    MonoBehaviour getMonoBehaviour();  
    /// 玩家开始交互
    void OnPlayerStartInteract(); 
    /// 玩家交互完成
    void OnPlayerInteract();
    /// 取消操作回调
    void OnPlayerInteractCancel();
}


public interface IInteractableItemReceiver : IInteractHandler   // RequireItem
{
    bool hasInteraction (int itemid);                          // 检查是否存在可交互的函数
    List<InteractEffectTable> GetInteractTable();                    // 获取交互效果表
    
}



public interface IInteractableItemHandler
{
    int GetInteractItemId();
    void HandleInteract();
}

