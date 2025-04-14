using Fusion;
using UnityEngine;

namespace DI_Network_Sandbox
{
    public class TestNetworkBehaviour : NetworkBehaviour
    {
        private void Awake()
        {
            TestLog.Log(gameObject.name + " is Awake");
        }
        
        public override void Spawned()
        {
            TestLog.Log(gameObject.name + " is Spawned. PlayerId: " + Object.InputAuthority);

            transform.position = Vector3.right * Object.InputAuthority.AsIndex;
        }
    }
}