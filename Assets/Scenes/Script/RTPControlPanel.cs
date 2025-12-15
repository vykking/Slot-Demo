using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RTPPanel : MonoBehaviour
{
    public MainNumber4 main;      
    public PlayerWallet wallet;   
    public SlotMachineController slotMachineController;

    [Header("RTP UI")]
    public TMP_Text targetRtpText;
    public TMP_Text currentRtpText;
    public Slider targetRtpSlider;

    [Header("Weight UI")]
    public Slider[] weightSliders;
    public TMP_Text[] weightLabels;

    void Start()
    {
        // 用 main 里的目标 RTP 初始化 slider
        targetRtpSlider.value = main.TargetRTP;
        UpdateUI();
    }

    // Slider 拖动 Target RTP 时调用
    public void OnTargetRtpChanged(float value)
    {
        main.TargetRTP = value;
        targetRtpText.text = (value * 100f).ToString("F2") + "%";
    }

    // 单个 weight slider 改变时调用，index 由 UI 传进来
    public void OnWeightChanged(int index)
    {
        float value = weightSliders[index].value;
        weightLabels[index].text = value.ToString("F1");

        // 写回 MainNumber4 中的权重数组
        main.ImageWeights[index] = value;
        ApplyWeightsToAllReels();
    }

    // 刷新整个面板
    /// <summary>
    /// 这里是为了让RTP 更新到 text 上面
    /// </summary>
    public void UpdateUI()
    {
        // 目标 RTP
        targetRtpText.text = (main.TargetRTP * 100f).ToString("F2") + "%";

        // 当前 RTP
        currentRtpText.text = (wallet.CurrentRTP * 100f).ToString("F2") + "%";

        // 各符号权重
        for (int i = 0; i < weightSliders.Length; i++)
        {
            weightSliders[i].value = main.ImageWeights[i];
            weightLabels[i].text = main.ImageWeights[i].ToString("F1");
        }
        ApplyWeightsToAllReels();
    }
    private void ApplyWeightsToAllReels()
    {
        foreach (var reel in slotMachineController.reels)
        {
            // 更新 reel 的概率带
            reel.ApplyWeights(main.ImageWeights);

            // 重新载入 strip
            reel.GetReelChild();
        }
    }

}
