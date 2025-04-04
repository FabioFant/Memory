using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button card;

    // Start is called before the first frame update
    void Start()
    {
        Memory memory = new(4, 4);

        float cardWidth = card.transform.localScale.x;
        float cardHeight = card.transform.localScale.y;

        transform.localScale = new Vector3(1, 1);
        transform.position = new Vector3(0, 0, 0);

        float currX = 0, currY = 0;
        for(int i = 0; i < memory.Height; i++)
        {
            currY = cardHeight * i + 5;
            currX = 0;
            for(int j = 0; j < memory.Width; j++)
            {
                Button newCard = Instantiate(card, transform);
                newCard.transform.localPosition = new Vector3(currX + cardWidth + 5, currY, 0);
                currX = currX + cardWidth + 5;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
