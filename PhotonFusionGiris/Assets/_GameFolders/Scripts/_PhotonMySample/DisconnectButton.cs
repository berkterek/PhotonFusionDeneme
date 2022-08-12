using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace Loops
{
    public class DisconnectButton : NetworkBehaviour
    {
        [SerializeField] Button _button;

        void Awake()
        {
            GetReference();
        }

        void OnValidate()
        {
            GetReference();
        }

        void OnEnable()
        {
            _button.onClick.AddListener(RPC_HandleOnButtonClicked);
        }

        void OnDisable()
        {
            _button.onClick.RemoveListener(RPC_HandleOnButtonClicked);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        void RPC_HandleOnButtonClicked()
        {
            NetworkManagerLoop.Instance.Disconnect();
        }

        private void GetReference()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }
        }
    }
}