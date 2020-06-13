using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class BoardController : MonoBehaviour
{
    [SerializeField] List<GameObject> characterPanels;
    public static BoardController instance;
    public List<Sprite> characters;
    public int xSize, ySize;
    public int startingHeight;

    private List<List<GameObject>> panels;
    Panel selectedPanel = null;
    Vector2 offset;
    int lastRow;

    public bool IsShifting { get; set; }
    public Panel SelectedPanel { get => selectedPanel; set => selectedPanel = value; }

    // Start is called before the first frame update
    void Start()
    {
        instance = GetComponent<BoardController>();
        GameObject panel = characterPanels[0];
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
                GameObject panelToSpawn = characterPanels[Random.Range(0, characterPanels.Count)];
                GameObject newPanel = Instantiate(panelToSpawn,
                    new Vector3(startX + (xOffset * x), startY + (yOffset * y)),
                    panelToSpawn.transform.rotation,
                    this.transform);
                newPanel.GetComponent<Panel>().XGridPos = x;
                newPanel.GetComponent<Panel>().YGridPos = y;
                panels[x].Add(newPanel);
                if (y > startingHeight)
                {
                    newPanel.GetComponent<Panel>().SetToNull();
                }
            }
        }
        lastRow = -1;
    }
    
    //Todo this code is the rough draft for adding a new row to the grid.
    public void CreateNewRow(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            float startX = transform.position.x;
            float startY = transform.position.y;

            for (int x = 0; x < xSize; x++)
            {
                GameObject panelToSpawn = characterPanels[Random.Range(0, characterPanels.Count)];
                GameObject newPanel = Instantiate(panelToSpawn,
                        new Vector3(startX + (offset.x * x), startY + (offset.y * lastRow)),
                        panelToSpawn.transform.rotation,
                        this.transform);
                panels[x].Add(newPanel);
            }
            lastRow -= 1; 
        }
    }


    public void GetClicked(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if(hit.collider != null && hit.collider.tag == "Panel")
            {
                Debug.Log("Clicked on: " + hit.collider.gameObject.name);
                if (selectedPanel == null)
                {
                    selectedPanel = hit.collider.gameObject.GetComponent<Panel>();
                    Debug.Log(selectedPanel.name);
                }
                else
                {
                    Panel otherPanel = hit.collider.gameObject.GetComponent<Panel>();
                    TrySwap(selectedPanel, otherPanel);
                }
            }
        }
    }

    private void TrySwap(Panel selectedPanel, Panel otherPanel)
    {
        //Update the grid
        panels[selectedPanel.XGridPos][selectedPanel.YGridPos] = otherPanel.gameObject;
        panels[otherPanel.XGridPos][otherPanel.YGridPos] = selectedPanel.gameObject;
        selectedPanel.Swap(otherPanel);
    }
}
