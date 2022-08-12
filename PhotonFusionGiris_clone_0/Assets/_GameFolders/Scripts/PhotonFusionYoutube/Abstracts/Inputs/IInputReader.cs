using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhotonFusionYoutube.Abstracts.Inputs
{
    public interface IInputReader
    {
        Vector3 DirectionInput { get; }
        Vector2 RotationInput { get; } 
        bool IsJumpInput { get; }
    }
}