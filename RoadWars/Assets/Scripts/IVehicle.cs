using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVehicle
{
    void Turn(Dir direction);
    void Accelerate();
    void Start();
    void Stop();
}
