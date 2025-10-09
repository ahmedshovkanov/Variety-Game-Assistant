using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ChessSetup : MonoBehaviour
{
    public GameObject MainCanvas;
    public ChessTimer ChessTimerRef;

    public TMP_InputField TimeField, AddedTimeField;

    public void NextBtn()
    {
        if (int.TryParse(TimeField.text, out int number))
        {
            ChessTimerRef.PresetTime = number;
        }
        else
        {
            ChessTimerRef.PresetTime = 600;
            Debug.LogWarning("TryParse else");
        }

        if (int.TryParse(AddedTimeField.text, out int numberAdd))
        {
            ChessTimerRef.PresetAddedTimePerTurn = numberAdd;
        }
        else
        {
            ChessTimerRef.PresetAddedTimePerTurn = 5;
            Debug.LogWarning("TryParse else");
        }

        MainCanvas.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
