namespace ProductService.DAL.EmailService
{
    public interface IEmailServiceContext
    {
        HttpClient HttpClient { get; }
        string purchaseEndpoint { get; }
    }
}