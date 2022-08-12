using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level103
{
    public class NetworkManagerLevel103 : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] NetworkRunner _networkRunnerPrefab;
        [SerializeField] PlayerLevel103 _playerPrefab;
        [SerializeField] NetworkSceneManagerDefault _networkSceneManager;

        Dictionary<PlayerRef,PlayerLevel103> _players;
        NetworkRunner _networkRunner;
        PlayerLevel103 _localPlayer;

        public static NetworkManagerLevel103 Instance { get; private set; }

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
            
            _players = new Dictionary<PlayerRef, PlayerLevel103>();
        }

        void Start()
        {
            StartQuick();
        }

        void StartQuick()
        {
            Connect();

            _networkRunner.ProvideInput = true;

            _networkRunner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.AutoHostOrClient,
                SessionName = "Level103",
                PlayerCount = 2,
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = _networkSceneManager
            });

            Debug.Log($"{nameof(StartQuick)} done");
        }

        void Connect()
        {
            if (_networkRunnerPrefab == null) return;

            _networkRunner = Instantiate(_networkRunnerPrefab, Vector3.zero, Quaternion.identity);
            _networkRunner.AddCallbacks(this);
            Debug.Log("Connect method done");
        }

        void Disconnect()
        {
            if (_networkRunner == null) return;

            _networkRunner.Shutdown();
            Debug.Log("Disconnect method done");
        }

        public void SetLocalPlayer(PlayerLevel103 localPlayer)
        {
            _localPlayer = localPlayer;
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                var spawnedPlayer = runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, player);
                _players.Add(player,spawnedPlayer);
                Debug.Log(_players.Count);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (_players.ContainsKey(player))
            {
                var leftPlayer = _players[player];
                runner.Despawn(leftPlayer.Object);
                _players.Remove(player);
                Debug.Log($"{nameof(OnPlayerLeft)} {player.PlayerId}");
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (_localPlayer == null) return;
            
            InputDataEntity inputDataEntity = new InputDataEntity();
            inputDataEntity.MoveDirection = _localPlayer.InputReader.MoveDirection;
            inputDataEntity.RotationDirection = _localPlayer.InputReader.MousePosition;
            inputDataEntity.Fire = _localPlayer.InputReader.Jump;
            
            input.Set(inputDataEntity);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) =>
            Debug.Log($"{nameof(OnShutdown)} method done");

        public void OnConnectedToServer(NetworkRunner runner) =>
            Debug.Log($"{nameof(OnConnectedToServer)} method done");

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Disconnect();
            Debug.Log($"{nameof(OnDisconnectedFromServer)} method done");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Disconnect();
            Debug.Log($"{nameof(OnConnectFailed)} method done");
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }
    }
}