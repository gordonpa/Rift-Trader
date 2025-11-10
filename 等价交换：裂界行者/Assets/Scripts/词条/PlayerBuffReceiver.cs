using UnityEngine;

public class PlayerBuffReceiver : MonoBehaviour, IBuffReceiver
{
    [SerializeField] private StatusSlot slot;

    private PlayerController_2 pc;
    private HPModule hp;
    private SlowAura aura;

    void Awake()
    {
        pc = GetComponent<PlayerController_2>();
        hp = GetComponent<HPModule>();
        aura = GetComponent<SlowAura>();
    }

    void Start() => RebuildBuff();

    public void RebuildBuff()
    {
        var card = slot.Card;
        if (card == null) return;

        // 1. 自爆（先清再订阅）
        pc.onDeath -= SelfExplode;
        if (card.hasBerserk)
            pc.onDeath += SelfExplode;

        // 2. 减速光环 - 卸下时先还原再关闭
        if (!card.hasSlowAura && aura.IsActive)
            aura.RestoreAndDisable();   // 🔥 还原敌人速度
        else if (card.hasSlowAura)
            aura.Enable(card.auraRadius, card.slowFactor);

        // 3. 锁血
        if (card.canStealHP)
            hp.EnterLockHP();
        else
            hp.locked = false;
    }

    private void SelfExplode()
    {
        Debug.Log("Player 自爆！");
        // TODO：粒子、伤害、销毁
    }
}