using Fusion;
using PhotonFusionYoutube.Abstracts.Inputs;
using PhotonFusionYoutube.Inputs;
using UnityEngine;

namespace PhotonFusionYoutube.Networks
{
    public class FusionNetworkPlayer : NetworkBehaviour,IPlayerLeft
    {
        [SerializeField] NetworkCharacterControllerPrototype networkCharacterControllerPrototype1;
        [SerializeField] Camera _camera;

        public IInputReader InputReader { get; set; }
        public bool IsPlayerHasAuthorize => Object.HasInputAuthority;

        void Awake()
        {
            InputReader = new InputReader();
        }

        public override void Spawned()
        {
            if (IsPlayerHasAuthorize)
            {
                Debug.Log("Local Player => " + Object.Name);
                FusionNetworkManager.Instance?.SetLocalPlayer(this);
            }
            else
            {
                Debug.Log("Remote Player");
                Destroy(_camera.gameObject);
            }
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (IsPlayerHasAuthorize)
            {
                Destroy(this.gameObject);    
            }
        }
        
        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData networkInputData))
            {
                Vector3 moveDirection = transform.forward * networkInputData.MoveDirection.z +
                                        transform.right * networkInputData.MoveDirection.x;
                moveDirection.Normalize();
                
                networkCharacterControllerPrototype1.Move(moveDirection);
            }
        }
    }
    
    public struct NetworkInputData : INetworkInput
    {
        public Vector3 MoveDirection;
        public float RotationX;
        public NetworkBool IsJump;
    }
}