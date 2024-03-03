using System.Net;
using System.Net.Mail;

namespace SocialSite.Services;

/// <summary>
/// Static class that provides functionality for sending emails via SMTP.<br/>
/// </summary>
public static class MailService
{

    /// <summary>
    /// Asynchronously deliver a mail. WIP
    /// </summary>
    public static async Task SendAsync(MailAddress recipient, string confirmurl)
    {
        var fromAddress = new MailAddress("", "Social Noreply");
        const string fromUser = "";
        const string fromPassword = "";
        const string subject = "Confirm Social Login";
        const string instancename = "social";
        var body = $"""
            Greetings {recipient.User},

            someone requested to be logged in to your account at {instancename}.
            If this was not requested by you, don't click anything in this message.

            If you intended to log in, click this link to complete the process:
            {confirmurl}

            Regards,
            The mail robot at {instancename}
            """;

        NetworkCredential cred = new(fromUser, fromPassword);
        SmtpClient smtp = new()
        {
            Host = "smtp.example.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Credentials = cred
        };
        using MailMessage message = new(fromAddress, recipient)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        smtp.UseDefaultCredentials = false;

        await smtp.SendMailAsync(message);
    }
}
