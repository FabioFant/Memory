using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    public GameObject container;
    public GameObject card;

    GameMemory memory;
    Color[] colors;

    public void StartNewGame((int, bool)[,] matrix)
    {
        Debug.Log("Creating game setup...");

        memory = new GameMemory(matrix);
        colors = new Color[memory.Height * memory.Width / 2];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Random.ColorHSV();
        }
        SetupCardGrid();

        Debug.Log("Game setup complete.");
    }

    void SetupCardGrid()
    {
        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }

        GridLayoutGroup gridLayout = container.GetComponent<GridLayoutGroup>();

        // IMPOSTA IL VINCOLO A COLONNE FISSE
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = memory.Width; // Numero di colonne = larghezza matrice

        for (int row = 0; row < memory.Height; row++)
        {
            for (int col = 0; col < memory.Width; col++)
            {
                GameObject newCard = Instantiate(card, container.transform);
                newCard.name = $"{row}{col}";
            }
        }

        // Opzionale: Forza l'aggiornamento del layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());
    }
}