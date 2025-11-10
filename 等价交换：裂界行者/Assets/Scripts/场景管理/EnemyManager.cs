using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private readonly HashSet<Enemy> aliveEnemies = new HashSet<Enemy>();

    [SerializeField] private bool startOpenIfNoEnemies = true;

    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // 等一帧，让所有 Enemy.Start() 先跑完
        StartCoroutine(CheckInitialOpen());
    }

    private System.Collections.IEnumerator CheckInitialOpen()
    {
        yield return null; // 等一帧
        if (startOpenIfNoEnemies && aliveEnemies.Count == 0)
            TryOpenPortal();
    }

    public void RegisterEnemy(Enemy e)
    {
        if (e == null || e.isDead) return;
        aliveEnemies.Add(e);
        e.onDeath += OnEnemyDied;
    }

    private void OnEnemyDied(Enemy deadEnemy)
    {
        aliveEnemies.Remove(deadEnemy);
        TryOpenPortal();
    }

    public void UnregisterSilently(Enemy e)
    {
        if (aliveEnemies.Remove(e))
            TryOpenPortal();
    }

    private void TryOpenPortal()
    {
        if (aliveEnemies.Count == 0)
        {
            PortalExit portal = FindObjectOfType<PortalExit>();
            if (portal) portal.SetOpen(true);
        }
    }

    public int AliveCount => aliveEnemies.Count;
}