using System;
using UnityEngine;

public interface IInteractHandler
{
    void OnPlayerFocus();
    void OnPlayerDefocus();
    MonoBehaviour getMonoBehaviour();
    
    void OnPlayerStartInteract(Action interactCallback); // 玩家开始交互
    void OnPlayerInteract(); // 玩家交互
    void OnPlayerInteractCancel(Action cancleCallback); // 取消操作回调
}
