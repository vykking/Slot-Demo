using UnityEngine;
using TMPro;

public class ForcedWinManager : MonoBehaviour
{
    public bool forceNextSpin;
    public TextMeshProUGUI text;
    
    public SymbolType[,] forcedMatrix;

    //强制中奖
    public void ForceAll(SymbolType symbol)
    {
        forcedMatrix = new SymbolType[3, 3]
        {
            { symbol, symbol, symbol },
            { symbol, symbol, symbol },
            { symbol, symbol, symbol },
        };

        forceNextSpin = true;
        text.GetComponent<TextMeshProUGUI>().text = $"Force:{symbol}";
    }

    public void ClearForce()
    {
        forceNextSpin = false;
        forcedMatrix = null;
    }

    #region forcewin
    public void ForceCherry()
    {
        ForceAll(SymbolType.Cherry);
    }

    public void ForceLemon()
    {
        ForceAll(SymbolType.Lemon);
    }

    public void ForceWatermelon()
    {
        ForceAll(SymbolType.Watermelon);
    }

    public void ForceSeven()
    {
        ForceAll(SymbolType.Seven);
    }

    public void ForceFreeSpin()
    {
        ForceAll(SymbolType.FreeSpin);
    }
    #endregion

}
