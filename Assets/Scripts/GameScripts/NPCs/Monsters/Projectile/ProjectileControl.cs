using System;
using UnityEngine;
/// <summary>
/// ���������
/// </summary>
public class ProjectileControl : MonoBehaviour
{
    public MonsterBaseFSM MonsterBaseFSM;
    // �ٶȴ�С
    public float m_speed = 0f;
    // ���䷽��
    private Vector3 m_direction;
    public Vector3 Direction {  get { return m_direction; } set { m_direction = value; } }
    // ����ʱ��
    private readonly float m_survivalTime = 5f;
    private float m_timer;
    // ��������շ���
    public Action<GameObject> ReturnFunc = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_speed == 0f)
        {
            m_speed = 2.5f;
        }
        m_timer = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += m_speed * Time.deltaTime * m_direction;
        m_timer += Time.deltaTime;
        if (m_timer >= m_survivalTime)
        {
            m_timer = 0f;
            if (ReturnFunc != null)
            {
                ReturnFunc(gameObject);
            }
            else
            {
                ReturnFunc = null;
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerControl>(out _) || other.gameObject.TryGetComponent<MonsterBaseFSM>(out _))
        {
            if (MonsterBaseFSM is DrownedOnesFSM)
            {
                EventManager.Instance.RunEvent(EventConstName.PLAYER_HURTED_BY_DROWNED_ONES_RANGED);
            }
        }
        if (ReturnFunc != null)
        {
            ReturnFunc(gameObject);
        }
        else
        {
            ReturnFunc = null;
            Destroy(gameObject);
        }
        
    }
}
