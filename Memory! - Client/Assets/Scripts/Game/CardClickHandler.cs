using UnityEngine;
using UnityEngine.UI;

public class ButtonClickHandler : MonoBehaviour
{
    ConnectionHandler connectionHandler;
    Button button;
    int col, row;

    // Start is called before the first frame update
    void Start()
    {
        GameObject canvas = GameObject.Find("Canvas");
        Transform connect_button = canvas.transform.Find("Connect_button");
        connectionHandler = connect_button.GetComponent<ConnectionHandler>();

        button = GetComponent<Button>();
        row = int.Parse(name[0].ToString());
        col = int.Parse(name[1].ToString());
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        connectionHandler.SetCard(row, col);
    }
}
