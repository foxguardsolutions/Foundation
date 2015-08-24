namespace FGS.Pump.MVC.Support
{
    public interface ISessionAbstraction
    {
        T Get<T>(string sessionID, string key);

        object this[string sessionID, string key] { get; set; }

        string GetSessionID();
    }
}
