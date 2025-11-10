using UnityEngine;

public class HitBox : MonoBehaviour
{
    public int damage = 20;
    public string targetTag = "Enemy";
    public float lifeTime = 0.35f;   // 特效存活时间

    void Start()
    {
        Destroy(gameObject, lifeTime);   // 时间到自动消失
    }

    /* 触发式伤害 */
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        // 这里调你自己的受伤接口
        if (other.TryGetComponent(out IDamageable d))
            d.TakeDamage(damage);
    }
}