using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelActivator : MonoBehaviour
{
    private int objectsEnteringCollider = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Selector")) { return; }

        if (!collision.GetComponent<Panel>().ActiveState)
        {
            Panel hitPanel = collision.GetComponent<Panel>();
            hitPanel.ActiveState = true;
            objectsEnteringCollider += 1;
        }
        BoardController bc = FindObjectOfType<BoardController>();
        if (objectsEnteringCollider == 6)
        {
            bc.StartCoroutine(bc.CheckAllPanels());
            objectsEnteringCollider = 0;
        }
    }
}
