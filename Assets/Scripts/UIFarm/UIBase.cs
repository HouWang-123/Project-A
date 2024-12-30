using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UIBase : MonoBehaviour, IUIShowHide
{
    //当游戏开始的时候这个方法就执行
    protected virtual void Awake()
    {
        if(gameObject.name.EndsWith("(Clone)"))
        {
            gameObject.name = gameObject.name.Replace("(Clone)", "");
        }
        //写一个数组，得到所有transform组件的所有子物体
        Transform[] allChilden = transform.GetComponentsInChildren<Transform>(true);
        //Debug.Log("Transform ");
        //遍历数组,数组中的子物体都加上UIBehaviour
        for (int i = 0; i < allChilden.Length; i++)
        {    //要使用到的子控件
            if (allChilden[i].name.EndsWith("_N"))
            {     //要使用到的子控件加上UIBehaviour
                allChilden[i].gameObject.AddComponent<UIBehaviour>();
            }
        }
        UIManager.Instance.RegistGameObject(transform.name, transform.name, gameObject);
    }
    //从UIManager里面得到子控件
    public GameObject GetWedgate(string widegateName)
    {   //Debug .Log ("执行");
        return UIManager.Instance.GetGameObject(transform.name, widegateName);
    }
    //得到子物体中的Behaviour
    public UIBehaviour GetBehaviour(string widegateName)
    {
        GameObject tmpObj = GetWedgate(widegateName);
        if (tmpObj != null)
        {
            return tmpObj.GetComponent<UIBehaviour>();
        }
        return null;
    }
    //有一个事件方法要通过子控件上的UIBehaviour得到事件方法
    public void AddButtonListen(string widageName, UnityAction action)
    {
        UIBehaviour tmpBehaviour = GetBehaviour(widageName);
        if (tmpBehaviour != null)
        {
            tmpBehaviour.AddButtonListen(action);
        }
    }
    public void AddToggleListen(string widageName, UnityAction<bool> action)
    {
        UIBehaviour tmpBehaviour = GetBehaviour(widageName);
        if(tmpBehaviour != null)
        {
            tmpBehaviour.AddToggleListen(action);
        }
    }
    public void SetToggleIsOn(string widageName, bool isOn)
    {
        UIBehaviour tmpBehaviour = GetBehaviour(widageName);
        if(tmpBehaviour != null)
        {
            tmpBehaviour.SetToggleIsON(isOn);
        }
    }
    public void ChangeTextConent(string widageName, string conent)
    {
        UIBehaviour tmpBehaviour = GetBehaviour(widageName);
        if (tmpBehaviour != null)
        {
            tmpBehaviour.ChangeTextConent(conent);
        }
    }

    public void AddDrag(string widageName, UnityAction<BaseEventData> action)
    {
        UIBehaviour tmpBehaviour = GetBehaviour(widageName);
        if (tmpBehaviour != null)
        {
            tmpBehaviour.AddDragInterface(action);
        }
    }
    /// <summary>
    /// 动态添加  begin drag  事件
    /// </summary>
    /// <param name="widageName"></param>
    /// <param name="action"></param>
    public void AddBeginDrag(string widageName, UnityAction<BaseEventData> action)
    {
        UIBehaviour tmpBehaviour = GetBehaviour(widageName);
        if (tmpBehaviour != null)
        {
            tmpBehaviour.AddOnBeginDrag(action);
        }
    }
    public void AddEndDrag(string widageName, UnityAction<BaseEventData> action)
    {
        UIBehaviour tmpBehaviour = GetBehaviour(widageName);
        if (tmpBehaviour != null)
        {
            tmpBehaviour.AddEndDrag(action);
        }
    }

    public void AddPointClick(string widageName,UnityAction <BaseEventData> action )
    {
        UIBehaviour tmpBehaviour = GetBehaviour(widageName);
        if (tmpBehaviour !=null)
        {
            tmpBehaviour.AddPointClick(action);
        }
    }
    public Image GetImage(string widageName)
    {
        UIBehaviour tmpBehaviour = GetBehaviour(widageName);
        if (tmpBehaviour != null)
        {
            return tmpBehaviour.GetImage();
        }
        return null;
    }
    
    public void Destroy()
    {
        UIManager.Instance.UnRegistPanel(transform.name);
        Destroy(gameObject);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
