using UnityEngine;

[CreateAssetMenu(menuName = "Symbol Data")]
public class SymbolData : ScriptableObject
{
    public string symbolName;
    public Sprite symbolSprite;
    public int payoutMultiplier;
    public int spawnWeight;
}
