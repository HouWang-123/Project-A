using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SearchService;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;

public class PlayerPickupController : MonoBehaviour
{
    public ItemBase currentPickup; // 目标拾取
    private List<ItemBase> Item2PickList; // 拾取列表

    public void Start()
    {
        Item2PickList = new List<ItemBase>();
        EventManager.Instance.RegistEvent("OnPickUpTargetChanges",(object item)=>
        {
            ChangeToTargetItem(item);
        });
    }

    private void Update()
    {
        PlayerChangePickupItem();
    }

    private void FixedUpdate()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.transform == null) return;
        ItemBase itemBase = hit.transform.GetComponentInParent<ItemBase>();
        if (itemBase == null) return;
        if (itemBase.PickUpTargeted) return;
        ChangeToTargetItem(itemBase);
        
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
            currentPickup.SetPickupable(true);
            currentPickup.SetTargerted(true);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Item"))
        {
            if (currentPickup != null)
            {
                currentPickup.SetTargerted(false); // 新物品进入范围后取消之前的目标
            }

            currentPickup = other.gameObject.GetComponent<ItemBase>();
            if (Item2PickList.Contains(currentPickup))
            {
                return;
            }
            if (currentPickup == null)
            {
                Debug.LogWarning("no ItemBase component assigned to " + other.gameObject.name);
                return;
            }
            Item2PickList.Add(currentPickup);
            currentPickup.SetPickupable(true);
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
    }

    private void UpdateCurrentPickup()
    {
        if (currentPickup == null) return;
        currentPickup.SetTargerted(true);
    }
}