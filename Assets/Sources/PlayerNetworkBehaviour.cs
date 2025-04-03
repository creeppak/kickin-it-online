using Fusion;
using UnityEngine;

public class PlayerNetworkBehaviour : NetworkBehaviour
{
    [SerializeField] private NetworkCharacterController _cc;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private NetworkBall _ballPrefab;
    
    private Vector3 _forward;

    private void OnValidate()
    {
        if (!_cc) _cc = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork() // apply input, tick physics
    {
        if (!GetInput(out OldNetworkInputData inputData)) return;
        
        inputData.Movement.Normalize();
        _cc.Move(inputData.Movement * _moveSpeed * Runner.DeltaTime);
        
        if (inputData.Movement.sqrMagnitude > 0.01f)
            _forward = inputData.Movement;
        
        if (HasStateAuthority)
        {
            FixedUpdateNetworkStateAuthorityOnly(inputData);
        }   
    }

    private void FixedUpdateNetworkStateAuthorityOnly(OldNetworkInputData inputData)
    {
        if (inputData.Buttons.IsSet(OldNetworkInputData.FireButton))
        {
            Runner.Spawn(
                _ballPrefab,
                transform.position + _forward, 
                Quaternion.LookRotation(_forward),
                Object.InputAuthority,
                (_, obj) => obj.GetComponent<NetworkBall>().Init());
        }
    }
}