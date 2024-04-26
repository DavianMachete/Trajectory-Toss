using UnityEngine;

namespace TS.Physics
{
    public abstract class Simulatable : MonoBehaviour
    {
        public bool IsSimulated { get; protected set; }
        
        [SerializeField] private Renderer[] renderers;
        
        public Simulatable CopyAndGetSimulatable() 
        {
            IsSimulated = false;
            var sim = Instantiate(this);
            sim.PrepareForSimulation();
            return sim;
        }

        public virtual void StartSimulation() => IsSimulated = true;
        
        private void PrepareForSimulation()
        {
            foreach (var r in renderers)
            {
                r.enabled = false;
            }
        }
    }
}