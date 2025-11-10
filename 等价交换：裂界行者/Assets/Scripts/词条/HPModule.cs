using UnityEngine;
using System.Collections;

public class HPModule : MonoBehaviour
{
    [SerializeField] private StatusSlot slot;
    [HideInInspector] public bool locked;   // 公开给外部关闭

    void Start()        // 或者由 BuffReceiver 统一在 Start 调用
    {
        // 如果卡片自带锁血，延迟启用
        if (slot && slot.Card && slot.Card.canStealHP)
            StartCoroutine(_LockNextFrame());
    }

    IEnumerator _LockNextFrame()
    {
        yield return null;   // 等一帧
        locked = true;
    }

    /* 由 BuffReceiver 调用：永久开启锁血 */
    public void EnterLockHP() => locked = true;

    /* 由 ModifyHP 调用：若会致死则强制 =1 */
    public int ClampToOneIfNeeded(int newHP)
        => locked && newHP <= 0 ? 1 : newHP;
}