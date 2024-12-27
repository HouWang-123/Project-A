using UnityEngine;

public class TestCamerControl : MonoBehaviour
{
    public Transform player;

    private Vector3 lerp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lerp = transform.position - player.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.position + lerp, 0.5f);
    }
}
