using UnityEngine;

public class MonsterColliderMono : MonoBehaviour
{
    public MonsterBaseFSM monsterBaseFSM;
    private void Start()
    {
        monsterBaseFSM = GetComponentInParent<MonsterBaseFSM>();
        if (monsterBaseFSM == null)
        {
            Debug.LogError("MonsterColliderMono/Start() => 请检查怪物的父物体是否有MonsterBaseFSM组件！");
        }
    }

    // 判断是否进入了光照范围
    private void OnTriggerEnter(Collider other)
    {
        // 如果碰撞到了光源的碰撞体
        if (other == monsterBaseFSM.LightComponent.LightFieldCollider)
        {
            if (monsterBaseFSM.LightComponent.isOn)
            {
                monsterBaseFSM.IsLightOnMonster = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other == monsterBaseFSM.LightComponent.LightFieldCollider)
        {
            if (monsterBaseFSM.LightComponent.isOn)
            {
                monsterBaseFSM.IsLightOnMonster = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == monsterBaseFSM.LightComponent.LightFieldCollider)
        {
            monsterBaseFSM.IsLightOnMonster = false;
        }
    }
}
