using System.Security.AccessControl;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FloatNumberBase : MonoBehaviour
{
    public Image image;
    public TextMeshPro m_Text;
    public ParticleData ParticlePreset;
    public int Ttl { get; set; } // 毫秒，控制出现时间
    private RectTransform rectTransform;
    private Sprite _sprite;
    
    private int _hurt;
    public int BaseTime; // 基础时间
    public int TimeUnit; // 时间流逝
    private float ShowupSpeed; // 出现速度
    private float UpSpeed; //上升速度
    private float UpSpeed_Decay; // 上升速度衰减,系统自行计算，不需要传参
    private int DropTime; // 开始掉落的时间
    private float DropSpeed; // 掉落速度
    private float DropSpeedIncrease; // 掉落提速，系统自动计算，不需要传参
    private Vector3 BiasVect; // 基础偏移方向
    private int BiasTimeBase; // 偏移初始时间
    private int BiasTime; // 偏移持续时间  需要传递参数
    private float BiasSpeedTime; // 偏移速度生效时间段
                                // 设置为 0~0.5之间的数字
                                // 如果没有传递，则默认0.25
                                // 如果传递不合法则默认0.25
    private float BiasSpeed; // 偏移速度，建议传递0.01~0.05
    // （起始20%的偏移时间内持续增加，80%~100% 时间内持续减少）
    private int DisapearTime; // 开始消失的时间
    private float DisapearSpeed; // 消失速度
    private float ScaleOffset; // 整体大小调整
    private float offsetX;
    private float offsetY;

    public void Initialize(int hurt, Sprite sprite,ParticleData Exdata)
    { ParticlePreset = Exdata; SetData(); setImage(hurt, sprite);
    }
    public void Initialize(int hurt, Sprite sprite)
    { SetData(); setImage(hurt, sprite); }
    private void SetData()
    {
        ParticlePreset.GenBias();
        ScaleOffset = ParticlePreset.ScaleOffset;
        Ttl = ParticlePreset.BaseTime; 
        BaseTime = Ttl;
        TimeUnit = ParticlePreset.TimeUnit;
        ShowupSpeed = ParticlePreset.ShowupSpeed;
        UpSpeed = ParticlePreset.UpSpeed;
        UpSpeed_Decay = UpSpeed * 0.03f;
        DropTime = ParticlePreset.DropTime;
        DropSpeed = ParticlePreset.DropSpeed;
        BiasVect = ParticlePreset.BiasVect;
        BiasSpeed = ParticlePreset.BiasSpread;
        BiasTimeBase = ParticlePreset.BiasTime;
        BiasTime = ParticlePreset.BiasTime;
        BiasSpeedTime = ParticlePreset.BiasSpeedTimeControl;
        DisapearTime = ParticlePreset.DisapearTime;
        DisapearSpeed = ParticlePreset.DisapearSpeed;
        offsetX = ParticlePreset.offsetX;
        offsetY = ParticlePreset.offsetY;
        DropSpeedIncrease = ParticlePreset.DropSpeed * 0.1f;
    }
    private void setImage(int hurt , Sprite sprite)
    {
        _sprite = sprite;
        _hurt = hurt;
        
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one * 0.01f;

        rectTransform.position += new Vector3(offsetX/100f,0,0);
        rectTransform.position += new Vector3(0,offsetY/100f,0);
        
        image.sprite = sprite;
        
        m_Text.text = _hurt.ToString();
    }
    private bool show;
    public void FixedUpdate()
    {
        if (rectTransform.localScale != Vector3.one && !show) // 是否完成展示
        {
            Show();
        }
        Up();
        if (BaseTime < Ttl - DropTime) // 是否达到开始掉落时间
        {
            Drop();
        }
        if (BiasTime > 0) // 偏移
        {
            Bias();
            BiasTime -= TimeUnit;
        }
        if (BaseTime < Ttl - DisapearTime) // 是否开始消失
        {
            Dispear();
        }
        if (BaseTime < 0 || transform.localScale.x < 0) // 是否存在
        {
            Destroy(gameObject);
        }
        BaseTime -= TimeUnit;
    }
    
    private void Bias() // 偏移控制
    {
        if (BiasTime >= BiasTimeBase * (1 - BiasSpeedTime))
        {
            BiasVect += BiasVect * (BiasSpeed * 0.01f);
        }
        rectTransform.position += BiasVect;
        if (BiasTime <= BiasTimeBase * BiasSpeedTime)
        {
            BiasVect -= BiasVect * (BiasSpeed * 0.01f);
        }
    }

    private void Show() // 展示控制
    {
        if (show)
        {
            return;
        }
        if (rectTransform.localScale.x < 1.0f + ScaleOffset)
        {
            rectTransform.localScale += Vector3.one * (ShowupSpeed * 0.01f);
        }

        if (1.0f - rectTransform.localScale.x + ScaleOffset < 0.0001)
        {
            show = true;
        }
    }

    private void Drop() // 掉落控制
    {
        rectTransform.position += Vector3.down * DropSpeed;
        DropSpeed += DropSpeedIncrease;
    }

    private void Dispear() // 消失控制
    {
        rectTransform.localScale -= Vector3.one * (DisapearSpeed * 0.01f);
    }

    private void Up() // 提升控制
    {
        if (UpSpeed < 0)
        {
            UpSpeed = 0;
        }
        rectTransform.position += Vector3.up * UpSpeed;
        if (UpSpeed > 0)
        {
            UpSpeed -= UpSpeed_Decay;
        }
    }
}