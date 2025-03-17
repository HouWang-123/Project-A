using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SearchService;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;

public class PlayerPickupController : MonoBehaviour
{
    public ItemBase currentPickup; // 目标拾取
    private List<ItemBase> Item2PickList; // 拾取列表
    private LayerMask ItemLayerMask;

    private void Awake()
    {
        ItemLayerMask = LayerMask.GetMask("Item");
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
            Debug.Log(hitinfo.transform.name);
            ItemBase ib = hitinfo.transform.GetComponent<ItemBase>();
            // if (ib == null)
            // {
            //     ib = hitinfo.transform.GetComponentInChildren<ItemBase>();
            // }
            
            if (ib == null) return;
            if (ib.DropState) return;
            EventManager.Instance.RunEvent(EventConstName.OnMouseFocusItemChanges, ib);
            if (ib == currentPickup) return;
            
            
            if (Item2PickList.Contains(ib))
            {
                ChangeToTargetItem(ib);
            }
        }
        else
        {
            EventManager.Instance.RunEvent<ItemBase>(EventConstName.OnMouseFocusItemChanges, null);
        }
        // Vector2 mousePosition = Mouse.current.position.ReadValue();
        // Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        // RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        // if (hit.transform == null) return;
        // ItemBase itemBase = hit.transform.GetComponentInParent<ItemBase>();
        // if (itemBase == null) return;
        // if (itemBase.PickUpTargeted) return;
        // if (itemBase.DropState) return;
        // ChangeToTargetItem(itemBase);
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
        if (other.gameObject.tag.Equals("Item"))
        {
            if (currentPickup != null)
            {
                ItemBase DropingEnters = other.gameObject.GetComponent<ItemBase>();
                if (DropingEnters.DropState)
                {
                    DropingEnters.OnDropCallback = () =>
                    {
                        Item2PickList.Add(DropingEnters);
                        UpdateCurrentPickup();
                        DropingEnters.SetPickupable(true);
                        currentPickup = DropingEnters;
                    };
                    return;
                }

                currentPickup.SetTargerted(false); // 新物品进入范围后取消之前的目标
            }

            ItemBase newEnter = other.gameObject.GetComponent<ItemBase>();

            if (newEnter.DropState) // 处理下落
            {
                newEnter.OnDropCallback = () =>
                {
                    Item2PickList.Add(newEnter);
                    newEnter.SetPickupable(true);
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
            newEnter.SetPickupable(true);
            currentPickup = newEnter;
            UpdateCurrentPickup();
        }
    }

    private void OnTriggerExit(Collider other) // 物品离开拾取范围
    {
        if (other.gameObject.tag.Equals("Item"))
        {
            ItemBase itemBase = other.gameObject.GetComponent<ItemBase>();
            itemBase.SetPickupable(false);
            itemBase.SetTargerted(false);
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
        EventManager.Instance.RunEvent<ItemBase>(EventConstName.OnMouseFocusItemChanges, null);
    }

    private void UpdateCurrentPickup()
    {
        if (currentPickup == null) return;
        foreach (ItemBase itemBase in Item2PickList)
        {
            itemBase.SetTargerted(false);
        }

        currentPickup.SetTargerted(true);
    }
}