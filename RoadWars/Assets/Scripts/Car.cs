using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : IVehicle {

    public float Speed { get { return speed; } }
    private float speed;

    public Vector3 Size { get { return size; } }
    private Vector3 size;

    public Vector3 Center { get { return size/2; } }

    public Vector3 Position { get { return position; } }
    private Vector3 position;

    public float Acceleration { get { return acceleration; } }
    private float acceleration;

    public bool IsTurning { get; private set; }
    private Lane carLane = Lane.Center;

    public GameObject GameObject;

    private BoxCollider carBounds;
    private Transform carTransform;
    private Animator carAnimator;
    private Rigidbody carRigidbody;

    public Car (GameObject go, Vector3? size = null, Vector3? position = null, float speed = 2f, float acceleration = 1f)
    {
        GameObject = go;
        this.speed = speed;
        this.acceleration = acceleration;
        this.size = size ?? Vector3.one;
        this.position = position ?? Vector3.zero;

        setCarGameObject();
    }

    private T getComponent<T>() where T : Component
    {
        T t = GameObject.GetComponent<T>();
        if (t == null)
            t = GameObject.AddComponent<T>();
        return t;
    }


    private void setCarGameObject()
    {
        carBounds = getComponent<BoxCollider>();
        carTransform = getComponent<Transform>();
        carAnimator = getComponent<Animator>();
        carRigidbody = getComponent<Rigidbody>();

        carBounds.size = Size;
        carBounds.center = Center;

        carTransform.position = Position;
    }

    public void Accelerate()
    {
        throw new System.NotImplementedException();
    }

    public void Start()
    {
        carRigidbody.velocity = carTransform.forward * speed;
    }

    public void Stop()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator Turn(Dir direction)
    {
        if (direction == Dir.Down || direction == Dir.Up || IsTurning) yield break;

        IsTurning = true;
        int degree = (direction == Dir.Left) ? -90 : 90;

        Quaternion fromAngle = carTransform.rotation;
        Quaternion toAngle = Quaternion.Euler(carTransform.eulerAngles + Vector3.up * degree);

        float turnTime = (Mathf.PI * 1.5f) / (2 * carRigidbody.velocity.magnitude);

        for (float t = 0; t <= 1.15f; t += Time.fixedDeltaTime / turnTime)
        {
            carTransform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
            yield return null;
        }
        IsTurning = false;
    }

    public IEnumerator SwitchLane(Dir dir)
    {
        throw new System.NotImplementedException();
    }
}
