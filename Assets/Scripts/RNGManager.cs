using UnityEngine;

public class RNGManager : MonoBehaviour
{
    [Header("Symbol Configuration")]
    [Tooltip("Drag and drop your 4 SymbolData ScriptableObjects here.")]
    [SerializeField] private SymbolData[] availableSymbols;

    private int totalWeight;

    private void Awake()
    {
        InitializeRNG();
    }

    private void Start()
    {
        ///Use this function call to check Fairness of RNG Algorithm.
        SimulateThousandSpins();
    }

    /// <summary>
    /// Caches the total weight of all symbols on startup to save performance during spins.
    /// </summary>
    private void InitializeRNG()
    {
        totalWeight = 0;

        if (availableSymbols == null || availableSymbols.Length == 0)
        {
            Debug.LogError("RNGManager: No SymbolData assigned in the inspector!");
            return;
        }

        foreach (SymbolData symbol in availableSymbols)
        {
            // Assuming your ScriptableObject has an integer field called 'spawnWeight'
            totalWeight += symbol.spawnWeight;
        }
    }

    /// <summary>
    /// Generates the final, mathematically fair outcome for a standard 3-reel spin.
    /// </summary>
    /// <returns>An array containing exactly 3 SymbolData objects.</returns>
    public SymbolData[] GenerateSpinResults()
    {
        SymbolData[] spinResults = new SymbolData[3];

        for (int i = 0; i < 3; i++)
        {
            spinResults[i] = GetWeightedRandomSymbol();
        }

        return spinResults;
    }

    /// <summary>
    /// The core algorithm. Picks a random number and evaluates it against the weighted table.
    /// </summary>
    private SymbolData GetWeightedRandomSymbol()
    {
        // Roll a random number between 0 (inclusive) and totalWeight (exclusive)
        int randomRoll = Random.Range(0, totalWeight);
        int currentWeightSum = 0;

        foreach (SymbolData symbol in availableSymbols)
        {
            currentWeightSum += symbol.spawnWeight;

            // If the roll falls within this symbol's weight bucket, select it
            if (randomRoll < currentWeightSum)
            {
                return symbol;
            }
        }

        // Fallback safety (should mathematically never be reached)
        Debug.LogWarning("RNGManager: Weighted selection bypassed, returning default symbol.");
        return availableSymbols[0];
    }

    /// <summary>
    /// Testing function to test the fairness of the RNG.
    /// </summary>
    private void SimulateThousandSpins()
    {
        int berryCount = 0;
        int bellCount = 0;
        int barCount = 0;
        int sevenCount = 0;

        int spins = 10000;

        for (int i = 0; i < spins; i++)
        {
            SymbolData result = GetWeightedRandomSymbol();

            // Adjust these string checks to match exactly what you named your SOs
            if (result.symbolName == "Berry") berryCount++;
            else if (result.symbolName == "Bell") bellCount++;
            else if (result.symbolName == "Bar") barCount++;
            else if (result.symbolName == "Seven") sevenCount++;
        }

        Debug.Log($"--- RNG SIMULATION ({spins} Spins) ---");
        Debug.Log($"Berries: {berryCount} ({(float)berryCount / spins * 100}%)");
        Debug.Log($"Bells: {bellCount} ({(float)bellCount / spins * 100}%)");
        Debug.Log($"Bars: {barCount} ({(float)barCount / spins * 100}%)");
        Debug.Log($"Sevens: {sevenCount} ({(float)sevenCount / spins * 100}%)");
    }
}