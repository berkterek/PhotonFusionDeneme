using System;
using Fusion;
using UnityEngine;

namespace Level103
{
    public class PlayerLevel103 : NetworkBehaviour,ISpawned
    {
        [SerializeField] BallLevel103 _ballPrefab;
        
        MoveWithTranslate _move;
        RotationWithMousePosition _rotation;
        Transform _transform;

        [Networked] TickTimer FireDelay { get; set; }
        public InputReader103 InputReader { get; private set; }
        
        void Awake()
        {
            _transform = transform;
            InputReader = new InputReader103();
            _move = new MoveWithTranslate(_transform);
            _rotation = new RotationWithMousePosition(_transform);
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out InputDataEntity inputEntity))
            {
                _move.FixedTick(inputEntity.MoveDirection * Runner.DeltaTime);
                _rotation.FixedTick(inputEntity.RotationDirection);

                if (FireDelay.ExpiredOrNotRunning(Runner))
                {
                    if (inputEntity.Fire)
                    {
                        FireDelay = TickTimer.CreateFromSeconds(Runner,1f);
                        Runner.Spawn(_ballPrefab, _transform.position + _transform.forward, _transform.rotation,
                            Object.InputAuthority);
                    }    
                }
            }
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                NetworkManagerLevel103.Instance.SetLocalPlayer(this);
            }
        }
    }
}