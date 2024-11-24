using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraState
{
    public void UpdateMovement(CameraMovement cameraMovement);
}
