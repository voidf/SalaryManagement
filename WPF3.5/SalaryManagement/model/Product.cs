using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DataModel
{
    [Serializable]
    struct Product: IComparable<Product>
    {
        public const string extension = ".product";

        public int id { get; set; }

        public string type { get; set; }         // 机床型号
        public byte period { get; set; }          // 0: 白班
                                                  // 1: 晚班
                                                  // 2: 夜班
        public string process_name { get; set; }  // 工序名称
        public int count_begin { get; set; }
        public int count_end { get; set; }        // 数量序号
        public int count { get; set; }            // 加工数量
        public string ps { get; set; }            // 备注
        public int salary { get; set; }           // 工资结算（按分钱算，元需要/100）
        public string content { get; set; }       // 杂班内容

        public int CompareTo(Product other)
        {
            return id.CompareTo(other.id);
        } // https://stackoverflow.com/questions/3309188/how-to-sort-a-listt-by-a-property-in-the-object
    }
}
