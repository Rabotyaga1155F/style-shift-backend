using Microsoft.AspNetCore.Identity;
using StyleShiftBackend.Models;

namespace StyleShiftBackend.Services
{
    public class NullEmailSender : IEmailSender<CustomUser>
    {
        public Task SendConfirmationLinkAsync(CustomUser user, string email, string confirmationLink) => Task.CompletedTask;
        public Task SendPasswordResetLinkAsync(CustomUser user, string email, string resetLink) => Task.CompletedTask;
        public Task SendEmailAsync(CustomUser user, string subject, string htmlMessage) => Task.CompletedTask;

        public Task SendPasswordResetCodeAsync(CustomUser user, string email, string resetCode)
        {
            throw new NotImplementedException();
        }
    }

}
