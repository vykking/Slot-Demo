using UnityEngine;
using System.Collections;
using static UnityEngine.Rendering.DebugUI.Table;

public class PayLineCheck : MonoBehaviour
{
    private SymbolType[,] randomNumber;

    [Header("Systems")]
    public PlayerWallet playerWallet;


    public GameObject[] paylineImages;  //线条

    public (int, int)[][] payLines = new (int, int)[][]
    {
        new (int, int)[] { (0,0), (0,1), (0,2) }, // 横排 1
        new (int, int)[] { (1,0), (1,1), (1,2) }, // 横排 2
        new (int, int)[] { (2,0), (2,1), (2,2) }, // 横排 3
        new (int, int)[] { (0,0), (1,0), (2,0) }, // 直排 1
        new (int, int)[] { (0,1), (1,1), (2,1) }, // 直排 2
        new (int, int)[] { (0,2), (1,2), (2,2) }, // 直排 3
        new (int, int)[] { (0,0), (1,1), (2,2) }, // 斜线 
        new (int, int)[] { (0,2), (1,1), (2,0) }  // 斜线 
    };

    /// <summary>
    /// 检测是否有中奖并且用paytable 兑换奖励
    /// </summary>
    public void paylineCheck()
    {
        // 先全部关掉
        foreach (var line in paylineImages)
            line.SetActive(false);

        for (int i = 0; i < payLines.Length; i++)
        {
            var line = payLines[i];

            SymbolType a = randomNumber[line[0].Item1, line[0].Item2];
            SymbolType b = randomNumber[line[1].Item1, line[1].Item2];
            SymbolType c = randomNumber[line[2].Item1, line[2].Item2];

            if (a == b && b == c)
            {
                playerWallet.PayTable(a);
                // 亮起中奖 PayLine
                paylineImages[i].SetActive(true);
            }
        }
    }

    public void UpdateMatrix(SymbolType[,] matrix)
    {
        randomNumber = matrix;  
        paylineCheck();        // 用原本写的中奖逻辑
    }


    public void ResetPayLineImage()
    {
        foreach (var line in paylineImages)
            line.SetActive(false);
    }

    

}
