using Fusion;
using UnityEngine;

namespace Loops
{
    public class PlayerLoop : NetworkBehaviour, ISpawned
    {
        [SerializeField] PlayerCharacterController _playerCharacterPrefab;
        [Networked] public PlayerType PlayerType { get; private set; }
        [Networked] public NetworkString<_32> Name { get; private set; }
        [Networked] public Color Color { get; private set; }
        [Networked] public NetworkBool Ready { get; private set; }
        [Networked] public PlayerCharacterController Avatar { get; private set; }

        public event System.Action<NetworkBool> OnIsReadyValueChanged;

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                NetworkManagerLoop.Instance.SetLocalPlayer(this);
            }

            NetworkManagerLoop.Instance.SetPlayer(Object.InputAuthority, this);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (Object.HasInputAuthority)
            {
                NetworkManagerLoop.Instance.RemoveOnPlayerLeft(this);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_SetPlayerType(PlayerType playerType)
        {
            PlayerType = playerType;
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_SetIsReady(NetworkBool ready)
        {
            Ready = ready;
            OnIsReadyValueChanged?.Invoke(Ready);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_SetName(NetworkString<_32> name)
        {
            Name = name;
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_SetColor(Color color)
        {
            Color = color;
        }

        public void SpawnAvatarCharacter()
        {
            var avatar = Runner.Spawn(_playerCharacterPrefab, SpawnPointController.RandomPosition, Quaternion.identity,
                this.Object.InputAuthority);
            Avatar = avatar;
        }
    }

    public enum PlayerType
    {
        PlayerNotJoin,
        PlayerOne,
        PlayerTwo
    }
}