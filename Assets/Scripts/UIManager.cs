using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

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
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button statsButton;
    [SerializeField] private Button instructionsButton;

    [Header("Popups")]
    [SerializeField] private GameObject lowCashPopup;
    [SerializeField] private GameObject bankruptPopup;
    [SerializeField] private GameObject optionsPopup;
    [SerializeField] private GameObject statsPopup;
    [SerializeField] private GameObject instructionsPopup;

    [Header("Polish Elements")]
    [SerializeField] private float balanceRollDuration = 1.0f;
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private RectTransform floatingTextSpawnPoint;

    [Header("Session Stats")]
    [SerializeField] private TextMeshProUGUI totalWageredText;
    [SerializeField] private TextMeshProUGUI totalWonText;
    [SerializeField] private TextMeshProUGUI netProfitText;

    private int currentDisplayedBalance;
    private int trackedBetAmount;
    private Coroutine balanceRollCoroutine;


    private void OnEnable()
    {
        // Subscribe to GameManager events
        GameManager.OnBalanceChanged += UpdateBalanceUI;
        GameManager.OnBetChanged += UpdateBetUI;
        GameManager.OnStateChanged += HandleGameStateChange;
        GameManager.OnWinProcessed += ShowWinMessage;
        GameManager.OnNotEnoughBalance += ShowLowCashPopup;
        GameManager.OnBankrupt += ShowBankruptPopup;
        GameManager.OnSpinStarted += TriggerBetDeductionVisuals;

        //Subscribe to Stats Manager
        StatsManager.OnStatsUpdated += UpdateStatsUI;
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
        GameManager.OnSpinStarted -= TriggerBetDeductionVisuals;

        //Unsubscribe to Stats Manager
        StatsManager.OnStatsUpdated -= UpdateStatsUI;
    }

   

    private void Start()
    {
        statusText.text = "PLACE YOUR BET";
        UpdateBetUI(100);
        currentDisplayedBalance = 0;
    }

    private void UpdateBalanceUI(int newBalance)
    {
        // If we are already rolling, stop the old roll and start a new one to the new target
        if (balanceRollCoroutine != null) StopCoroutine(balanceRollCoroutine);

        balanceRollCoroutine = StartCoroutine(RollBalanceText(currentDisplayedBalance, newBalance));
    }

    private void UpdateBetUI(int newBet)
    {
        trackedBetAmount = newBet;
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
        SpawnFloatingText($"+{payout}", Color.green);
    }

    private void ShowLowCashPopup()
    {
        if (lowCashPopup != null)
        {
            lowCashPopup.SetActive(true);
            SetAllButtonsUninteractive();
        }

    }
    public void CloseLowCashPopup()
    {
        if (lowCashPopup != null)
        {
            lowCashPopup.SetActive(false);
            SetAllButtonsInteractive();

            // Optional: Reset the status text
            if (statusText.text == "PLACE YOUR BET") return;
            statusText.text = "PLACE YOUR BET";
        }
    }

    private void ShowBankruptPopup()
    {
        if (bankruptPopup != null)
        {
            SetAllButtonsUninteractive();

            bankruptPopup.SetActive(true);
            statusText.text = "GAME OVER";
        }
    }
    public void RestartGame()
    {

        SetAllButtonsInteractive();

        // For a quick restart, you can simply reload the active scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowOptionsPopUp()
    {
        if (optionsPopup != null)
        {
            SetAllButtonsUninteractive();

            optionsPopup.SetActive(true);
        }
    }

    public void CloseOptionsPopUp()
    {
        if (optionsPopup != null)
        {
            SetAllButtonsInteractive();
            optionsPopup.SetActive(false);
        }
    }


    //Show and hide stats popup panel(attached to OnClick in inspector)
    public void ShowStatsPopUp()
    {
        if (statsPopup != null)
        {
            SetAllButtonsUninteractive();
            statsPopup.SetActive(true);
        }
    }

    public void CloseStatsPopUp()
    {
        if (statsPopup != null)
        {
            SetAllButtonsInteractive();
            statsPopup.SetActive(false);
        }
    }

    //Show and hide Instructions popup Panel.
    public void ShowInstructionsPopUp()
    {
        if (instructionsPopup != null)
        {
            SetAllButtonsUninteractive();
            instructionsPopup.SetActive(true);
        }
    }

    public void CloseInstructionsPopUp()
    {
        if (instructionsPopup != null)
        {
            SetAllButtonsInteractive();
            instructionsPopup.SetActive(false);
        }
    }

    private void TriggerBetDeductionVisuals(SymbolData[] results)
    {
        SpawnFloatingText($"-{trackedBetAmount}", Color.red);
    }
    private IEnumerator RollBalanceText(int startValue, int targetValue)
    {
        float elapsed = 0f;

        while (elapsed < balanceRollDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / balanceRollDuration;

            // Add a simple Ease-Out mathematical curve so the numbers slow down naturally
            float easeOutT = t * (2f - t);

            currentDisplayedBalance = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, easeOutT));
            balanceText.text = $"BALANCE: {currentDisplayedBalance}";

            yield return null;
        }

        // Hard lock at the end to ensure no math drift
        currentDisplayedBalance = targetValue;
        balanceText.text = $"BALANCE: {currentDisplayedBalance}";
    }

    private void SpawnFloatingText(string message, Color color)
    {
        if (floatingTextPrefab == null || floatingTextSpawnPoint == null) return;

        // Instantiate the text at the spawn point inside the Canvas
        GameObject popUp = Instantiate(floatingTextPrefab, floatingTextSpawnPoint.position, Quaternion.identity, floatingTextSpawnPoint);

        // Setup the text and color
        FloatingText ftScript = popUp.GetComponent<FloatingText>();
        if (ftScript != null)
        {
            ftScript.Setup(message, color);
        }
    }

    private void UpdateStatsUI(int wagered, int won, int net)
    {
        if (totalWageredText != null) totalWageredText.text = $"WAGERED: {wagered}";
        if (totalWonText != null) totalWonText.text = $"WON: {won}";

        if (netProfitText != null)
        {
            netProfitText.text = $"PROFIT: {net}";
            // Optional Polish: Color code the profit
            netProfitText.color = net >= 0 ? Color.green : Color.red;
        }
    }

    private void SetAllButtonsUninteractive()
    {
        spinButton.interactable = false;
        increaseBetButton.interactable = false;
        decreaseBetButton.interactable = false;
        instructionsButton.interactable = false;
        optionsButton.interactable = false;
        statsButton.interactable = false;

    }

    private void SetAllButtonsInteractive()
    {
        spinButton.interactable = true;
        increaseBetButton.interactable = true;
        decreaseBetButton.interactable = true;
        instructionsButton.interactable = true;
        optionsButton.interactable = true;
        statsButton.interactable = true;
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