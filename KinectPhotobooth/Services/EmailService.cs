using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KinectPhotobooth.Services
{
    public class EmailService
    {
        private string _fromEmail = "noreply@timelesseventsamarillo.com";
        private string _SMTPEmailServer = "mailtrap.io";
        private string _SMTPEmailUserID = "33983c83167720651";
        private string _SMTPEmailPassword = "2f8ddb1ab11376";
        private static readonly string _emailSubject = "Your Photo Booth Picture";


        //This is the body of the email being sent.  In the future, this should be a txt file which could be modified.  For now, it's a string :-(
        #region Email Body
        private static readonly string body = @"
<p style='font-size:24px'>Greetings from Timeless Events!</p>
<br>
We hope you had a fantastic time at <a href='http://timelesseventsamarillo.com'>Timeless Events</a>. The attached image is your photo taken at the photo booth.
<br>
<br>




Best,
<br>
Timeless Events
";
        #endregion

        public Task<bool> SendMail(string sendToEmail, BitmapEncoder bitmap)
        {

            bool returnValue = true;

            MailMessage msg = new MailMessage(_fromEmail, sendToEmail, _emailSubject, body);
                msg.BodyEncoding = System.Text.Encoding.Unicode;
                msg.IsBodyHtml = true;

            string filename = "PhotoboothPhoto.png";
            ContentType ct = new ContentType();
            ct.MediaType = MediaTypeNames.Image.Jpeg;
            ct.Name = filename;

            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream);
            stream.Position = 0;
            Attachment data = new Attachment(stream, filename);
            msg.Attachments.Add(data);
            Task<bool> t = new Task<bool>(() =>
            {
                try
                {
                    SmtpClient smtpClient = new SmtpClient(_SMTPEmailServer)
                    {
                        UseDefaultCredentials = false,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new NetworkCredential(_SMTPEmailUserID, _SMTPEmailPassword),
                    };

                    smtpClient.Send(msg);
                    smtpClient.Dispose();
                }
                catch (Exception ex)
                {

                     returnValue = false;
                }
                finally
                {
                    msg.Dispose();
                }

                return returnValue;
            });

            t.Start();




            return t;



        }
    }
}
