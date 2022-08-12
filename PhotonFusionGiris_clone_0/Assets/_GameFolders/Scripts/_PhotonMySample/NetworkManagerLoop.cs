using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Loops
{
    public class NetworkManagerLoop : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] PlayerLoop _localPlayer;
        [SerializeField] PlayerLoop _playerLoopPrefab;
        [SerializeField] NetworkRunner _networkRunnerPrefab;
        [SerializeField] NetworkSceneManagerDefault _sceneManagerLoop;
        [SerializeField] string _lobbyId = "Coop";
        
        NetworkRunner _networkRunner;

        readonly Dictionary<PlayerRef, PlayerLoop> _players = new Dictionary<PlayerRef, PlayerLoop>();

        public static NetworkManagerLoop Instance { get; private set; }

        public event System.Action OnPlayerLeftRoom;
        public event System.Action<PlayerLoop> OnPlayerEnterRoom;

        public PlayerLoop LocalPlayer => _localPlayer;
        public bool AreAllPlayerReady => _players.Values.All(x => x.Ready);
        public int PlayerCount => _players.Values.Count;
        public IReadOnlyCollection<PlayerLoop> Players => _players.Values.ToList();

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

        public void EnterLobby()
        {
            EnterLobbyAsync();
        }

        private async void EnterLobbyAsync()
        {
            Connect();

            var result = await _networkRunner.JoinSessionLobby(SessionLobby.Custom, _lobbyId);

            if (!result.Ok)
            {
                Debug.Log(result.ShutdownReason);
            }
            else
            {
                Debug.Log("Connect to lobby");
            }
        }

        private void Connect()
        {
            _networkRunner = FindObjectOfType<NetworkRunner>();

            if (_networkRunner == null)
            {
                _networkRunner = Instantiate(_networkRunnerPrefab, this.transform);
            }

            _networkRunner.AddCallbacks(this);
        }

        public void Disconnect()
        {
            if (_networkRunner == null) return;

            if (Application.isPlaying)
            {
                DisconnectAsync();
            }
            else
            {
                _networkRunner.Shutdown();
            }
        }

        private async Task DisconnectAsync()
        {
            OnPlayerLeftRoom?.Invoke();

            await Task.Delay(3000);
            await _networkRunner.Shutdown();
            
            SceneManager.LoadScene("_Menu_Loop");
        }

        public void CreateOrJoinRoom(string roomName)
        {
            Connect();

            _networkRunner.ProvideInput = true;

            _networkRunner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.AutoHostOrClient,
                CustomLobbyName = "Coop",
                SceneManager = _sceneManagerLoop,
                Scene = SceneManager.GetActiveScene().buildIndex + 1,
                SessionName = roomName,
                PlayerCount = 2
            });
        }

        public void SetLocalPlayer(PlayerLoop localPlayer)
        {
            _localPlayer = localPlayer;
        }

        public void SetPlayer(PlayerRef playerRef, PlayerLoop playerLoop)
        {
            if (!_players.ContainsKey(playerRef))
            {
                _players[playerRef] = playerLoop;
                playerLoop.transform.name += "_" + playerRef.PlayerId;
                playerLoop.transform.SetParent(_networkRunner.transform);
                Debug.Log("Player Count => " + _players.Count);
            }
        }

        public void RemoveOnPlayerLeft(PlayerLoop playerLoop)
        {
            Debug.Log(nameof(RemoveOnPlayerLeft));
            if (_players.ContainsKey(playerLoop.Object.InputAuthority))
            {
                _players.Remove(playerLoop.Object.InputAuthority);
            }
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                var spawnPlayer = runner.Spawn(_playerLoopPrefab, Vector3.zero, Quaternion.identity, player);
                OnPlayerEnterRoom?.Invoke(spawnPlayer);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"{player.PlayerId} disconnected.");

            if (_players.TryGetValue(player, out PlayerLoop playerobj))
            {
                if (playerobj.Object != null && playerobj.Object.HasStateAuthority)
                {
                    Debug.Log("Despawning Player");
                    runner.Despawn(playerobj.Object);
                    OnPlayerLeftRoom?.Invoke();
                }

                _players.Remove(player);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (_localPlayer == null) return;
            if (_localPlayer.Avatar == null) return;

            InputDataLoop inputData = new InputDataLoop();
            inputData.MoveDirection = _localPlayer.Avatar.Input.MovePosition;
            
            input.Set(inputData);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Disconnect();
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