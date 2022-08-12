using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Loops
{
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerReadyPanelLoop : NetworkBehaviour
    {
        [SerializeField] PlayerType _playerType;
        [SerializeField] TextProNetwork _playerNameText;
        [SerializeField] TMP_Text _readyMessageText;
        [SerializeField] Button _readyButton;
        [SerializeField] GameObject _readyObject;
        [Networked] public PlayerLoop PlayerLoop { get; private set; }
        [Networked] public NetworkBool IsTaken { get; private set; }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);

            if (IsTaken)
            {
                StartCoroutine(BindProcessAsync());
            }
        }

        void OnEnable()
        {
            _readyButton.onClick.AddListener(HandleOnButtonClicked);
        }

        void OnDisable()
        {
            _readyButton.onClick.RemoveListener(HandleOnButtonClicked);
        }

        public void Bind(PlayerLoop playerLoop)
        {
            if (IsTaken) return;

            PlayerLoop = playerLoop;
            this.Object.AssignInputAuthority(PlayerLoop.Object.InputAuthority);

            StartCoroutine(BindAsync());
        }

        IEnumerator BindAsync()
        {
            Debug.Log(PlayerLoop.Object.Name);
            IsTaken = true;

            yield return new WaitForSeconds(1f);

            PlayerLoop.RPC_SetPlayerType(_playerType);
            PlayerLoop.RPC_SetName(_playerType.ToString());
            PlayerLoop.OnIsReadyValueChanged += RPC_HandleOnValueChanged;

            yield return BindProcessAsync();
        }

        private IEnumerator BindProcessAsync()
        {
            yield return new WaitForSeconds(1f);

            PlayerLoop.RPC_SetIsReady(false);
            _playerNameText.RPC_TextValueChanged(PlayerLoop.PlayerType.ToString());
            Debug.Log(NetworkManagerLoop.Instance.LocalPlayer.Object.Name);
            RPC_ReadyObjectSetActive();
        }

        [Rpc(RpcSources.All, RpcTargets.InputAuthority)]
        void RPC_ReadyObjectSetActive()
        {
            _readyObject.SetActive(IsTaken);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_HandleOnValueChanged(NetworkBool value)
        {
            if (value)
            {
                _readyMessageText.text = "Ready";
            }
            else
            {
                _readyMessageText.text = "Not Ready";
            }
        }

        void HandleOnPlayerLeftRoom()
        {
            if (PlayerLoop == null) return;

            RPC_LeftRoomProcess();
        }

        void HandleOnButtonClicked()
        {
            PlayerLoop.RPC_SetIsReady(!PlayerLoop.Ready);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_LeftRoomProcess()
        {
            IsTaken = false;
            PlayerLoop.OnIsReadyValueChanged -= RPC_HandleOnValueChanged;
            _playerNameText.RPC_TextValueChanged(PlayerType.PlayerNotJoin.ToString());
            PlayerLoop = null;
            _readyObject.SetActive(false);
        }

        public void Bind()
        {
            StartCoroutine(BindProcessAsync());
        }
    }
}