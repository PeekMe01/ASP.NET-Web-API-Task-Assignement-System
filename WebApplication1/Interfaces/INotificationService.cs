using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface INotificationService
    {
        void NotifyUsernameChanged(User user);
    }
}
