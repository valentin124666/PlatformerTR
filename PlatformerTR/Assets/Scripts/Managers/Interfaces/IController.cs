namespace Managers.Interfaces
{
    public interface IController 
    {
        bool IsInit { get; }

        void Init();
        
        void Dispose();

        void ResetAll();

    }
}
