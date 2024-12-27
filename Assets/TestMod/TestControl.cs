using UnityEngine;

public class TestControl : MonoBehaviour
{

    public float speed = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 vector3 = transform.position;
        vector3.x += x * speed * Time.deltaTime;
        vector3.z += z * speed * Time.deltaTime;
        transform.position = vector3;
    }
}
