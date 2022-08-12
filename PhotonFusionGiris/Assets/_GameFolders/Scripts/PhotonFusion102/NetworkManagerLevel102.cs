using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level102
{
    public class NetworkManagerLevel102 : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] Player102 _player102;
        [SerializeField] NetworkRunner _networkRunnerPrefab;
        [SerializeField] NetworkSceneManagerDefault _loader;

        Dictionary<PlayerRef, Player102> _players;
        Player102 _localPlayer;
        NetworkRunner _networkRunner;

        public static NetworkManagerLevel102 Instance { get; private set; }

        void Awake()
        {
            SingletonThisObject();

            GetReference();
            _players = new Dictionary<PlayerRef, Player102>();
        }

        void OnValidate()
        {
            GetReference();
        }

        void Start()
        {
            StartQuickGame();
        }

        public void SetLocalPlayer(Player102 localPlayer)
        {
            _localPlayer = localPlayer;
        }

        private void Connect()
        {
            if (_networkRunnerPrefab == null) return;

            _networkRunner = Instantiate(_networkRunnerPrefab, this.transform);
            _networkRunner.AddCallbacks(this);
        }

        private void Disconnect()
        {
            if (_networkRunner == null) return;

            _networkRunner.Shutdown();
        }

        private void StartQuickGame()
        {
            Connect();

            _networkRunner.ProvideInput = true;

            _networkRunner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.AutoHostOrClient,
                SessionName = "Level102",
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = _loader,
                PlayerCount = 2
            });
        }

        private void GetReference()
        {
            if (_loader == null)
            {
                _loader = GetComponent<NetworkSceneManagerDefault>();
            }
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log(nameof(OnConnectedToServer));
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.Log(nameof(OnDisconnectedFromServer));
            Disconnect();
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                var spawnPlayer = runner.Spawn(_player102, Vector3.zero, Quaternion.identity, player);
                _players.Add(player, spawnPlayer);
                Debug.Log(_players.Count);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (_players.TryGetValue(player, out Player102 leftPlayer))
            {
                runner.Despawn(leftPlayer.Object);
                _players.Remove(player);
                Debug.Log(_players.Count);
            }
        }

        void SingletonThisObject()
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

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (_localPlayer == null) return;

            InputDataEntity inputDataEntity = new InputDataEntity();
            Vector2 moveDirection = _localPlayer.InputReader.Direction;
            inputDataEntity.PositionX = moveDirection.x;
            inputDataEntity.PositionZ = moveDirection.y;

            input.Set(inputDataEntity);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log(nameof(OnShutdown));
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
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