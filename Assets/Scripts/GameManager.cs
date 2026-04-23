using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // --- State Machine ---
    public enum GameState { Idle, Spinning, Evaluating, Payout }
    public GameState CurrentState { get; private set; }

    // --- Events  ---
    public static event Action<GameState> OnStateChanged;
    public static event Action<int> OnBalanceChanged;
    public static event Action<SymbolData[]> OnSpinStarted;
    public static event Action<int> OnWinProcessed;
    public static event Action<int> OnBetChanged;

    // --- References & Variables ---
    [Header("Dependencies")]
    [SerializeField] private RNGManager rngManager;

    [Header("Economy")]
    [SerializeField] private int startingBalance = 10000;
    [SerializeField] private int currentBet = 100; // Hardcoded for now, can hook to UI later

    private int currentBalance;
    private SymbolData[] currentSpinResults;

    private void Start()
    {
        currentBalance = startingBalance;
        ChangeState(GameState.Idle);
        OnBalanceChanged?.Invoke(currentBalance);
    }

    private void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);
    }

    /// <summary>
    /// Called by the UI Spin Button.
    /// </summary>
    public void RequestSpin()
    {
        if (CurrentState != GameState.Idle) return;
        if (currentBalance < currentBet)
        {
            Debug.LogWarning("Not enough balance!");
            return;
        }

        // Deduct bet and update UI
        currentBalance -= currentBet;
        OnBalanceChanged?.Invoke(currentBalance);

        // Lock the game state
        ChangeState(GameState.Spinning);

        // Get the math instantly
        currentSpinResults = rngManager.GenerateSpinResults();

        // Broadcast the results to the reels so they know what to inject
        OnSpinStarted?.Invoke(currentSpinResults);
    }

    /// <summary>
    /// Called by the ReelManager when all physical reels have stopped moving.
    /// </summary>
    public void ReelsFinishedStopping()
    {
        if (CurrentState != GameState.Spinning) return;

        ChangeState(GameState.Evaluating);
        EvaluateWin();
    }

    private void EvaluateWin()
    {
        // Check if all 3 symbols match
        if (currentSpinResults[0] == currentSpinResults[1] && currentSpinResults[1] == currentSpinResults[2])
        {
            int winMultiplier = currentSpinResults[0].payoutMultiplier;
            int payoutAmount = currentBet * winMultiplier;

            Debug.Log($"WIN! Payout: {payoutAmount}");

            ChangeState(GameState.Payout);
            ProcessPayout(payoutAmount);
        }
        else
        {
            Debug.Log("Loss. Better luck next time.");
            ChangeState(GameState.Idle); // Reset for next spin
        }
    }

    private void ProcessPayout(int amount)
    {
        // Add winnings
        currentBalance += amount;
        OnBalanceChanged?.Invoke(currentBalance);
        OnWinProcessed?.Invoke(amount);

        // After payout animations finish, reset to Idle. 
        // For now, we instantly reset.
        ChangeState(GameState.Idle);
    }


    /// <summary>
    /// These functions are used to increase or decrease the bet.
    /// </summary>
    public void IncreaseBet()
    {
        if (CurrentState != GameState.Idle) return;
        currentBet += 50; // Increase by 50 (or whatever you prefer)
        OnBetChanged?.Invoke(currentBet);
    }

    public void DecreaseBet()
    {
        if (CurrentState != GameState.Idle) return;
        if (currentBet <= 50) return; // Prevent betting 0 or negative
        currentBet -= 50;
        OnBetChanged?.Invoke(currentBet);
    }
}