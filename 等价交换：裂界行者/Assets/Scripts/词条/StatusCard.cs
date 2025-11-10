// Assets/Scripts/Exchange/StatusCard.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatusCard", menuName = "Exchange/StatusCard")]
public class StatusCard : ScriptableObject
{
    [Header("1. 视觉")]
    public string cardName = "Unnamed Card";
    // 在 UI 文本/粒子/光环颜色里使用，一键换主题
    public Color themeColor = Color.white;

    [Header("2. 战斗词条")]
    // 狂热（自爆）
    public bool hasBerserk;        // 是否携带词条
    public float explodeRadius = 5f;// 伤害半径
    public float explodeDelay;      // 0=瞬间自爆，>0=延迟自爆（欺诈彩蛋）

    // 锁血 -------------------------------------------
    public bool canStealHP;        // 是否锁血
    public float lockHPTime = 3f;// 锁血持续时长

    [Header("减速光环")]
    public bool hasSlowAura;     // 是否开启
    public float auraRadius = 4f; // 扫描半径
    public float slowFactor = 0.5f; // 0~1  0.5=减速50%

    /* 后续新词条继续往下加即可，保持 bool + 参数 的格式 */
}