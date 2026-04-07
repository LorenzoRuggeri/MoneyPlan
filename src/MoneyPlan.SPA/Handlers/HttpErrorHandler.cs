using MoneyPlan.SPA.Services;
using Radzen;

namespace MoneyPlan.SPA.Handlers
{
    public class HttpErrorHandler : DelegatingHandler
    {
        private readonly HttpErrorBus _errorBus;

        public HttpErrorHandler(HttpErrorBus errorBus)
        {
            _errorBus = errorBus;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var response = await base.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
#if DEBUG
                _errorBus.Publish((int)response.StatusCode, request.RequestUri?.PathAndQuery ?? "");
#endif
            }

            return response;
        }
    }
}
