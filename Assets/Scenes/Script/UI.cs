using TMPro;
using UnityEngine;
using static SlotMachineController;

public class UI : MonoBehaviour
{
    #region system
    public PlayerWallet playerWallet;
    public FreeSpinCheck freeSpinCheck;
    public SlotMachineController slotMachineController;
    public ReelController[] reels;
    #endregion

    [Header("赢的UI")]
    public GameObject winUI;

    [Header("玩家的钱")]
    public TextMeshProUGUI walletText;

    [Header("赢的钱")]
    public TextMeshProUGUI moneyScoreText;

    [Header("UIToggleObject")]
    public GameObject RTPPanel;
    public GameObject InfoPanel;

    private bool isShowingWinMoney = false;

    public SpinSpeedMode selectedSpeedMode = SpinSpeedMode.Normal;

    private void Start()
    {
        moneyScoreText.text = playerWallet.MoneyScoreGet.ToString();
        winUI.SetActive(false);
        UIUpdate(eUIToString.Money);
    }
    public enum eUIToString
    {
        All,
        Win,
        FreeSpin,
        Money
    }

    public void UIUpdate(eUIToString type = eUIToString.All)
    {
        switch (type)
        {

            case eUIToString.FreeSpin:
                moneyScoreText.text = $"FreeSpin :{freeSpinCheck.FreeSpinCountCheck}";
                break;

            case eUIToString.Money:
                walletText.text = "Money : " + playerWallet.WalletGet;
                break;
        }
    }

    public void ShowWinUI(bool isFreeSpin)
    {
        winUI.SetActive(true);

        if (isFreeSpin)
        {
            // freeSpin() 已经改 freeSpinCount 了，这里只负责显示剩余次数
            ShowFreeSpinCount();
        }
        else
        {
            winUI.SetActive(true);
            //winUI.GetComponent<TextMeshProUGUI>().text = "YOU WIN!";
            //UIUpdate(eUIToString.Win);
            PlayerWinUIMoneyUpdate();
        }
    }

    //ui
    //TODO:showfreespincount 可以改名字 如 UIShowFreeSpinCount，wintext 哪一行可以直接用UIUpdate 感觉会比较整齐
    public void ShowFreeSpinCount()
    {
        if (isShowingWinMoney) return;

        if (freeSpinCheck.FreeSpinCountCheck > 0)
        {
            winUI.SetActive(true);
            //winUI.GetComponent<TextMeshProUGUI>().text = $"FreeSpin : {freeSpinCheck.FreeSpinCountCheck}";
            UIUpdate(eUIToString.FreeSpin);
        }
        else
        {
            // 没有 FreeSpin 了才关掉
            winUI.SetActive(false);
        }
    }

    public void ResetWinMoneyDisplay()
    {
        isShowingWinMoney = false;

        // 如果 Freespin > 0 → 显示 Freespin 数字
        if (freeSpinCheck.FreeSpinCountCheck > 0)
        {
            UIUpdate(eUIToString.FreeSpin);
        }
        else
        {
            winUI.SetActive(false);
        }
    }

    

    public void PlayerWinUIMoneyUpdate()
    {
        isShowingWinMoney = true;
        moneyScoreText.text = playerWallet.MoneyScoreGet.ToString();
    }

    public void playerWinUIMoneyReset()
    {
        if (freeSpinCheck.FreeSpinCountCheck == 0)
        {
            playerWallet.MoneyScoreGet = 0;
            moneyScoreText.text = playerWallet.MoneyScoreGet.ToString();
        }
    }

    public void ToggleSpeed(bool isFast)
    {
        // ❌ 正在旋转，不允许切换
        if (slotMachineController.isStopping)
            return;

        selectedSpeedMode = isFast ? SpinSpeedMode.Fast : SpinSpeedMode.Normal;

        // 写入 SlotMachineController
        slotMachineController.currentSpeedMode = selectedSpeedMode;
    }

    public void AudioPlayClick()
    {
        AudioManager.Instance.Play(AudioManager.Instance.buttonClick);
    }

    public void WinUI(bool enable)
    {
        winUI.SetActive(enable);
    }

    public void RtpPanelControler(bool toogle)
    {
        RTPPanel.SetActive(toogle);
    }

    public void UIInfoPanel(bool toogle)
    {
        InfoPanel.SetActive(toogle);
    }

}