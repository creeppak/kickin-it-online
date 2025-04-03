using Fusion;
using UnityEngine;

public struct OldNetworkInputData : INetworkInput
{
    public const byte FireButton = 10;
    
    public Vector3 Movement;
    public NetworkButtons Buttons;
}