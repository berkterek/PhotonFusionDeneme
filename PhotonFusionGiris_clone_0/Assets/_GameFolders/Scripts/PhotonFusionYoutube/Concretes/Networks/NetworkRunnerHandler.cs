using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PhotonFusionYoutube.Networks
{
    public class NetworkRunnerHandler : MonoBehaviour
    {
        [SerializeField] NetworkRunner _networkRunnerPrefab;

        NetworkRunner _networkRunner;

        void Start()
        {
            _networkRunner = Instantiate(_networkRunnerPrefab);

            var clientTask = InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(),
                SceneManager.GetActiveScene().buildIndex, (networkRunner) => Debug.Log("Server network runner started"));
        }

        Task InitializeNetworkRunner(NetworkRunner networkRunner, GameMode gameMode, NetAddress address, SceneRef scene, System.Action<NetworkRunner> initialized)
        {
            var sceneManager = networkRunner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>()
                .FirstOrDefault();

            if (sceneManager == null)
            {
                sceneManager = networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            }

            networkRunner.ProvideInput = true;

            return networkRunner.StartGame(new StartGameArgs
            {
                GameMode = gameMode,
                Address = address,
                Scene = scene,
                SessionName = "TestRoom",
                Initialized = initialized,
                SceneManager = sceneManager
            });
        }
    }    
}