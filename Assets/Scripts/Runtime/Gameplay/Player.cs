using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TS.Inputs;

namespace TS.Gameplay
{
    public class Player : MonoBehaviour
    {
        public Vector3 TargetPos => target.position; 
        
        [SerializeField] private TouchManager touchManager;
        [SerializeField] private Transform ballSpawn;
        [SerializeField] private Ball ballPrefab;
        [SerializeField] private Transform target;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask raycastIgnore;
        [SerializeField] private float targetMovmentSens = 30f;
        [SerializeField] private Vector2 targetZLimits = new Vector2(-10f, 200f);
        [SerializeField] private Vector2 targetXLimits = new Vector2(-50f, 50f);

        private Ball _currentBall;
        private readonly List<Ball> _ballsPool = new();

        public Action<Ball> OnStartAiming;
        public Action OnThrown;


        #region Methods -> Unity Callbacks

        private void Awake()
        {
            touchManager.OnSingleTouchDown += OnOnSingleTouchDown;
            touchManager.OnSingleTouchDrag += OnOnSingleTouchDrag;
            touchManager.OnSingleTouchUp += OnOnSingleTouchUp;
        }

        private void OnDestroy()
        {
            touchManager.OnSingleTouchDown -= OnOnSingleTouchDown;
            touchManager.OnSingleTouchDrag -= OnOnSingleTouchDrag;
            touchManager.OnSingleTouchUp -= OnOnSingleTouchUp;
        }

        private void Update()
        {
            foreach (var ball in _ballsPool.Where(ball => ball.IsOld))
            {
                ball.SetActive(false);
            }
        }

        #endregion

        #region Methods -> Private

        private void OnOnSingleTouchDrag(Vector2 currentPos, Vector2 deltaPos)
        {
            // add offset for finger view
            // currentPos.y += 100;
            var touchPos = Vector3.forward * 5f;
            touchPos.x = currentPos.x;
            touchPos.y = currentPos.y;
            
            // try hit ground by a Raycast
            var ray = mainCamera.ScreenPointToRay(touchPos);
            if(!UnityEngine.Physics.Raycast(ray,out var hit,Mathf.Infinity,raycastIgnore))
                return;

            var targetPos = hit.point;
            targetPos.x = Mathf.Clamp(targetPos.x, targetXLimits.x, targetXLimits.y);
            targetPos.z = Mathf.Clamp(targetPos.z, targetZLimits.x, targetZLimits.y);
            target.position = Vector3.Lerp(target.position, targetPos, targetMovmentSens * Time.deltaTime);
        }
        
        private Vector3 GetCameraForward()
        {
            var cameraForward = mainCamera.transform.forward;
            cameraForward.y = 0f;
            cameraForward = cameraForward.normalized;
            return cameraForward;
        }

        private Vector3 GetCameraRight()
        {
            var forward = GetCameraForward();
            var right = Vector3.Cross(forward, Vector3.down);
            right = right.normalized;
            return right;
        }

        private void OnOnSingleTouchDown(Vector2 pos)
        {
            _currentBall = PoolABall();
            
            var ballT = _currentBall.transform;
            ballT.position = ballSpawn.position;
            ballT.rotation = ballSpawn.rotation;
            
            OnStartAiming?.Invoke(_currentBall);
        }

        private void OnOnSingleTouchUp(Vector2 pos)
        {
            _currentBall.Throw();
            OnThrown?.Invoke();
        }

        private Ball PoolABall()
        {
            var ball = _ballsPool.FirstOrDefault(b => b.IsOld);

            if (ball is null)
            {
                ball = Instantiate(ballPrefab);
                _ballsPool.Add(ball);
            }
            
            ball.Initialize(this);
            ball.SetActive(true);
            return ball;
        }

        #endregion
    }
}