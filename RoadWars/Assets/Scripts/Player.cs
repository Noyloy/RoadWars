using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Dir? pendingTurn = null; // Intent for the turn
    private const float TURN_LOOKAHEAD_TIME = 1.5f; // How far ahead to look for a turn (in seconds)

    private Car pCar;
    private TouchSwipeManager swipeManager = new TouchSwipeManager();

    void Start()
    {
        Debug.Log("starting player script");
        pCar = CarFactory.GetPizza(gameObject);
        pCar.Start();
    }
	
	void Update () {
        // Continuous update for car physics/movement
        pCar.Update();

        // 1. Detect Input
        Dir? input = swipeManager.DetectSwipe();

        // Keyboard fallback
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) input = Dir.Left;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) input = Dir.Right;

        if (input.HasValue)
        {
            HandleInput(input.Value);
        }
    }

    private void HandleInput(Dir dir)
    {
        if (IsApproachingTurn(out Collider turnTrigger))
        {
            // We are approaching a turn. Interpret this input as a Turn intent.
            // We do NOT switch lanes.
            Debug.Log("Approaching Turn: Storing Pending Turn " + dir);
            pendingTurn = dir;
        }
        else
        {
            // We are on a straight road (or far from turn). Interpret as Lane Switch.
            Debug.Log("Straight Road: Switching Lane " + dir);
            StartCoroutine(pCar.SwitchLane(dir));
        }
    }

    private bool IsApproachingTurn(out Collider hitCollider)
    {
        hitCollider = null;
        float lookDist = pCar.Speed * TURN_LOOKAHEAD_TIME;

        // Raycast forward from car position
        Ray ray = new Ray(pCar.GameObject.transform.position, pCar.GameObject.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, lookDist);

        foreach (var hit in hits)
        {
            // Check for Triggers that are likely Turn Triggers
            // Logic: Is Trigger, Not Respawn, Not Player, Not Car Root
            if (hit.collider.isTrigger &&
                !hit.collider.CompareTag("Respawn") &&
                hit.collider.gameObject != gameObject &&
                hit.collider.transform.root != transform.root)
            {
                hitCollider = hit.collider;
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Respawn")){
            return;
        }

        // It is a Turn Trigger.
        if (pendingTurn.HasValue)
        {
            // Execute the turn
            StartCoroutine(pCar.Turn(pendingTurn.Value));
            pendingTurn = null;
        }
    }
}
