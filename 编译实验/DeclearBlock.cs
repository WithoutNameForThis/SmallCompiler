using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 编译实验
{
    partial class GrammarTree
    {
        /// <summary>
        /// 声明变量块的处理函数
        /// </summary>
        static class DeclearBlock
        {
            private static List<string> _IDList = new List<string>();
            private static int _Type = 0;

            //因为声明块的特殊结构 可以直接利用深度遍历对其进行处理
            public static void ProcessDeclearBlock(GrammarItem grammarItem,VarTable vt)
            {
                if (grammarItem.Sign == null) return;
                for(int i = 0; i < grammarItem.Sign.Length; i++)
                {
                    if (grammarItem.Sign[i] < 1000)
                    {
                        if (grammarItem.Sign[i] == 38)
                        {
                            _IDList.Add(((Word)grammarItem.Items[i]).Name);
                        }
                    }
                    else
                    {
                        if (grammarItem.Sign[i] == 1005|| grammarItem.Sign[i] == 1008)
                        {
                            ProcessDeclearBlock((GrammarItem)grammarItem.Items[i], vt);
                        }
                        if (grammarItem.Sign[i] == 1006)
                        {
                           _Type = ((GrammarItem)grammarItem.Items[i]).Sign[0];
                            Add2VarTable(vt);
                        }
                        if (grammarItem.Sign[i] == 1007)
                        {
                            if (((GrammarItem)grammarItem.Items[0]).Sign[0]==1004)
                            {
                                ProcessDeclearBlock((GrammarItem)grammarItem.Items[0],vt);
                            }               
                        }
                    }
                }


            }
            public static void Add2VarTable(VarTable vt)
            {
                foreach(string str in _IDList)
                {
                    vt.AddVar(str, "0", _Type);
                }
                _IDList.Clear();

            }

        }
    }



}
