using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFactory {
    public static Vector3 BASE_CAR_POS = new Vector3(-2.5f, 0.78f, 4.37f);
    public static Vector3 BASE_CAR_SCALE = Vector3.one;

    public static Car GetPizza(GameObject gameObject)
    {
        return new Car(gameObject, BASE_CAR_SCALE, BASE_CAR_POS);
    }
}
