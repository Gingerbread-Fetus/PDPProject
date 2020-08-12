using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectorBar : MonoBehaviour
{
    Panel selectedPanel;
    private Vector3 offset = new Vector3(0.468f, 0, 0);
    BoardController boardController;

    private void Start()
    {
        boardController = FindObjectOfType<BoardController>();
    }

    public void SetSelected(Panel newPanel)
    {
        selectedPanel = newPanel;
        transform.position = newPanel.transform.position + offset;
        selectedPanel.SelectPanel();
        boardController.SelectedPanel = selectedPanel;
    }

    public void MoveSelector(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<Vector2>();
        if (ctx.performed)
        {
            RaycastHit2D[] hitArray = Physics2D.RaycastAll(selectedPanel.transform.position, value);
            if (hitArray.Length > 1)
            {
                RaycastHit2D hit = hitArray[1];
                Panel hitPanel = hit.collider.GetComponent<Panel>();
                if (hitPanel.ActiveState && hitPanel.XGridPos <= 4)
                {
                    selectedPanel.DeselectPanel();
                    SetSelected(hitPanel);
                }
            } 
        }

    }

    public void MoveDown()
    {
        RaycastHit2D[] hitArray = Physics2D.RaycastAll(selectedPanel.transform.position, Vector2.down);
        if (hitArray.Length > 1)
        {
            RaycastHit2D hit = hitArray[1];
            Panel hitPanel = hit.collider.GetComponent<Panel>();
            if (hitPanel.ActiveState && hitPanel.XGridPos <= 4)
            {
                selectedPanel.DeselectPanel();
                SetSelected(hitPanel);
            }
        }
    }
}
