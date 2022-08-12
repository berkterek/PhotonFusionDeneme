using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Loops
{
    public class SpawnPointController : MonoBehaviour
    {
        [SerializeField] Transform[] _minMaxTrasforms;
        [SerializeField] Transform _randomPoint;

        static Vector3 _randomPosition;
        static SpawnPointController _instance;

        void Awake()
        {
            _instance = this;
        }

        public static Vector3 RandomPosition
        {
            get
            {
                _instance.GetRandomPosition();
                return _randomPosition;
            }
        }

        [ContextMenu(nameof(GetRandomPosition))]
        private void GetRandomPosition()
        {
            float maxX = Random.Range(_minMaxTrasforms[2].position.x, _minMaxTrasforms[3].position.x);
            float maxZ = Random.Range(_minMaxTrasforms[0].position.z, _minMaxTrasforms[3].position.z);
            Vector3 randomPosition = new Vector3(maxX, 0f, maxZ);

            _randomPoint.position = randomPosition;
            _randomPosition = _randomPoint.position;
        }

        void OnDrawGizmos()
        {
            OnDrawGizmosSelected();
        }

        void OnDrawGizmosSelected()
        {
            if (_minMaxTrasforms.Length <= 0 || _minMaxTrasforms == null) return;
            
            Gizmos.color = Color.red;

            for(int i = 0; i < _minMaxTrasforms.Length; i++)
            {
                if (i + 1 >= _minMaxTrasforms.Length)
                {
                    Gizmos.DrawLine(_minMaxTrasforms[i].position,_minMaxTrasforms[0].position);
                }
                else
                {
                    Gizmos.DrawLine(_minMaxTrasforms[i].position,_minMaxTrasforms[i + 1].position);    
                }
            }
        }
    }    
}