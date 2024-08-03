namespace WebApplication1.Interfaces
{
    public interface IEmailNotification
    {
        void SendEmail(string to, string subject, string body);
    }
}
