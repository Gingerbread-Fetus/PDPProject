using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class BoardController : MonoBehaviour
{
    [SerializeField] List<Panel> characterPanels;
    public static BoardController instance;
    public int xSize, ySize;
    [HideInInspector] public List<Panel> nullPanels = new List<Panel>();
    [HideInInspector] public List<Panel> movingPanels = new List<Panel>();
    [HideInInspector] public List<Panel> movedPanels = new List<Panel>();
    [HideInInspector] public bool isWaiting = false;

    Panel selectedPanel = null;
    CameraController cameraController;
    Vector2 offset;
    int lastRowPosition;
    int lastRowSpawned;

    public bool IsShifting { get; set; }
    public Panel SelectedPanel { get => selectedPanel; set => selectedPanel = value; }
    public int LastRowPosition { get => lastRowPosition; set => lastRowPosition = value; }
    public int LastRowSpawned { get => lastRowSpawned; set => lastRowSpawned = value; }
        
    // Start is called before the first frame update
    void Start()
    {
        cameraController = FindObjectOfType<CameraController>();
        instance = GetComponent<BoardController>();
        Panel panel = characterPanels[0];
        offset = panel.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
    }
    
    private void CreateBoard(float xOffset, float yOffset)
    {
        float startX = transform.position.x;
        float startY = transform.position.y;

        Panel[] previousLeft = new Panel[ySize];
        Panel previousAbove = null;

        for(int x = 0; x < xSize; x++)
        {
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
            }
        }
        LastRowPosition = -ySize;
        LastRowSpawned = ySize;
    }
    
    //This method moves the top row to the bottom so that the object pool stays consistent
    public void MoveToBottom(Panel hitPanel)
    {
        float xPos = hitPanel.transform.position.x;
        float startY = transform.position.y;
        //TODO there are only six types of block, so randomizing a row of six from them lacks a little something, so I may change this later.
        List<Panel> possiblePanels = new List<Panel>(characterPanels);
        Panel nextPanel = possiblePanels[Random.Range(0, possiblePanels.Count)];
        possiblePanels.Remove(nextPanel);
        
        hitPanel.transform.position = new Vector3(xPos, startY + (offset.y * LastRowPosition));
        hitPanel.SetType(nextPanel);
        //hitPanel.Invoke("ClearAllMatches", 1.0f);
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
        if (clickedPanel.transform.position.y == otherPanel.transform.position.y && Math.Abs(clickedPanel.XGridPos - otherPanel.XGridPos ) == 1 && !IsShifting)
        {
            IsShifting = true;
            clickedPanel.Swap(otherPanel);
            clickedPanel.Sort();
            otherPanel.Sort();
            clickedPanel.Invoke("ClearAllMatches", 0.1f);
            otherPanel.Invoke("ClearAllMatches", 0.1f);
            StartCoroutine(WaitForBoardToUpdate());
        }
        else
        {
            Debug.Log(IsShifting);
        }
        selectedPanel = null;
    }

    private IEnumerator WaitForBoardToUpdate()
    {
        yield return new WaitUntil(() => (nullPanels.Count == 0) && (movingPanels.Count == 0));
        //The idea is that here once all the panels are done updating, I find some way to check
        //All the panels and keep iterating through it
        StartCoroutine(CheckAllPanels());
    }

    private IEnumerator CheckAllPanels()
    {
        Panel panel = null;
        foreach (Transform child in transform)
        {
            panel = child.GetComponent<Panel>();

            if (panel.Type.Equals(Panel.PanelType.Null)) { continue; }//Nulls can't generate matches

            IsShifting = true;
            panel.ClearAllMatches();
            yield return new WaitUntil(() => (nullPanels.Count == 0) && (movingPanels.Count == 0));
        }
        //TODO: If any matches found, start again, do it until no more matches found.
        IsShifting = false;
    }
}
