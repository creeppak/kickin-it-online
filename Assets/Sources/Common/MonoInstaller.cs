using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sources.Common
{
    public abstract class MonoInstaller : MonoBehaviour, IInstaller
    {
        public abstract void Install(IContainerBuilder builder);
    }
}