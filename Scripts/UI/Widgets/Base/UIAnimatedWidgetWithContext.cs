namespace Game.Scripts.UI.Widgets.Base
{
    public abstract class UIAnimatedWidgetWithContext<TContext> : UIAnimatedWidget where TContext : UIContext
    {
        public TContext Context { get; private set; }

        public virtual void OnShowStart(TContext context)
        {
            OnShowStart();
            Context = context;
        }
    }
}