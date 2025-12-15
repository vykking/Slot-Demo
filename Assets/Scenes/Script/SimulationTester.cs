using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimulationTester : MonoBehaviour
{
    public SlotMachineController slot;  
    public PayLineCheck paylineChecker; 
    public BetSide betSide;             

    public TMP_InputField testCountInput;
    public TMP_Text resultText;         

    [Header("Simulation Settings")]
    public int testCount = 1000;

    //--- 内部统计 ---
    private int totalBet = 0;
    private int totalWin = 0;

    private int[] symbolCount = new int[5];    
    private int[] paylineHits = new int[8];    

    public void RunSimulation()
    {

        if (testCountInput != null && !string.IsNullOrEmpty(testCountInput.text))
        {
            if (int.TryParse(testCountInput.text, out int value) && value > 0)
            {
                testCount = value;
            }
            else
            {
                Debug.LogWarning("[SimulationTester] 输入的测试次数无效，使用原本 testCount");
            }
        }

        ResetStats();

        for (int i = 0; i < testCount; i++)
            SimulateOneSpin();

        float RTP = totalWin / (float)totalBet * 100f;

        ShowResult(RTP);
    }
    
    //每一轮之后reset
    private void ResetStats()
    {
        totalBet = 0;
        totalWin = 0;

        for (int i = 0; i < 5; i++)
            symbolCount[i] = 0;

        for (int i = 0; i < 8; i++)
            paylineHits[i] = 0;
    }


    /// <summary>
    /// 通过权重，随机产生指定次数的3x3 不需要旋转，然后在传给侦测输赢
    /// </summary>
    private void SimulateOneSpin()
    {
        int bet = betSide.BetMoneyGet;
        totalBet += bet;

        //随机产生 3x3 
        SymbolType[,] matrix = new SymbolType[3, 3];

        for (int col = 0; col < 3; col++)
        {
            var strip = slot.reels[col].reelSymbols;     //这里的reelsymbols虽然是虚造的，但是场上的symbols 也是根据这个比例做的
            int stripLen = strip.Length;
            for (int row = 0; row < 3; row++)
            {
                SymbolType s = strip[Random.Range(0, stripLen)]; //和旧的逻辑一样
                matrix[row, col] = s;
                symbolCount[(int)s]++;
            }
        }
        
        int multiplier = PayOnlyReturnWin(matrix);
        int win = multiplier * bet;
        totalWin += win;
    }


    //计算输赢
    public int PayOnlyReturnWin(SymbolType[,] matrix)
    {
        int win = 0;

        for (int i = 0; i < paylineChecker.payLines.Length; i++)
        {
            var line = paylineChecker.payLines[i];
            SymbolType a = matrix[line[0].Item1, line[0].Item2];
            SymbolType b = matrix[line[1].Item1, line[1].Item2];
            SymbolType c = matrix[line[2].Item1, line[2].Item2];

            if (a == b && b == c)
            {
                paylineHits[i]++;
                win += playerPay(a); 
            }
        }
        return win;
    }


    //这里比较笨，懒惰从playerWallet读取
    private int playerPay(SymbolType s)
    {
        switch (s)
        {
            case SymbolType.Cherry: return 1;
            case SymbolType.Lemon: return 2;
            case SymbolType.Watermelon: return 4;
            case SymbolType.Seven: return 6;
            case SymbolType.FreeSpin: return 0;
        }
        return 0;
    }


    //这边有用GPT 教我排版==
    private void ShowResult(float RTP)
    {
        string result = "";
        result += $"Test Spins: {testCount}\n";
        result += $"Total Bet: {totalBet}\n";
        result += $"Total Win: {totalWin}\n";
        result += $"RTP: {RTP:F2}%\n\n";

        result += "Symbol Frequency:\n";
        for (int i = 0; i < 5; i++)
            result += $"{(SymbolType)i}: {symbolCount[i]}\n";

        result += "\nPayline Hits:\n";
        for (int i = 0; i < 8; i++)
            result += $"Line {i + 1}: {paylineHits[i]}\n";

        resultText.text = result;
    }
}
