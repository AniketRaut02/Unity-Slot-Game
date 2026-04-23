using System;
using System.Collections;
using UnityEngine;

public class ReelManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Reel[] reels; // Assign Reel_1, Reel_2, Reel_3 here
    [SerializeField] private GameManager gameManager; // Assign the GameManager

    [Header("Spin Timings")]
    [Tooltip("How long all reels spin before the first one stops.")]
    [SerializeField] private float baseSpinTime = 1.5f;
    [Tooltip("The delay between each subsequent reel stopping.")]
    [SerializeField] private float staggerTime = 0.5f;


    //---------EVENTS----------
    public static event Action OnSingleReelStopped;


    private int stoppedReelsCount = 0;

    private void OnEnable()
    {
        // Subscribe to the GameManager's spin command
        GameManager.OnSpinStarted += StartReelSequence;

        // Subscribe to each reel's individual stop announcement
        foreach (Reel reel in reels)
        {
            reel.OnReelStopped += HandleReelStopped;
        }
    }

    private void OnDisable()
    {
        // Always unsubscribe to prevent memory leaks
        GameManager.OnSpinStarted -= StartReelSequence;

        foreach (Reel reel in reels)
        {
            reel.OnReelStopped -= HandleReelStopped;
        }
    }

    private void StartReelSequence(SymbolData[] results)
    {
        stoppedReelsCount = 0;

        // Command all reels to start their infinite visual spin
        foreach (Reel reel in reels)
        {
            reel.StartSpinning();
        }

        // Orchestrate the staggered stopping sequence
        StartCoroutine(StopReelsRoutine(results));
    }

    private IEnumerator StopReelsRoutine(SymbolData[] results)
    {
        // Wait for the initial theatrical spin duration
        yield return new WaitForSeconds(baseSpinTime);

        for (int i = 0; i < reels.Length; i++)
        {
            // Send the precise mathematical target to the specific reel
            reels[i].StopReel(results[i]);

            // Wait a moment before stopping the next one to create tension
            if (i < reels.Length - 1)
            {
                yield return new WaitForSeconds(staggerTime);
            }
        }
    }

    private void HandleReelStopped()
    {
        stoppedReelsCount++;
        OnSingleReelStopped?.Invoke();

        // If all reels have snapped perfectly into their final distance-based positions
        if (stoppedReelsCount >= reels.Length)
        {
            // Hand control back to the FSM for evaluation
            gameManager.ReelsFinishedStopping();
        }
    }
}