using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class UIManager : MonoBehaviour
{
    [Header("Text Displays")]
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI betText;
    [SerializeField] private TextMeshProUGUI statusText; // To show "WIN!" or "SPINNING..."

    [Header("Buttons")]
    [SerializeField] private Button spinButton;
    [SerializeField] private Button increaseBetButton;
    [SerializeField] private Button decreaseBetButton;

    private void OnEnable()
    {
        // Subscribe to GameManager events
        GameManager.OnBalanceChanged += UpdateBalanceUI;
        GameManager.OnBetChanged += UpdateBetUI;
        GameManager.OnStateChanged += HandleGameStateChange;
        GameManager.OnWinProcessed += ShowWinMessage;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        GameManager.OnBalanceChanged -= UpdateBalanceUI;
        GameManager.OnBetChanged -= UpdateBetUI;
        GameManager.OnStateChanged -= HandleGameStateChange;
        GameManager.OnWinProcessed -= ShowWinMessage;
    }

    private void Start()
    {
        statusText.text = "PLACE YOUR BET";
        UpdateBetUI(100);
    }

    private void UpdateBalanceUI(int newBalance)
    {
        balanceText.text = $"BALANCE: {newBalance}";
    }

    private void UpdateBetUI(int newBet)
    {
        betText.text = $"BET: {newBet}";
    }

    private void HandleGameStateChange(GameManager.GameState newState)
    {
        // Only allow button interactions if the game is completely Idle
        bool isIdle = (newState == GameManager.GameState.Idle);

        spinButton.interactable = isIdle;
        increaseBetButton.interactable = isIdle;
        decreaseBetButton.interactable = isIdle;

        if (newState == GameManager.GameState.Spinning)
        {
            statusText.text = "GOOD LUCK...";
        }
        else if (newState == GameManager.GameState.Idle && statusText.text == "GOOD LUCK...")
        {
            statusText.text = "TRY AGAIN!";
        }
    }

    private void ShowWinMessage(int payout)
    {
        statusText.text = $"BIG WIN! +{payout}";
    }
}