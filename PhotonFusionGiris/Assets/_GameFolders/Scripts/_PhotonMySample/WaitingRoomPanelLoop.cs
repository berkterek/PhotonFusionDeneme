using Fusion;
using UnityEngine;

namespace Loops
{
    public class WaitingRoomPanelLoop : NetworkBehaviour
    {
        [SerializeField] PlayerPanelLoop _playerPanelLoop;
        [SerializeField] RectTransform[] _rectTransforms;
        [SerializeField] int _index = 0;

        void OnEnable()
        {
            NetworkManagerLoop.Instance.OnPlayerEnterRoom += RPC_HandleOnPlayerEnterRoom;
        }

        void OnDisable()
        {
            NetworkManagerLoop.Instance.OnPlayerEnterRoom -= RPC_HandleOnPlayerEnterRoom;   
        }
        
        [Rpc(RpcSources.All,RpcTargets.All)]
        void RPC_HandleOnPlayerEnterRoom(PlayerLoop player)
        {
            Debug.Log(player.Object.Name);
            var playerPanel = player.Runner.Spawn(_playerPanelLoop, _rectTransforms[_index].position, Quaternion.identity, player.Object.InputAuthority,
                (runner, networkObject) =>
                {
                    networkObject.transform.SetParent(_rectTransforms[_index].transform);
                });
            
            playerPanel.Bind(player);
            playerPanel.RectTransform.position = _rectTransforms[_index].position;
            
            _index++;
        }
    }
}