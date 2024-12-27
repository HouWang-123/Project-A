using UnityEngine;

public class IniteTest : MonoBehaviour
{

    private void Start()
    {
        Vector3 angles = transform.localEulerAngles;
        angles.x += 45;
        transform.localEulerAngles = angles;
    }


}
