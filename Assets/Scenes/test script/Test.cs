using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public int row;
    public int col;
    int imageLenght = 0;

    public Image[] image;
    public Image[] image1;
    public GameObject winText;

    private int[,] randomNumber;

    private void Start()
    {
        winText.SetActive(false);
        
        randomNumber = new int[row, col];

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                randomNumber[i, j] = Random.Range(0, image.Length);
                image1[imageLenght].sprite = image[randomNumber[i, j]].sprite;
                imageLenght++;
                if (imageLenght >= 9)
                {
                    imageLenght = 0;
                }
            }
        }


        if (randomNumber[0 , 0] == randomNumber[0, 1] && randomNumber[0 , 0] == randomNumber[0, 2])
        {
            winText.SetActive (true);
        }
        if (randomNumber[1, 0] == randomNumber[1, 1] && randomNumber[1, 0] == randomNumber[1, 2])
        {
            winText.SetActive(true);
        }
        if (randomNumber[2, 0] == randomNumber[2, 1] && randomNumber[2, 0] == randomNumber[2, 2])
        {
            winText.SetActive(true);
        }
        if (randomNumber[0, 0] == randomNumber[1, 0] && randomNumber[0, 0] == randomNumber[2, 0])
        {
            winText.SetActive(true);
        }
        if (randomNumber[0, 1] == randomNumber[1, 1] && randomNumber[0, 1] == randomNumber[2, 1])
        {
            winText.SetActive(true);
        }
        if (randomNumber[0, 2] == randomNumber[1, 2] && randomNumber[0, 2] == randomNumber[2, 2])
        {
            winText.SetActive(true);
        }
        if (randomNumber[0, 0] == randomNumber[1, 1] && randomNumber[0, 0] == randomNumber[2, 2])
        {
            winText.SetActive(true);
        }
        if (randomNumber[0, 2] == randomNumber[1, 1] && randomNumber[0, 2] == randomNumber[2, 0])
        {
            winText.SetActive(true);
        }

    }
}