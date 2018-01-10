using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _12306ByXX.Common
{
    public class Passenger
    {
        //passenger_type_name':'\u6210\u4EBA','delete_time':'2017/10/23','isUserSelf':'N','passenger_id_type_code':'1',
        //'passenger_name':'\u67F3\u59E3','total_times':'99','passenger_id_type_name':'\u4E8C\u4EE3\u8EAB\u4EFD\u8BC1','passenger_type':'1','passenger_id_no':'421023198706154926','mobile_no':''}
        public string passenger_type_name { get; set; }
        public string delete_time { get; set; }
        public string isUserSelf { get; set; }

        public string passenger_id_type_code { get; set; }
        public string passenger_name { get; set; }
        public string total_times { get; set; }
        public string passenger_id_type_name { get; set; }
        public string passenger_type { get; set; }
        public string passenger_id_no { get; set; }
        public string mobile_no { get; set; }

    }
}
