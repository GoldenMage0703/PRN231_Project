using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO.Password
{
	public class EmailSettings
	{
		public string SmtpServer { get; set; }
		public int Port { get; set; }
		public string SenderEmail { get; set; }
		public string SenderPassword { get; set; }
	}
	public interface IEmailService
	{
		Task SendEmailAsync(string to, string subject, string body);
	}
	public class EmailService : IEmailService
	{
		private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailAsync(string to, string subject, string body)
		{
			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:SenderEmail"]));
			email.To.Add(MailboxAddress.Parse(to));
			email.Subject = subject;
			email.Body = new TextPart("plain") { Text = body };

			using var smtp = new SmtpClient();

			// Kết nối tới máy chủ SMTP của Gmail
			await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

			// Xác thực bằng tài khoản email và mật khẩu ứng dụng
			await smtp.AuthenticateAsync(_configuration["EmailSettings:SenderEmail"], _configuration["EmailSettings:SenderPassword"]);

			// Gửi email
			await smtp.SendAsync(email);

			// Ngắt kết nối
			await smtp.DisconnectAsync(true);
		}


	}
}
