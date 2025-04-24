using Spine.Unity;
using System;
using System.Collections.Generic;
using cfg.interact;
using Spine;
using UnityEngine;

/// <summary>
/// 篮球框的Mono
/// </summary>
public class BasketballHoopMono : SceneObjects, IRoomRiddleItem
{
    [Header("解密房间的引用")] public RoomRiddleMonoBase riddleGameObject;

    [SerializeField] [Header("骨骼动画组件")] private SkeletonAnimation skeletonAnimation;

    // 目前场景中篮球的数量
    private bool statusFromeTracker;
    public int BasketBallCount;
    private int _basketNumPerStep;
    // 放入篮筐的篮球

    public BasketballHoopStatus BasketballHoopStatus
    {
        get { return _basketballHoopStatus; }
        set { _basketballHoopStatus = value; }
    }

    private BasketballHoopStatus _basketballHoopStatus;

    public override void InitItem(int id, TrackerData trackerData = null)
    {
        base.InitItem(id, trackerData);
        if (trackerData != null)
        {
            _basketballHoopStatus = trackerData.TrackableBaseData as BasketballHoopStatus;
            BasketBallCount = _basketballHoopStatus.BasketBallCount;
        }
    }


    // 仅在首次调用 Update 方法之前调用 Start
    protected override void Start()
    {
        base.Start();
        if (!statusFromeTracker)
        {
            _basketballHoopStatus.BasketBallCount = BasketBallCount;
            _basketNumPerStep = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(_basketballHoopStatus.BasketBallCount / 3.0)));
            TrackEntry trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, "step1", false);
            trackEntry.TimeScale = 0f;
        }
        else
        {
            SetAnimation();
        }



    }

    // 如果另一个碰撞器进入了触发器，则调用 OnTriggerEnter
    private void OnTriggerEnter(Collider other)
    {
        // 是否为篮球
        if (other.TryGetComponent<Throwable>(out Throwable throwableCom))
        {
            if (isItemDone())
            {
                return;
            }
            if (throwableCom.ItemID == 230002)
            {
                Destroy(other.gameObject);
                PushBasketBall();
            }
        }
    }

    public void PushBasketBall()
    {
        // 根据放入篮筐的篮球数量判断应该播放哪个动画，篮筐一共有三个动画，step1、step2、step3
        if (_basketballHoopStatus.MyBasketballs < _basketballHoopStatus.BasketBallCount)
        {
            _basketballHoopStatus.MyBasketballs++;
            SetAnimation();
        }
    }

    private bool GetBaseketball = false;
    public void GetBasketball()
    {
        GetBaseketball = true;
        _basketballHoopStatus.MyBasketballs--;
        SetAnimation();
        
    }
    private void SetAnimation()
    {
        Debug.Log("篮球框容量" + _basketballHoopStatus.BasketBallCount);
        Debug.Log("篮球框个数" + _basketballHoopStatus.MyBasketballs);
        if (statusFromeTracker || GetBaseketball)
        {
            skeletonAnimation.state.SetEmptyAnimation(0, 0.1f); // 停止当前轨道的动画
            if (_basketballHoopStatus.MyBasketballs == 0)
            {
                TrackEntry trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, "step1", false);
                trackEntry.TimeScale = 0f;
            }
            else if (_basketballHoopStatus.MyBasketballs % _basketNumPerStep == 0)
            {
                TrackEntry trackEntry = skeletonAnimation.state.SetAnimation(0,
                    $"step{_basketballHoopStatus.MyBasketballs / _basketNumPerStep}", false);
                trackEntry.TimeScale = 999999f;
            }
            else if (_basketballHoopStatus.MyBasketballs == _basketballHoopStatus.BasketBallCount)
            {
                TrackEntry trackEntry = skeletonAnimation.state.SetAnimation(0, "step3", false);
                trackEntry.TimeScale = 999999f;
                // 生成安全区域
                _basketballHoopStatus.isDone = true;
            }
            statusFromeTracker = false;
            GetBaseketball = false;
        }
        else
        {
            skeletonAnimation.state.SetEmptyAnimation(0, 0.1f); // 停止当前轨道的动画
            if (_basketballHoopStatus.MyBasketballs % _basketNumPerStep == 0)
            {
                if (_basketballHoopStatus.MyBasketballs / _basketNumPerStep == 0)
                {
                    TrackEntry trackEntry2 = skeletonAnimation.state.SetAnimation(0,
                        $"step1", false);
                    trackEntry2.TimeScale = 1f;
                    return;
                }
                TrackEntry trackEntry = skeletonAnimation.state.SetAnimation(0,
                    $"step{_basketballHoopStatus.MyBasketballs / _basketNumPerStep}", false);
                trackEntry.TimeScale = 1f;
            }
        }
        if (_basketballHoopStatus.MyBasketballs == _basketballHoopStatus.BasketBallCount)
        {
            TrackEntry trackEntry = skeletonAnimation.state.SetAnimation(0, "step3", false);
            trackEntry.TimeScale = 1f;
            // 生成安全区域
            _basketballHoopStatus.isDone = true;
            //todo 启用安全区
        }
        else
        {
            _basketballHoopStatus.isDone = false;
            //todo 禁用安全区
        }
    }
    
    public bool isItemDone()
    {
        return _basketballHoopStatus.isDone;
    }

    public GameObject GetGO()
    {
        return gameObject;
    }

    // InteractFunctions
    public override void OnPlayerStartInteract(int itemid)
    {
        base.OnPlayerStartInteract(itemid);
        int findInteractEffectById = GameItemInteractionHub.FindInteractEffectById(itemid, ItemID);
        InteractEffect interactEffect = GameTableDataAgent.InteractEffectTable.Get(findInteractEffectById);
        GameInteractSystemExtendedCode.ExecuteInteraction(interactEffect.CodeExecuteID, gameObject);
    }

    public override void OnPlayerFocus(int itemid)
    {
        base.OnPlayerFocus(itemid);
        int findInteractEffectById = GameItemInteractionHub.FindInteractEffectById(itemid, ItemID);
        InteractEffect interactEffect = GameTableDataAgent.InteractEffectTable.Get(findInteractEffectById);
        string translation = LocalizationTool.Instance.GetTranslation(interactEffect.ShortText);
        Debug.Log(translation);
    }

    protected override void GenerateItemStatus()
    {
        if (MyItemStatus != null) return;
        _basketballHoopStatus = new BasketballHoopStatus();
        MyItemStatus = _basketballHoopStatus;
    }
    public override void SetItemStatus(ItemStatus itemStatus)
    {
        base.SetItemStatus(itemStatus);
        statusFromeTracker = true;
        BasketballHoopStatus basketballHoopStatus = MyItemStatus as BasketballHoopStatus;
        BasketballHoopStatus = basketballHoopStatus;
        _basketNumPerStep = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(BasketballHoopStatus.BasketBallCount / 3.0)));
    }

    public int MyBasketballCount()
    {
        return _basketballHoopStatus.MyBasketballs;
    }
}

public class BasketballHoopStatus : ItemStatus
{
    public int BasketBallCount;
    public bool isDone;
    public int MyBasketballs;
}