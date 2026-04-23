using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

    [Header("Popups")]
    [SerializeField] private GameObject lowCashPopup;
    [SerializeField] private GameObject bankruptPopup;
    [SerializeField] private GameObject optionsPopup;


    private void OnEnable()
    {
        // Subscribe to GameManager events
        GameManager.OnBalanceChanged += UpdateBalanceUI;
        GameManager.OnBetChanged += UpdateBetUI;
        GameManager.OnStateChanged += HandleGameStateChange;
        GameManager.OnWinProcessed += ShowWinMessage;
        GameManager.OnNotEnoughBalance += ShowLowCashPopup;
        GameManager.OnBankrupt += ShowBankruptPopup;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        GameManager.OnBalanceChanged -= UpdateBalanceUI;
        GameManager.OnBetChanged -= UpdateBetUI;
        GameManager.OnStateChanged -= HandleGameStateChange;
        GameManager.OnWinProcessed -= ShowWinMessage;
        GameManager.OnNotEnoughBalance -= ShowLowCashPopup;
        GameManager.OnBankrupt -= ShowBankruptPopup;
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

    private void ShowLowCashPopup()
    {
        if (lowCashPopup != null)
        {
            lowCashPopup.SetActive(true);
            spinButton.interactable = false;
            increaseBetButton.interactable = false;
            decreaseBetButton.interactable = false;
        }

    }
    public void CloseLowCashPopup()
    {
        if (lowCashPopup != null)
        {
            lowCashPopup.SetActive(false);
            spinButton.interactable = true;
            increaseBetButton.interactable = true;
            decreaseBetButton.interactable = true;

            // Optional: Reset the status text
            if (statusText.text == "PLACE YOUR BET") return;
            statusText.text = "PLACE YOUR BET";
        }
    }

    private void ShowBankruptPopup()
    {
        if (bankruptPopup != null)
        {
            bankruptPopup.SetActive(true);
            statusText.text = "GAME OVER";
        }
    }
    public void RestartGame()
    {
        // For a quick restart, you can simply reload the active scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowOptionsPopUp()
    {
        if (optionsPopup != null)
        {
            optionsPopup.SetActive(true);
        }
    }

    public void CloseOptionsPopUp()
    {
        if (optionsPopup != null)
        {
            optionsPopup.SetActive(false);
        }
    }
    /// <summary>
    /// Handles quitting the game gracefully across different platforms.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quit Game Requested.");

#if UNITY_EDITOR
        // If we are testing in the Unity Editor, this stops Play Mode
        UnityEditor.EditorApplication.isPlaying = false;

#elif UNITY_WEBGL
        // Application.Quit() is ignored in WebGL.
        // The standard practice is to disable the UI or show a farewell message.
        if (statusText != null)
        {
            statusText.text = "THANKS FOR PLAYING! YOU CAN CLOSE THE TAB.";
        }
        
        // Optional: If you want to lock the game, you can disable the spin button here.
        if (spinButton != null) spinButton.interactable = false;

#else
        // If this is a standalone Windows/Mac/Linux or Mobile build, this will close the app
        Application.Quit();
#endif
    }


}