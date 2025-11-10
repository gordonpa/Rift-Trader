using UnityEngine;
using UnityEngine.UI;

public class SimpleCrosshair : MonoBehaviour
{
    [SerializeField] private Image hLine;   // 横线
    [SerializeField] private Image vLine;   // 竖线
    [Header("外观")]
    public Color color = Color.white;
    public float length = 20f;   // 单条线长
    public float thickness = 2f; // 线宽

    void Start()
    {
        // 自动生成两条线
        hLine = MakeLine("HLine", transform);
        vLine = MakeLine("VLine", transform);
        ApplyLook();
    }

    Image MakeLine(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(Image));
        go.transform.SetParent(parent, false);
        return go.GetComponent<Image>();
    }

    void ApplyLook()
    {
        // 横线
        hLine.rectTransform.sizeDelta = new Vector2(length, thickness);
        hLine.rectTransform.anchoredPosition = Vector2.zero;
        hLine.color = color;

        // 竖线
        vLine.rectTransform.sizeDelta = new Vector2(thickness, length);
        vLine.rectTransform.anchoredPosition = Vector2.zero;
        vLine.color = color;
    }

    // 运行时想改色/大小直接调
    public void SetColor(Color c) { color = c; ApplyLook(); }
    public void SetSize(float len, float thick) { length = len; thickness = thick; ApplyLook(); }
}