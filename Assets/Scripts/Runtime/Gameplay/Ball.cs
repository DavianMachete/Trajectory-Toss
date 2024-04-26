using UnityEngine;

using TS.Physics;

namespace TS.Gameplay
{
    public class Ball : Simulatable
    {
        public bool IsOld => Time.time - _throwTime > lifeTime &&
                             _isThrown;

        public Vector3 Velocity => rb.velocity;
        public Vector3 ContactPosition => 
            _lastCollision == null ? transform.position :
                _lastCollision.contacts[0].point;
        public Vector3 ContactNormal =>
            _lastCollision == null ? Vector3.up : 
                _lastCollision.contacts[0].normal;

        [SerializeField] private float lifeTime = 20f;
        [SerializeField] private float speed = 20f;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private PhysicsHandler physicsHandler;

        private Player _player;

        private Collision _lastCollision;
        private bool _isThrown;
        private float _throwTime;

        #region Public -> Methods

        public void Initialize(Player player)
        {
            _player = player;
            
            rb.isKinematic = true;
            rb.useGravity = false;
            
            _isThrown = false;
        }

        public void Throw()
        {
            _throwTime = Time.time;
            
            rb.isKinematic = false;
            rb.useGravity = true;
            
            rb.velocity = CalculateVelocity();
            
            _isThrown = true;
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void DestroyBall()
        {
            Destroy(gameObject);
        }
        
        public override void StartSimulation()
        {
            gameObject.layer = LayerMask.NameToLayer("Simulation");
            physicsHandler.OnCollisionEntered += (c)=>
            {
                gameObject.layer = LayerMask.NameToLayer("Player");
                IsSimulated = true;
            };
            
            Throw();
        }

        #endregion

        #region Methods -> Private

        private Vector3 CalculateVelocity()
        {
            var distanceVector = _player.TargetPos - transform.position;
            var distance = distanceVector.magnitude;
            var t = distance / speed;

            var v0 = Vector3.zero;
            var v0X = distanceVector.x / t;
            var v0Z = distanceVector.z / t;
            var g = UnityEngine.Physics.gravity.y;
            var v0Y = distanceVector.y / t - 0.5f * g * t;

            v0.x = v0X;
            v0.z = v0Z;
            v0.y = v0Y;

            return v0;
        }

        #endregion
    }
}