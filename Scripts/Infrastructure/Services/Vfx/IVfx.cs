namespace LevelElements.Vfx
{
    public interface IVfx
    {
        void Dispose();
        void Play();
        bool ShouldUsePool { get; }
    }
}