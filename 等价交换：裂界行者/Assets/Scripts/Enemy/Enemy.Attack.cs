// Enemy.Attack.cs
using UnityEngine;

public partial class Enemy
{
    /* ---------- 进入射程后每帧调用 ---------- */
    partial void AttackBehaviour()
    {
        // 1. 始终面向玩家
        Vector3 lookDir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                            Quaternion.LookRotation(lookDir), 0.15f);

        // 2. 冷却开火
        if (Time.time >= lastFireTime + fireInterval)
        {
            lastFireTime = Time.time;
            Fire();
        }
    }

    private void Fire()
    {
        Vector3 spawnPos = (muzzle ? muzzle : transform).position;

        // 主角脚底+0.7 作为真实目标点
        Vector3 targetPos = player.position + Vector3.up * 0.7f;
        Vector3 dir = (targetPos - spawnPos).normalized;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(dir));
        bullet.GetComponent<BulletStraight>().Init(dir);

        anim.SetTrigger("attack");
    }
}