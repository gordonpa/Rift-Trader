using UnityEngine;
using TMPro;

public class LevelStartPanel : MonoBehaviour
{
    [Header("面板")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descText;

    [Header("本关内容（直接输入）")]
    [SerializeField] private string levelTitle = "关卡标题";
    [SerializeField][TextArea(3, 5)] private string levelDescription = "说明文字";

    [Header("出圈即恢复（可选）")]
    [SerializeField] private bool resumeOnExit = true;

    private int clickCount = 0;        // 点击计数
    private bool isClosed = false;

    void Start()
    {
        Time.timeScale = 0f;           // ① 暂停
        titleText.text = levelTitle;
        descText.text = levelDescription;
        panel.SetActive(true);         // ② 显示
    }

    void Update()
    {
        if (isClosed) return;

        // 任意左键点击
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;
            if (clickCount >= 2)       // ③ 两次点击
                ClosePanel();
        }
    }

    void ClosePanel()
    {
        isClosed = true;
        panel.SetActive(false);
        Time.timeScale = 1f;           // ④ 恢复
        SendMessage("OnLevelStartPanelClosed", SendMessageOptions.DontRequireReceiver);
    }

    // 外部手动再打开（可选）
    public void OpenPanel()
    {
        if (isClosed && !resumeOnExit) return;
        Time.timeScale = 0f;
        clickCount = 0;
        panel.SetActive(true);
    }
}