using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using _12306ByXX.Common;

namespace _12306ByXX
{
    public partial class MainForm : Form
    {
        public void SetGridBoxText(string text)
        {
            this.gb_main.Text = text;
        }

        private List<Station> _lsStations;
        private Station _fromStation, _toStation;

        private const string _agent =
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36";

        private List<Passenger> _lsPassenger;

        private const string stationUrl =
            "https://kyfw.12306.cn/otn/resources/js/framework/station_name.js?station_version=1.8971";

        private bool isLoginOut = false;

        public List<Passenger> AllPassengers
        {
            set { _lsPassenger = value; }
        }

        private LoginForm _parentForm;
        public LoginForm ParenForm { get; set; }

        private CookieContainer _cookie = new CookieContainer();

        public CookieContainer Cookie
        {
            set { _cookie = value; }
        }

        private string _type = "ADULT";

        private string _date;

        private List<QueryTicket> tickets;

        private Dictionary<string, string> leftSeat;

        private const string defaultTicket = "----";
        public MainForm()
        {
            InitializeComponent();
            this.Closed += MainForm_Closed;
        }

        private void MainForm_Closed(object sender, EventArgs e)
        {
            if (isLoginOut)
            {
                ParenForm.Check.LoadCaptchaImg();
                ParenForm.Show();
            }
            else
            {
                Environment.Exit(Environment.ExitCode);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (_lsPassenger.Count > 0)
            {
                foreach (Passenger item in _lsPassenger)
                {
                    CheckBox checkBox = new CheckBox {Name = item.passenger_id_no, Text = item.passenger_name};
                    if (item.isUserSelf.Equals("Y"))
                    {
                        checkBox.Checked = true;
                    }
                    pl_passengers.Controls.Add(checkBox);
                }
            }
            LogHelper.Info("登录成功！");
            rb_normal.Checked = true;
            _lsStations = GetStations();
        }

        /// <summary>
        /// 获取车站
        /// </summary>
        /// <returns></returns>
        private List<Station> GetStations()
        {
            Stream stream = HttpHelper.Get(_agent, stationUrl, _cookie);
            StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
            string content = streamReader.ReadToEnd();
            content = content.Replace("var station_names =", "").Replace("'", "").Replace(";", "");

            string[] arrContents = content.Split(new[] {'@'}, StringSplitOptions.RemoveEmptyEntries);

            LogHelper.Info("获取车站成功！");

            return
                arrContents.Select(item => item.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries))
                    .Select(arrStation => new Station
                    {
                        Shorthand = arrStation[0],
                        Name = arrStation[1],
                        Code = arrStation[2],
                        Pinyin = arrStation[3],
                        FirstLetter = arrStation[4],
                        Order = arrStation[5]
                    }).ToList();
        }

        private void tb_station_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            string text = tb.Text.Trim();
            List<Station> ls =
                _lsStations.Where(
                    x =>
                        x.Name.Contains(text) || x.Pinyin.ToUpper().Contains(text.ToUpper()) ||
                        x.Shorthand.ToUpper().Contains(text.ToUpper()))
                    .ToList();
            BindingSource bs = new BindingSource();
            bs.DataSource = ls;
            if (tb.Name.Equals("tb_stationFrom"))
            {
                lb_from.DataSource = bs;
                lb_from.DisplayMember = "Name";
                lb_from.ValueMember = "Code";
                lb_from.Visible = true;
            }
            else
            {
                lb_to.DataSource = bs;
                lb_to.DisplayMember = "Name";
                lb_to.ValueMember = "Code";
                lb_to.Visible = true;
            }

        }

        private void lb_to_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListBox lb = sender as ListBox;

