using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace Loops
{
    public class PlayerSelectionButtonNetwork : NetworkBehaviour
    {
        [Networked]  NetworkString<_32> Message { get; set; }
        [Networked] NetworkBool IsActive { get; set; }

        [SerializeField] Image _image;
        [SerializeField] Button _button;
        [SerializeField] Text _text;

        public event System.Action OnButtonClicked;

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
            _button.onClick.AddListener(HandleOnButtonClicked);
        }

        void OnDisable()
        {
            _button.onClick.RemoveListener(HandleOnButtonClicked);
        }

        void HandleOnButtonClicked()
        {
            OnButtonClicked?.Invoke();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_DisableProcess()
        {
            Message = string.Empty;
            IsActive = false;

            SetValues(IsActive, Message.Value);
        }

        private void SetValues(NetworkBool boolValue, string message)
        {
            _button.interactable = boolValue;
            _image.enabled = boolValue;
            _text.text = message;
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_SetActive()
        {
            Message = "Player Selection";
            IsActive = true;

            SetValues(IsActive, Message.Value);
        }

        private void GetReference()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }

            if (_image == null)
            {
                _image = GetComponent<Image>();
            }

            if (_text == null)
            {
                _text = GetComponentInChildren<Text>();
            }
        }
    }
}