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

    private GameObject[,] panels;
    Vector2 offset;

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
        panels = new GameObject[xSize, ySize];

        float startX = transform.position.x;
        float startY = transform.position.y;

        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                if (y < startingHeight)
                {
                    GameObject newPanel = Instantiate(panel,
                                new Vector3(startX + (xOffset * x), startY + (yOffset * y)),
                                panel.transform.rotation,
                                this.transform);
                    panels[x, y] = newPanel; 
                }
                else
                {
                    panels[x, y] = null;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            CreateNewRow();
        }
    }

    public void CreateNewRow()
    {
        if (MoveUp())
        {
        }
    }

    /// <summary>
    /// Atempts to move all of the panels up one row. If it cant, it sets the lose condition and returns false.
    /// </summary>
    /// <returns></returns>    
    public bool MoveUp()
    {
        Debug.Log("Moving panels up");
        for (int x = xSize - 1; x >= 0; x--)
        {
            for (int y = ySize - 1; y >= 0 ; y--)
            {
                if(panels[x,y] == null) { continue; }
                if(y >= ySize) { return false; }//TODO need to test this
                Vector3 originalPosition = panels[x, y].transform.position;
                panels[x, y].transform.position = new Vector3(originalPosition.x, originalPosition.y + offset.y);
                panels[x, y + 1] = panels[x, y];//Move it up one block
            }
        }
        return true;
    }
}
