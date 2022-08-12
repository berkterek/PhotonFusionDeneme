using Fusion;
using TMPro;
using UnityEngine;

namespace Loops
{
    public class WaitingRoomPanelNewLoop : NetworkBehaviour
    {
        [SerializeField] TMP_Text _discountTimeText;
        [SerializeField] int _countValue = 3;
        [SerializeField] PlayerReadyPanelLoop[] _playerReadyPanels;

        [Networked] bool IsFirstTime { get; set; } = true;
        [Networked] TickTimer ReadyCountingTimer { get; set; }
        [Networked(OnChanged = nameof(HandleOnDiscountValueChanged))] float DiscountTimeValue { get; set; }

        bool _oneTimeOperation = false;

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
            NetworkManagerLoop.Instance.OnPlayerEnterRoom += RPC_HandleOnPlayerEnterRoom;
        }

        void OnDisable()
        {
            NetworkManagerLoop.Instance.OnPlayerEnterRoom -= RPC_HandleOnPlayerEnterRoom;
        }

        void Update()
        {
            if (NetworkManagerLoop.Instance.PlayerCount != 2) return;
            
            if (!NetworkManagerLoop.Instance.AreAllPlayerReady) return;

            if (ReadyCountingTimer.ExpiredOrNotRunning(Runner))
            {
                if (IsFirstTime)
                {
                    IsFirstTime = false;
                    ReadyCountingTimer = TickTimer.CreateFromSeconds(Runner, _countValue);
                    RPC_SetActiveText(true);
                    return;
                }
                
                //StartGame
                
            }

            DiscountTimeValue = (float)ReadyCountingTimer.RemainingTime(Runner);

            if (DiscountTimeValue <= 0f)
            {
                if (!_oneTimeOperation)
                {
                    _oneTimeOperation = true;
                    
                    RPC_StartGame();
                }
            }
        }

        [Rpc(RpcSources.All,RpcTargets.All)]
        void RPC_SetActiveText(bool value)
        {
            _discountTimeText.gameObject.SetActive(value);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        void RPC_HandleOnPlayerEnterRoom(PlayerLoop player)
        {
            foreach (var playerReadyPanelLoop in _playerReadyPanels)
            {
                if (!playerReadyPanelLoop.IsTaken)
                {
                    playerReadyPanelLoop.Bind(player);
                    break;
                }
                else
                {
                    playerReadyPanelLoop.Bind();
                }
            }

            if (player == NetworkManagerLoop.Instance.LocalPlayer)
            {
                Object.AssignInputAuthority(player.Object.InputAuthority);
            }
        }

        [Rpc(RpcSources.All,RpcTargets.All)]
        void RPC_StartGame()
        {
            foreach (var playerLoop in NetworkManagerLoop.Instance.Players)
            {
                playerLoop.SpawnAvatarCharacter();
            }
            
            this.gameObject.SetActive(false);
        }

        private static void HandleOnDiscountValueChanged(Changed<WaitingRoomPanelNewLoop> value)
        {
            value.Behaviour._discountTimeText.SetText(value.Behaviour.DiscountTimeValue.ToString("00"));
        }

        private void GetReference()
        {
            if (_playerReadyPanels == null || _playerReadyPanels.Length == 0)
            {
                _playerReadyPanels = GetComponentsInChildren<PlayerReadyPanelLoop>();
            }
        }
    }
}