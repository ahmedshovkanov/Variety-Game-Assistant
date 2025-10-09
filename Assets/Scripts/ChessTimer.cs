using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class ChessTimer : MonoBehaviour
{
    [System.Serializable]
    public class PlayerTimer
    {
       

        public TextMeshProUGUI timeText;
        public Button playerButton;
        //public GameObject activeIndicator;
        public float timeRemaining;
        public bool isActive;
        public bool hasTime;

        public void UpdateDisplay()
        {
            if (timeText != null)
            {
                timeText.text = FormatTime(timeRemaining);
                timeText.color = hasTime ? Color.white : Color.red;
            }

            //if (activeIndicator != null)
            //    activeIndicator.SetActive(isActive);
        }

        private string FormatTime(float timeInSeconds)
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public int PresetTime, PresetAddedTimePerTurn;

    [Header("Player Timers")]
    public PlayerTimer player1;
    public PlayerTimer player2;

    [Header("Game Settings")]
    //public float initialTimeMinutes = 5f;
    //public float incrementSeconds = 0f; // Time added after each move

    [Header("UI Elements")]
    public Button startButton;
    //public Button pauseButton;
    //public Button resetButton;
    //public TextMeshProUGUI gameStatusText;

    [Header("Sounds")]
    public AudioClip timeWarningSound;
    public AudioClip timeUpSound;
    public AudioClip switchPlayerSound;

    private bool gameRunning = false;
    private AudioSource audioSource;
    private float timeWarningThreshold = 10f; // Show warning when less than 10 seconds

    void Start()
    {
        InitializeTimers();
        SetupButtonListeners();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (gameRunning)
        {
            UpdateActiveTimer();
            CheckTimeWarnings();
        }
    }

    private void InitializeTimers()
    {
        float initialTimeSeconds = PresetTime;

        Debug.Log(initialTimeSeconds);

        player1.timeRemaining = initialTimeSeconds;
        player2.timeRemaining = initialTimeSeconds;
        player1.hasTime = true;
        player2.hasTime = true;
        player1.isActive = false;
        player2.isActive = false;

        player1.UpdateDisplay();
        player2.UpdateDisplay();

        UpdateGameStatus("Ready to Start");
    }

    private void SetupButtonListeners()
    {
        // Player buttons
        player1.playerButton.onClick.AddListener(() => SwitchTurn(false));
        player2.playerButton.onClick.AddListener(() => SwitchTurn(true));

        // Control buttons
        startButton.onClick.AddListener(StartGame);
        //pauseButton.onClick.AddListener(PauseGame);
        //resetButton.onClick.AddListener(ResetGame);

        // Initially disable player buttons until game starts
        player1.playerButton.interactable = false;
        player2.playerButton.interactable = false;
    }

    public void StartGame()
    {
        if (!gameRunning)
        {
            gameRunning = true;
            player1.playerButton.interactable = true;
            player2.playerButton.interactable = true;

            // Player 1 starts
            player1.isActive = true;
            player1.UpdateDisplay();
            player2.UpdateDisplay();

            UpdateGameStatus("Game Running - Player 1's Turn");
            startButton.interactable = false;
        }
    }

    public void PauseGame()
    {
        gameRunning = !gameRunning;

        if (gameRunning)
        {
            UpdateGameStatus("Game Resumed");
            startButton.interactable = false;
        }
        else
        {
            UpdateGameStatus("Game Paused");
            startButton.interactable = true;
        }
    }

    public void ResetGame()
    {
        gameRunning = false;
        InitializeTimers();

        player1.playerButton.interactable = false;
        player2.playerButton.interactable = false;
        startButton.interactable = true;
    }

    private void UpdateActiveTimer()
    {
        if (player1.isActive && player1.hasTime)
        {
            player1.timeRemaining -= Time.deltaTime;
            if (player1.timeRemaining <= 0)
            {
                player1.timeRemaining = 0;
                player1.hasTime = false;
                PlayerTimeOut(1);
            }
            player1.UpdateDisplay();
        }
        else if (player2.isActive && player2.hasTime)
        {
            player2.timeRemaining -= Time.deltaTime;
            if (player2.timeRemaining <= 0)
            {
                player2.timeRemaining = 0;
                player2.hasTime = false;
                PlayerTimeOut(2);
            }
            player2.UpdateDisplay();
        }
    }

    public void HomeBtn()
    {
        SceneManager.LoadScene(0);
    }

    private void SwitchTurn(bool switchedFromPlayer1)
    {
        if (!gameRunning) return;

        // Play switch sound
        if (switchPlayerSound != null)
            audioSource.PlayOneShot(switchPlayerSound);

        // Apply time increment to the player who just moved
        if (switchedFromPlayer1 && player1.hasTime)
        {
            player2.timeRemaining += PresetAddedTimePerTurn;
        }
        else if (!switchedFromPlayer1 && player2.hasTime)
        {
            player1.timeRemaining += PresetAddedTimePerTurn;
        }

        // Switch active player
        player1.isActive = !player1.isActive;
        player2.isActive = !player2.isActive;

        player1.UpdateDisplay();
        player2.UpdateDisplay();

        UpdateGameStatus($"Game Running - {(player1.isActive ? "Player 1's" : "Player 2's")} Turn");
    }

    private void PlayerTimeOut(int playerNumber)
    {
        gameRunning = false;
        player1.playerButton.interactable = false;
        player2.playerButton.interactable = false;

        // Play time up sound
        if (timeUpSound != null)
            audioSource.PlayOneShot(timeUpSound);

        UpdateGameStatus($"Player {playerNumber} ran out of time! {(playerNumber == 1 ? "Player 2" : "Player 1")} wins!");

        // Visual feedback for time out
        StartCoroutine(TimeOutFlash(playerNumber == 1 ? player1.timeText : player2.timeText));
    }

    private void CheckTimeWarnings()
    {
        // Check player 1
        if (player1.isActive && player1.hasTime && player1.timeRemaining <= timeWarningThreshold && player1.timeRemaining > 0)
        {
            if (!IsInvoking("PlayWarningSound"))
            {
                InvokeRepeating("PlayWarningSound", 0f, 1f);
            }
        }

        // Check player 2
        if (player2.isActive && player2.hasTime && player2.timeRemaining <= timeWarningThreshold && player2.timeRemaining > 0)
        {
            if (!IsInvoking("PlayWarningSound"))
            {
                InvokeRepeating("PlayWarningSound", 0f, 1f);
            }
        }

        // Stop warning if both players have more than warning threshold
        if ((player1.timeRemaining > timeWarningThreshold || !player1.isActive) &&
            (player2.timeRemaining > timeWarningThreshold || !player2.isActive))
        {
            CancelInvoke("PlayWarningSound");
        }
    }

    private void PlayWarningSound()
    {
        if (timeWarningSound != null)
            audioSource.PlayOneShot(timeWarningSound);
    }

    private void UpdateGameStatus(string status)
    {
        //if (gameStatusText != null)
         //   gameStatusText.text = status;
    }

    private IEnumerator TimeOutFlash(TextMeshProUGUI textToFlash)
    {
        Color originalColor = textToFlash.color;
        for (int i = 0; i < 6; i++)
        {
            textToFlash.color = (i % 2 == 0) ? Color.red : originalColor;
            yield return new WaitForSeconds(0.3f);
        }
        textToFlash.color = originalColor;
    }

    // Public methods for external control
    public void SetTime(int playerNumber, float minutes, float seconds)
    {
        float totalSeconds = minutes * 60f + seconds;
        if (playerNumber == 1)
            player1.timeRemaining = totalSeconds;
        else if (playerNumber == 2)
            player2.timeRemaining = totalSeconds;
    }

    public void AddTime(int playerNumber, float seconds)
    {
        if (playerNumber == 1)
            player1.timeRemaining += seconds;
        else if (playerNumber == 2)
            player2.timeRemaining += seconds;
    }

    public float GetRemainingTime(int playerNumber)
    {
        return playerNumber == 1 ? player1.timeRemaining : player2.timeRemaining;
    }

    public bool IsGameRunning()
    {
        return gameRunning;
    }
}