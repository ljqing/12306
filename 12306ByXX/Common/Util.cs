using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _12306ByXX.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SeatAttribute : Attribute
    {
        public string Code { get; set; }

        public string Name { get; set; }
    }


    /// <summary>
    /// json实体类
    /// </summary>
    public class Data
    {
        /// <summary>
        /// 
        /// </summary>
        public string flag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object map { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> result { get; set; }
    }

    public class QueryJsonEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public Data data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int httpstatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string messages { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
    }

    public class QueryTicket
    {
        public string Train_No { get; set; }

        public string Station_Train_Code { get; set; }

        /// <summary>
        /// 发站电报码
        /// </summary>
        public string From_Station_TeleCode { get; set; }

        /// <summary>
        /// 发站名称
        /// </summary>
        public string From_Station_Name { get; set; }

        /// <summary>
        /// 到站电报码
        /// </summary>
        public string To_Station_TeleCode { get; set; }

        /// <summary>
        /// 到站名称
        /// </summary>
        public string To_Station_Name { get; set; }

        public string Start_Time { get; set; }

        public string Arrive_Time { get; set; }

        public int Day_Difference { get; set; }

        /// <summary>
        /// 历时
        /// </summary>
        public string LastedTime { get; set; }

        public string SecretStr { get; set; }

        [Seat(Code = "",Name = "其他")]
        public string Qt_Num { get; set; }
        [Seat(Code = "WZ", Name = "无座")]
        public string Wz_Num { get; set; }
        [Seat(Code = "1", Name = "硬座")]
        public string Yz_Num { get; set; }
        [Seat(Code = "99", Name = "软座")]
        public string Rz_Num { get; set; }
        [Seat(Code = "3", Name = "硬卧")]
        public string Yw_Num { get; set; }
        [Seat(Code = "F", Name = "动卧")]
        public string Dw_Num { get; set; }
        [Seat(Code = "4", Name = "软卧")]
        public string Rw_Num { get; set; }
        [Seat(Code = "6", Name = "高级软卧")]
        public string Gr_Num { get; set; }
        [Seat(Code = "O", Name = "二等座")]
        public string Ze_Num { get; set; }
        [Seat(Code = "M", Name = "一等座")]
        public string Zy_Num { get; set; }
        [Seat(Code = "9", Name = "商务座")]
        public string Swz_Num { get; set; }
    }

    public static class ExtensionMethods
    {
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }

    public class ValidateMessages
    {
    }

    public class HttpJsonEntity<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public string validateMessagesShowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int httpstatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public T data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> messages { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ValidateMessages validateMessages { get; set; }
    }

    public class InitInfo
    {
        public string SubmitToken { get; set; }
        public string LeftTicketInfo { get; set; }
        public string KeyCheck { get; set; }
        public string Location { get; set; }
        public string PurposeCodes { get; set; }

    }
}