            Station station = lb.SelectedItem as Station;
            if (lb.Name.Equals("lb_from"))
            {
                tb_stationFrom.Text = station.Name;
                _fromStation = station;
                lb_from.Visible = false;
            }
            else
            {
                tb_stationTo.Text = station.Name;
                _toStation = station;
                lb_to.Visible = false;
            }
        }

        /// <summary>
        /// 获取选择的乘车人
        /// </summary>
        /// <returns></returns>
        private List<Passenger> GetPassaPassengers()
        {
            List<Passenger> passengers = new List<Passenger>();
            foreach (CheckBox checkBox in pl_passengers.Controls)
            {
                if (checkBox.Checked)
                {
                    Passenger item = _lsPassenger.FirstOrDefault(x => x.passenger_id_no.Equals(checkBox.Name));
                    passengers.Add(item);
                    LogHelper.Info("乘车人：" + item.passenger_name);
                }
            }
            return passengers;
        }

        /// <summary>
        /// 获取坐次
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetSeatType()
        {
            List<string> ls = new List<string>();
            foreach (CheckBox ckBox in flp_seatTypes.Controls)
            {
                if (ckBox.Checked)
                {
                    ls.Add(ckBox.Name.Split('_')[1]);
                }
            }
            return ls;
        }

        private string queryUrl = "";
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private void btn_search_Click(object sender, EventArgs e)
        {
            DateTime selected = dtpicker.Value.Date;
            if (selected < DateTime.Now.Date)
            {
                MessageBox.Show("出发日期不能小于当前日期！");
                return;
            }
            if (_fromStation == null)
            {
                MessageBox.Show("出发站不能为空！");
                return;
            }
            if (_toStation == null)
            {
                MessageBox.Show("到达站不能为空！");
                return;
            }
            _date = dtpicker.Value.ToString("yyyy-MM-dd");
            if (rb_student.Checked == true)
            {
                _type = "0X00";
            }
            string initUrl = "https://kyfw.12306.cn/otn/leftTicket/init";
            string htmlContent = HttpHelper.StringGet(_agent, initUrl, _cookie);
            string regexStr = @"var CLeftTicketUrl = '(.*?)'";
            string leftTicketUrl = Regex.Match(htmlContent, regexStr).Value.Replace("'", "").Split('=')[1].Trim();
            queryUrl =
                string.Format(
                    "https://kyfw.12306.cn/otn/{0}?leftTicketDTO.train_date={1}&leftTicketDTO.from_station={2}&leftTicketDTO.to_station={3}&purpose_codes={4}",
                    leftTicketUrl, _date, _fromStation.Code, _toStation.Code, _type);
            LogHelper.Info("余票查询URL:" + queryUrl);
            try
            {
                timer =new System.Windows.Forms.Timer {Interval = 3000};
                timer.Tick += timer_Tick;
                if (ckb_autoQuery.Checked)
                {
                    timer.Enabled = true;
                }
                else
                {
                    if (!QueryTickets(queryUrl))
                    {
                        MessageBox.Show("系统异常！");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("QueryTicket error", ex);
                MessageBox.Show(ex.Message);
                return;
            }

        }

        private int i = 0;

        private void timer_Tick(object sender, EventArgs e)
        {
            i = i + 1;
            QueryTickets(queryUrl);
            string msg = "第" + i + "次查询";
            lb_queryInfo.Text = msg;
            LogHelper.Info(msg);
        }


        private bool QueryTickets(string queryUrl)
        {
            string content = HttpHelper.StringGet(_agent, queryUrl, _cookie);
            try
            {
                //转json
                QueryJsonEntity contentEntity =
                    JsonConvert.DeserializeObject<QueryJsonEntity>(content);

                Dictionary<string, string> trainDictionary =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(contentEntity.data.map.ToString());

                tickets = new List<QueryTicket>();
                if (contentEntity.data.result.Count > 0)
                {
                    List<string> ticketsContent = contentEntity.data.result;
                    foreach (var s in ticketsContent)
                    {
                        QueryTicket ticket = new QueryTicket();
                        string[] item = s.Split('|');
                        ticket.SecretStr = item[0];
                        ticket.Remark = item[1];
                        ticket.Train_No = item[2];
                        ticket.Station_Train_Code = item[3];
                        ticket.From_Station_Name = trainDictionary[item[6]];
                        ticket.To_Station_Name = trainDictionary[item[7]];
                        ticket.Start_Time = item[8];
                        ticket.Arrive_Time = item[9];
                        ticket.LastedTime = item[10];
                        ticket.Gr_Num = ScreenInfo(item[21]);
                        ticket.Qt_Num = ScreenInfo(item[22]);
                        ticket.Rw_Num = ScreenInfo(item[23]);
                        ticket.Rz_Num = ScreenInfo(item[25]);
                        ticket.Wz_Num = ScreenInfo(item[26]);
                        ticket.Yw_Num = ScreenInfo(item[28]);
                        ticket.Yz_Num = ScreenInfo(item[29]);
                        ticket.Ze_Num = ScreenInfo(item[30]);
                        ticket.Zy_Num = ScreenInfo(item[31]);
                        ticket.Swz_Num = ScreenInfo(item[32]);
                        ticket.Dw_Num = ScreenInfo(item[33]);
                        tickets.Add(ticket);
                    }
                    dgv_tickets.AutoGenerateColumns = false;
                    dgv_tickets.DataSource = tickets;
                    dgv_tickets.DoubleBuffered(true);
                    dgv_tickets.Rows[0].Selected = false;
                    lb_queryInfo.Text = "余票查询成功！";
                }
                else
                {
                    LogHelper.Info("查询结果为空");
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(content,ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 默认显示 -
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string ScreenInfo(string item)
        {
            string info = defaultTicket;
            if (!string.IsNullOrEmpty(item))
            {
                info = item;
            }
            return info;
        }

        private void ckb_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void dgv_tickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgv_tickets.CurrentRow == null) return;
                if (dgv_tickets.SelectedRows.Count > 1)
                {
                    return;
                }
                string secretStr = dgv_tickets.CurrentRow.Cells["SecretStr"].Value.ToString();
                LogHelper.Info("车次secretStr：" + secretStr);

                QueryTicket selectedTrain = tickets.FirstOrDefault(x => x.SecretStr.Equals(secretStr));
                if (CheckIsNoTicket(selectedTrain))
                {
                    MessageBox.Show("此车次无票！");
                    return;
                }
                IEnumerable<string> arrySeats = leftSeat.Keys.ToList().Intersect(GetSeatType());
                if (!arrySeats.Any())
                {
                    MessageBox.Show("无此座位，请重新选择");
                    return;
                }
                string buySeat = arrySeats.FirstOrDefault();
                List<Passenger> selectedPassengers = GetPassaPassengers();
                if (selectedPassengers.Count == 0)
                {
                    MessageBox.Show("未选择乘客！");
                    return;
                }
                string msg = "";
                if (BuyTicket(secretStr, selectedPassengers, buySeat, selectedTrain, out msg))
                {
                    LogHelper.Info("订票成功！");
                }
                else
                {
                    MessageBox.Show(msg);
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show("系统异常,请重试！");
                LogHelper.Error("error:",exception);
            }
           
        }
        /// <summary>
        /// 时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        } 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void login_out_Click(object sender, EventArgs e)
        {
            const string loginOutUrl = "https://kyfw.12306.cn/otn/login/loginOut";
            HttpHelper.Get(_agent, loginOutUrl, _cookie);
            isLoginOut = true;
            this.Close();
        }
        /// <summary>
        /// 是否有票
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        private bool CheckIsNoTicket(QueryTicket ticket)
        {
            leftSeat =new Dictionary<string, string>();
            bool result = true;
            var t = ticket.GetType();
            //由于我们只在Property设置了Attibute,所以先获取Property
            var properties = t.GetProperties();
            foreach (var property in properties)
            {
                if (!property.IsDefined(typeof (SeatAttribute), false)) continue;
                var propertyValue = property.GetValue(ticket) as string;
                if (propertyValue != null && (!propertyValue.Equals(defaultTicket) && !propertyValue.Equals("无")))
                {
                    result = false;
                    SeatAttribute attribute = (SeatAttribute)property.GetCustomAttributes(typeof(SeatAttribute), true).FirstOrDefault();
                    leftSeat.Add(attribute.Code, attribute.Name);
                }
            }
            return result;
        }

        /// <summary>
        /// 预提交订单
        /// </summary>
        /// <param name="secretStr"></param>
        /// <returns></returns>
        private bool SubmitOrderRequest(string secretStr,out string msg)
        {
            msg = "";
            try
            {
                string url = "https://kyfw.12306.cn/otn/leftTicket/submitOrderRequest";
                string data =
                    string.Format(
                        "secretStr={0}&train_date={1}&back_train_date={2}&tour_flag=dc&purpose_codes={3}&query_from_station_name={4}&query_to_station_name={5}&undefined=",
                        secretStr,
                        _date, DateTime.Now.ToString("yyyy-MM-dd"), _type, _fromStation.Name, _toStation.Name);
                string ret = HttpHelper.StringPost(_agent, url, data, _cookie);
                HttpJsonEntity<string> retEntity =
                    JsonConvert.DeserializeObject<HttpJsonEntity<string>>(ret);
                if (retEntity.messages.Count > 0)
                {
                    msg = retEntity.messages[0];
                }
                return retEntity.status.ToUpper().Equals("TRUE") && retEntity.httpstatus.Equals(200);
            }
            catch (Exception ex)
            {
                LogHelper.Error("SubmitOrderRequest",ex);
                return false;
            }
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <returns></returns>
        private InitInfo GetInitInfo()
        {

            InitInfo info = new InitInfo();
            string url = "https://kyfw.12306.cn/otn/confirmPassenger/initDc";
            string data = "_json_att=";
            string response = HttpHelper.StringPost(_agent, url, data, _cookie);
            string regexStr = @"var globalRepeatSubmitToken = '(.*?)'";
            string submitToken = Regex.Match(response, regexStr).Value.Replace("'", "").Split('=')[1].Trim();
            info.SubmitToken = submitToken;

            regexStr = @"'leftTicketStr':'(.*?)'";
            string leftInfo = Regex.Match(response, regexStr).Value.Replace("'", "").Split(':')[1].Trim();
            info.LeftTicketInfo = leftInfo;

            regexStr = @"'key_check_isChange':'(.*?)'";
            string keyCheck = Regex.Match(response, regexStr).Value.Replace("'", "").Split(':')[1].Trim();
            info.KeyCheck = keyCheck;

            regexStr = @"'train_location':'(.*?)'";
            string location = Regex.Match(response, regexStr).Value.Replace("'", "").Split(':')[1].Trim();
            info.Location = location;

            regexStr = @"'purpose_codes':'(.*?)'";
            string codes = Regex.Match(response, regexStr).Value.Replace("'", "").Split(':')[1].Trim();
            info.PurposeCodes = codes;

            LogHelper.Info("initDc:" + url + "成功！");
            return info;
        }

        /// <summary>
        /// 核查订单
        /// </summary>
        /// <param name="passengers"></param>
        /// <param name="buySeat"></param>
        /// <param name="info"></param>
        /// <param name="randCode"></param>
        /// <param name="passengerTicketStr"></param>
        /// <param name="oldPassengerStr"></param>
        /// <returns></returns>
        private bool CheckOrderInfo(List<Passenger> passengers, string buySeat, InitInfo info, out string randCode,
            out string passengerTicketStr, out string oldPassengerStr)
        {
            randCode = "";
            passengerTicketStr = string.Empty;
            oldPassengerStr = string.Empty;
            try
            {
                string url = "https://kyfw.12306.cn/otn/confirmPassenger/checkOrderInfo";
                foreach (Passenger passenger in passengers)
                {
                    string passengerticket = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", buySeat, "0", "1",
                        passenger.passenger_name, passenger.passenger_id_type_code, passenger.passenger_id_no,
                        passenger.mobile_no, 'N');
                    passengerTicketStr = passengerTicketStr + passengerticket + "_";

                    string oldPassenger = string.Format("{0},{1},{2},{3}", passenger.passenger_name,
                        passenger.passenger_id_type_code, passenger.passenger_id_no, passenger.passenger_type);
                    oldPassengerStr = oldPassengerStr + oldPassenger + "_";
                }

                string data =
                    string.Format(
                        "cancel_flag=2&bed_level_order_num=000000000000000000000000000000&passengerTicketStr={0}&oldPassengerStr={1}&tour_flag=dc&randCode=&_json_att=&REPEAT_SUBMIT_TOKEN={2}",
                        passengerTicketStr.TrimEnd('_'), oldPassengerStr, info.SubmitToken);
                LogHelper.Info("CheckOrderInfo data+" + data);
                HttpJsonEntity<Dictionary<string, string>> contentEntity =
                    HttpHelper.Post(_agent, url, data, _cookie);
                if (contentEntity.status.ToUpper().Equals("TRUE") && contentEntity.httpstatus.Equals(200))
                {
                    if (contentEntity.data.ContainsKey("ifShowPassCode") &&
                        contentEntity.data["ifShowPassCode"].ToUpper().Equals("Y"))
                    {
                        ImgCodeForm form = new ImgCodeForm();
                        form.Cookie = _cookie;
                        form.Agent = _agent;
                        randCode = form.RandCode;
                        form.ShowDialog();
                    }
                    LogHelper.Info("checkOrderInfo" + url + "成功");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Error("CheckOrderInfo", ex);
                return false;
            }

        }

        /// <summary>
        /// 获取排队数
        /// </summary>
        /// <param name="selectedTrain"></param>
        /// <param name="buySeat"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool GetQueueCount(QueryTicket selectedTrain, string buySeat, InitInfo info)
        {
            try
            {
                string url = "https://kyfw.12306.cn/otn/confirmPassenger/getQueueCount";
                DateTime dt;
                DateTime.TryParse(_date, out dt);

                string trainDate = UrlEncode(dt.ToString("ddd MMM dd yyyy ",
                    CultureInfo.CreateSpecificCulture("en-GB")) + "00:00:00 GMT+0800 (CST)");
                string fromStationTelecode = _fromStation.Code;
                string toStationTelecode = _toStation.Code;
                string stationTrainCode = selectedTrain.Station_Train_Code;
                string trainNo = selectedTrain.Train_No;

                string data =
                    string.Format(
                        "train_date={0}&train_no={1}&stationTrainCode={2}&seatType={3}&fromStationTelecode={4}&toStationTelecode={5}&leftTicket={6}&purpose_codes={7}&train_location={8}&_json_att=&REPEAT_SUBMIT_TOKEN={9}",
                        trainDate, trainNo, stationTrainCode, buySeat, fromStationTelecode, toStationTelecode,
                        info.LeftTicketInfo, info.PurposeCodes, info.Location, info.SubmitToken);
                HttpJsonEntity<Dictionary<string, string>> contentEntity =
                    HttpHelper.Post(_agent, url, data, _cookie);
                LogHelper.Info(selectedTrain.Station_Train_Code + " GetQueueCount成功");
                return contentEntity.status.ToUpper().Equals("TRUE") && contentEntity.httpstatus.Equals(200);
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetQueueCount", ex);
                return false;
            }
        }

        /// <summary>
        /// 确认购票
        /// </summary>
        /// <param name="passengerTicketStr"></param>
        /// <param name="oldPassengerStr"></param>
        /// <param name="randCode"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool ConfirmSingleForQueue(string passengerTicketStr, string oldPassengerStr, string randCode,
            InitInfo info)
        {
            try
            {
                string url = "https://kyfw.12306.cn/otn/confirmPassenger/confirmSingleForQueue";
                string data =
                    string.Format(
                        "passengerTicketStr={0}&oldPassengerStr={1}&randCode={2}&purpose_codes={3}&key_check_isChange={4}C&leftTicketStr={5}&train_location={6}&choose_seats=&seatDetailType=000&whatsSelect=1&roomType=00&dwAll=N&_json_att=&REPEAT_SUBMIT_TOKEN={7}",
                        passengerTicketStr, oldPassengerStr, randCode, info.PurposeCodes, info.KeyCheck, info.LeftTicketInfo,
                        info.Location,
                        info.SubmitToken);
                HttpJsonEntity<Dictionary<string, string>> contentEntity =
                     HttpHelper.Post(_agent, url, data, _cookie);
                return contentEntity.status.ToUpper().Equals("TRUE") && contentEntity.httpstatus.Equals(200);
            }
            catch (Exception ex)
            {
                LogHelper.Error("ConfirmSingleForQueue", ex);
                return false;
            }
        }

        /// <summary>
        /// 购票
        /// </summary>
        /// <param name="info"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool QueryOrderWaitTime(InitInfo info, out string msg)
        {
            msg = "购票失败";
            try
            {
                string timeSpan = GetTimeStamp();
                string url =
                    string.Format(
                        "https://kyfw.12306.cn/otn/confirmPassenger/queryOrderWaitTime?random={0}&tourFlag=dc&_json_att=&REPEAT_SUBMIT_TOKEN={1}",
                        timeSpan, info.SubmitToken);
                string response = HttpHelper.StringGet(_agent, url, _cookie);
                HttpJsonEntity<Dictionary<string, string>> contentEntity =
                    JsonConvert.DeserializeObject<HttpJsonEntity<Dictionary<string, string>>>(response);
                if (contentEntity.status.ToUpper().Equals("TRUE") &&
                    contentEntity.httpstatus.Equals(200))
                {
                    if (contentEntity.data.ContainsKey("msg"))
                    {
                        msg = contentEntity.data["msg"];
                        return false;
                    }
                    if (contentEntity.data.ContainsKey("orderId") &&
                        !string.IsNullOrEmpty(contentEntity.data["orderId"]))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Error("QueryOrderWaitTime", ex);
                return false;
            }

        }

        private void ckb_autoQuery_CheckedChanged(object sender, EventArgs e)
        {
            if (!ckb_autoQuery.Checked)
            {
                i = 0;
                lb_queryInfo.Text = "";
                timer.Stop();
            }
            else if(ckb_autoQuery.Checked)
            {
                timer.Start();
            }
        }

        private List<string> _lsSecretStr = new List<string>();
        private bool isAutoBuy = false;
        private void btn_autoBuy_Click(object sender, EventArgs e)
        {
            try
            {
                if (_lsSecretStr.Count == 0)
                {
                    MessageBox.Show("请先选择车次！");
                    return;
                }
                buyTimer = new System.Windows.Forms.Timer();
                buyTimer.Interval = 3000;
                if (isAutoBuy)
                {
                    isAutoBuy = false;
                    buyTimer.Stop();
                    btn_autoBuy.Text = "抢票";
                    return;
                }
                else
                {
                    isAutoBuy = true;
                    btn_autoBuy.Text = "暂停";
                    buyTimer.Enabled = true;
                }
                buyTimer.Tick += buyTimer_Tick;
            }
            catch (Exception ex)
            {
               LogHelper.Error("购票失败！",ex);
            }

        }

        int j = 0;
        private System.Windows.Forms.Timer buyTimer;

        private void buyTimer_Tick(object sender, EventArgs e)
        {
            foreach (string secretStr in _lsSecretStr)
            {
                j++;
                QueryTicket selectedTrain = tickets.FirstOrDefault(x => x.SecretStr.Equals(secretStr));
                if (selectedTrain == null) continue;
                LogHelper.Info("第" + j + "次购票：" + selectedTrain.Station_Train_Code);
                bool noTicket = CheckIsNoTicket(selectedTrain);
                bool noSeat = false;
                IEnumerable<string> arrySeats = leftSeat.Keys.ToList().Intersect(GetSeatType());
                string buySeat = arrySeats.FirstOrDefault();
                if (!arrySeats.Any())
                {
                    noSeat = true;
                }
                if (noTicket || noSeat)
                {
                    LogHelper.Info(selectedTrain.Station_Train_Code + "无票");
                    QueryTickets(queryUrl);
                    continue;
                }
                List<Passenger> selectedPassengers = GetPassaPassengers();
                string msg = "";
                if (BuyTicket(secretStr, selectedPassengers, buySeat, selectedTrain, out msg))
                {
                    buyTimer.Stop();
                }
            }
        }


        private bool BuyTicket(string secretStr, List<Passenger> selectedPassengers, string buySeat, QueryTicket selectedTrain,out string msg)
        {
            if (SubmitOrderRequest(secretStr, out msg))
            {
                lb_queryInfo.Text = "预提交订单成功！";
                InitInfo info = GetInitInfo();
                lb_queryInfo.Text = "获取页面信息成功！";
                if (info != null)
                {
                    string randCode, passengerTicketStr, oldPassengerStr;
                    if (CheckOrderInfo(selectedPassengers, buySeat, info, out randCode, out passengerTicketStr,
                        out oldPassengerStr))
                    {
                        lb_queryInfo.Text = "核查订单成功！";
                        if (GetQueueCount(selectedTrain, buySeat, info))
                        {
                            lb_queryInfo.Text = "获取排队人数成功！";
                            if (ConfirmSingleForQueue(passengerTicketStr, oldPassengerStr, randCode, info))
                            {
                                lb_queryInfo.Text = "开始排队！";
                                MessageBox.Show(QueryOrderWaitTime(info, out msg) ? "订票成功，请及时查询及支付订单！" : msg);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void ckb_multiBuy_CheckedChanged(object sender, EventArgs e)
        {

            if (ckb_multiBuy.Checked)
            {
                dgv_tickets.MultiSelect = true;
            }
        }

        private void dgv_tickets_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var rows = dgv_tickets.SelectedRows;
            foreach (DataGridViewRow row in rows)
            {
                string str = row.Cells["SecretStr"].Value.ToString();
                string trainNo = row.Cells["TrianCode"].Value.ToString();
                if (!string.IsNullOrEmpty(str))
                {
                    _lsSecretStr.Add(str);
                }
                else
                {
                    MessageBox.Show(trainNo + " SecretStr为空，请重新查询或选择其他车次！");
                }
            }
        }
    }
}
