using Fusion;
using R3;
using UnityEngine;
using VContainer;

namespace KickinIt.Simulation.Gates
{
    internal class GatesTrigger : NetworkBehaviour
    {
        [SerializeField] private new BoxCollider collider;
        [SerializeField] private LayerMask ballMask;
        [SerializeField] private Color gizmoColor = Color.red;
        
        private readonly Collider[] _overlapBuffer = new Collider[8];
        private readonly Subject<Unit> _onGoal = new();
        
        private Transform _colliderTransform;
        private Mesh _cubeMesh;
        private PhysicsScene _physicsScene;

        public Observable<Unit> OnGoal => _onGoal;

        [Inject]
        private void Configure(PhysicsScene physicsScene)
        {
            _physicsScene = physicsScene;
        }

        private void OnValidate()
        {
            if (collider == null)
            {
                collider = GetComponent<BoxCollider>();
                collider.isTrigger = true;
            }
        }

        private void Awake()
        {
            _colliderTransform = collider.transform;
        }

        private void OnDrawGizmos()
        {
            DrawGizmos(gizmoColor / 2);
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos(gizmoColor);
        }

        private void DrawGizmos(Color color)
        {
            if (collider == null) return;
            
            if (_colliderTransform == null)
            {
                _colliderTransform = collider.transform;
            }
            
            if (_cubeMesh == null)
            {
                _cubeMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            }
            
            Gizmos.color = color;
            
            var center = _colliderTransform.TransformPoint(collider.center);
            var rotation = _colliderTransform.rotation;
            var extents = Vector3.Scale(_colliderTransform.lossyScale, collider.size / 2f);
            
            Gizmos.DrawWireMesh(_cubeMesh, center, rotation, extents * 2f);
        }

        public override void FixedUpdateNetwork()
        {
            if (!Runner.IsForward) return;
            
            var center = _colliderTransform.TransformPoint(collider.center);
            var rotation = _colliderTransform.rotation;
            var halfExtents = Vector3.Scale(_colliderTransform.lossyScale, collider.size / 2f);
            
            var overlapCount = _physicsScene.OverlapBox(
                center,
                halfExtents,
                _overlapBuffer,
                rotation,
                ballMask,
                QueryTriggerInteraction.Collide // for whatever reason ignoring triggers skips the ball (which is not a trigger) todo: double check this
            );

            if (overlapCount > 0)
            {
                _onGoal.OnNext(Unit.Default);
            }
        }
    }
}