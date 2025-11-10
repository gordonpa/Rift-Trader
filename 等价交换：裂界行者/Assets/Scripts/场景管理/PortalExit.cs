// PortalExit.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PortalExit : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    public bool IsOpen { get; private set; } = false;

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    public void SetOpen(bool open)
    {
        IsOpen = open;
        Debug.Log($"[PortalExit] 门状态：{(IsOpen ? "开启" : "关闭")}");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[PortalExit] 碰撞进入：{other.name}  Tag:{other.tag}  门状态：{IsOpen}");
        if (IsOpen && other.CompareTag(playerTag))
        {
            Debug.Log("[PortalExit] 玩家触碰已开启的传送门 → 进入下一关");
            GameManager.Instance.LoadNextLevel();
        }
    }
}