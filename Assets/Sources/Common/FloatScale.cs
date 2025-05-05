using UnityEngine;

namespace Sources.Common
{
    [ExecuteAlways]
    public class FloatScale : MonoBehaviour
    {
        [field:SerializeField] public float Scale { get; set; }
        
        private Transform _transform;

        private Transform Transform => _transform ? _transform : _transform = transform;

        private void Reset()
        {
            Scale = Transform.localScale.x;
        }

        private void Update()
        {
            Transform.localScale = new Vector3(Scale, Scale, Scale);
        }
    }
}