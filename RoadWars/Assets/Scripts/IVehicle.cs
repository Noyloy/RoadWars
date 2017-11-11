using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVehicle
{
    IEnumerator Turn(Dir direction);
    IEnumerator SwitchLane(Lane lane);
    void Accelerate();
    void Start();
    void Stop();
}
