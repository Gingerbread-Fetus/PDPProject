using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Game Over Hit");
        if(collision.gameObject.GetComponent<Panel>().Type != Panel.PanelType.Null)
        {
            Debug.Log("Call game over");
        }
    }
}
