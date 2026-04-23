using UnityEngine;
using UnityEngine.UI;

public class SlotNode : MonoBehaviour
{
    [SerializeField] private Image symbolImage;
    public SymbolData CurrentSymbol { get; private set; }
    public RectTransform RectTrans { get; private set; }

    private void Awake()
    {
        RectTrans = GetComponent<RectTransform>();
    }

    public void SetSymbol(SymbolData newSymbol)
    {
        CurrentSymbol = newSymbol;
        symbolImage.sprite = newSymbol.symbolSprite;
    }
}