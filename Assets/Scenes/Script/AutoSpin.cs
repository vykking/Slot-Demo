using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoSpin : MonoBehaviour
{
    [Header("Systems")]
    public SlotMachineController slotMachineController;
    public PlayerWallet playerWallet;

    public bool autoSpin;

    void Start()
    {
        autoSpin = false;
    }


    //启动按钮
    public void ToggleAutoSpin()
    {
        autoSpin = !autoSpin;

        if (autoSpin)
            StartCoroutine(AutoSpinCoroutine());
        else
            StopCoroutine(AutoSpinCoroutine());
    }


    //autospin
    private IEnumerator AutoSpinCoroutine()
    {
        while (autoSpin)
        {
            slotMachineController.OnPlayButton();
        
            yield return new WaitUntil(() => !slotMachineController.isStopping && !IsAnyReelSpinning());

            // 停
            yield return new WaitForSeconds(0.3f);

            // 如果钱用完停止
            if (playerWallet.WalletGet <= 0) autoSpin = false;
            
        }
    }

    //为了让autoSpin 找得到所有都停下来了
    private bool IsAnyReelSpinning()
    {
        foreach (var reel in slotMachineController.reels)
            if (reel.isSpinning || reel.IsSnapped == false)
                return true;

        return false;
    }
}
