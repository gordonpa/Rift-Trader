using UnityEngine;
using UnityEngine.Events;

public class MagicManager : MonoBehaviour
{
    public static MagicManager Instance { get; private set; }

    [Header("松手事件")]
    public UnityEvent onSwap;          // 音效/粒子/EE消耗等

    GroundCircleCaster caster;         // 缓存引用

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // 在任何地方找到玩家身上的 GroundCircleCaster
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Debug.LogError("找不到 Tag 为 Player 的物体！");
        caster = player.GetComponent<GroundCircleCaster>();
        if (caster == null) Debug.LogError("玩家身上没有 GroundCircleCaster！");
    }

    void Update()
    {
        
    }

    public void TryPerformSwap()
    {
        var target = caster.currentTarget;
        if (target == null) return;

        // 1. 灭高亮
        target.SetHighlight(false);

        // 拿玩家 Transform
        Transform playerTf = caster.transform;

        // 缓存原坐标
        Vector3 pPos = playerTf.position;
        Vector3 tPos = target.transform.position;

        // ****** 关键：关闭 CC，否则坐标被拉回 ******
        CharacterController cc = playerTf.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        // 整体瞬移
        Vector3 tmp = playerTf.position;
        playerTf.position = target.transform.position;
        target.transform.position = tmp+ Vector3.up * 0.825f;

        // ****** 再打开 ******
        if (cc) cc.enabled = true;

        // 3. 广播
        onSwap.Invoke();
        target.OnSwapFinished();
    }
}