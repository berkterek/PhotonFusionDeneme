using Fusion;
using TMPro;
using UnityEngine;

namespace Loops
{
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerPanelLoop : NetworkBehaviour
    {
        [SerializeField] RectTransform _rectTransform;
        [SerializeField] TMP_Text _readyMessageText;
        [SerializeField] TMP_Text _playerNameText;
        [SerializeField] GameObject _readyObject;

        PlayerLoop _playerLoop;

        public RectTransform RectTransform => _rectTransform;

        void Awake()
        {
            GetReference();
        }

        void OnValidate()
        {
            GetReference();
        }

        void GetReference()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
        }

        public void Bind(PlayerLoop localPlayer)
        {
            _playerLoop = localPlayer;
            _playerLoop.OnIsReadyValueChanged += HandleOnIsReadyValueChanged;
            _playerNameText.text = _playerLoop.PlayerType.ToString();
            _readyObject.SetActive(true);
        }

        void OnDisable()
        {
            if (_playerLoop == null) return;
            
            _playerLoop.OnIsReadyValueChanged -= HandleOnIsReadyValueChanged;
        }

        void HandleOnIsReadyValueChanged(NetworkBool value)
        {
            _readyMessageText.text = value ? "Ready" : "NOT Ready";
        }
    }    
}

