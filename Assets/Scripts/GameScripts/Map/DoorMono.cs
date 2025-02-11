using System;
using UnityEngine;
using cfg.scene;

public class DoorMono : MonoBehaviour
{
    private Doors doorData;

    private bool doorEnabled = true;

    public void SetData(int v)
    {
        SetData(GameTableDataAgent.DoorsTable.Get(v));
    }

    public void SetData(Doors data)
    {
        doorData = data;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(doorEnabled && other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Comming!!!!");
            GameControl.Instance.ChangeRoom(doorData.ToRoomID,doorData.ToDoorID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            doorEnabled = true;
        }
    }

    public Transform GetPlayerPoint()
    {
        doorEnabled = false;
        return transform;
    }
}
