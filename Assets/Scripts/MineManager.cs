using UnityEngine;
using TMPro;

public class MineManager : MonoBehaviour
{
    public int totalMines = 6;
    private int clearedMines = 0;

    public TextMeshProUGUI statusText;

    void Start()
    {
        UpdateUI();
    }

    // This method should be called by each mine when it is cleared
    public void OnMineCleared()
    {
        clearedMines++;
        UpdateUI();

        if (clearedMines >= totalMines)
        {
            ShowVictoryMessage();
        }
    }

    void UpdateUI()
    {
        if (statusText != null)
            statusText.text = $"Mines Cleared: {clearedMines} / {totalMines}";
    }

    void ShowVictoryMessage()
    {
        if (statusText != null)
            statusText.text = "All Cleaned!";

        // Optionally, you can also trigger other victory effects here (e.g., play a sound, show a particle effect, etc.)
    }
}