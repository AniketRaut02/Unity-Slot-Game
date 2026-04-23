using UnityEngine;

public class LeverController : MonoBehaviour
{
    [SerializeField] private Animator leverAnimator;
    private readonly int pullTrigger = Animator.StringToHash("LeverPressed");

    private void Awake()
    {
        // Grab the animator if not assigned in inspector
        if (leverAnimator == null) leverAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // Listen for a VALIDATED spin from the brain of the game
        GameManager.OnSpinStarted += TriggerLeverPull;
    }

    private void OnDisable()
    {
        GameManager.OnSpinStarted -= TriggerLeverPull;
    }

    private void TriggerLeverPull(SymbolData[] results)
    {
        // We don't need the math results here, we just need to know it's time to animate!
        leverAnimator.SetTrigger(pullTrigger);
    }
}