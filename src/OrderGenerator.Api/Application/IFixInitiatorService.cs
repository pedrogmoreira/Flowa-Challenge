namespace OrderGenerator.Api.Application;

public interface IFixInitiatorService
{
    Task<Models.OrderResponse> SendOrderAsync(Models.OrderRequest request, TimeSpan timeout);
}