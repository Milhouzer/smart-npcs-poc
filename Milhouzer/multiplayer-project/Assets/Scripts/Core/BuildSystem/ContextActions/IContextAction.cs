using UnityEngine;

namespace Milhouzer.Core.BuildSystem.ContextActions
{
    [CreateAssetMenu(fileName = "IContextAction", menuName = "IContextAction", order = 0)]
    public abstract class ContextAction<T> : ScriptableObject, IContextAction<T> where T : IContextActionHandler
    {
        public void Execute(T handler) => ExecuteImpl(handler);

        protected abstract void ExecuteImpl(T handler);
    }

    public interface IContextAction<T> where T : IContextActionHandler {
        public void Execute(T handler);
    }

    public interface IContextActionHandler { }
}