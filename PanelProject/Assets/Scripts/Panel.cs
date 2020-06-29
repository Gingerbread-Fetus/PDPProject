﻿using System.Collections;
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
    [SerializeField] float hoverTime = 0.1f;

    public PanelType Type { get => type; set => type = value; }
    public int XGridPos { get => xGridPos; set => xGridPos = value; }
    public SpriteRenderer BackgroundSprite { get => backgroundSprite; set => backgroundSprite = value; }
    public SpriteRenderer CharacterSprite { get => characterSprite; set => characterSprite = value; }
    public bool ActiveState { get => activeState; set => activeState = value; }

    int xGridPos = 0;
    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private bool matchFound = false;
    private bool activeState = true;
    BoardController boardController;

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
        boardController.nullPanels.Add(this);
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
            boardController.movingPanels.Add(this);
            StartCoroutine(SortToBottom());
        }
    }

    private IEnumerator SortToTop()
    {
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
        int tempX, tempY;
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
            
            while (hitArray.Length > 1 && hit.collider.GetComponent<Panel>().Type == this.Type)
            {
                matchingPanels.Add(hit.collider.gameObject);
                hitArray = Physics2D.RaycastAll(hit.collider.transform.position, castDir);
                if(hitArray.Length < 2) { continue; }
                hit = hitArray[1];
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

    public bool CheckForMatch()
    {
        return false;
    }
}
