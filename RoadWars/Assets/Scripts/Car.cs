using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : IVehicle {

    public float Speed { get; private set; }

    // Lane Logic: -1 (Left), 0 (Center), 1 (Right)
    public int CurrentLane { get; private set; } = 0;

    // Config - Public for Inspector tuning
    public float LaneWidth = 2.0f;
    public float BaseTurnRadius = 5.5f;
    public float LaneSwitchDuration = 0.5f;

    public bool IsTurning { get; private set; }
    public bool IsSwitchingLane { get; private set; }

    public GameObject GameObject;
    private Rigidbody carRigidbody;
    private Transform carTransform;
    private Animator carAnimator;

    // Keep track of active coroutines to stop them if needed
    private Coroutine switchLaneCoroutine;

    public Car (GameObject go, Vector3? size = null, Vector3? position = null, float speed = 2f, float acceleration = 1f)
    {
        GameObject = go;
        this.Speed = speed;
        // Assume we start at Center Lane (0).

        setCarGameObject();
        if (size.HasValue)
        {
            carTransform.localScale = size.Value;
        }
        if (position.HasValue)
        {
            carTransform.position = position.Value;
        }
    }

    private void setCarGameObject()
    {
        carTransform = GameObject.GetComponent<Transform>();
        carRigidbody = GameObject.GetComponent<Rigidbody>();
        carAnimator = GameObject.GetComponent<Animator>();

        if (carRigidbody == null)
            carRigidbody = GameObject.AddComponent<Rigidbody>();

        if (GameObject.GetComponent<Collider>() == null)
            GameObject.AddComponent<BoxCollider>();

        carRigidbody.useGravity = true;
        carRigidbody.isKinematic = false;
        carRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void Start()
    {
        UpdateVelocity();
    }

    public void Update()
    {
        // If we are relying on Physics for straight movement, we update velocity.
        // If we are Kinematic (Turning/Switching), we don't need to update velocity,
        // the Coroutine handles movement.
        if (!IsTurning && !IsSwitchingLane)
        {
             if (carRigidbody.isKinematic) carRigidbody.isKinematic = false;
             UpdateVelocity();
        }
    }

    private void UpdateVelocity()
    {
        if (carRigidbody != null && !carRigidbody.isKinematic)
        {
            Vector3 forward = carTransform.forward;
            forward.y = 0;
            if (forward.sqrMagnitude > 0.001f) forward.Normalize();

            Vector3 velocity = forward * Speed;
            velocity.y = carRigidbody.velocity.y;
            carRigidbody.velocity = velocity;
        }
    }

    public void Accelerate()
    {
        // Placeholder
    }

    public void Stop()
    {
        carRigidbody.velocity = Vector3.zero;
    }

    public IEnumerator Turn(Dir direction)
    {
        if (IsTurning) yield break;

        // Interrupt Lane Switch if active
        if (IsSwitchingLane)
        {
            IsSwitchingLane = false; // This flag breaks the SwitchLane loop
            // We don't explicitly stop the coroutine because the flag check in SwitchLane handles it.
            // But to be safe, we reset kinematic immediately.
        }

        IsTurning = true;
        carRigidbody.velocity = Vector3.zero;
        carRigidbody.isKinematic = true; // Disable physics interference

        // Calculate Radius based on Lane
        float sign = (direction == Dir.Left) ? 1f : -1f;
        float radius = BaseTurnRadius + (sign * CurrentLane * LaneWidth);

        // Pivot Point
        Vector3 pivotDir = (direction == Dir.Right) ? carTransform.right : -carTransform.right;
        Vector3 pivotPoint = carTransform.position + (pivotDir * radius);

        // Calculate Arc Length and Duration
        float arcLength = (Mathf.PI * radius) / 2f;
        float duration = arcLength / Speed;

        float totalAngle = (direction == Dir.Right) ? 90f : -90f;
        float angleTraversed = 0f;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float dt = Time.deltaTime;
            float stepAngle = (totalAngle / duration) * dt;

            // Correction for overshooting
            if (Mathf.Abs(angleTraversed + stepAngle) > 90f)
            {
                stepAngle = (totalAngle > 0 ? 90f : -90f) - angleTraversed;
            }

            carTransform.RotateAround(pivotPoint, Vector3.up, stepAngle);
            angleTraversed += stepAngle;
            timeElapsed += dt;

            yield return null;
        }

        IsTurning = false;

        // Re-enable physics for straight road
        carRigidbody.isKinematic = false;
        UpdateVelocity();
    }

    public IEnumerator SwitchLane(Dir direction)
    {
        // direction must be Left or Right
        if (direction != Dir.Left && direction != Dir.Right) yield break;

        int laneChange = (direction == Dir.Right) ? 1 : -1;
        int nextLane = CurrentLane + laneChange;

        // Check bounds
        if (nextLane < -1 || nextLane > 1) yield break;

        if (IsSwitchingLane || IsTurning) yield break;
        IsSwitchingLane = true;

        // We use Kinematic for lane switch to ensure exact positioning
        // Although usually we want forward physics to continue.
        // If we make it kinematic, we must MANUALLY move forward too.
        // It's easier to keep Physics for Forward, and use Transform for Sideways.
        // BUT: Rigidbody physics might fight the Transform.Translate if not Kinematic?
        // Actually, Translate works on Transform. Physics updates next frame.
        // If we don't set velocity.x/z to 0, it might drift.
        // Let's TRY leaving it non-kinematic (Physics handles Forward), and we just nudge sideways?
        // Issue: Friction.
        // SAFEST: Go Kinematic and handle BOTH Forward and Sideways movement manually.

        carRigidbody.isKinematic = true;

        float movedDist = 0f;
        float totalDist = LaneWidth;
        float sideSpeed = totalDist / LaneSwitchDuration;

        Vector3 moveDir = (direction == Dir.Right) ? carTransform.right : -carTransform.right;

        while (movedDist < totalDist)
        {
            if (!IsSwitchingLane) break; // Interrupted by Turn

            float dt = Time.deltaTime;
            float step = sideSpeed * dt;
            if (movedDist + step > totalDist) step = totalDist - movedDist;

            // Move Sideways
            carTransform.Translate(moveDir * step, Space.World);

            // Move Forward (simulate speed)
            carTransform.Translate(carTransform.forward * Speed * dt, Space.World);

            movedDist += step;
            yield return null;
        }

        if (IsSwitchingLane) // Only commit lane change if not interrupted
        {
            CurrentLane = nextLane;
            IsSwitchingLane = false;

            // Restore Physics
            carRigidbody.isKinematic = false;
            UpdateVelocity();
        }
    }
}
