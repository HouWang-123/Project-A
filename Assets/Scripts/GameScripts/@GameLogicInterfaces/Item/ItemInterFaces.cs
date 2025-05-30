/// <summary>
/// 可是拾取接口
/// </summary>
public interface IPickUpable            // 可否被拾取接口
{
    public void OnItemPickUp();  // 拾取或举起
    public void OnItemDrop(bool fastdrop, bool ignoreBias,bool playerReversed);
   //   在玩家丢弃物品后需要调用，用于将物品归还到场景物品节点，与Player分离
   //        2025.2.14更新参数
   //        fastdrop:
   //        是否忽略物品下落动画
   
   //        2025.3.1 添加参数 ignoreBias
   //        是否忽略物品丢弃后的随机偏移
   //        2025.3.1 OnItemDrop 后会强制修正物体角度
   public void SetTargerted(bool v);
   public void SetPickupable(bool v);
}

// 可否举起
public interface ILiftable : IPickUpable
{
    
}


/// <summary>
/// 2025年3.3日添加接口，用于纯数据道具
/// </summary>
// 用作仅对数据进行修改的物品
public interface IDataItem
{
    public void OnItemGet();
}
// 可否进入道具栏
public interface ISlotable
{
    public int GetItemId();
}

// 道具栏中可否进行堆叠
public interface IStackable : ISlotable
{
    /// <summary>
    /// 获取该物品的最大堆叠数量
    /// </summary>
    /// <returns></returns>
    public int GetMaxStackValue();
    /// <summary>
    /// 修改物体最大堆叠
    /// </summary>
    /// <param name="Count"></param>
    public void ChangeStackCount(int Count);
    /// <summary>
    /// 获取当前物体的堆叠数量
    /// </summary>
    /// <returns></returns>
    public int GetStackCount();
}
/// <summary>
/// 是否属于Buff物品
/// </summary>
public interface IBuffedItem
{
    /// <summary>
    /// 获取buffId
    /// </summary>
    /// <returns></returns>
    public int GetBuffId();
}

/// <summary>
/// 需要添加 Trowstate
/// </summary>
public interface IThrowable
{
    void OnThrow();      /// <summary>
                         /// Start
                         /// </summary>
}