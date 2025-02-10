using UnityEngine;

public class Ak47 : Weapon
{
    public override void Fire()
    {
    }
    public override void OnItemPickUp()
    {
        Debug.Log("获得物品" + ItemData.NAME);
    }

}
