using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TS.Physics;

namespace TS.Gameplay
{
    [RequireComponent(typeof(LineRenderer))]
    public class TrajectoryDrawer : MonoBehaviour
    {
        
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform target;
        [SerializeField] private Player player;
        [SerializeField] private PhysicsManager physicsManager;
        [SerializeField] private int linePositionsMaxCount = 200;

        private LineRenderer _line;
        private Ball _currentBall;

        #region Unity Callbacks

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            _line.enabled = false;
            
            target.gameObject.SetActive(false);

            player.OnStartAiming += OnStartAiming;
            player.OnThrown += OnThrown;
        }

        private void OnDestroy()
        {
            player.OnStartAiming -= OnStartAiming;
            player.OnThrown -= OnThrown;
        }

        private void Update()
        {
            if(!_line.enabled)
                return;

            DrawTrajectory();
        }

        #endregion

        #region Methods -> Private
        
        private void OnStartAiming(Ball ball)
        {
            _currentBall = ball;
            
            _line.enabled = true;
            target.gameObject.SetActive(true);
        }

        private void OnThrown()
        {
            _line.enabled = false;
            target.gameObject.SetActive(false);
        }

        private void DrawTrajectory()
        {
            var ball = physicsManager.AddSimulation(_currentBall);
            if (ball is null)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                Debug.LogError("ball is null");
                return;
            }

            ball.Initialize(player);
            var ballT = ball.transform;
            ballT.position = spawnPoint.position;
            ballT.rotation = spawnPoint.rotation;
            ball.StartSimulation();

            var index = 0;
            while (!ball.IsSimulated &&
                   _line.positionCount < linePositionsMaxCount)
            {
                physicsManager.Simulate();
                _line.positionCount = index + 1;
                _line.SetPosition(index, ball.transform.position);
                index++;
            }

            target.position = ball.ContactPosition;
            var forward = ball.Velocity * -1f;
            target.rotation = Quaternion.LookRotation(forward, ball.ContactNormal);
            ball.DestroyBall();
        }

        #endregion
    }   
}