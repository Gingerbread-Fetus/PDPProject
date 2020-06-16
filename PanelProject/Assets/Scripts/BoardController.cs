﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class BoardController : MonoBehaviour
{
    [SerializeField] List<Panel> characterPanels;
    public static BoardController instance;
    public int xSize, ySize;

    private List<List<Panel>> panels;
    Panel selectedPanel = null;
    Vector2 offset;
    int lastRowPosition;
    int lastRowSpawned;

    public bool IsShifting { get; set; }
    public Panel SelectedPanel { get => selectedPanel; set => selectedPanel = value; }

    // Start is called before the first frame update
    void Start()
    {
        instance = GetComponent<BoardController>();
        Panel panel = characterPanels[0];
        offset = panel.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
    }

    private void CreateBoard(float xOffset, float yOffset)
    {
        panels = new List<List<Panel>>();

        float startX = transform.position.x;
        float startY = transform.position.y;

        Panel[] previousLeft = new Panel[ySize];
        Panel previousAbove = null;

        for(int x = 0; x < xSize; x++)
        {
            List<Panel> newColumn = new List<Panel>();
            panels.Add(newColumn);
            for(int y = 0; y < ySize; y++)
            {
                List<Panel> possiblePanels = new List<Panel>(characterPanels);

                possiblePanels.Remove(previousLeft[y]);
                possiblePanels.Remove(previousAbove);

                GameObject panelToSpawn = possiblePanels[Random.Range(0, possiblePanels.Count)].gameObject;
                GameObject newPanelObject = Instantiate(panelToSpawn,
                    new Vector3(startX + (xOffset * x), startY - (yOffset * y)),
                    panelToSpawn.transform.rotation,
                    this.transform);
                Panel newPanel = newPanelObject.GetComponent<Panel>();
                previousLeft[y] = newPanel.GetComponent<Panel>();
                previousAbove = newPanel.GetComponent<Panel>();
                newPanel.GetComponent<Panel>().XGridPos = x;
                newPanel.GetComponent<Panel>().YGridPos = y;
                panels[x].Add(newPanel);
            }
        }
        lastRowPosition = -ySize;
        lastRowSpawned = ySize;
    }
    
    //Todo this code is the rough draft for adding a new row to the grid.
    public void CreateNewRow(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            float startX = transform.position.x;
            float startY = transform.position.y;
            //TODO there are only six types of block, so randomizing a row of six from them lacks a little something, so I may change this later.
            List<Panel> possiblePanels = new List<Panel>(characterPanels);
            Panel nextPanel = null;

            for (int x = 0; x < xSize; x++)
            {
                nextPanel = possiblePanels[Random.Range(0, possiblePanels.Count)];
                possiblePanels.Remove(nextPanel);
                GameObject panelToSpawn = nextPanel.gameObject;
                GameObject newPanelObject = Instantiate(panelToSpawn,
                        new Vector3(startX + (offset.x * x), startY + (offset.y * lastRowPosition)),
                        panelToSpawn.transform.rotation,
                        this.transform);
                Panel newPanel = newPanelObject.GetComponent<Panel>();
                newPanel.XGridPos = x;
                newPanel.YGridPos = lastRowSpawned;
                panels[x].Add(newPanel);
                newPanel.Invoke("ClearAllMatches", 1.0f);
            }
            lastRowPosition -= 1;
            lastRowSpawned += 1;
        }
    }

    public void CreateNewRow()
    {
        float startX = transform.position.x;
        float startY = transform.position.y;
        //TODO there are only six types of block, so randomizing a row of six from them lacks a little something, so I may change this later.
        List<Panel> possiblePanels = new List<Panel>(characterPanels);
        Panel nextPanel = null;

        for (int x = 0; x < xSize; x++)
        {
            nextPanel = possiblePanels[Random.Range(0, possiblePanels.Count)];
            possiblePanels.Remove(nextPanel);
            GameObject panelToSpawn = nextPanel.gameObject;
            GameObject newPanelObject = Instantiate(panelToSpawn,
                    new Vector3(startX + (offset.x * x), startY + (offset.y * lastRowPosition)),
                    panelToSpawn.transform.rotation,
                    this.transform);
            Panel newPanel = newPanelObject.GetComponent<Panel>();
            newPanel.XGridPos = x;
            newPanel.YGridPos = lastRowSpawned;
            panels[x].Add(newPanel);
            newPanel.Invoke("ClearAllMatches", 1.0f);
        }
        lastRowPosition -= 1;
        lastRowSpawned += 1;
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
                }
                else
                {
                    Panel otherPanel = hit.collider.gameObject.GetComponent<Panel>();
                    TrySwap(selectedPanel, otherPanel);
                }
            }
        }
    }

    private void TrySwap(Panel clickedPanel, Panel otherPanel)
    {
        if (clickedPanel.YGridPos == otherPanel.YGridPos && Math.Abs(clickedPanel.XGridPos - otherPanel.XGridPos ) == 1)
        {
            //Update the grid
            panels[clickedPanel.XGridPos][clickedPanel.YGridPos] = otherPanel;
            panels[otherPanel.XGridPos][otherPanel.YGridPos] = clickedPanel;

            clickedPanel.Swap(otherPanel);
            clickedPanel.Invoke("ClearAllMatches", 1.0f);
            otherPanel.Invoke("ClearAllMatches", 1.0f);
        }
        selectedPanel = null;
    }
}
