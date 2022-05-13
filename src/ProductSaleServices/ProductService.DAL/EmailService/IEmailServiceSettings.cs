namespace ProductService.DAL.EmailService
{
    public interface IEmailServiceSettings
    {
        public string EmailServiceAddress { get; set; }
        public string PurchaseEndpoint { get; set; }
    }
}