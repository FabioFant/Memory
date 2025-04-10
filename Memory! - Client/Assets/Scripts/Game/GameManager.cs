using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject container;
    public GameObject card;

    GameMemory memory;
    Color[] colors;

    public void StartNewGame((int, bool)[,] matrix)
    {
        memory = new GameMemory(matrix);
        colors = new Color[memory.Height * memory.Width / 2];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Random.ColorHSV();
        }

        SetupCardGrid();
    }

    void SetupCardGrid()
    {
        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }

        GridLayoutGroup gridLayout = container.GetComponent<GridLayoutGroup>();
        gridLayout.constraintCount = memory.Width;

        for (int row = 0; row < memory.Height; row++)
        {
            for (int col = 0; col < memory.Width; col++)
            {
                GameObject newCard = Instantiate(card, container.transform);
                newCard.name = $"{row}{col}";
            }
        }
    }
}