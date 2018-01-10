using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _12306ByXX
{
    public partial class ImgCodeForm : Form
    {
        public string Agent { get; set; }

        public CookieContainer Cookie { get; set; }

        public string RandCode { get; set; }

        public ImgCodeForm()
        {
            InitializeComponent();
        }

        private void ImgCodeForm_Load(object sender, EventArgs e)
        {
            captchaCheck.Cookie = Cookie;
            captchaCheck.Agent = Agent;
            RandCode = captchaCheck.RandCode;
            captchaCheck.Login += Close;
        }
    }
}
