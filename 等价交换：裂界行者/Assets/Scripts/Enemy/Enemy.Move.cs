// Enemy.Move.cs
using UnityEngine;

public partial class Enemy
{
    partial void ChaseBehaviour()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                            Quaternion.LookRotation(dir), 0.1f);

        // 只在射程外才移动
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange)       // 关键：射程外才追
        {
            if (cc)
                cc.Move(dir * moveSpeed * Time.deltaTime);
            else
                transform.position += dir * moveSpeed * Time.deltaTime;
        }
    }
}