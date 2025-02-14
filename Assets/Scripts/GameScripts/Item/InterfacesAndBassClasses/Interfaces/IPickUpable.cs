/// <summary>
/// 可是拾取接口
/// </summary>
public interface IPickUpable            // 可否被拾取接口
{
    void OnItemPickUp();  // 拾取
    void OnItemDrop(bool fastdrop);    // 放下 2025.2.14更新参数
                                       //        fastdrop:
                                       //        是否忽略物品下落动画（放在过场使用）
}
