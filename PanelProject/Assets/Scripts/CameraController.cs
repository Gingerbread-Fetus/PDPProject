using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public bool moving = true;
    public float moveSpeed = 1f;
    int objectsEnteringCollider = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 goal = new Vector3();
        if (moving)
        {
            goal = transform.position - new Vector3(0, 1, 0);
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, goal, step);
        }

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            moving = !moving;//TODO probably going to change this later, but thought it made a good case for testing.
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        objectsEnteringCollider += 1;
        BoardController bc = FindObjectOfType<BoardController>();
        bc.MoveToBottom(collision.gameObject.GetComponent<Panel>());
        if (objectsEnteringCollider == 6)
        {
            bc.LastRowPosition -= 1;
            bc.LastRowSpawned += 1;
            objectsEnteringCollider = 0;
        }
    }
}
