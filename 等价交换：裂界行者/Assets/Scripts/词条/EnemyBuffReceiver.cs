using UnityEngine;
using System.Collections;

public class EnemyBuffReceiver : MonoBehaviour, IBuffReceiver
{
    [SerializeField] private StatusSlot slot;
    [SerializeField] private GameObject countdownUI;

    private Enemy enemy;

    void Awake() => enemy = GetComponent<Enemy>();
    void Start() => RebuildBuff();

    public void RebuildBuff()
    {
        var card = slot.Card;
        if (card == null) return;

        // 1. 自爆（先清再订阅）
        enemy.onDeath -= SelfExplode;
        if (card.hasBerserk)
            enemy.onDeath += SelfExplode;

        // 2. 减速光环 - 死亡即关 + 交接还原
        var aura = GetComponent<SlowAura>();
        enemy.onDeath -= _ => aura.RestoreAndDisable();

        // 🔥 卸卡片时：先还原玩家速度再关闭
        if (!card.hasSlowAura && aura.IsActive)
            aura.RestoreAndDisable();
        else if (card.hasSlowAura)
        {
            aura.Enable(card.auraRadius, card.slowFactor);
            enemy.onDeath += _ => aura.RestoreAndDisable();
        }

        // 3. 锁血
        var hp = GetComponent<HPModule>();
        if (card.canStealHP)
            hp.EnterLockHP();
        else
            hp.locked = false;
    }

    /* -------------- 自爆 -------------- */
    private void SelfExplode(Enemy _) => StartCoroutine(DoExplodeRoutine());

    private IEnumerator DoExplodeRoutine()
    {
        var card = slot.Card;
        if (card.explodeDelay > 0)
        {
            if (countdownUI) countdownUI.SetActive(true);
            yield return new WaitForSeconds(card.explodeDelay);
            if (countdownUI) countdownUI.SetActive(false);
        }
        DoExplode();
    }

    private void DoExplode()
    {
        Debug.Log("Enemy 自爆！");
        Destroy(gameObject);
    }
}