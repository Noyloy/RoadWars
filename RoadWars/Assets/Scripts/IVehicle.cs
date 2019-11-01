using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVehicle
{
    IEnumerator Turn(Dir direction);
    IEnumerator SwitchLane(Dir dir);
    void Accelerate();
    void Start();
    void Stop();
}
