using UnityEngine;

namespace Milhouzer.Core.BuildSystem.ContextActions
{
    public abstract class ContextAction : ScriptableObject, IContextAction<IContextActionHandler>
    {
        public void Execute(IContextActionHandler handler) => ExecuteImpl(handler);

        protected abstract void ExecuteImpl(IContextActionHandler handler);
    }

    public interface IContextAction<T> where T : IContextActionHandler {
        public void Execute(T handler);
    }

    public interface IContextActionHandler { }
}