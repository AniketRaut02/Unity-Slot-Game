using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [Tooltip("For one-shot sounds like clicks, thuds, and wins.")]
    [SerializeField] private AudioSource sfxSource;
    [Tooltip("For looping sounds like the mechanical spinning.")]
    [SerializeField] private AudioSource loopSource;
    [Tooltip("For BG music.")]
    [SerializeField] private AudioSource bgSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip spinLoopClip;
    [SerializeField] private AudioClip reelStopClip;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip buttonClickClip; 
    [SerializeField] private AudioClip leverPullClip;
    [SerializeField] private AudioClip lowCashClip;
    [SerializeField] private AudioClip bankruptClip;
    private void OnEnable()
    {
        // Subscribe to our existing architecture
        GameManager.OnSpinStarted += PlaySpinSound;
        GameManager.OnWinProcessed += PlayWinSound;
        GameManager.OnStateChanged += HandleStateChange;
        ReelManager.OnSingleReelStopped += PlayReelStopSound;
        GameManager.OnNotEnoughBalance += PlayLowCashSound;
        GameManager.OnBankrupt += PlayBankruptSound;
    }

    private void OnDisable()
    {
        GameManager.OnSpinStarted -= PlaySpinSound;
        GameManager.OnWinProcessed -= PlayWinSound;
        GameManager.OnStateChanged -= HandleStateChange;
        ReelManager.OnSingleReelStopped -= PlayReelStopSound;
        GameManager.OnNotEnoughBalance -= PlayLowCashSound;
        GameManager.OnBankrupt -= PlayBankruptSound;
    }

    private void PlaySpinSound(SymbolData[] results)
    {
        if (leverPullClip != null)
        {
            sfxSource.PlayOneShot(leverPullClip);
        }
        if (spinLoopClip != null)
        {
            loopSource.clip = spinLoopClip;
            loopSource.loop = true;
            loopSource.Play();
        }
    }

    private void PlayReelStopSound()
    {
        if (reelStopClip != null)
        {
            sfxSource.PlayOneShot(reelStopClip);
        }
    }

    private void HandleStateChange(GameManager.GameState state)
    {
        // Stop the spinning loop sound when we transition out of the Spinning state
        if (state == GameManager.GameState.Evaluating || state == GameManager.GameState.Idle)
        {
            loopSource.Stop();
        }
    }

    private void PlayWinSound(int payout)
    {
        if (winClip != null)
        {
            sfxSource.PlayOneShot(winClip);
        }
    }

    // A public method so your UI buttons can trigger it via the Unity Inspector
    public void PlayClickSound()
    {
        if (buttonClickClip != null)
        {
            sfxSource.PlayOneShot(buttonClickClip);
        }
    }
    private void PlayLowCashSound()
    {
        if (lowCashClip != null)
        {
            sfxSource.PlayOneShot(lowCashClip);
        }
    }

    private void PlayBankruptSound()
    {
        if (bankruptClip != null)
        {
            sfxSource.PlayOneShot(bankruptClip);
        }
        if (bgSource != null)
        {
            bgSource.Stop();
        }
    }
}