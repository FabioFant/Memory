using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject container;
    public GameObject card;

    public GameObject statusText;

    public Text client1_points;
    public Text client2_points;

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
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = memory.Width;

        for (int row = 0; row < memory.Height; row++)
        {
            for (int col = 0; col < memory.Width; col++)
            {
                GameObject newCard = Instantiate(card, container.transform);
                newCard.name = $"{row}{col}";
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());
    }

    public async void ActionOnCards(int row1, int col1, int row2, int col2, bool isRight)
    {
        Debug.Log($"Action on cards: ({row1}, {col1}) and ({row2}, {col2})");

        if (isRight)
        {
            FlipCard(row1, col1);
            FlipCard(row2, col2);
        }
        else
        {
            FlipCard(row1, col1);
            FlipCard(row2, col2);

            await Task.Delay(1000);

            UnflipCard(row1, col1);
            UnflipCard(row2, col2);
        }
    }

    void FlipCard(int row, int col)
    {
        GameObject cardToUpdate = container.transform.Find($"{row}{col}").gameObject;
        Image cardImage = cardToUpdate.GetComponent<Image>();
        int colorIndex = memory.GetID(row, col);
        cardImage.color = colors[colorIndex];
    }

    void UnflipCard(int row, int col)
    {
        GameObject cardToUpdate = container.transform.Find($"{row}{col}").gameObject;
        Image cardImage = cardToUpdate.GetComponent<Image>();
        int colorIndex = memory.GetID(row, col);
        cardImage.color = card.GetComponent<Image>().color;
    }

    public void UpdateStatusText(string message)
    {
        Text status = statusText.GetComponent<Text>();
        status.text = message;
    }

    public void UpdateClientPoints(int client1, int client2)
    {
        client1_points.text = "Your points: " + client1.ToString();
        client2_points.text = "Opponent's points: " + client2.ToString();

        if(client1 > client2)
        {
            client1_points.fontSize = 36;
            client2_points.fontSize = 24;
        }
        else if (client1 < client2)
        {
            client1_points.fontSize = 24;
            client2_points.fontSize = 36;
        }
        else
        {
            client1_points.fontSize = 24;
            client2_points.fontSize = 24;
        }
    }

    private void Start()
    {
        UpdateStatusText("Welcome to Memory!");
        client1_points.text = "";
        client2_points.text = "";
    }
}