using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 编译实验
{
    //符号表 用其内部列表中的序列值作为其地址
    class VarTable
    {
        //符号列表
        List<VarItem> _VarTable = new List<VarItem>();



        //获取器
        public List<VarItem> VarTableA { get => _VarTable; }

        //添加指定类型
        public int AddVar(string VName,string Value,int Type)
        {
            foreach(VarItem vie in _VarTable)
            {
                if(vie.Name == VName)
                {
                    throw new Exception(VName+"已经被定义！");
                }
            }

            VarItem vi = new VarItem();
            vi.Value = Value;
            vi.Type = Type;
            vi.Name = VName;

            string Name = Guid.NewGuid().ToString();

            _VarTable.Add(vi);

            return _VarTable.Count-1;
        }

        //添加常量
        public int AddTemp(string Value)
        {
            return AddVar("UnNamed_" + _VarTable.Count.ToString(), Value, 0);
        }

        //添加临时变量
        public int AddTemp()
        {
            return AddVar("Temp_"+_VarTable.Count.ToString(), "0", 0);
        }

        //获取指定名称的变量名
        public int GetVar(string Name)
        {
            for(int i = 0; i < _VarTable.Count; i++)
            {
                if (_VarTable.ElementAt(i).Name == Name)
                {
                    return i;
                }
            }
            return -1;
        }

        public class VarItem
        {
            public int Type;
            public string Value;
            public string Name;
        }
    }
    

}
