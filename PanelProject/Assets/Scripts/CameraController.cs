using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public bool moving = true;
    public float moveSpeed = 1f;
    int objectsEnteringCollider = 0;
    BoardController board;
    [SerializeField] float speedIncreaseFactor = 2f;
    [SerializeField] float speedDifficultyFactor = 1f;
    [SerializeField] float startingBoostSpeed = 5f;
    [SerializeField] float startingBoostTime = 5f;
    float timeLeft = 0f;

    public float TimeLeft { get => timeLeft; set => timeLeft = value; }

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardController>();
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
    }

    public void MoveFaster(InputAction.CallbackContext ctx)
    {
        Debug.Log("Move Faster");
        if (ctx.started)
        {
            moveSpeed *= speedIncreaseFactor;
        }
        else if (ctx.canceled)
        {
            moveSpeed /= speedIncreaseFactor;
        }
    }

    public IEnumerator PauseCamera(float pauseTime)
    {
        if (moving)
        {
            moving = false;
            timeLeft = pauseTime;
            while(timeLeft > 0)
            {
                yield return new WaitForSeconds(0.1f);
                timeLeft -= 0.1f;
            }
            moving = true;
        }
        else
        {
            timeLeft += pauseTime;
            yield break;
        }
        if(timeLeft < 0) { timeLeft = 0; }
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

    private IEnumerator MoveToStart()
    {
        var temp = moveSpeed;
        moveSpeed = startingBoostSpeed;
        yield return new WaitForSeconds(startingBoostTime);
        moveSpeed = temp;
    }

    public void IncreaseDifficulty()
    {
        moveSpeed += speedDifficultyFactor;
    }
}
