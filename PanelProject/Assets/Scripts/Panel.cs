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
    public int YGridPos { get => yGridPos; set => yGridPos = value; }

    int xGridPos, yGridPos = 0;
    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private bool matchFound = false;

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
        tempy = yGridPos;

        xGridPos = otherPanel.XGridPos;
        yGridPos = otherPanel.YGridPos;

        otherPanel.XGridPos = tempx;
        otherPanel.YGridPos = tempy;

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
            Debug.Log(
                "Raycast from: " + transform.position +
                " Called by: " + this.gameObject.name +
                " Towards: " + castDir +
                " Panel type: " + Enum.GetName(typeof(PanelType), type) +
                " Collider hit: " + hit.collider.name +
                " Hit it's own collider?: " + hit.collider.Equals(this.GetComponent<BoxCollider2D>())
            );//Checking if the objects are hitting their own colliders
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
}
