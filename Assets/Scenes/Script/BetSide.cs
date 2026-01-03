using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BetSide : MonoBehaviour
{
    public PlayerWallet playerWallet;

    public TextMeshProUGUI betMoneyUI;
    public TMP_Dropdown dropdown;

    private int betMoney;
    public int[] betOptions = { 20, 40, 80, 160, 320, 640, 1280 };

    void Start()
    {
        //// 设成当前选项对应下注（一般为 0）
        betMoney = betOptions[0];
        betMoneyUI.text = betMoney.ToString();  
    }

    public void IncreaseBet()
    {
        int currentIndex = System.Array.IndexOf(betOptions, betMoney);

        if (currentIndex < betOptions.Length - 1 && playerWallet.WalletGet >= betOptions[currentIndex + 1])
        {
            betMoney = betOptions[currentIndex + 1];
            Debug.Log("Bet increased → " + betMoney);
            betMoneyUI.text = betMoney.ToString();
        }
    }

    public void DecreaseBet()
    {
        int currentIndex = System.Array.IndexOf(betOptions, betMoney);

        if (currentIndex > 0)
        {
            betMoney = betOptions[currentIndex - 1];
            Debug.Log("Bet decreased → " + betMoney);
            betMoneyUI.text = betMoney.ToString();

        }
    }

    public void MaxBet()
    {
        for (int i = betOptions.Length - 1; i >= 0; i--)
        {
            if (playerWallet.WalletGet >= betOptions[i])
            {
                betMoney = betOptions[i];
                Debug.Log("MAX bet → " + betMoney);
                betMoneyUI.text = betMoney.ToString();
                break;
            }
        }
    }

    public int BetMoneyGet
    {
        get { return betMoney; }
        set { betMoney = value; }
    }
}

