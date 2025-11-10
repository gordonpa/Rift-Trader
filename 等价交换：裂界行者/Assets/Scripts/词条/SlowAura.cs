using System.Collections.Generic;
using UnityEngine;

public class SlowAura : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    private float radius;
    private float factor;
    private bool active = true;
    public bool IsActive => active;   // 加这一行

    // ******** 1. 缓存圈内玩家 ********
    private readonly HashSet<PlayerController_2> inRangePlayers = new();

    public void Enable(float r, float f)
    {
        radius = r; factor = f; active = true;
    }

    public void Disable() => active = false;

    /* 2. 立即还原并关闭 */
    public void RestoreAndDisable()
    {
        foreach (var p in inRangePlayers)
            p.SetSpeedFactor(1f);
        inRangePlayers.Clear();
        Disable();
    }
    void Update()
    {
        if (!active) return;
        
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, playerLayer);

        HashSet<PlayerController_2> nowInRange = new();

        foreach (var c in cols)
        {
            // 如果是玩家，并且我就是玩家，则跳过
            if (c.CompareTag("Player") && gameObject.CompareTag("Player"))
                continue;

            var p = c.GetComponent<PlayerController_2>();
            if (!p) continue;
            nowInRange.Add(p);
            if (!inRangePlayers.Contains(p))
                p.SetSpeedFactor(factor);
        }

        foreach (var p in inRangePlayers)
            if (!nowInRange.Contains(p))
                p.SetSpeedFactor(1f);

        inRangePlayers.Clear();
        inRangePlayers.UnionWith(nowInRange);
    }

    void OnDrawGizmosSelected()
    {
        if (active) Gizmos.DrawWireSphere(transform.position, radius);
    }

}


