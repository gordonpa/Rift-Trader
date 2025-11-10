using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Renderer))]
public class ExchangeTarget : MonoBehaviour
{
    [Header("交换权限")]
    public bool allowSwap = true;

    [Header("高亮材质")]
    public Material highlightMat;        // Q 键（青色）
    public Material highlightMat_E;      // 新增：E 键高亮材质（拖不同颜色）

    [SerializeField] bool useMaterialPropertyBlock = true;

    [Header("事件")]
    public UnityEvent onSwapStart;
    public UnityEvent onSwapComplete;

    [HideInInspector] public bool inArea;

    static readonly int ColorID = Shader.PropertyToID("_BaseColor");

    Renderer mRenderer;
    Material originalMat;
    MaterialPropertyBlock mpb;

    void Awake()
    {
        mRenderer = GetComponent<Renderer>();
        originalMat = mRenderer.material;
        if (useMaterialPropertyBlock) mpb = new MaterialPropertyBlock();
    }

    /// <summary>
    /// 高亮本体：Q = 青色，E = 红色（材质替换版）
    /// </summary>
    public void SetHighlight(bool on, bool isE = false)
    {
        if (!allowSwap) return;
        inArea = on;

        Material targetMat = isE ? highlightMat_E : highlightMat;
        // ✅ 添加这行日志
        Debug.Log($"[{gameObject.name}] isE={isE}, 材质={targetMat?.name}, 颜色={targetMat?.color}");

        if (useMaterialPropertyBlock && targetMat != null)
        {
            if (on)
            {
                Color c = targetMat.color;
                mpb.SetColor(ColorID, c);
                mRenderer.SetPropertyBlock(mpb);
            }
            else
            {
                mRenderer.SetPropertyBlock(null);
            }
        }
        else
        {
            // 指定材质替换（Inspector 拖进来）
            mRenderer.material = on ? targetMat : originalMat;
        }

        // 强制刷新
        mRenderer.enabled = false;
        mRenderer.enabled = true;
    }

    /// <summary>
    /// 松手还原（对称 Q）
    /// </summary>
    public void RestoreHighlight()
    {
        SetHighlight(false, false);   // 取消高亮，回到原材质
    }

    public bool OnSwapConfirmed()
    {
        if (!allowSwap) return false;
        onSwapStart.Invoke();
        return true;
    }

    public void OnSwapFinished()
    {
        onSwapComplete.Invoke();
    }

    void OnDestroy()
    {
        if (mRenderer.material != originalMat)
            DestroyImmediate(mRenderer.material);
    }
}