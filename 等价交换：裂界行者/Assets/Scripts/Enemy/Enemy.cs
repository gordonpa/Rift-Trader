// Enemy.cs
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public partial class Enemy : MonoBehaviour, IDamageable
{
    #region ====== 公共字段 ======
    [Header("HP")]
    public int maxHP = 100;
    public int currentHP;
    public bool isDead = false;

    [Header("AI Settings")]
    [SerializeField] private float scanRadius = 8f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float scanInterval = 0.2f;
    [SerializeField] private float loseTime = 3f;
    [SerializeField] private float moveSpeed = 4f;

    [Header("远程攻击")]
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform muzzle;          // 可空
    [SerializeField] protected float fireInterval = 1f;
    private float lastFireTime;

    [Header("血条")]
    [SerializeField] private Slider hpSlider;   // 拖刚才建的 Slider

    public System.Action<Enemy> onDeath;   // 1. 改成泛型 Action

    // 目标相关
    protected Transform player;          // 给 partial 文件用
    protected float loseSightTimer;
    protected bool hasTarget => player != null;
    #endregion

    #region ====== 组件缓存 ======
    protected Animator anim;
    protected CharacterController cc;
    #endregion

    #region ====== 生命周期 ======
    private void Start()
    {
        currentHP = maxHP;
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        StartCoroutine(ScanLoop());

        if (hpSlider)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }

        // ===== 新增：注册到管理器 =====
        EnemyManager.Instance?.RegisterEnemy(this);
    }
    #endregion

    #region ====== 外部接口 ======
    public void TakeDamage(int amount)
    {
        if (isDead) return;
        int raw = currentHP - amount;   // 敌人用减法
                                        // 锁血：≤0 强制 1 点
        currentHP = GetComponent<HPModule>()?.ClampToOneIfNeeded(raw) ?? raw;

        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        if (hpSlider) hpSlider.value = currentHP;
        if (currentHP <= 0) Die();   // 锁血时永远不会进这里
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        anim.SetBool("die", true);
        onDeath?.Invoke(this);               // 2. 传自己出去
    }

    // Animator 事件
    public void DieEnd() => Destroy(gameObject);
    #endregion

    #region ====== AI 框架 ======
    // 扫描协程
    private IEnumerator ScanLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(scanInterval);
        while (true)
        {
            if (!isDead) Scan();
            yield return wait;
        }
    }

    private void Scan()
    {
        int playerLayer = 1 << LayerMask.NameToLayer("Player");
        var cols = Physics.OverlapSphere(transform.position, scanRadius, playerLayer);

        Transform nearest = null;
        float minDist = float.MaxValue;
        foreach (var c in cols)
        {
            float d = Vector3.Distance(transform.position, c.transform.position);
            if (d < minDist) { nearest = c.transform; minDist = d; }
        }

        // 目标状态机
        if (nearest != null)
        {
            player = nearest;
            loseSightTimer = 0f;
            if (minDist <= attackRange)
                AttackBehaviour();
            else
                ChaseBehaviour();
        }
        else if (player != null)
        {
            loseSightTimer += scanInterval;
            if (loseSightTimer >= loseTime) player = null;
        }
    }
    #endregion

    #region ====== 给 partial 的抽象桩 ======
    // 在 Move 文件实现
    partial void ChaseBehaviour();

    // 在 Attack 文件实现
    partial void AttackBehaviour();
    #endregion
}