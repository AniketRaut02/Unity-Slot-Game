using System;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public int TotalWagered { get; private set; }
    public int TotalWon { get; private set; }
    public int NetProfit => TotalWon - TotalWagered;

    // Broadcasts the updated stats: (Wagered, Won, Net)
    public static event Action<int, int, int> OnStatsUpdated;

    private void OnEnable()
    {
        GameManager.OnBetDeducted += HandleBetDeducted;
        GameManager.OnWinProcessed += HandleWinProcessed;
    }

    private void OnDisable()
    {
        GameManager.OnBetDeducted -= HandleBetDeducted;
        GameManager.OnWinProcessed -= HandleWinProcessed;
    }

    private void HandleBetDeducted(int amount)
    {
        TotalWagered += amount;
        BroadcastStats();
    }

    private void HandleWinProcessed(int amount)
    {
        TotalWon += amount;
        BroadcastStats();
    }

    private void BroadcastStats()
    {
        OnStatsUpdated?.Invoke(TotalWagered, TotalWon, NetProfit);
    }
}