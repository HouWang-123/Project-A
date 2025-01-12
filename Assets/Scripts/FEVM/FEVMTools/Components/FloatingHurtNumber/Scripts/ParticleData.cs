using System;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class ParticleData:MonoBehaviour
{
    public int BaseTime; // 基础时间
    public int TimeUnit; // 时间流逝

    public float ShowupSpeed; // 出现速度
    public float UpSpeed; //上升速度
    
    public int DropTime; // 开始掉落的时间
    public float DropSpeed; // 掉落速度
    
    public Vector3 BiasVect; // 基础偏移方向
    public float BiasSpread; // 偏移速度，建议传递0.01~0.05
    public int BiasTime; // 偏移持续时间  需要传递参数
    public float BiasSpeedTimeControl; // 偏移速度生效时间段
    // 设置为 0~0.5之间的数字
    // 如果没有传递，则默认0.25
    // 如果传递不合法则默认0.25
    // （起始20%的偏移时间内持续增加，80%~100% 时间内持续减少）
    
    public int DisapearTime; // 开始消失的时间
    public float DisapearSpeed; // 消失速度
    public bool Circle_spread; // 是否圆形分散
    public int MaxX; // 散布X
    public int MaxY;
    public int MinX; // 散步Y
    public int MinY;
    public float ScaleOffset;

    public float offsetX; // 基础偏移
    public float offsetY;
    
    private void Awake()
    {
        GenBias();
    }
    public void GenBias()
    {
        Random r = new Random();
        if (!Circle_spread)
        {
            BiasVect = new Vector3(r.Next(MinX, MaxX)/100f, r.Next(MinY, MaxY)/100f, 0);
        }
        else
        {
            BiasVect = Vector3.up;
            BiasVect = BiasVect * r.Next(MinY,MaxY) * 0.01f;
            Quaternion rotate = Quaternion.Euler(0, 0, r.Next(-18000, 18000)*0.01f);
            BiasVect = rotate * BiasVect;
        }
    }
}