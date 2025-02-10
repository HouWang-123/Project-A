/// <summary>
/// 可是拾取接口
/// </summary>
public interface IPickUpable            // 可否被拾取接口
{
    void OnItemPickUp();  // 拾取
    void OnItemDrop();    // 放下
}
