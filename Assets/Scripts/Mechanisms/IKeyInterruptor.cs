namespace Mechanisms
{
    public interface IKeyInterruptor
    {
        bool IsKeyActivated { get; }

        void Activate();
        void Deactivate();
    }
}
