using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public enum PanelType
    {
        Blue,
        Green,
        Orange,
        Red,
        Yellow,
        Pink,
        Null
    }

    [SerializeField] PanelType type = PanelType.Null;
    [SerializeField] SpriteRenderer backgroundSprite;
    [SerializeField] SpriteRenderer characterSprite;
    [SerializeField] float hoverTime = 0.1f;
    [SerializeField] Material inactiveMaterial;
    [SerializeField] Material activeMaterial;

    public PanelType Type { get => type; set => type = value; }
    public int XGridPos { get => xGridPos; set => xGridPos = value; }
    public SpriteRenderer BackgroundSprite { get => backgroundSprite; set => backgroundSprite = value; }
    public SpriteRenderer CharacterSprite { get => characterSprite; set => characterSprite = value; }
    

    int xGridPos = 0;
    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private bool matchFound = false;
    private bool activeState = true;
    BoardController boardController;

    public bool ActiveState
    {
        get => activeState;
        set
        {
            activeState = value;
            ActivatePanel();
        }
    }
    
    private void Start()
    {
        boardController = FindObjectOfType<BoardController>();
    }
    
    public void SetType(Panel nextPanel)
    {
        type = nextPanel.Type;
        backgroundSprite.color = nextPanel.backgroundSprite.color;
        backgroundSprite.sprite = nextPanel.BackgroundSprite.sprite;
        characterSprite.color = nextPanel.characterSprite.color;
        characterSprite.sprite = nextPanel.CharacterSprite.sprite;
    }
        
    public void SetToNull()
    {
        type = PanelType.Null;
        backgroundSprite.sprite = null;
        characterSprite.sprite = null;
        matchFound = false;
        StartCoroutine(SortToTop());
    }
    
    public void Sort()
    {
        if (type.Equals(PanelType.Null))
        {
            StartCoroutine(SortToTop());
        }
        else
        {
            StartCoroutine(SortToBottom());
        }
    }

    private IEnumerator SortToTop()
    {
        boardController.nullPanels.Add(this);
        if (!type.Equals(PanelType.Null)) { Debug.Break(); }
        RaycastHit2D[] hitArray = Physics2D.RaycastAll(transform.position, Vector2.up);
        Panel hitPanel = hitArray[0].collider.GetComponent<Panel>();
        while (hitArray.Length > 1)
        {
            hitPanel = hitArray[1].collider.GetComponent<Panel>();
            Swap(hitPanel);
            yield return new WaitForSeconds(hoverTime);
            hitArray = Physics2D.RaycastAll(transform.position, Vector2.up);
        }
        boardController.nullPanels.Remove(this);
    }
    
    private IEnumerator SortToBottom()
    {
        boardController.movingPanels.Add(this);
        RaycastHit2D[] hitArray = Physics2D.RaycastAll(transform.position, Vector2.down);
        Panel hitPanel = hitArray[0].collider.GetComponent<Panel>();
        while (hitArray.Length > 1)
        {
            hitPanel = hitArray[1].collider.GetComponent<Panel>();
            if (hitPanel.type.Equals(PanelType.Null))
            {
                Swap(hitPanel); 
            }
            else
            {
                break;
            }
            yield return new WaitForSeconds(hoverTime);
            hitArray = Physics2D.RaycastAll(transform.position, Vector2.down);
        }
        boardController.movingPanels.Remove(this);
    }
    
    public void Swap(Panel otherPanel)
    {
        int tempX;
        Vector3 tmpPosition = transform.position;
        tempX = xGridPos;

        xGridPos = otherPanel.XGridPos;

        otherPanel.XGridPos = tempX;

        transform.position = otherPanel.transform.position;
        otherPanel.transform.position = tmpPosition;
    }

    public override bool Equals(object obj)
    {
        if(obj == null || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Panel panel = (Panel)obj;
            return panel.Type == this.Type;
        }
    }
    
    private List<GameObject> FindMatch(Vector2 castDir)
    {
        List<GameObject> matchingPanels = new List<GameObject>();
        RaycastHit2D[] hitArray = Physics2D.RaycastAll(transform.position, castDir);
        RaycastHit2D hit;
        if (hitArray.Length > 1)
        {
            hit = hitArray[1];//Because the object will always collide with itself on this raycast.
            
            while (hitArray.Length > 1 && hit.collider.GetComponent<Panel>().Type == this.Type)
            {
                //TODO: Found a problem with it crashing in here
                if (!hit.collider.GetComponent<Panel>().ActiveState) { break; }
                matchingPanels.Add(hit.collider.gameObject);
                hitArray = Physics2D.RaycastAll(hit.collider.transform.position, castDir);
                if(hitArray.Length < 2) { continue; }
                hit = hitArray[1];
            }
        }
        return matchingPanels;
    }
        
    private void GetMatches(Vector2[] paths)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        for (int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }
        if (matchingTiles.Count >= 2)
        {
            matchFound = true;
        }
    }

    public bool CheckMatches()
    {
        GetMatches(new Vector2[2] { Vector2.left, Vector2.right });
        GetMatches(new Vector2[2] { Vector2.up, Vector2.down });

        return matchFound;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("MainCamera"))
        {
            StopAllCoroutines();
        }
    }

    private void ActivatePanel()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (activeState)
        {
            spriteRenderer.material = activeMaterial;
        }
        else
        {
            spriteRenderer.material = inactiveMaterial;
        }
    }
}
