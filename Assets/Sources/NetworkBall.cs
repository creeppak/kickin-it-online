using Fusion;
using UnityEngine;

public class NetworkBall : NetworkBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _sinSpeed = 2f;
    [SerializeField] private float _sinAmplitude = 1f;
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private AnimationCurve _pathCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    
    [Networked] private Vector3 _startPosition { get; set; }
    [Networked] private float _startTime { get; set; }
    
    [Networked] private TickTimer _selfDestructTimer { set; get; }

    public void Init()
    {
        _selfDestructTimer = TickTimer.CreateFromSeconds(Runner, _lifeTime);
    }

    public override void Spawned()
    {
        _startPosition = transform.position;
        _startTime = Runner.SimulationTime;
    }

    public override void FixedUpdateNetwork()
    {
        // Check self destruct timer
        if (_selfDestructTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
            return;
        }
        
        var timePassed = Runner.SimulationTime - _startTime;
        var distance = _speed * timePassed;
        Debug.Log(distance);
        var forwardMotion = transform.forward * distance;
        var sinMotion = -transform.right * _pathCurve.Evaluate(timePassed * _sinSpeed) * _sinAmplitude;
        transform.position = _startPosition + forwardMotion + sinMotion;
    }
}