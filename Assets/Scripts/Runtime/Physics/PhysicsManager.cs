using UnityEngine;
using UnityEngine.SceneManagement;

using TS.Gameplay;

namespace TS.Physics
{
    public class PhysicsManager : MonoBehaviour
    {
        [SerializeField] private Obstacle[] obstacles;
        [SerializeField] private float simulationSpeed; 
        
        private Scene _simulationScene;
        private PhysicsScene _physicsScene;
        
        private const string SimulationSceneName = "Simulation";

        #region Methods -> Unity Callbacks

        private void Awake()
        {   
            // Create a simulation Scene for physics.
            _simulationScene = SceneManager.CreateScene(SimulationSceneName, new CreateSceneParameters(LocalPhysicsMode.Physics3D));
            _physicsScene = _simulationScene.GetPhysicsScene();

            foreach (var obstacle in obstacles)
            {
                AddSimulation(obstacle);
            }
        }

        #endregion
        
        #region Methods -> Publics

        public void Simulate()
        {
            var step = simulationSpeed * Time.fixedDeltaTime;
            _physicsScene.Simulate(step);
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public T AddSimulation<T>(T simulatable) where T : Simulatable
        {
            var sim = simulatable.CopyAndGetSimulatable();
            var simT = sim as T;
            var simGameObject = sim.gameObject;
            SceneManager.MoveGameObjectToScene(simGameObject, _simulationScene);
            return simT;
        }
        
        #endregion
    }   
}