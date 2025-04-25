using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SearchService;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

public class PlayerPickupController : MonoBehaviour
{
    public ItemBase currentPickup; // 目标拾取
    private List<ItemBase> Item2PickList; // 拾取列表
    private LayerMask ItemLayerMask;

    private void Awake()
    {
        ItemLayerMask = LayerMask.GetMask("Item","Interactive");
    }

    public void Start()
    {
        Item2PickList = new List<ItemBase>();
        EventManager.Instance.RegistEvent("OnPickUpTargetChanges", (object item) => { ChangeToTargetItem(item); });
    }

    private void Update()
    {
        PlayerChangePickupItem();
    }

    private void FixedUpdate()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Debug.DrawRay(ray.origin,ray.direction * 400,Color.red);
        bool hit = Physics.Raycast(ray.origin, ray.direction, out var hitinfo, 20000, ItemLayerMask);
        if (hit)
        {
            ItemBase ib = hitinfo.transform.GetComponent<ItemBase>();
            if (ib != null)
            {
                if (ib.DropState) return;
                EventManager.Instance.RunEvent<Object>(EventConstName.OnMouseFocusItemChanges,ib);
                if (Item2PickList.Contains(ib))
                {
                    if (ib == currentPickup) return;
                    ChangeToTargetItem(ib);
                    return;
                }
            }
            
            // 可交互优先级低于一般物品
            IInteractHandler interactHandler = hitinfo.transform.GetComponent<IInteractHandler>();   // 可交互优先级大于一般可拾取物品
            if (interactHandler != null)
            {
                MonoBehaviour monoBehaviour = interactHandler.getMonoBehaviour();
                EventManager.Instance.RunEvent<Object>(EventConstName.OnMouseFocusItemChanges, monoBehaviour);
            }
        }
        else
        {
            EventManager.Instance.RunEvent<Object>(EventConstName.OnMouseFocusItemChanges, null);
        }
    }

    float changekeyPressTime;
    bool isfirstInput = false;

    private void PlayerChangePickupItem()
    {
        if (changeItemTooggle)
        {
            changekeyPressTime += Time.deltaTime;
            if (isfirstInput)
            {
                ChangeNextPickupTarget();
                isfirstInput = false;
            }
            else if (changekeyPressTime >= 0.2f)
            {
                ChangeNextPickupTarget();
                changekeyPressTime = 0;
                isfirstInput = false;
            }

            UpdateCurrentPickup();
        }
        else
        {
            changekeyPressTime = 0;
            isfirstInput = true;
        }
    }

    private bool changeItemTooggle;

    public void ChangeItemToogle(bool toggle)
    {
        changeItemTooggle = toggle;
    }

    public void ChangeNextPickupTarget() // 拾取范围内目标拾取物品转换逻辑
    {
        if (Item2PickList.Count == 0)
        {
            currentPickup = null;
            return;
        }

        if (currentPickup != null)
        {
            currentPickup.SetTargerted(false);
        }

        int RangeLength = Item2PickList.Count;
        int nextindex = Item2PickList.IndexOf(currentPickup) + 1;
        if (Item2PickList.IndexOf(currentPickup) + 1 == RangeLength)
        {
            nextindex = 0;
        }

        currentPickup = Item2PickList[nextindex];
        currentPickup.SetPickupable(true);
        currentPickup.SetTargerted(true);
    }

    public void ChangeToTargetItem(object item) // 拾取范围内目标拾取物品转换逻辑
    {
        ItemBase ib = item as ItemBase;
        if (Item2PickList.Contains(ib))
        {
            if (currentPickup != null)
            {
                currentPickup.SetTargerted(false);
            }

            currentPickup = ib;
            UpdateCurrentPickup();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        ItemBase newEnter = other.gameObject.GetComponent<ItemBase>();
        if (newEnter == null) return;
        if (newEnter is IPickUpable toPick)
        {
            if (currentPickup != null)
            {

                if (newEnter.DropState)
                {
                    newEnter.OnDropCallback = () =>
                    {
                        Item2PickList.Add(newEnter);
                        UpdateCurrentPickup();
                        toPick.SetPickupable(true);
                        currentPickup = newEnter;
                    };
                    return;
                }
                
                toPick.SetTargerted(false); // 新物品进入范围后取消之前的目标
            }
            
            if (newEnter.DropState) // 处理下落
            {
                newEnter.OnDropCallback = () =>
                {
                    Item2PickList.Add(newEnter);
                    toPick.SetPickupable(true);
                    currentPickup = newEnter;
                    UpdateCurrentPickup();
                };
                return;
            }

            if (newEnter == null)
            {
                Debug.LogWarning("no ItemBase component assigned to " + other.gameObject.name);
                return;
            }

            if (Item2PickList.Contains(newEnter)) return;
            Item2PickList.Add(newEnter);
            toPick.SetPickupable(true);
            currentPickup = newEnter;
            UpdateCurrentPickup();
        }
        
    }

    private void OnTriggerExit(Collider other) // 物品离开拾取范围
    {
        ItemBase itemBase = other.gameObject.GetComponent<ItemBase>();
        if (itemBase == null) return;
        if (itemBase is IPickUpable toleave)
        {
            itemBase = other.gameObject.GetComponent<ItemBase>();
            toleave.SetPickupable(false);
            toleave.SetTargerted(false);
            itemBase.OnDropCallback = null;
            Item2PickList.Remove(itemBase);
            ChangeNextPickupTarget(); // 重新设定一个目标拾取
            UpdateCurrentPickup();
        }
    }

    public void PlayerPickupItem()
    {
        if (currentPickup == null) return;
        Destroy(currentPickup.gameObject);
        Item2PickList.Remove(currentPickup);
        currentPickup = null;
        EventManager.Instance.RunEvent<Object>(EventConstName.OnMouseFocusItemChanges, null);
    }

    private void UpdateCurrentPickup()
    {
        if (currentPickup == null) return;
        foreach (ItemBase itemBase in Item2PickList)
        {
            itemBase.SetTargerted(false);
        }

        if (GameRunTimeData.Instance.characterBasicStatManager.GetStat().LiftedItem == null)
        {
            currentPickup.SetTargerted(true);
        }
    }
}