using System;
using Game.Assets;
using Game.Data;
using Game.Data.Fields;
using Game.Data.Fields.Stats;
using UnityEngine;

namespace Game.Unit.Damageable
{
    public interface IDamageable : IDataController, IAssetInstance
    {
        Collider Collider { get; }
        IsBattleStateField IsBattleStateField { get; } 
        
        HealthField HealthField { get; }

        bool IsAlive { get; }
        
        public event Action<IDamageable> OnKill;
        public event Action<float> OnDamage;

        void ReceiveDamage(IDamageable source, float count);

        bool IsCanKill(float count);
    }
}