using UnityEngine;
using UnityEngine.EventSystems;

public class ClickShowHurt : MonoBehaviour , IPointerClickHandler
{
    public Camera cam;
    public GameObject FloatNum;
    public GameObject ParticlePreset;
    public bool use_external;
    public Sprite sp;
    private bool MakeThings;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        var rect = GetComponent<RectTransform>();
        Vector3 position = Vector3.zero;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, cam, out position);
        System.Random r = new System.Random();
        
        GameObject instance = Instantiate(FloatNum, position, Quaternion.identity);
        FloatNumberBase floatNumberBase = instance.GetComponent<FloatNumberBase>();
        GameObject particleDataInstance = Instantiate(ParticlePreset, position, Quaternion.identity);
        ParticleData particleData = particleDataInstance.GetComponent<ParticleData>();
    
        particleData.gameObject.transform.SetParent(floatNumberBase.transform);
        
        if (use_external)
        {
            floatNumberBase.Initialize(r.Next(80,200),sp,particleData);
        }
        else
        {
            floatNumberBase.Initialize(r.Next(80,200), sp);
        }
    }
}
