using JetBrains.Annotations;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public UI ui;
    public BetSide betSide;
    public FreeSpinCheck freeSpinCheck;

    [Header("玩家的钱")]
    private int wallet;
    private int moneyScore;

    private float totalBet = 0f;
    private float totalWin = 0f;

    private void Start()
    {
        ReadPlayerPrefMoney();
    }

    public static class PrefKeys
    {
        public const string Wallet = "playerWallet";
    }


    /// <summary>
    /// 侦测是否赢，并且价钱和上传去UI
    ///负责：
    ///- 扣除下注
    /// - 计算中奖金额
    /// - 更新钱包
    /// - 累积 RTP 统计
    /// </summary>
    /// <param name="isWin">是否赢</param>
    /// <param name="multi">是多少钱</param>
    public void updateMoney(bool isWin, int multi)
    {

        totalBet += betSide.BetMoneyGet;

        if (isWin)
        {
            AudioManager.Instance.Play(AudioManager.Instance.win);
            moneyScore += betSide.BetMoneyGet * multi;
            PlayerWalletGet += betSide.BetMoneyGet * multi;
            ui.PlayerWinUIMoneyUpdate();
            totalWin += betSide.BetMoneyGet * multi;
        }
        else
        {
            PlayerWalletGet -= betSide.BetMoneyGet;
        }
        //currentRTP = totalWin / totalBet;
    }

    public void PayTable(SymbolType symbol)
    {
        switch (symbol)
        {
            case SymbolType.Cherry:
                updateMoney(true, 1);
                ui.ShowWinUI(false);
                break;

            case SymbolType.Lemon:
                updateMoney(true, 2);
                ui.ShowWinUI(false);
                break;
            case SymbolType.Watermelon:
                updateMoney(true, 4);
                ui.ShowWinUI(false);
                break;
            case SymbolType.Seven:
                updateMoney(true, 6);
                ui.ShowWinUI(false);
                break;

            case SymbolType.FreeSpin:
                freeSpinCheck.freeSpin();
                ui.ShowWinUI(true);
                break;
        }
    }

    public void PlayerWalletCheck()
    {
        if (WalletGet == 0)
        {
            return;
        }
    }

    public void ReadPlayerPrefMoney()
    {
        wallet = PlayerPrefs.GetInt(PrefKeys.Wallet, wallet);
    }


    //这里出现两个wallet 的getset，主要一个是用于UI
    public int PlayerWalletGet
    {
        get { return wallet; }
        set
        {
            wallet = value;
            PlayerPrefs.SetInt(PrefKeys.Wallet, wallet);
            PlayerPrefs.Save();
            ui.UIUpdate(UI.eUIToString.Money);
        }
    }

    public int MoneyScoreGet
    {
        get { return moneyScore; }
        set { moneyScore = value; }
    }

    public int WalletGet
    {
        get { return wallet; }
        set {
            wallet = value;
            PlayerPrefs.SetInt(PrefKeys.Wallet, wallet);
            PlayerPrefs.Save();
        }
    }

    public float CurrentRTP
    {
        get
        {
            if (totalBet <= 0f) return 0f;
            return totalWin / totalBet;
        }
    }
}