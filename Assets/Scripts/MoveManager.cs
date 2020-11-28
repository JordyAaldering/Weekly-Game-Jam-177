﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;

    private int movesLeft;

    public bool HasMovesLeft => movesLeft > 0;

    public void Initialize(int initialMoves)
    {
        movesLeft = initialMoves;
        UpdateCounterText();
    }

    public void DecreaseCounter()
    {
        if (movesLeft <= 0) {
            Debug.LogWarning("move counter decreased below zero");
            return;
        }

        movesLeft--;
        UpdateCounterText();
    }

    private void UpdateCounterText()
    {
        counterText.text = $"Moves Left: {movesLeft}";
    }
}
