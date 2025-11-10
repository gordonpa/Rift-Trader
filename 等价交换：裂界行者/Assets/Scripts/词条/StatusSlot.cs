// Assets/Scripts/Exchange/StatusSlot.cs
using UnityEngine;
using TMPro;

public class StatusSlot : MonoBehaviour
{
    [SerializeField] private StatusCard currentCard;
    [SerializeField] private TextMeshProUGUI uiText;   // TMP нд╠╬

    public StatusCard Card => currentCard;

    void Start() => RefreshUI();

    public void SwapCard(StatusSlot targetSlot)
    {
        if (targetSlot == null) return;

        (currentCard, targetSlot.currentCard) = (targetSlot.currentCard, currentCard);

        RefreshUI();
        targetSlot.RefreshUI();

        GetComponent<IBuffReceiver>()?.RebuildBuff();
        targetSlot.GetComponent<IBuffReceiver>()?.RebuildBuff();
    }

    void RefreshUI()
    {
        if (uiText) uiText.text = currentCard ? currentCard.cardName : "нч";
    }
}