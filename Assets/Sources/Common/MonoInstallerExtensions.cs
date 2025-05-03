using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sources.Common
{
    public static class MonoInstallerExtensions
    {
        public static void RunMonoInstallers(this IContainerBuilder builder, GameObject gameObject)
        {
            var installers = gameObject.GetComponents<IInstaller>();

            foreach (var installer in installers)
            {
                installer.Install(builder);
            }
        }
    }
}