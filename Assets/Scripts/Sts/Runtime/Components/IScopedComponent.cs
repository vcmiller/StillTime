using StillTime.Sts.Resources;

namespace StillTime.Sts.Runtime.Components {
    public interface IScopedComponent : IStateComponent {
        public void ResetScope(Scope scope);
    }
}
