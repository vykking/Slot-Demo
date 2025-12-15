using System.Collections.Generic;
using UnityEngine;

public class RTP : MonoBehaviour
{
    public SlotMachineController slotMachineController;
    public PayLineCheck payLineCheck;

    void Start()
    {
        PrintReelStatistics();
        CalculateRTP_AllPaylines();
    }

    /// <summary>
    /// Debug 使用，爲了尋找RTP 做一個儲存質
    /// 这里必须老实和面试官说我目前的等级是没办法写出来的，我是参考gpt 的写法才有办法写出一个计算RTP的
    /// </summary>
    private void PrintReelStatistics()
    {
        Debug.Log("========== reel的状态 ==========");

        for (int r = 0; r < slotMachineController.reels.Length; r++)
        {
            ReelController reel = slotMachineController.reels[r];
            Transform t = reel.transform;

            Dictionary<SymbolType, int> count = new Dictionary<SymbolType, int>();

            foreach (SymbolType st in System.Enum.GetValues(typeof(SymbolType)))
                count[st] = 0;

            for (int i = 0; i < t.childCount; i++)
            {
                SymbolTag tag = t.GetChild(i).GetComponent<SymbolTag>();
                if (tag != null)
                {
                    count[tag.type]++;
                }
            }

            Debug.Log($"--- Reel {r} ---");
            foreach (var kv in count)
            {
                Debug.Log($"{kv.Key}: {kv.Value}");
            }
        }

        Debug.Log("===========================================");
    }

    [ContextMenu("Print Simple RTP")]
    private void PrintSimpleRTP()
    {
        Debug.Log("====== Simple RTP Calculator ======");

        // 你的 Reel（每轴）
        ReelController[] reels = slotMachineController.reels;

        // paytable (你可以自己调倍率)
        Dictionary<SymbolType, float> paytable = new Dictionary<SymbolType, float>()
    {
        { SymbolType.Cherry, 1f },
        { SymbolType.Lemon, 2f },
        { SymbolType.Watermelon, 4f },
        { SymbolType.Seven, 8f },
        { SymbolType.FreeSpin, 0f } // FreeSpin 本身不给钱
    };

        // 统计每个符号在每条 Reel 的数目
        Dictionary<SymbolType, int[]> counts = new Dictionary<SymbolType, int[]>();
        foreach (SymbolType s in System.Enum.GetValues(typeof(SymbolType)))
            counts[s] = new int[reels.Length];

        int[] reelSizes = new int[reels.Length];

        for (int r = 0; r < reels.Length; r++)
        {
            Transform reelTransform = reels[r].transform;
            int childCount = reelTransform.childCount;
            reelSizes[r] = childCount;

            for (int i = 0; i < childCount; i++)
            {
                SymbolType type = reelTransform.GetChild(i).GetComponent<SymbolTag>().type;
                counts[type][r]++;
            }
        }

        // 现在计算 RTP（单线）
        float RTP = 0f;

        foreach (SymbolType s in System.Enum.GetValues(typeof(SymbolType)))
        {
            float p = 1f;

            // 三个轴同时为 s
            for (int r = 0; r < reels.Length; r++)
            {
                p *= (float)counts[s][r] / reelSizes[r];
            }

            float EV = p * paytable[s];
            RTP += EV;

            Debug.Log($"{s} | P(3连) = {p * 100f:F4}% | Pay = {paytable[s]}x | EV = {EV:F5}");
        }

        Debug.Log($"====== Total RTP (Single Payline) = {RTP * 100f:F2}% ======");
    }

    // =====================================================
    // ================  RTP Calculator (Simple) ============
    // =====================================================

    // paytable（你可以之后调整赔率）
    private readonly int[] payValues = { 1, 2, 4, 8, 0 };
    // Cherry=1x, Lemon=2x, Watermelon=4x, Seven=8x, FreeSpin=0x

    private void CalculateRTP_AllPaylines()
    {
        Debug.Log("===== RTP Calculator (8 Paylines) =====");

        // 1. 基本防呆
        if (slotMachineController == null || slotMachineController.reels == null || slotMachineController.reels.Length == 0)
        {
            Debug.LogError("[RTP] SlotMachineController 或 reels 没。");
            return;
        }

        int reelCount = slotMachineController.reels.Length;

        // 确保每个 reel 的 reelSymbols 都有填、而且长度一致
        int symbolCount = -1;
        for (int r = 0; r < reelCount; r++)
        {
            var strip = slotMachineController.reels[r].reelSymbols;

            if (strip == null || strip.Length == 0)
            {
                Debug.LogError($"[RTP] Reel {r} 的 reelSymbols 是空的（Length = 0）。");
                return;
            }

            if (symbolCount < 0)
                symbolCount = strip.Length; // 第一个 reel 的长度
            else if (strip.Length != symbolCount)
            {
                Debug.LogError($"[RTP] Reel {r} 的符号数量({strip.Length}) 和 Reel0({symbolCount}) 不一致，暂时不计算 RTP。");
                return;
            }
        }

        // 到这里 symbolCount 一定 > 0
        float invSymbolCount = 1f / symbolCount;

        // 2. 统计每个 Reel 各 Symbol 的数量
        int[][] counts = new int[reelCount][];
        for (int r = 0; r < reelCount; r++)
        {
            counts[r] = new int[5]; // 5 个 symbol
            foreach (var sym in slotMachineController.reels[r].reelSymbols)
            {
                counts[r][(int)sym]++;
            }
        }

        // 3. 逐条 payline 计算 RTP
        float[] lineRTP = new float[8]; // 8 条 payline
        float totalRTP = 0f;

        for (int i = 0; i < 8; i++)
        {
            float prCherry = 1f;
            float prLemon = 1f;
            float prWatermelon = 1f;
            float prSeven = 1f;
            float prFreeSpin = 1f;

            foreach (var pos in payLineCheck.payLines[i])
            {
                int col = pos.Item2; // 只用到列（0,1,2）

                prCherry *= counts[col][0] * invSymbolCount;
                prLemon *= counts[col][1] * invSymbolCount;
                prWatermelon *= counts[col][2] * invSymbolCount;
                prSeven *= counts[col][3] * invSymbolCount;
                prFreeSpin *= counts[col][4] * invSymbolCount;
            }

            float rtp =
                prCherry * payValues[0] +
                prLemon * payValues[1] +
                prWatermelon * payValues[2] +
                prSeven * payValues[3] +
                prFreeSpin * payValues[4];

            lineRTP[i] = rtp;
            totalRTP += rtp;

            Debug.Log($"Line {i + 1} RTP：{rtp * 100f:F2}%");
        }

        Debug.Log($"★ TOTAL RTP (All 8 Paylines)：{totalRTP * 100f:F2}%");
        Debug.Log("========================================");
    }
    
}
