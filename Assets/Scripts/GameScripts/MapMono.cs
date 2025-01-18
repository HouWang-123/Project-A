using UnityEngine;

public class MapMono : MonoBehaviour
{
    public Transform PlayerPoint;

    private void Awake()
    {
        
    }

    private void Start()
    {
        GameControl.Instance.GetGamePlayer().transform.position = PlayerPoint.position;
    }

}
