using System;
using UnityEngine;

public interface IInteractHandler
{
    void OnPlayerFocus();              // 获得交互焦点
    void OnPlayerDefocus();            // 失去交互焦点
    MonoBehaviour getMonoBehaviour();  // 必须实现，否则功能无法使用
    
    void OnPlayerStartInteract();      // 玩家开始交互
    void OnPlayerInteract();           // 玩家交互完成
    void OnPlayerInteractCancel();     // 取消操作回调
}
