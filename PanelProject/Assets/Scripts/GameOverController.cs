using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    [SerializeField] GameOverPanel gameOverCanvas;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Panel>().Type != Panel.PanelType.Null)
        {
            Debug.Log("Call game over");
            gameOverCanvas.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
