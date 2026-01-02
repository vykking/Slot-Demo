using UnityEngine;

public class MainNumber4 : MonoBehaviour
{
    #region script

    [Header("Core")]
    public SlotMachineController slotMachineController;
    public FreeSpinCheck freeSpinCheck;
    public PayLineCheck payLineCheck;
    public ForcedWinManager forcedWinManager;

    [Header("Data")]
    public PlayerWallet playerWallet;

    [Header("UI")]
    public UI ui;
    public BetSide betSide;

    #endregion script

    #region rtpCheck

    [Header("RTP / Weight System OLD")]
    public float[] imageWeights = { 28f, 24f, 22f, 17f, 9f };
    public float totalBet = 0f;
    public float totalWin = 0f;
    public float currentRTP = 1f;
    public int winStreak = 0;// 连胜连败
    public int loseStreak = 0;
    public float minWeightMultiplier = 0.6f;// 最低与最高倍率（保护用）
    public float maxWeightMultiplier = 1.8f;
    [Header("RTP 控制")]
    [Range(0.5f, 1.5f)]
    private float targetRTP = 0.96f;
    public float[] ImageWeights => imageWeights;
    public float CurrentRTP => currentRTP;

    #endregion rtpCheck

    /// <summary>
    /// 这里会扣钱和侦测freespin 如果是freespin 那就不会扣除
    /// Spin 开始前：
    /// 1. 重置 UI
    /// 2. 处理 FreeSpin 状态
    /// 3. 显示 PayLine
    /// </summary>
    public void OnSpinStart()
    {
        ui.ResetWinMoneyDisplay();
        freeSpinCheck.Check();
        freeSpinCheck.FreeSpinText();
        payLineCheck.ResetPayLineImage();
        ui.playerWinUIMoneyReset();
    }

    public void OnSpinMatrixResult(SymbolType[,] matrix)
    {
        payLineCheck.UpdateMatrix(matrix);
        ui.UIUpdate(UI.eUIToString.Money);
        freeSpinCheck.FreeSpinProtect();
    }

    

    //为了计算rtp的
    public float TargetRTP
    {
        get => targetRTP;
        set => targetRTP = Mathf.Clamp(value, 0.5f, 1.5f);
    }

    #region old logic

    //控制输赢
    private int GetWeightedRandomImage()
    {
        // Test Mode (optional)
        if (Random.Range(0, 100) < 0)
            return (int)SymbolType.FreeSpin;

        float[] adjusted = new float[imageWeights.Length];
        imageWeights.CopyTo(adjusted, 0);

        float rtpBias = (currentRTP < targetRTP) ? 1.1f : 0.9f;
        float streakBias = (loseStreak >= 3) ? 1.2f : (winStreak >= 3 ? 0.8f : 1f);

        float multiplier = Mathf.Clamp(rtpBias * streakBias, minWeightMultiplier, maxWeightMultiplier);

        for (int i = 0; i < adjusted.Length; i++)
            adjusted[i] *= multiplier;

        adjusted[(int)SymbolType.FreeSpin] *= 0.75f;

        float total = 0;
        foreach (float w in adjusted) total += w;

        float rnd = Random.Range(0f, total);
        float cum = 0f;

        for (int i = 0; i < adjusted.Length; i++)
        {
            cum += adjusted[i];
            if (rnd < cum)
            {
                Debug.Log($"[Result] {((SymbolType)i)}, RTP:{currentRTP:F2}, Multiplier:{multiplier:F2}");
                return i;
            }
        }
        Debug.Log($"[RNG] Fallback Symbol = {(SymbolType)(adjusted.Length - 1)}, RTP:{currentRTP:F2}, WinStreak:{winStreak}, LoseStreak:{loseStreak}, Multiplier:{multiplier:F2}");
        return adjusted.Length - 1;
    }

    #endregion old logic
}