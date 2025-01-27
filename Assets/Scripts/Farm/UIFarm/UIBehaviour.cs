using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class UIBehaviour : MonoBehaviour, IUIShowHide
{
    void Awake()
    {
        //查找所有包含UIBase的父物体
      //将子类主动上报给UIManager
      //得到所有挂由UIBase脚本的父物体
        UIBase tmpBase = transform.GetComponentInParent<UIBase>();
        //主动上报给UIManager
        UIManager.Instance.RegistGameObject(tmpBase.name, transform.name, gameObject);
    }
    //因为UIBehaviour挂在要使用的子控件身上，所以在脚本里面添加事件，通过代理将事件传给UIBase,在用过父子关系传给使用者
   // Button事件
    public void AddButtonListen(UnityAction action)
    {
        Button tmpBtn = transform.GetComponent<Button>();
        if (tmpBtn != null)
        {
            //tmpBtn.onClick.AddListener(() =>
            //{
            //    try
            //    {
            //        EventManager.Instance.RuneEvent(GameConstStr.ButtonAudioEvent);
            //    }
            //    catch(System.Exception)
            //    {

            //        throw;
            //    }
            //});
            tmpBtn.onClick.AddListener(action);
        }
    }
    //Silder事件
    public void AddSilderListen(UnityAction<float> action)
    {
        Slider tmpBtn = transform.GetComponent<Slider>();
        if (tmpBtn != null)
        {
            tmpBtn.onValueChanged.AddListener(action);
        }

    }
    //InputFiled事件
    public void AddInputFiledEndEditorListen(UnityAction<string> action)
    {
        InputField tmpBtn = transform.GetComponent<InputField>();
        if (tmpBtn != null)
        {
            tmpBtn.onEndEdit.AddListener(action);
        }
    }
    //InputFiled事件
    public void AddInputFiledOnValueChangedListen(UnityAction<string> action)
    {
        InputField tmpBtn = transform.GetComponent<InputField>();
        if (tmpBtn != null)
        {
            tmpBtn.onValueChanged.AddListener(action);
        }
    }

    public void AddToggleListen(UnityAction<bool> action)
    {
        Toggle toggle = transform.GetComponent<Toggle>();
        if(toggle != null)
        {
            toggle.onValueChanged.AddListener(action);
        }
    }

    internal void SetToggleIsON(bool isOn)
    {
        Toggle toggle = transform.GetComponent<Toggle>();
        if(toggle != null)
        {
            toggle.isOn = isOn;
        }
    }

    // 文字事件
    public void ChangeTextConent(string conent)
    {
        TextMeshProUGUI tmpText = transform.GetComponent<TextMeshProUGUI>();
        if(tmpText != null)
        {
            tmpText.text = conent;
            return;
        }
        Text tmpBtn = transform.GetComponent<Text>();
        if (tmpBtn != null)
        {
            tmpBtn.text = conent;
            return;
        }
    }
    //改变图片事件
    public void ChangeImageConent(Sprite conent)
    {
        Image tmpBtn = transform.GetComponent<Image>();
        if (tmpBtn != null)
        {
            tmpBtn.sprite = conent;
        }
    }
    //动态添加 接口事件
    public void AddDragInterface(UnityAction<BaseEventData> action)
    {
        //获取事件系统
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        //如果不存在，动态添加
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }
        //事件实体
        EventTrigger.Entry entry = new EventTrigger.Entry();
        //事件类型
        entry.eventID = EventTriggerType.Drag;
        //事件回调
        entry.callback = new EventTrigger.TriggerEvent();
        //添加回调函数
        entry.callback.AddListener(action);
        //监听事件
        trigger.triggers.Add(entry);
    }
    public void AddPointClick(UnityAction<BaseEventData> action)
    {//获取事件系统
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        //如果不存在，动态添加
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }
        //事件实体
        EventTrigger.Entry entry = new EventTrigger.Entry();
        //事件类型
        entry.eventID = EventTriggerType.PointerClick;
        //事件回调
        entry.callback = new EventTrigger.TriggerEvent();
        //添加回调函数
        entry.callback.AddListener(action);
        //监听事件
        trigger.triggers.Add(entry);
    }
    public void AddOnBeginDrag(UnityAction<BaseEventData> action)
    {
        //获取事件系统
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        //如果不存在，动态添加
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }
        //事件实体
        EventTrigger.Entry entry = new EventTrigger.Entry();
        //事件类型
        entry.eventID = EventTriggerType.BeginDrag;
        //事件回调
        entry.callback = new EventTrigger.TriggerEvent();
        //添加回调函数
        entry.callback.AddListener(action);
        //监听事件
        trigger.triggers.Add(entry);
    }
    public void AddEndDrag(UnityAction<BaseEventData> action)
    {
        //获取事件系统
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        //如果不存在，动态添加
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }
        //事件实体
        EventTrigger.Entry entry = new EventTrigger.Entry();
        //事件类型
        entry.eventID = EventTriggerType.EndDrag;
        //事件回调
        entry.callback = new EventTrigger.TriggerEvent();
        //添加回调函数
        entry.callback.AddListener(action);
        //监听事件
        trigger.triggers.Add(entry);
    }
    public Image GetImage()
    {
        Image tmpBtn = transform.GetComponent<Image>();
        return tmpBtn;
    }
    public void SetSelect()
    {
        Selectable selectable = transform.GetComponent<Selectable>();
        if (selectable != null)
        {
            selectable.Select();
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
