using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using _12306ByXX.Common;

namespace _12306ByXX
{
    public partial class CaptchaCheck : UserControl
    {
        public delegate void LoginDelegate();

        public LoginDelegate Login;
        public CookieContainer Cookie { get; set; }

        public string RandCode { get; set; }
        /// <summary>
        /// 0 登录 1购票
        /// </summary>
        public string Type { get; set; }

        public string Token { get; set; }
        public string Agent { get; set; }
        private List<Point> _clickPoints = null;
        private const int ClickImgSize = 32;
        private readonly string basePath = AppDomain.CurrentDomain.BaseDirectory;
        public CaptchaCheck()
        {
            InitializeComponent();
        }

        private void CaptchaCheck_Load(object sender, EventArgs e)
        {
            LoadCaptchaImg();
        }

        /// <summary>
        /// 加载验证码
        /// </summary>
        public void LoadCaptchaImg()
        {
            Random random = new Random();
            double randomValue = random.NextDouble();
            if (Cookie == null)
            {
                Cookie = new CookieContainer();
            }
            _clickPoints = new List<Point>();
            string type = "login";
            string rand = "sjrand";

            if (Type == "1")
            {
                type = "passenger";
                rand = "randp";
            }
            string url = string.Format("https://kyfw.12306.cn/otn/passcodeNew/getPassCodeNew?module={0}&rand={1}&{2}", type, rand,randomValue);
            Stream content = HttpHelper.Get(Agent, url, Cookie);
            if (content == null)
            {
                MessageBox.Show("加载验证码失败！");
                return;
            }
            LogHelper.Info("加载验证码成功！");
            Image mImage = Image.FromStream(content);
            pb_image.AutoSize = true;
            pb_image.BackgroundImage = mImage;
            if (content != null) content.Close();
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            LoadCaptchaImg();
        }

        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            try
            {
                string answer = "";
                if (_clickPoints.Count > 0)
                {
                    answer = _clickPoints.Aggregate(answer,
                        (current, p) => current + (p.X + 10) + ',' + (p.Y - 20) + ',');
                }
                RandCode = answer.TrimEnd(',');
                LogHelper.Info("验证码为：" + RandCode);
                string postUrl = "https://kyfw.12306.cn/otn/passcodeNew/checkRandCodeAnsyn";
                string postDataStr = "randCode=" + answer.TrimEnd(',') + "&rand=sjrand";
                if (Type == "1")
                {
                    postDataStr = "randCode=" + answer.TrimEnd(',') + "&rand=randp&_json_att=&REPEAT_SUBMIT_TOKEN=" +
                                  Token;
                }
                HttpJsonEntity<Dictionary<string, string>> retEntity =
                    HttpHelper.Post(Agent, postUrl, postDataStr, Cookie);
                if (retEntity.status.ToUpper().Equals("TRUE") && retEntity.httpstatus.Equals(200))
                {
                    if (retEntity.data["result"] != "1")
                    {
                        MessageBox.Show("验证码校验失败！");
                        LoadCaptchaImg();
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        LogHelper.Info("验证码通过！");
                        Login();
                    }
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("登录失败，请重试！");
                LoadCaptchaImg();
                LogHelper.Error("CaptchaCheck()失败", ex);
            }
        }

        private void pb_image_MouseDown(object sender, MouseEventArgs e)
        {
            int x = e.Location.X;
            int y = e.Location.Y;
            if (e.Button != MouseButtons.Left) return;
            if (y <= 30) return;
            Point point =
                _clickPoints.FirstOrDefault(
                    p => p.X <= e.X && e.X <= p.X + ClickImgSize && p.Y <= e.Y && e.Y <= p.Y + ClickImgSize);
            Graphics g = pb_image.CreateGraphics();
            if (!point.IsEmpty)
            {
                //再次点击时取消点击标志（用背景将点击验证码图片覆盖）
                g.DrawImage(pb_image.BackgroundImage,
                    new Rectangle(point.X, point.Y, ClickImgSize, ClickImgSize),
                    new Rectangle(point.X, point.Y, ClickImgSize, ClickImgSize), GraphicsUnit.Pixel);
                _clickPoints.Remove(point);
            }
            else
            {
                Image clickImg =
                    new Bitmap(basePath + "/Resources/click.png");
                
                g.DrawImage(clickImg, new Point(x - 10, y - 10));
                _clickPoints.Add(new Point(x - 10, y - 10));
            }
        }
    }
}
