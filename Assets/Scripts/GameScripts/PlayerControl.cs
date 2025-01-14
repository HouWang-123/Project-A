using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody playerRG;
    private Transform playerRenderer;

    public float Speed = 5f;

    //前后移动的速度比率
    private float fToB = 0.6f;

    private void Start()
    {
        playerRG = GetComponent<Rigidbody>();
        playerRenderer = transform.GetChild(0);
        playerRenderer.localEulerAngles = GameConstData.DefAngles;
    }


    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if(x != 0 || z != 0)
        {
            PlayerMove(new Vector3(x, 0, z), Speed * Time.deltaTime);
        }
        //Debug.Log(Input.mousePosition);
        Vector3 v = Camera.main.WorldToScreenPoint(transform.position);
        if(Input.mousePosition.x < v.x)
        {
            playerRenderer.localScale = GameConstData.ReverseScale;
        }
        else
        {
            playerRenderer.localScale = Vector3.one;
        }
    }


    private void PlayerMove(Vector3 vector, float speed)
    {
        if(playerRG != null)
        {
            if((vector.x > 0 && playerRenderer.localScale.x < 0) || (vector.x < 0 && playerRenderer.localScale.x > 0))
            {
                speed *= fToB;
            }
            playerRG.Move(vector * speed + transform.position, Quaternion.identity);
        }
    }



}
