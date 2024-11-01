namespace Utils
{
    public interface IManager<in T> where T : IManagerSettings
    {
        void InitClientManager(T managerSettings);
        void InitServerManager(T managerSettings);
    }

    public interface IManagerSettings { }
}