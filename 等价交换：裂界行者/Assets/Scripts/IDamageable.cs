// 放在 Assets/Scripts 里，文件名 IDamageable.cs
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount);

}

public interface IExchangeable
{
    bool CanExchange(GameObject p);          // 能否交易
    void OnExchangeStart(GameObject p);      // 交易开始
    void OnExchangeEnd();                // 交易结束（成功或失败）
    bool IsCheating { get; }             // 是否欺诈
}
