using System.Net;
using UnityEngine;

public class BaseWeaponBehavior : MonoBehaviour
{
    // 武器攻击
    public int WeaponID;
    public virtual void OnWeaponAttack(float DamageAmount){}
}
