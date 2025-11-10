using UnityEngine;

public class BulletStraight : MonoBehaviour
{
    [SerializeField] private int damage = 50;   // 每种子弹自带伤害
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 5f;

    private Vector3 direction;

    /* 只需要传方向，伤害自己管 */
    public void Init(Vector3 shootDir)
    {
        direction = shootDir.normalized;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 取玩家脚本并扣血
            other.GetComponent<PlayerController_2>()?.ModifyHP(-damage);
            Destroy(gameObject);
        }
    }
}