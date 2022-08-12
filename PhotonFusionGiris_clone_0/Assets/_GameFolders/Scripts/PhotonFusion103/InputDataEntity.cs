using Fusion;
using UnityEngine;

namespace Level103
{
    public struct InputDataEntity : INetworkInput
    {
        public Vector2 MoveDirection { get; set; }
        public Vector2 RotationDirection { get; set; }
        public bool Fire { get; set; }
    }
}