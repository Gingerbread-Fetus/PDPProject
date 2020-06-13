using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
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
        tempy = yGridPos;

        xGridPos = otherPanel.XGridPos;
        yGridPos = otherPanel.YGridPos;

        otherPanel.XGridPos = tempx;
        otherPanel.YGridPos = tempy;

        transform.position = otherPanel.transform.position;
        otherPanel.transform.position = tmpPosition;
    }

    void OnMouseDown()
    {
        Debug.Log("Panel clicked: " + gameObject.name);
    }

}
