using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFactory {
    public static Car GetPizza(GameObject gameObject)
    {
        return new Car(gameObject, new Vector3(0.43f, 0.51f, 1.04f), new Vector3(-2.5f, 0.714f, 4.37f));
    }
}
