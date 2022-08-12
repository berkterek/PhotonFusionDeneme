using Fusion;
using TMPro;
using UnityEngine;

namespace Loops
{
    public class TextProNetwork : NetworkBehaviour
    {
        [SerializeField] TMP_Text _text;

        void Awake()
        {
            GetReference();
        }
        
        void OnValidate()
        {
            GetReference();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_TextValueChanged(string value)
        {
            _text.text = value;
        }

        private void GetReference()
        {
            if (_text == null)
            {
                _text = GetComponent<TMP_Text>();
            }
        }
    }
}