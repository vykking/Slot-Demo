using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class SlotMachineController : MonoBehaviour
{

    public ReelController[] reels;
    public MainNumber4 main;

    public TMP_Text playButtonText;


    public float spinDuration = 1.2f;   // 旋转多久后开始停
    public float stopGap = 0.3f;        // 每个卷轴之间的停顿
    public UnityEngine.UI.Button playButton; // 可选：转动时禁用按钮

    public bool isStopping = false;


    #region 加速按钮
    /// <summary>提速按钮
    /// 让玩家自己切换
    /// </summary>
    public SpinSpeedMode currentSpeedMode = SpinSpeedMode.Normal;

    public void ApplySpeedMode()
    {
        foreach (var reel in reels)
        {
            switch (currentSpeedMode)
            {
                case SpinSpeedMode.Normal:
                    reel.SetSpeedProfile(
                        spinSpeed: 15f,
                        slow: 6f,
                        deceleration: 5f
                    );
                    break;

                case SpinSpeedMode.Fast:
                    reel.SetSpeedProfile(
                        spinSpeed: 30f,
                        slow: 30f,
                        deceleration: 30f
                    );
                    break;
            }
        }
    }

    //防止在旋转过程可以加速
    public bool CanChangeSpeed()
    {
        if (isStopping) return false;

        foreach (var reel in reels)
        {
            if (reel.isSpinning)
                return false;
        }

        return true;
    }
    #endregion


    /// <summary>
    /// 进入旋转,以及控制不让出现额外旋转
    /// Play 按钮入口：
    /// 1. 状态检查
    /// 2. 套用速度模式
    /// 3. 播放音效
    /// 4. 启动 Spin 流程
    /// </summary>
    public void OnPlayButton()
    {
        // 正在旋转或正在停轮就忽略点击
        if (isStopping || isSpinningAll()) return;
        ApplySpeedMode();
        AudioManager.Instance.SpinSound(true);
        StartCoroutine(SpinAndAutoStop());
    }

    private bool isSpinningAll()
    {
        foreach (var r in reels)
        {
            if (r.isSpinning) return true;
        }
        return false;
    }

    /// <summary>
    /// 启动按钮，扣钱，旋转，停下，回传输赢数据
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpinAndAutoStop()
    {
        // UI：开始一轮
        playButtonText.text = "SPIN";
        if (playButton) playButton.interactable = false;

        isStopping = false;

        // 通知 Main：这一轮开始（扣钱或扣 FreeSpin；隐藏 winUI）
        main.OnSpinStart();

        // 开始转
        foreach (var r in reels) r.StartSpin();

        // 旋转一段时间
        yield return new WaitForSeconds(spinDuration);
        // 开始依序停
        isStopping = true;
        yield return StopReelsSequentially();   // ← 等待全部停好（见下一步）

        // 允许下一轮
        isStopping = false;
        if (playButton) playButton.interactable = true;
        playButtonText.text = "SPIN";
    }
    

    /// <summary>
    /// 这里是为了侦测中奖结果并且侦测输赢（用MainNumber4）
    /// </summary>
    /// <returns></returns>
    //TODO：感觉可以把debug 做另外一个function 看起来会更加整齐
    private IEnumerator StopReelsSequentially()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].StopSpin();
            // IsSnapped 是为了侦测是否所有条件都完成了停止条件
            yield return new WaitUntil(() => reels[i].IsSnapped);
            AudioManager.Instance.Play(AudioManager.Instance.reelStop);
        }

        // 全部停好后组装 3x3 矩阵
        SymbolType[,] matrix = new SymbolType[3, 3];

        for (int reel = 0; reel < reels.Length; reel++)
        {
            SymbolType[] rows = reels[reel].GetRowResult(); // 上/中/下
            matrix[0, reel] = rows[0];
            matrix[1, reel] = rows[1];
            matrix[2, reel] = rows[2];
        }

        if (main.forcedWinManager.forceNextSpin &&
        main.forcedWinManager.forcedMatrix != null)
        {
            Debug.Log("强制中奖模式：画面也对齐指定矩阵");

            SymbolType[,] forced = main.forcedWinManager.forcedMatrix;

            for (int col = 0; col < 3; col++)
            {
                reels[col].ForceAlign(
                    forced[0, col],   // top
                    forced[1, col],   // mid
                    forced[2, col]    // bottom
                );
            }

            // 替换中奖结果
            matrix = forced;

            // 清空强制中奖
            main.forcedWinManager.ClearForce();
        }
        // 继续正常流程 → win check + UI 更新
        AudioManager.Instance.SpinSound(false);
        main.OnSpinMatrixResult(matrix);

#if UNITY_EDITOR
        //这是为了debug 而已，看每条线有什么
        for (int r = 0; r < 3; r++)
        {
            string rowStr = "";
            for (int c = 0; c < 3; c++)
                rowStr += matrix[r, c] + " ";
            Debug.Log($"Row {r}: {rowStr}");
        }
#endif

        // 稍微暂停
        yield return new WaitForSeconds(stopGap);
    }


}
