using UnityEngine;
using UnityEngine.UI;

public class FloatNumberComponent : MonoBehaviour
{
    public ParticleData ParticlePreset;
    public RectTransform m_rectTransform;
    public Text t;
    private int Ttl { get; set; }
    private float showUpSpeed,upSpeed,upSpeedDecay,dropSpeed,dropSpeedIncrease,biasSpeedTime,
        biasSpeed,disapearSpeed,scaleOffset,offsetX,offsetY;
    private int baseTime,timeUnit,dropTime,biasTimeBase,biasTime,disapearTime;
    private Vector3 BiasVect;
    private bool show;
    private bool positionOffset;
    public void OnInit(int num,ParticleData Exdata) { ParticlePreset = Exdata; SetData(); t.text = num.ToString(); }
    public void OnInit(int num) { SetData(); t.text = num.ToString(); }
    private void SetData()
    {
        ParticlePreset.GenBias();
        scaleOffset = ParticlePreset.ScaleOffset; Ttl = ParticlePreset.BaseTime; 
        baseTime = Ttl; timeUnit = ParticlePreset.TimeUnit; showUpSpeed = ParticlePreset.ShowupSpeed;
        upSpeed = ParticlePreset.UpSpeed; upSpeedDecay = upSpeed * 0.03f; dropTime = ParticlePreset.DropTime;
        dropSpeed = ParticlePreset.DropSpeed; BiasVect = ParticlePreset.BiasVect; biasSpeed = ParticlePreset.BiasSpread;
        biasTimeBase = ParticlePreset.BiasTime; biasTime = ParticlePreset.BiasTime; biasSpeedTime = ParticlePreset.BiasSpeedTimeControl;
        disapearTime = ParticlePreset.DisapearTime; disapearSpeed = ParticlePreset.DisapearSpeed;
        offsetX = ParticlePreset.offsetX; offsetY = ParticlePreset.offsetY; dropSpeedIncrease = ParticlePreset.DropSpeed * 0.1f;
        positionOffset = true;
    }
    private void FixedUpdate()
    { if (m_rectTransform.localScale != Vector3.one && !show) // 是否完成展示
        { Show(); }
        if (positionOffset) {
            m_rectTransform.position += new Vector3(offsetX, offsetY, 0);
            positionOffset = false;
        }
        Up();
        if (baseTime < Ttl - dropTime) // 是否达到开始掉落时间
        { Drop(); }
        if (biasTime > 0) // 偏移
        { Bias(); biasTime -= timeUnit; }
        if (baseTime < Ttl - disapearTime) // 是否开始消失
        { Dispear(); }
        if (baseTime < 0 || transform.localScale.x < 0) // 是否存在
        { Destroy(gameObject); }
        baseTime -= timeUnit;
    }
    private void Bias() // 偏移控制
    {
        if (biasTime >= biasTimeBase * (1 - biasSpeedTime))
        { BiasVect += BiasVect * (biasSpeed * 0.01f); }
        m_rectTransform.position += BiasVect;
        if (biasTime <= biasTimeBase * biasSpeedTime)
        { BiasVect -= BiasVect * (biasSpeed * 0.01f); }
    }
    private void Show() // 展示控制
    {
        if (show)
        { return; }
        if (m_rectTransform.localScale.x < 1.0f + scaleOffset)
        { m_rectTransform.localScale += Vector3.one * (showUpSpeed * 0.01f); }
        if (1.0f - m_rectTransform.localScale.x + scaleOffset < 0.0001)
        { show = true; }
    }
    private void Drop() // 掉落控制
    {
        m_rectTransform.position += Vector3.down * dropSpeed;
        dropSpeed += dropSpeedIncrease;
    }
    private void Dispear() // 消失控制
    {
        m_rectTransform.localScale -= Vector3.one * (disapearSpeed * 0.01f);
    }
    private void Up() // 提升控制
    {
        if (upSpeed < 0) { upSpeed = 0; }
        m_rectTransform.position += Vector3.up * upSpeed;
        if (upSpeed > 0) { upSpeed -= upSpeedDecay; }
    }
}