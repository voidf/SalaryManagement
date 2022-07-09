using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataModel
{
    [Serializable]
    struct Product
    {
        public string type;         // 机床型号
        public byte period;         // 0: 白班
                                    // 1: 晚班
                                    // 2: 夜班
        public string process_name; // 工序名称
        public int count_begin;
        public int count_end;       // 数量序号
        public int count;           // 加工数量
        public string ps;           // 备注
        public int salary;          // 工资结算（按分钱算，元需要/100）
        public string content;      // 杂班内容
    }
}
