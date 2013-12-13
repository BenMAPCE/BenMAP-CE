using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    /// <summary>
    /// 选择项类，用于ComboBox或者ListBox添加项
    /// </summary>
    public class ListItem
    {
        public ListItem(string sid, string sname)
        {
            _id = sid;
            _name = sname;
        }

        public override string ToString()
        {
            return this._name;
        }

        private string _id = string.Empty;
        public string ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        private string _name = string.Empty;
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }
    }
}
