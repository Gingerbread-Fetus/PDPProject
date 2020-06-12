using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardController : MonoBehaviour
{
    public static BoardController instance;
    public List<Sprite> characters;
    [SerializeField] GameObject panel;
    public int xSize, ySize;
    public int startingHeight;

    private List<List<GameObject>> panels;
    Vector2 offset;
    int lastRow;

    public bool IsShifting { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        instance = GetComponent<BoardController>();

        offset = panel.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
    }

    private void CreateBoard(float xOffset, float yOffset)
    {
        panels = new List<List<GameObject>>();

        float startX = transform.position.x;
        float startY = transform.position.y;

        for(int x = 0; x < xSize; x++)
        {
            List<GameObject> newColumn = new List<GameObject>();
            panels.Add(newColumn);
            for(int y = 0; y < ySize; y++)
            {
                GameObject newPanel = Instantiate(panel,
                    new Vector3(startX + (xOffset * x), startY + (yOffset * y)),
                    panel.transform.rotation,
                    this.transform);
                panels[x].Add(newPanel);
                var spriteRenderer = newPanel.GetComponent<SpriteRenderer>();
                if (y > startingHeight)
                {
                    spriteRenderer.sprite = null;
                }
            }
        }
        lastRow = -1;
    }
    
    public void CreateNewRow(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            float startX = transform.position.x;
            float startY = transform.position.y;

            for (int x = 0; x < xSize; x++)
            {
                GameObject newPanel = Instantiate(panel,
                        new Vector3(startX + (offset.x * x), startY + (offset.y * lastRow)),
                        panel.transform.rotation,
                        this.transform);
                panels[x].Add(newPanel);
            }
            lastRow -= 1; 
        }
    }

}
