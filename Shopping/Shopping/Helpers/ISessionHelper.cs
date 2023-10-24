

namespace Shopping.Helpers
{
    public interface ISessionHelper
    {
        void SetObjectAsJson<T>(ISession session, string key, T value);
        T GetObjectFromJson<T>(ISession session, string key);
    }
}
