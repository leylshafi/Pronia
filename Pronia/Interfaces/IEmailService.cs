namespace Pronia.Interfaces
{
	public interface IEmailService
	{
		Task SendMailAsync(string emailTo, string subject, string body, bool isHTML = false);
	}
}
