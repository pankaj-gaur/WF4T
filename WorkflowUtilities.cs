using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using log4net;
using log4net.Config;
using Microsoft.Win32;
using Tridion.Logging;
using System.Text;

namespace Tridion.Extensions.EventSystem
{
    public class WorkflowUtilities
    {
        public static void sendWFNotification(string strFrom, string strTo, string strSubject, string strBody, string strSMTP, bool IsHtml)
        {
            String[] recipients = strTo.Split(';');
            if (recipients.Length == 0)
            {
                return;
            }
            MailMessage mailMsg = new MailMessage();
            SmtpClient smtpClient = new SmtpClient(strSMTP, 25);
            mailMsg.HeadersEncoding = Encoding.UTF8;
            mailMsg.BodyEncoding = Encoding.UTF8;
            mailMsg.SubjectEncoding = Encoding.UTF8;
            mailMsg.From = new MailAddress(strFrom); //Sender Mail Id
            // Add each recipient to the TO list
            foreach (string recipient in recipients)
            {
                mailMsg.To.Add(recipient);                   //Receiver Mail Id
            }
            //mailMsg.CC.Add(strCC);                   //Perfoemer Mail Id
            mailMsg.Subject = strSubject;            //Subject of the Mail   
            mailMsg.Body = strBody;                  //Text of the Mail  
            mailMsg.IsBodyHtml = IsHtml;
            smtpClient.Send(mailMsg);                   //Send the Mail
        }

        public static string GetTridionInstallPath()
        {
            string output = string.Empty;
            string regKeyPath = string.Empty;
            try
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    regKeyPath = @"Software\Wow6432Node\Tridion";
                }
                else
                {
                    regKeyPath = @"Software\Tridion";
                }
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey(regKeyPath);
                output = regKey.GetValue("InstallDir").ToString();
            }
            catch (Exception ex)
            {
                Logger.Write("Error retrieving Tridion's Install path. \n" + ex.Message, "WorkflowConfig", LoggingCategory.General, TraceEventType.Information);
            }
            return output;
        }

    }
}
