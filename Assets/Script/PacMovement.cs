using UnityEngine;

public class PacmanMovement : MonoBehaviour
{
    public float speed = 5f; // Speed of Pacman
    private Vector2 direction = Vector2.zero; // Current direction of movement
    private Animator animator; // Animator for Pacman

    void Start()
    {
        // Get the Animator component attached to Pacman
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Check for player input and set the direction accordingly
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            direction = Vector2.up;
            animator.Play("PacUp"); // Play up animation
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            direction = Vector2.down;
            animator.Play("PacDown"); // Play down animation
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            direction = Vector2.left;
            animator.Play("PacLeft"); // Play left animation
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            direction = Vector2.right;
            animator.Play("PacRight"); // Play right animation
        }

        // Move Pacman in the set direction
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
