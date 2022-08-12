using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Loops
{
    public class CreateOrJoinPanel : MonoBehaviour
    {
        [SerializeField] Button _button;
        [SerializeField] TMP_InputField _roomNameInputField;

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
            StartCoroutine(HandleOnButtonClickedAsync());
        }

        IEnumerator HandleOnButtonClickedAsync()
        {
            yield return null;
            string roomName = _roomNameInputField.text;

            NetworkManagerLoop.Instance.CreateOrJoinRoom(roomName);
        }
    }    
}