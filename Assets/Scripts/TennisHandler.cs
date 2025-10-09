using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TennisHandler : MonoBehaviour
{
    public int[] Score;
    public TMP_Text[] ScoreText;

    private void OnEnable()
    {
        Score = new int[2] { 0, 0 };
        //Score[0] = 0; Score[1] = 0;

    }

    public void BtnPress(int index)
    {
        Score[index]++;
        UpdateUI();
    }

    public void UpdateUI()
    {
        ScoreText[0].text = Score[0].ToString();
        ScoreText[1].text = Score[1].ToString();
    }

    public void HomeBtn()
    {
        SceneManager.LoadScene(0);
    }

    public void ResetToTennis()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
