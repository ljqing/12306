using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace _12306ByXX.Common
{
    public class EmailHelper
    {
        private readonly string _host = string.Empty;
        private readonly int _port = 25;

        private readonly string _fromEmailAddress = string.Empty;
        private readonly string _fromEmailPassword = string.Empty;

        #region [ 属性(邮件相关) ]

        /// <summary>
        /// 收件人 Email 列表，多个邮件地址之间用 半角逗号 分开
        /// </summary>
        public string ToList { get; set; }

        /// <summary>
        /// 邮件的抄送者，支持群发，多个邮件地址之间用 半角逗号 分开
        /// </summary>
        public string CcList { get; set; }

        /// <summary>
        /// 邮件的密送者，支持群发，多个邮件地址之间用 半角逗号 分开
        /// </summary>
        public string BccList { get; set; }

        /// <summary>
        /// 邮件标题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 邮件正文
        /// </summary>
        public string Body { get; set; }

        private bool _isBodyHtml = true;

        /// <summary>
        /// 邮件正文是否为Html格式
        /// </summary>
        public bool IsBodyHtml
        {
            get { return _isBodyHtml; }
            set { _isBodyHtml = value; }
        }

        /// <summary>
        /// 附件列表
        /// </summary>
        public List<Attachment> AttachmentList { get; set; }

        #endregion


        private EmailHelper(string host, int port, string fromAddress, string fromPassWord)
        {
            _host = host;
            _port = port;
            _fromEmailAddress = fromAddress;
            _fromEmailPassword = fromPassWord;
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        public void Send()
        {
            SmtpClient smtp = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = false,
                Host = _host,
                Port = _port,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(_fromEmailAddress, _fromEmailPassword),
            };

            MailMessage mailMessage = new MailMessage
            {
                Priority = MailPriority.Normal,
                From = new MailAddress(_fromEmailAddress, "订票提醒", Encoding.GetEncoding(936))
            };

            //收件人
            if (!string.IsNullOrEmpty(this.ToList))
                mailMessage.To.Add(this.ToList);
            //抄送人
            if (!string.IsNullOrEmpty(this.CcList))
                mailMessage.CC.Add(this.CcList);
            //密送人
            if (!string.IsNullOrEmpty(this.BccList))
                mailMessage.Bcc.Add(this.BccList);
            mailMessage.Subject = this.Subject; //邮件标题
            mailMessage.SubjectEncoding = Encoding.GetEncoding(936); //这里非常重要，如果你的邮件标题包含中文，这里一定要指定，否则对方收到的极有可能是乱码。
            mailMessage.IsBodyHtml = this.IsBodyHtml; //邮件正文是否是HTML格式
            mailMessage.BodyEncoding = Encoding.GetEncoding(936); //邮件正文的编码， 设置不正确， 接收者会收到乱码
            mailMessage.Body = this.Body; //邮件正文
            //邮件附件
            if (this.AttachmentList != null && this.AttachmentList.Count > 0)
            {
                foreach (Attachment attachment in this.AttachmentList)
                {
                    mailMessage.Attachments.Add(attachment);
                }
            }
            //发送邮件，如果不返回异常， 则大功告成了。
            smtp.Send(mailMessage);
        }

        private static string host = "smtp.sina.com";
        private static int port = 25;
        private static string frompwd = "XEt-gc2-CLy-VST";
        private static string from = "ljqing45@sina.com";

        public static bool Send(string email, string user, string train)
        {
            EmailHelper helper = new EmailHelper(host, port, from, frompwd);
            helper.ToList = email;
            string body = user + "您好：" + train + "已订票成功，请及时查看并处理";
            helper.Body = body;
            helper.IsBodyHtml = false;
            helper.Subject = "订票成功提醒";

            try
            {
                helper.Send();
                return true;
            }
            catch (Exception exception)
            {
                LogHelper.Error("发送邮件失败", exception);
                return false;
            }
        }

    }
}
