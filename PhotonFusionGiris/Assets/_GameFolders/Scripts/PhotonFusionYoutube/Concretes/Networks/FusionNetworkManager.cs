using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace PhotonFusionYoutube.Networks
{
    public class FusionNetworkManager : MonoBehaviour,INetworkRunnerCallbacks
    {
        [SerializeField] FusionNetworkPlayer _playerPrefab;
        [SerializeField] Transform[] _spawnTransforms;

        int _spawnIndexCount = 0;

        public static FusionNetworkManager Instance { get; private set; }
        FusionNetworkPlayer _localPlayer; 

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void SetLocalPlayer(FusionNetworkPlayer localPlayer)
        {
            if (localPlayer.IsPlayerHasAuthorize)
            {
                _localPlayer = localPlayer;
            }
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                Debug.Log(nameof(OnPlayerJoined));
                runner.Spawn(_playerPrefab, _spawnTransforms[_spawnIndexCount].position, Quaternion.identity, player);
            }

            _spawnIndexCount++;

            if (_spawnIndexCount >= _spawnTransforms.Length)
            {
                _spawnIndexCount = 0;
            }
        }
        
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (_localPlayer == null) return;

            NetworkInputData inputData = new NetworkInputData();
            inputData.MoveDirection = _localPlayer.InputReader.DirectionInput;
            inputData.RotationX = _localPlayer.InputReader.RotationInput.y;
            inputData.IsJump = _localPlayer.InputReader.IsJumpInput;
            
            input.Set(inputData);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log(nameof(OnPlayerLeft));
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            Debug.Log(nameof(OnInputMissing));
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log($"On shut down triggered reason => <color=red>{shutdownReason}</color>");
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("<color=green>Connected</color> to the photon server");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.Log("<color=red>Disconnected</color> to the photon server");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Debug.Log(nameof(OnConnectRequest));
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.Log(nameof(OnConnectFailed));
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            Debug.Log(nameof(OnUserSimulationMessage));
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Debug.Log(nameof(OnSessionListUpdated));
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            Debug.Log(nameof(OnCustomAuthenticationResponse));
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log(nameof(OnHostMigration));
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data)
        {
            Debug.Log(nameof(OnReliableDataReceived));
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            Debug.Log(nameof(OnSceneLoadDone));
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log(nameof(OnSceneLoadStart));
        }
    }    
}