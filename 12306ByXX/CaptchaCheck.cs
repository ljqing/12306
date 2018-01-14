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
        /// <summary>
        /// 验证码链接地址
        /// </summary>
        public string LinkAddress { get; set; }

        public string Token { get; set; }

        public string Agent { get; set; }

        private List<Point> _clickPoints = null;
        private const int ClickImgSize = 32;
       // private readonly string basePath = AppDomain.CurrentDomain.BaseDirectory;
        public CaptchaCheck()
        {
            InitializeComponent();
        }

        private void CaptchaCheck_Load(object sender, EventArgs e)
        {
            Agent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36";
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
            string address = "https://kyfw.12306.cn/otn/passcodeNew/getPassCodeNew?module={0}&rand={1}&{2}";
            if (LinkAddress == "1")
            {
                address = "https://kyfw.12306.cn/passport/captcha/captcha-image?login_site=E&module={0}&rand={1}&{2}";
            }
            string url = string.Format(address, type, rand, randomValue);
            Stream content = HttpHelper.Get(Agent, url, Cookie);
            if (content == null)
            {
                MessageBox.Show("加载验证码失败！");
                return;
            }
            LogHelper.Info("加载验证码成功！");
            try
            {
                Image mImage = Image.FromStream(content);
                pb_image.AutoSize = true;
                pb_image.BackgroundImage = mImage;
                if (content != null) content.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载验证码失败！");
                LogHelper.Error("验证码失败",ex);
            }

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
                bool checkResult = false;
                checkResult = LinkAddress == "1" ? CaptchaCheck1(RandCode) : CaptchaCheck0(RandCode);
                if (checkResult)
                {
                    Thread.Sleep(1000);
                    LogHelper.Info("验证码通过！");
                    Login();
                }
                else
                {
                    MessageBox.Show("验证码校验失败！");
                    LoadCaptchaImg();
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("登录失败，请重试！");
                LoadCaptchaImg();
                LogHelper.Error("CaptchaCheck()失败", ex);
            }
        }

        private bool CaptchaCheck0(string answer)
        {
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
                if (retEntity.data["result"] == "1")
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 验证码1
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        private bool CaptchaCheck1(string answer)
        {
            string postUrl = "https://kyfw.12306.cn/passport/captcha/captcha-check";
            string postDataStr = "answer=" + answer + "&login_site=E&rand=sjrand";
            if (Type == "1")
            {
                //todo 提交订单时验证码校验参数未确定
            }
            string content = HttpHelper.StringPost(Agent, postUrl, postDataStr, Cookie);
            Dictionary<string, string> retDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            if (retDic.ContainsKey("result_code") && retDic["result_code"].Equals("4"))
            {
                return true;
            }
            return false;
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
                    new Bitmap("../../Resources/click.png");
                
                g.DrawImage(clickImg, new Point(x - 10, y - 10));
                _clickPoints.Add(new Point(x - 10, y - 10));
            }
        }
    }
}
