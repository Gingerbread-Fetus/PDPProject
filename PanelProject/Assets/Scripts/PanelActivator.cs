using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelActivator : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Activator hit: ");
        Panel hitPanel = collision.GetComponent<Panel>();
        hitPanel.ActiveState = true;
    }
}
