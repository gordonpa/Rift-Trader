using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// ��������� �� ����Բ�� �� ��� ExchangeTarget
/// ���� Q �ڼ����Ŀ�꣬���ָֻ�
/// </summary>
public class GroundCircleCaster : MonoBehaviour
{
    [Header("ɨ����״")]
    public float radius = 6f;          // Բ�뾶
    public float height = 3f;          // ���ߣ�����ռ䣩

    [Header("���������")]
    public LayerMask groundLayer;
    public LayerMask enemyLayer;

    [Header("����")]
    public KeyCode holdKey = KeyCode.Q;

    [Header("����")]
    public bool drawGizmos = true;

    [Header("E键状态交换")]
    public KeyCode selectKey = KeyCode.E;          // 对称 Q
    private ExchangeTarget selectedTarget;         // 当前 E 选中目标

    [Header("词条UI")]
    [SerializeField] private TextMeshProUGUI targetCardText;   // 拖右上角 TMP

    public ExchangeTarget currentTarget { get; private set; }
    Vector3 groundCenter;              // ���һ�ε������

    ExchangeTarget lastTarget;         // ��ǰ����Ŀ�꣨������ƣ�

    void Awake()
    {
        if (Camera.main == null) Debug.LogError("û�� MainCamera��");
    }

    void Update()
    {
        // 原 Q 键逻辑不动
        if (Input.GetKey(holdKey))
            CastNow();

        if (Input.GetKeyUp(holdKey) && lastTarget != null)
        {
            lastTarget.SetHighlight(false);
            lastTarget = null;
        }

        // E 按住实时（新增）
        if (Input.GetKey(selectKey))          // ← 持续按住
            CastNow();

        if (Input.GetKeyUp(selectKey) && selectedTarget != null)  // ← 松手还原
        {
            selectedTarget.SetHighlight(false, true);
            selectedTarget = null;
        }
    }

    /// <summary>
    /// �ⲿҲ�ɵ��ã��������
    /// </summary>
    public void CastNow()
    {
        // 1-4. 射线检测和选目标（保持不变）
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            currentTarget = null;
            return;
        }
        groundCenter = hit.point;

        Vector3 basePt = groundCenter;
        Vector3 topPt = basePt + Vector3.up * height;
        Collider[] cols = Physics.OverlapCapsule(basePt, topPt, radius, enemyLayer);

        List<ExchangeTarget> valid = cols
            .Select(c => c.GetComponent<ExchangeTarget>())
            .Where(et => et != null && et.allowSwap)
            .ToList();

        ExchangeTarget nearest = null;
        float minDist = Mathf.Infinity;
        foreach (var et in valid)
        {
            float d = Vector3.Distance(et.transform.position, groundCenter);
            if (d < minDist) { nearest = et; minDist = d; }
        }
        currentTarget = nearest;

        // ✅ 5. Q键高亮：仅在未按E键时执行
        if (!Input.GetKey(selectKey))
        {
            if (lastTarget != null && lastTarget != currentTarget)
                lastTarget.SetHighlight(false);
            if (currentTarget != null)
                currentTarget.SetHighlight(true); // Q键：isE=false
            lastTarget = currentTarget;
        }

        // ✅ 6. E键UI
        StatusSlot selfSlot = GetComponent<StatusSlot>();
        if (Input.GetKey(selectKey))
            targetCardText.text = selfSlot.Card ? selfSlot.Card.cardName : "空";
        else
            targetCardText.text = selfSlot.Card ? selfSlot.Card.cardName : "";

        // 7. E键高亮：完全隔离
        if (Input.GetKey(selectKey)) // ✅ 添加条件
        {
            if (selectedTarget != null && selectedTarget != currentTarget)
            {
                selectedTarget.SetHighlight(false, true);
                selectedTarget = null;
            }
            if (currentTarget != null)
            {
                if (selectedTarget != currentTarget)
                    selectedTarget = currentTarget;

                selectedTarget.SetHighlight(true, true); // E键青
            }
        }
        else
        {
            // E键松开时清理
            if (selectedTarget != null)
            {
                selectedTarget.SetHighlight(false, true);
                selectedTarget = null;
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        // ������Բ
        if (Application.isPlaying)
            DrawWireCircle(groundCenter, radius);
        else
            DrawWireCircle(transform.position, radius);
        // ������
        Gizmos.DrawWireSphere(groundCenter, 0.2f);
        Gizmos.DrawWireSphere(groundCenter + Vector3.up * height, 0.2f);
    }

    void DrawWireCircle(Vector3 center, float rad)
    {
        const int seg = 32;
        Vector3 prev = center + Vector3.forward * rad;
        for (int i = 1; i <= seg; i++)
        {
            float ang = 2 * Mathf.PI * i / seg;
            Vector3 next = center + new Vector3(Mathf.Sin(ang), 0, Mathf.Cos(ang)) * rad;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
#endif
}