using System;
using Settings;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public interface IDamageReceiver 
    {
        Enumerators.EntityType EntityType { get; }
        Vector3 PositionReceiver { get; }
        event Action<float> GetDamageEvent;
        bool CheckCollider(Collider2D collider2D);
        void GetDamage(float damageValue);
    }
}
