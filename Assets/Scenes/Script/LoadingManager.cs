using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public PlayerWallet playerWallet;
    public TextMeshProUGUI money;

    public void Awake()
    {
        SetMoneyToText();
        Debug.Log(money.text);
    }

    public void LoadingToGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadingToMenu()
    {
        SceneManager.LoadScene(0);
        AudioManager.Instance.SpinSound(false);
    }

    public void AddMoney(int value)
    {
        playerWallet.WalletGet += value;
        SetMoneyToText();
    }

    public void ResetWallet()
    {
        playerWallet.WalletGet = 1000;
        SetMoneyToText();
    }

    public void SetMoneyToText()
    {
        playerWallet.ReadPlayerPrefMoney();
        money.text = playerWallet.WalletGet.ToString();
        Debug.Log(money.text);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("quit");
    }
}