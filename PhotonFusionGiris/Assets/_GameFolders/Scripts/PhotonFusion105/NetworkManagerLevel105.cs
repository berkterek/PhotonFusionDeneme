using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level105
{
    public class NetworkManagerLevel105 : MonoBehaviour
    {
        [SerializeField] NetworkRunner _networkRunnerPrefab;
        [SerializeField] PlayerLevel105 _playerPrefab;
        [SerializeField] NetworkSceneManagerDefault _networkSceneManager;

        Dictionary<PlayerRef, PlayerLevel105> _players;

        public INetworkCallBackHelper NetworkRunnerHelper { get; private set; }

        public static NetworkManagerLevel105 Instance { get; private set; }

        void Awake()
        {
            SingletonThisObject();
            GetReference();
            NetworkRunnerHelper = new NetworkRunnerHelper();
            _players = new Dictionary<PlayerRef, PlayerLevel105>();
        }

        void Start()
        {
            var runner = Instantiate(_networkRunnerPrefab, this.transform);

            runner.ProvideInput = true;

            runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.AutoHostOrClient,
                SessionName = "Level105",
                PlayerCount = 2,
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = _networkSceneManager
            });

            runner.AddCallbacks(NetworkRunnerHelper);
        }

        void OnEnable()
        {
            NetworkRunnerHelper.OnPlayerJoinedGame += HandleOnPlayerJoinGame;
            NetworkRunnerHelper.OnPlayerLeftGame += HandleOnPlayerLeftGame;
        }

        void OnDisable()
        {
            NetworkRunnerHelper.OnPlayerJoinedGame -= HandleOnPlayerJoinGame;
            NetworkRunnerHelper.OnPlayerLeftGame -= HandleOnPlayerLeftGame;
        }

        void OnValidate()
        {
            GetReference();
        }

        void HandleOnPlayerJoinGame(NetworkRunner runner, PlayerRef playerRef)
        {
            var joinedPlayer = runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, playerRef);
            _players.Add(playerRef, joinedPlayer);
            Debug.Log(_players.Count);
        }

        void HandleOnPlayerLeftGame(NetworkRunner runner, PlayerRef playerRef)
        {
            if (_players.ContainsKey(playerRef))
            {
                var leftPlayer = _players[playerRef];
                runner.Despawn(leftPlayer.Object);
                _players.Remove(playerRef);
                Debug.Log(_players.Count);
            }
        }

        private void GetReference()
        {
            if (_networkSceneManager == null)
            {
                _networkSceneManager = GetComponentInChildren<NetworkSceneManagerDefault>();
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
    }

    public class NetworkRunnerHelper : INetworkCallBackHelper
    {
        PlayerLevel105 _localPlayer;

        public event System.Action<NetworkRunner, PlayerRef> OnPlayerJoinedGame;
        public event System.Action<NetworkRunner, PlayerRef> OnPlayerLeftGame;

        public void SetLocalPlayer(PlayerLevel105 localPlayer)
        {
            _localPlayer = localPlayer;
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                OnPlayerJoinedGame?.Invoke(runner, player);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            OnPlayerLeftGame?.Invoke(runner, player);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (_localPlayer == null) return;

            InputDataEntity105 data = new InputDataEntity105();

            var inputReader = _localPlayer.InputReader;
            data.MoveDirection = inputReader.MoveDirection;
            data.IsFire = inputReader.Fire;

            input.Set(data);
        }

        public void OnConnectedToServer(NetworkRunner runner) => Debug.Log(nameof(OnConnectedToServer));

        public void OnDisconnectedFromServer(NetworkRunner runner) => Debug.Log(nameof(OnDisconnectedFromServer));

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) =>
            Debug.Log($"{nameof(OnShutdown)} {shutdownReason}");

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

        public void OnSceneLoadDone(NetworkRunner runner) => Debug.Log(nameof(OnSceneLoadDone));

        public void OnSceneLoadStart(NetworkRunner runner) => Debug.Log(nameof(OnSceneLoadStart));
    }

    public interface INetworkCallBackHelper : INetworkRunnerCallbacks
    {
        void SetLocalPlayer(PlayerLevel105 localPlayer);
        event System.Action<NetworkRunner, PlayerRef> OnPlayerJoinedGame;
        event System.Action<NetworkRunner, PlayerRef> OnPlayerLeftGame;
    }
}