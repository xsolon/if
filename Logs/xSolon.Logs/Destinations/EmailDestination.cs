// xSolon.Instructions.DTO - EmailDestination.cs
// Last edited by martin: 2014_09_24 12:03 AM
// Created : 2014_04_13

#region imports

using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;

#endregion

namespace xSolon.Instructions.DTO.Destinations
{
    public class EmailDestination : BaseLogDestination
    {
        public string From = string.Empty;

        public string SMTPServer = string.Empty;

        public string SubjectLine = string.Empty;

        public string To = string.Empty;

        public override void Commit()
        {
            try
            {
                var message = GetMessage(LogList);

                var mail = new MailMessage();

                mail.Body = message;

                mail.Subject = SubjectLine;

                mail.From = new MailAddress(From);

                To.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .ToList()
                    .ForEach(i => mail.To.Add(i));

                SmtpClient client = null;

                if (String.IsNullOrEmpty(SubjectLine))
                {
                    client = new SmtpClient();
                }
                else
                {
                    client = new SmtpClient(SMTPServer);
                }

                client.Send(mail);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        public override object Clone()
        {
            var res = new EmailDestination
            {
                AutoCommit = AutoCommit,
                Level = Level,
                LogList = LogList.ToList(),
                From = From,
                SMTPServer = SMTPServer,
                SubjectLine = SubjectLine,
                To = To
            };

            return res;
        }
    }
}