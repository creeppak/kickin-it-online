using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
    public static class UniTaskExtensions
    {
        public static async void ReleaseYetLog(this Task task)
        {
            try
            {
                await task;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }
        
        public static async void ReleaseYetLog(this UniTask task)
        {
            try
            {
                await task;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        public static ValueTask ToValueTask(this Task task)
        {
            return new ValueTask(task);
        }
    }
}