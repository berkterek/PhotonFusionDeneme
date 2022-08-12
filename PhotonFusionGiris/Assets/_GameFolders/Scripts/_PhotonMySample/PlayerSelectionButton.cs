using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Loops
{
    public class PlayerSelectionButton : NetworkBehaviour
    {
        [SerializeField] PlayerLoop _playerLoop;
        [SerializeField] PlayerType _playerType;
        [SerializeField] TMP_Text _messageText;
        [SerializeField] Button _button;

        [Networked] public NetworkBool IsTaken { get; private set; }

        void Awake()
        {
            GetReference();
        }

        void OnValidate()
        {
            GetReference();
        }

        IEnumerator Start()
        {
            _messageText.text = _playerType.ToString();
            yield return new WaitForSeconds(2f);

            this.gameObject.SetActive(!IsTaken);
        }

        void OnEnable()
        {
            _button.onClick.AddListener(RPC_HandleOnButtonClicked);
        }

        void OnDisable()
        {
            _button.onClick.RemoveListener(RPC_HandleOnButtonClicked);
        }

        [Rpc(RpcSources.All,RpcTargets.All)]
        void RPC_HandleOnButtonClicked()
        {
            IsTaken = true;

            _playerLoop = NetworkManagerLoop.Instance.LocalPlayer;
        }

        private void GetReference()
        {
            if (_messageText == null)
            {
                _messageText = GetComponentInChildren<TMP_Text>();
            }

            if (_button == null)
            {
                _button = GetComponent<Button>();
            }
        }
    }
}