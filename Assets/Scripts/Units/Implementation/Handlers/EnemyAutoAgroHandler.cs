using Game.Core;
using Game.Unit.Damageable;
using Game.Units.Enemies;
using UnityEngine;

namespace Game.Unit.Handlers
{
    public class EnemyAutoAgroHandler : IHandler<IDamageable>
    {
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private float _radius;
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_targetData.Instance.transform.position, _radius);
        }
#endif
        
        private void LateUpdate()
        {
            var colliders = Physics.OverlapSphere(_targetData.Instance.transform.position, _radius, _layerMask);

            foreach (var collider in colliders)
            {
                if (collider.gameObject.TryGetComponent(out EnemyController enemyController))
                {
                    enemyController.Agro(_targetData);
                }
            }
        }
    }
}