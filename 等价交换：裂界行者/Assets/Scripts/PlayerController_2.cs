using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterController))]
public partial class PlayerController_2 : MonoBehaviour, IDamageable
{
    [Header("玩家属性")]
    public float walkSpeed = 4.5f;
    public float runSpeed = 7f;
    public float jumpSpeed = 8f;
    public float gravity = -15f;

    [Header("动画")]
    public Animator anim;

    Vector3 move;               // 相机空间归一化输入
    Transform camTransform;     // 主相机
    CharacterController cc;
    Vector3 velocity;           // 含重力速度
    bool jumpPressed;           // 空格按下标记

    [Header("近战剑气")]
    public GameObject slashEffect;   // 拖特效预制体
    public Transform castPoint;     // 胸前/手前空物体

    [Header("交换")]
    public KeyCode selectKey = KeyCode.E;   // 新增

    private float speedFactor = 1f;   // 1=正常，0.5=减速50%
    /* 魔法锁输入 */
    bool isMagicPlaying = false;


    void Awake()
    {
        cc = GetComponent<CharacterController>();
        camTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        if (anim == null) anim = GetComponentInChildren<Animator>();

        Awake_Stats();          // 初始化血量/能量
    }

    void Update()
    {
        // 1. 收集输入
        if (Input.GetKeyDown(KeyCode.Space)) jumpPressed = true;

        // 2. 移动+跳跃（动画也在里面立即触发）
        HandleMovement();

        // 3. 其余动画参数
        UpdateAnimator();

        //近战特效
        if (Input.GetKeyDown(KeyCode.Mouse0))          // 鼠标左键
            Instantiate(slashEffect, castPoint.position, castPoint.rotation);

        // 4. 魔法逻辑（与跳跃无关）
        if (Input.GetKeyUp(KeyCode.Q) && !isMagicPlaying)
        {
            if (!ModifyEE(-1)) return;

            Vector3 camForward = Vector3.ProjectOnPlane(camTransform.forward, Vector3.up).normalized;
            if (camForward.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(camForward);

            anim.SetTrigger("IsMagic");
            isMagicPlaying = true;
        }

        // 5. E 键状态交换（对称 Q）
        if (Input.GetKeyUp(KeyCode.E))
        {
            StatusSlot playerSlot = GetComponent<StatusSlot>();
            StatusSlot enemySlot = GetComponent<GroundCircleCaster>()?.currentTarget?.GetComponent<StatusSlot>();

            Debug.Log($"[E] 按下  playerSlot={playerSlot != null}  enemySlot={enemySlot != null}  EE={EE}");

            if (playerSlot && enemySlot && ModifyEE(-1))
            {
                Debug.Log($"[E] 开始交换  玩家卡={playerSlot.Card?.cardName}  敌人卡={enemySlot.Card?.cardName}");
                playerSlot.SwapCard(enemySlot);
                Debug.Log($"[E] 交换完成  玩家卡={playerSlot.Card?.cardName}  敌人卡={enemySlot.Card?.cardName}");
            }
            else
            {
                Debug.Log("[E] 条件不足，交换失败");
            }
        }
    }

    public void TakeDamage(int amount) => ModifyHP(-amount);

    public void SetSpeedFactor(float f) => speedFactor = Mathf.Clamp01(f);

    void HandleMovement()
    {
        /* 魔法播放时锁水平移动，重力继续 */
        float h = isMagicPlaying ? 0f : Input.GetAxis("Horizontal");
        float v = isMagicPlaying ? 0f : Input.GetAxis("Vertical");
        bool running = Input.GetKey(KeyCode.LeftShift);
        // 原速度计算后乘系数
        float speed = (running ? runSpeed : walkSpeed) * speedFactor;

        // 相机方向
        Vector3 forward = Vector3.ProjectOnPlane(camTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(camTransform.right, Vector3.up).normalized;
        move = Vector3.ClampMagnitude(forward * v + right * h, 1f);

        // 水平移动
        Vector3 horizontalVel = move * speed;
        cc.Move(horizontalVel * Time.deltaTime);

        /* 垂直：重力 + 跳跃 */
        if (cc.isGrounded)
        {
            velocity.y = -0.5f;   // 贴地小值
            if (jumpPressed)
            {
                velocity.y = jumpSpeed;

                // ✅ 立即触发动画，与物理同帧
                anim.SetTrigger("IsJump");

                jumpPressed = false;   // 清除按键标记
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
        cc.Move(velocity * Time.deltaTime);

        // 旋转
        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }

    void UpdateAnimator()
    {
        if (anim == null) return;

        // 跳跃触发已移到 HandleMovement，这里只更新 Move Speed
        float speed = move.magnitude * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed) * speedFactor;
        anim.SetFloat("Move Speed", speed, 0.1f, Time.deltaTime);
    }

    /* 魔法结束回调（Animation Event 调用） */
    void PerformSwap()
    {
        if (MagicManager.Instance != null)
            MagicManager.Instance.TryPerformSwap();
        isMagicPlaying = false;
    }
}

public partial class PlayerController_2
{
    [Header("血量与能量")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int maxEE = 5;

    [Header("UI")]
    [SerializeField] private Slider hpSlider;        // 可选：血条
    [SerializeField] private Image[] eeCells;        // 5 格能量格（拖进来）

    public System.Action onDeath;   // 自爆/死亡回调

    private int currentHP;
    private int currentEE;

    public int HP => currentHP;
    public int EE => currentEE;


    /* 初始化 */
    private void Awake_Stats()
    {
        currentHP = maxHP;
        currentEE = maxEE;
        UpdateUI();
    }

    public void ModifyHP(int delta)
    {
        int raw = currentHP + delta;
        // 锁血：≤0 强制 1 点
        currentHP = GetComponent<HPModule>()?.ClampToOneIfNeeded(raw) ?? raw;

        currentHP = Mathf.Clamp(currentHP, 0, maxHP);   // 再夹一次 0~max
        Debug.Log($"HP={currentHP}  delta={delta}");
        UpdateUI();
        if (currentHP <= 0) OnDeath();   // 锁血时永远不会进这里
    }

    /* 外部调用：消耗或回复能量 */
    public bool ModifyEE(int delta)
    {
        int tmp = currentEE + delta;
        if (tmp < 0) return false;          // 不够扣
        currentEE = Mathf.Clamp(tmp, 0, maxEE);
        UpdateUI();
        return true;
    }

    /* 演示：牺牲 20% 血换 5 秒无敌 */
    public void SacrificeForBuff()
    {
        int cost = Mathf.CeilToInt(maxHP * 0.2f);
        if (currentHP <= cost) return;
        ModifyHP(-cost);
        // TODO: 触发 5 秒无敌 + 高伤
    }

    /* 刷新 UI */
    private void UpdateUI()
    {
        // 血条
        if (hpSlider) hpSlider.value = (float)currentHP / maxHP;

        // 能量格（5 格）
        if (eeCells != null)
        {
            for (int i = 0; i < eeCells.Length; i++)
                eeCells[i].enabled = i < currentEE;
        }
    }

    private void OnDeath()
    {

        anim.SetBool("die", true); // 触发死亡动画

    }
}

