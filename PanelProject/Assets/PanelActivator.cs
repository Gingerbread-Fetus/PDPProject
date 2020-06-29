using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelActivator : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Activator hit: ");
        //collision.GetComponent<Panel>().IsActive = true;
    }
}
