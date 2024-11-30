namespace Mechanisms
{
    public interface IMechanism
    {
        bool IsActive { get; }

        void ReceiveSignal(bool signal);
        void SendSignal(bool signal) => ReceiveSignal(signal);
    }
}
