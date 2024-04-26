using System;
using UnityEngine;

namespace TS.Physics
{
    public class PhysicsHandler : MonoBehaviour
    {
        public event Action<Collision> OnCollisionEntered;
        
        #region Methods -> Unity Callbacks

        private void OnCollisionEnter(Collision other)
        {
            OnCollisionEntered?.Invoke(other);
        }

        #endregion
    }
}