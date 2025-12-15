using UnityEngine;
using System.Collections;

public class FreeSpinCheck : MonoBehaviour
{
    #region system
    [Header("UI")]
    public UI ui;
    public BetSide betSide;

    [Header("Systems")]
    public SlotMachineController slotMachineController;
    public AutoSpin autoSpinCheck;

    [Header("Data")]
    public PlayerWallet playerWallet;
    #endregion

    private int freeSpinCount = 0;

    public void Check()
    {
        if (freeSpinCount > 0)
        {
            freeSpinCount--;
        }
        else
        {
            if (playerWallet.WalletGet >= betSide.BetMoneyGet)
                playerWallet.updateMoney(false, 0);
        }

        ui.UIUpdate(UI.eUIToString.FreeSpin);
        ui.ShowFreeSpinCount();
    }

    public void freeSpin()
    {
        AudioManager.Instance.Play(AudioManager.Instance.freeSpin);
        freeSpinCount += 10;
        ui.ShowFreeSpinCount();
        StartCoroutine(FreeSpinAuto());
        //betButton.interactable = false;
    }

    #region freeSpinAuto
    private IEnumerator FreeSpinAuto()
    {
        yield return new WaitForSeconds(0.3f);

        if (freeSpinCount > 0 && autoSpinCheck.autoSpin == false)
        {
            slotMachineController.OnPlayButton();
        }
    }
    public void FreeSpinProtect()
    {
        if (freeSpinCount > 0)
        {
            ui.ShowFreeSpinCount();
        }

        if (freeSpinCount > 0 && slotMachineController != null)
        {
            StartCoroutine(FreeSpinAuto());
        }
    }
    #endregion


    public void FreeSpinText()
    {
        if (freeSpinCount <= 0)
            ui.winUI.SetActive(false);
    }

    public int FreeSpinCountCheck
    { 
        get { return freeSpinCount;} 
        set { freeSpinCount = value; }
    }

}
