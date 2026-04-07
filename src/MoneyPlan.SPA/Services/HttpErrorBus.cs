namespace MoneyPlan.SPA.Services
{
    public class HttpErrorBus
    {
        public event Action<int, string>? OnHttpError;

        public void Publish(int statusCode, string path) =>
            OnHttpError?.Invoke(statusCode, path);
    }
}
