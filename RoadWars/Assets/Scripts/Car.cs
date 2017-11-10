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
    
    public GameObject GameObject;
    private BoxCollider carBounds;
    private Transform carTransform;

    public Car (GameObject go, Vector3? size, Vector3? position, float speed = 2f, float acceleration = 1f)
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

        carBounds.size = Size;
        carBounds.center = Center;

        carTransform.position = Position;
        //carTransform.localScale = Size;
    }

    public void Accelerate()
    {
        throw new System.NotImplementedException();
    }

    public void Start()
    {
        throw new System.NotImplementedException();
    }

    public void Stop()
    {
        throw new System.NotImplementedException();
    }

    public void Turn(Dir direction)
    {
        throw new System.NotImplementedException();
    }
}
