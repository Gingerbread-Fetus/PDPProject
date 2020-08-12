using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class BoardController : MonoBehaviour
{
    [SerializeField] List<Panel> characterPanels;
    [SerializeField] AudioClip clearEffect;
    [SerializeField, Tooltip("How far from the top the panels start spawning")] int startingHeight = 6;
    public static BoardController instance;
    public int xSize, ySize;
    [HideInInspector] public List<Panel> nullPanels = new List<Panel>();
    [HideInInspector] public List<Panel> movingPanels = new List<Panel>();
    [HideInInspector] public bool isWaiting = false;

    Panel selectedPanel = null;
    List<Panel> possiblePanels;
    CameraController cameraController;
    Vector2 offset;
    int lastRowPosition;
    int lastRowSpawned;
    bool isShifting;
    private List<Panel> matchedPanels = new List<Panel>();
    int randomPanelNumber = 0;
    Panel[] childPanels;

    public Panel SelectedPanel { get => selectedPanel; set => selectedPanel = value; }
    public int LastRowPosition { get => lastRowPosition; set => lastRowPosition = value; }
    public int LastRowSpawned { get => lastRowSpawned; set => lastRowSpawned = value; }
    
    public bool IsShifting
    {
        get
        {
            return isShifting;
        }
        set
        {
            isShifting = value;
        }
    }    
    // Start is called before the first frame update
    void Start()
    {
        cameraController = FindObjectOfType<CameraController>();
        instance = GetComponent<BoardController>();
        Panel panel = characterPanels[0];
        offset = panel.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
        possiblePanels = new List<Panel>(characterPanels);
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

                newPanel.name = Enum.GetName(typeof(Panel.PanelType), newPanel.Type) + " " + x + " " + newPanel.transform.position.y;
            }
        }
        LastRowPosition = -ySize;
        LastRowSpawned = ySize;
        childPanels = GetComponentsInChildren<Panel>();
        cameraController.StartCoroutine("MoveToStart");
    }

    //This method moves the top row to the bottom so that the object pool stays consistent
    public void MoveToBottom(Panel hitPanel)
    {
        float xPos = hitPanel.transform.position.x;
        float startY = transform.position.y;
        Panel nextPanel = GetRandomPanelWithRemoval();

        hitPanel.transform.position = new Vector3(xPos, startY + (offset.y * LastRowPosition));
        hitPanel.SetType(nextPanel);
        hitPanel.ParticleSystem = nextPanel.ParticleSystem;
        hitPanel.ActiveState = false;
        if (nullPanels.Contains(hitPanel))
        {
            nullPanels.Remove(hitPanel);
        }
        hitPanel.name = Enum.GetName(typeof(Panel.PanelType), hitPanel.Type) + " " + hitPanel.XGridPos + " " + hitPanel.transform.position.y;
    }

    private Panel GetRandomPanelWithRemoval()
    {
        //TODO choose one, remove a panel every other, reset the list every six panels
        int randIndex = Random.Range(0, possiblePanels.Count);
        Panel nextPanel = possiblePanels[randIndex];
        possiblePanels.RemoveAt(randIndex);
        randomPanelNumber++;

        if(randomPanelNumber%6 == 0)
        {//Reset every 6 items
            randomPanelNumber = 0;
            possiblePanels = new List<Panel>(characterPanels);
        }

        return nextPanel;
    }

    public void GetClicked(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if(hit.collider != null && hit.collider.tag == "Panel")
            {
                Panel clickedPanel = hit.collider.gameObject.GetComponent<Panel>();
                if (!clickedPanel.ActiveState) { return;
                }
                if (selectedPanel == null && !isShifting)
                {
                    selectedPanel = hit.collider.gameObject.GetComponent<Panel>();
                    selectedPanel.SelectPanel();
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
        if (clickedPanel.transform.position.y == otherPanel.transform.position.y 
            && Math.Abs(clickedPanel.XGridPos - otherPanel.XGridPos ) == 1 
            && !IsShifting
            && clickedPanel.ActiveState
            && otherPanel.ActiveState)
        {
            clickedPanel.Swap(otherPanel);
            clickedPanel.Sort();
            otherPanel.Sort();
            StartCoroutine(CheckAllPanels());
        }
        selectedPanel.DeselectPanel();
        selectedPanel = null;
    }

    public IEnumerator CheckAllPanels()
    {
        yield return new WaitUntil(() => (nullPanels.Count == 0) && (movingPanels.Count == 0));
        IsShifting = true;
        Panel panel = null;
        for(int i = 0; i < childPanels.Length; i++)
        {
            panel = childPanels[i];

            if (panel.Type.Equals(Panel.PanelType.Null) || !panel.ActiveState) { continue; }//Nulls can't generate matches

            float cameraTime = 0;
            if (panel.CheckMatches())
            {
                matchedPanels.Add(panel);
                cameraTime += 1;
            }
            if (cameraController.moving)
            {
                cameraController.StartCoroutine(cameraController.PauseCamera(CalculateWaitTime(cameraTime))); 
            }
            else
            {
                cameraController.TimeLeft += CalculateWaitTime(cameraTime);
            }
        }
        if(matchedPanels.Count > 0)
        {
            Scorekeeper.score += 100 * matchedPanels.Count * (DisplayTimer.timeAsInt + 1);

            foreach (Panel matchedPanel in matchedPanels)
            {
                //TODO: Make call to particle system here
                matchedPanel.SetToNull();
            }
            AudioSource.PlayClipAtPoint(clearEffect, Camera.main.transform.position);

            yield return new WaitUntil(() => (nullPanels.Count == 0) && (movingPanels.Count == 0));
            
            matchedPanels.Clear();
            StartCoroutine(CheckAllPanels());
        }
        IsShifting = false;
    }

    private float CalculateWaitTime(float count)
    {
        return (float)count / 2f;
    }
}
