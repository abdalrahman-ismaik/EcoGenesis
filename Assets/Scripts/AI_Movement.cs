using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movement : MonoBehaviour
{
    // Reference to the Animator component
    Animator animator;

    // Speed at which the AI character moves
    public float moveSpeed = 0.2f;

    // Stores the position where the AI stops
    Vector3 stopPosition;

    // Timers for walking and waiting
    float walkTime;
    public float walkCounter;
    float waitTime;
    public float waitCounter;

    // Randomly chosen direction: 0 = forward, 1 = right, 2 = left, 3 = backward
    int WalkDirection;

    // Whether the AI is currently walking
    public bool isWalking;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Randomize walk and wait times to prevent identical behavior across all AI
        walkTime = Random.Range(3, 6);
        waitTime = Random.Range(5, 7);

        waitCounter = waitTime;
        walkCounter = walkTime;

        // Begin by choosing a random direction
        ChooseDirection();
    }

    void Update()
    {
        if (isWalking)
        {
            // Set animation to running
            animator.SetBool("isRunning", true);

            // Decrease walk timer
            walkCounter -= Time.deltaTime;

            // Move in the chosen direction
            switch (WalkDirection)
            {
                case 0: // Forward
                    transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    break;
                case 1: // Right
                    transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                    break;
                case 2: // Left
                    transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                    break;
                case 3: // Backward
                    transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                    break;
            }

            // Move the character forward in the set direction
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            // Stop walking after the timer expires
            if (walkCounter <= 0)
            {
                stopPosition = transform.position;
                isWalking = false;
                transform.position = stopPosition; // Stop movement
                animator.SetBool("isRunning", false); // Switch to idle animation
                waitCounter = waitTime; // Start waiting
            }
        }
        else
        {
            // Decrease wait timer
            waitCounter -= Time.deltaTime;

            // Choose a new direction once done waiting
            if (waitCounter <= 0)
            {
                ChooseDirection();
            }
        }
    }

    // Randomly picks a direction for the AI to walk
    public void ChooseDirection()
    {
        WalkDirection = Random.Range(0, 4); // 0 to 3
        isWalking = true;
        walkCounter = walkTime;
    }
}
