using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
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

    public PanelType Type { get => type; set => type = value; }
    public int XGridPos { get => xGridPos; set => xGridPos = value; }
    public SpriteRenderer BackgroundSprite { get => backgroundSprite; set => backgroundSprite = value; }
    public SpriteRenderer CharacterSprite { get => characterSprite; set => characterSprite = value; }

    int xGridPos = 0;
    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private bool matchFound = false;

    private void Update()
    {
        //If the panel is null, we want to check if there is anything above it.
        if (type.Equals(PanelType.Null))
        {
            SwapUp();
        }
    }

    private void SwapUp()
    {
        Vector2 upVector = new Vector2(0, 1);
        RaycastHit2D[] hitArray = Physics2D.RaycastAll(transform.position, upVector);
        //If we don't hit at least 2 objects then the panel is at the top.
        if(hitArray.Length >= 2)
        {
            //Do nothing if the one above it is also null.
            if (hitArray[1].transform.GetComponent<Panel>().Type.Equals(PanelType.Null)) { return; }
            RaycastHit2D hit = hitArray[1];
            Vector3 tempPos = transform.position;
            transform.position = hit.transform.position;
            hit.transform.position = tempPos;
            hitArray = Physics2D.RaycastAll(transform.position, upVector);
        }
    }

    public void SetToNull()
    {
        type = PanelType.Null;
        backgroundSprite.sprite = null;
        characterSprite.sprite = null;
    }

    public void Swap(Panel otherPanel)
    {
        int tempx, tempy;
        Vector3 tmpPosition = transform.position;
        tempx = xGridPos;

        xGridPos = otherPanel.XGridPos;

        otherPanel.XGridPos = tempx;

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

    private GameObject GetAdjacent(Vector2 castDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if(hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private List<GameObject> GetAllAdjacentPanels()
    {
        List<GameObject> adjacentPanels = new List<GameObject>();
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacentPanels.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentPanels;
    }

    private List<GameObject> FindMatch(Vector2 castDir)
    {
        List<GameObject> matchingPanels = new List<GameObject>();
        RaycastHit2D[] hitArray = Physics2D.RaycastAll(transform.position, castDir);
        RaycastHit2D hit;
        if (hitArray.Length > 1)
        {
            hit = hitArray[1];//Because the object will always collide with itself on this raycast.
            Debug.DrawRay(transform.position, castDir, Color.red, 5.4f);
            
            while (hitArray.Length > 1 && hit.collider.GetComponent<Panel>().Type == this.Type)
            {
                matchingPanels.Add(hit.collider.gameObject);
                hitArray = Physics2D.RaycastAll(hit.collider.transform.position, castDir);
                if(hitArray.Length < 2) { continue; }
                hit = hitArray[1];
                Debug.DrawRay(hit.collider.transform.position, castDir, Color.blue, 5.4f);
            }
        }
        return matchingPanels;
    }

    private void ClearMatch(Vector2[] paths)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        for(int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }
        if (matchingTiles.Count >= 2)
        {
            for(int i = 0; i < matchingTiles.Count; i++)
            {
                matchingTiles[i].GetComponent<Panel>().SetToNull();
            }
            matchFound = true;
        }
    }

    public void ClearAllMatches()
    {
        if(type == PanelType.Null)
        {
            return;
        }

        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });

        if (matchFound)
        {
            SetToNull();
            matchFound = false;
            //TODO play sound effect
        }
    }

    public void SetType(Panel nextPanel)
    {
        type = nextPanel.Type;
        backgroundSprite.color = nextPanel.backgroundSprite.color;
        backgroundSprite.sprite = nextPanel.BackgroundSprite.sprite;
        characterSprite.color = nextPanel.characterSprite.color;
        characterSprite.sprite = nextPanel.CharacterSprite.sprite;
    }
}
