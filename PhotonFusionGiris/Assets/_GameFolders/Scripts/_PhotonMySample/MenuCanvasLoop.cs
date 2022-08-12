using System.Collections;
using UnityEngine;

namespace Loops
{
    public class MenuCanvasLoop : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            
            NetworkManagerLoop.Instance.EnterLobby();
        }
    }    
}

