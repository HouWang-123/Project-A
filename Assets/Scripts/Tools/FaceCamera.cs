using UnityEngine;
/// <summary>
/// Sprite始终朝向摄像机
/// </summary>
public class FaceCamera : MonoBehaviour
{
    // 引用摄像机
    public Camera targetCamera;

    // 如果未指定摄像机，则使用主摄像机
    private Camera mainCamera;
    // 哪个Obj要面向摄像机
    [SerializeField]
    private GameObject wantToFaceObj;

    void Start()
    {
        // 如果未指定摄像机，则获取主摄像机
        if (targetCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("没有找到主摄像机，请为FaceCamera脚本指定一个摄像机。");
            }
        }
        if(wantToFaceObj == null )
        {
            wantToFaceObj = transform.GetChild(0).gameObject;
        }
    }

    void LateUpdate()
    {
        // 使用指定的摄像机或主摄像机
        Camera cameraToUse = targetCamera != null ? targetCamera : mainCamera;

        if (cameraToUse != null)
        {
            wantToFaceObj.transform.forward = cameraToUse.transform.forward;
        }
    }
}

