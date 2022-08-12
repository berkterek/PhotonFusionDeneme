using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loops
{
    public class SceneManagerLoop : NetworkSceneManagerBase
    {
        string _networkSceneName = string.Empty;
        
        public void SetNetworkSceneName(string networkSceneName)
        {
            _networkSceneName = networkSceneName;
        }
        
        public void LoadSceneByName(string sceneName)
        {
            StartCoroutine(LoadSceneByNameAsync(sceneName));
        }

        private IEnumerator LoadSceneByNameAsync(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
        
        protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
        {
            Debug.Log($"Switching Scene from {prevScene} to {newScene}");

            List<NetworkObject> sceneObjects = new List<NetworkObject>();

            yield return SceneManager.LoadSceneAsync(_networkSceneName, LoadSceneMode.Single);
            var loadedScene = SceneManager.GetSceneByName(_networkSceneName);
            Debug.Log($"Loaded scene {_networkSceneName}: {loadedScene}");
            sceneObjects = FindNetworkObjects(loadedScene, disable: false);

            // Delay one frame
            yield return null;
            finished(sceneObjects);

            Debug.Log($"Switched Scene from {prevScene} to {newScene} - loaded {sceneObjects.Count} scene objects");
        }
    }    
}

