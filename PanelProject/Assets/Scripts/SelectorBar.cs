using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectorBar : MonoBehaviour
{
    private Vector3 offset = new Vector3(0.468f, 0, 0);
    BoardController boardController;

    private void Start()
    {
        boardController = FindObjectOfType<BoardController>();
    }

    public void SetSelected(Panel newPanel)
    {
        transform.position = newPanel.transform.position + offset;
    }

    public void MoveSelector(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            var value = ctx.ReadValue<Vector2>();
            RaycastHit2D[] hitArray = Physics2D.RaycastAll(boardController.SelectedPanel.transform.position, value);
            if(hitArray.Length > 1)
            {
                Panel nextPanel = hitArray[1].collider.GetComponent<Panel>();
                if (nextPanel.ActiveState && nextPanel.XGridPos <= 4)
                {
                    SetSelected(nextPanel); 
                }
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Panel"))
        {
            Panel hitPanel = collision.GetComponent<Panel>();
            boardController.SelectedPanel = hitPanel;
        }

        if (collision.CompareTag("Game Over"))
        {
            RaycastHit2D[] hitArray = Physics2D.RaycastAll(boardController.SelectedPanel.transform.position, Vector2.down);
            Panel panelBelow = hitArray[1].collider.GetComponent<Panel>();
            SetSelected(panelBelow);
        }
    }

}
