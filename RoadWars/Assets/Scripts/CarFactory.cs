using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFactory {
    public static Car GetPizza(GameObject gameObject)
    {
        return new Car(gameObject);
    }
}
