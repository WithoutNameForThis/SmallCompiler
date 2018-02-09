using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 编译实验
{
    //中间代码列表
    class MLList
    {
        public List<MLanguage> List = new List<MLanguage>();

        //将在下一个插入四元式的位置
        public int WillBe()
        {
            return List.Count;
        }

        public void Add(string op, int arg1, int arg2, int result)
        {
            MLanguage ml = MLanguage.Get(op,arg1,arg2,result);

            List.Add(ml);
            
        }

        //按照其依赖的顺序进行排序 因为对表达式处理 可能导致生成的四元式其排列出现错误
        private void SortPr(int start,List<MLanguage> list)
        {


            for(int i = 0; i < this.List.Count; i++)
            {
                if (List.ElementAt(i).Result == start )
                {
                    list.Insert(0, List.ElementAt(i));
                    

                    SortPr(List.ElementAt(i).Arg1,list);
                    SortPr(List.ElementAt(i).Arg2,list);
                }
            }
  

        }
        public void Sort(int start)
        {
            List<MLanguage> list = new List<MLanguage>();
            this.SortPr(start,list);
            this.List = list;
        }

        //返回汇编代码 因为有汇编跳转的问题 不能确定跳转前的状态 导致无法进行对寄存器的优化
        //可能的解决方案 向四元式中插入不翻译但是用于标记的语句 对代码的一部分进行标记 记录不会被跳转进的代码段
        public string To8086Code(VarTable vt)
        {
            List<int> flg_label = new List<int>();

            string Result = "";

            string var_item = "";
            for(int i = 0; i < vt.VarTableA.Count; i++)
            {
                var_item += vt.VarTableA.ElementAt(i).Value + (i == vt.VarTableA.Count - 1?"":",");
            }


            Result +=
                "DATAS SEGMENT" + Environment.NewLine +
                "      symtable db " + var_item + Environment.NewLine +
                "      X DW 10000,1000,100,10,1"+Environment.NewLine+
                "DATAS ENDS" + Environment.NewLine +
                "STACKS SEGMENT" + Environment.NewLine +
                "STACKS ENDS" + Environment.NewLine +
                "CODES SEGMENT" + Environment.NewLine +
                "    ASSUME CS:CODES,DS:DATAS,SS:STACKS" + Environment.NewLine +
                "START:" + Environment.NewLine +
                "    MOV AX,DATAS" + Environment.NewLine +
                "    MOV DS,AX" + Environment.NewLine +
                "    MOV SI,offset X" + Environment.NewLine +
                "    xor dx,dx" + Environment.NewLine +
                Environment.NewLine;


            for (int i=0;i<this.List.Count;i++ )
            {
                MLanguage ml = this.List.ElementAt(i);
                if (ml.OP == "j="|| ml.OP == "j"||ml.OP == "j>" || ml.OP == "j<" || ml.OP == "j<=" || ml.OP == "j>="||ml.OP == "je" || ml.OP == "jz")
                {
                    flg_label.Add(ml.Result);
                }
                //if (InFlg(flg_label, i))
                //{
                    Result += "cmd" + i.ToString() + ":\r\n";
                //}
                

                Result += ml.To8086Code();

                if (i == List.Count - 1)
                {

                    Result += "cmd" + (i+1).ToString() + ":\r\n";
                    
                }

            }

            Result +=
                "MOV AH,4CH" + Environment.NewLine +
                "INT 21H" + Environment.NewLine + 
                Environment.NewLine + 
                Environment.NewLine +

                "PrintNum:\r\npush si\r\nmov bx,ax\r\nMOV CX,5\r\nl1:div word ptr [SI]\r\npush dx\r\nCMP CX,1\r\nJZ l2\r\ncmp dx,bx\r\njz skip ;\r\nl2: mov dl,al\r\nOR DL,30H\r\nmov ah,02h\r\nint 21h\r\nskip: pop ax\r\nxor dx,dx\r\nadd SI,2\r\nLOOP l1\r\npop si\r\npush dx  \r\nMOV DL,0DH  \r\nMOV AH,2  \r\nINT 21H  \r\nMOV DL,0AH  \r\nMOV AH,2  \r\nINT 21H  \r\npop dx\r\n\r\nret\r\n" + Environment.NewLine +

                "CODES ENDS" + Environment.NewLine +
                "    END START";


            return Result;

        }
        
    }
    
    //中间代码
    class MLanguage
    {
        public string OP = "";
        public int Arg1 = -1;
        public int Arg2 = -1;
        public int Result = -1;

  
        private MLanguage() { }
        public static MLanguage Get(string op, int arg1, int arg2, int result)
        {
            MLanguage ml = new MLanguage();
            ml.OP = op;
            ml.Arg1 = arg1;
            ml.Arg2 = arg2;
            ml.Result = result;
            return ml;

        }

        //单句生成中间代码
        public string To8086Code()
        {
            //直译
            switch (OP)
            {
                case "+":
                    return string.Format(" mov al,symtable[{0}]\r\n add al,symtable[{1}] \r\n mov symtable[{2}],al\r\n"
                        , Arg1.ToString(),Arg2.ToString(),Result.ToString());
                case "-":
                    return string.Format(" mov al,symtable[{0}]\r\n sub al,symtable[{1}] \r\n mov symtable[{2}],al\r\n"
                        , Arg1.ToString(), Arg2.ToString(), Result.ToString());
                case "*":
                    return string.Format(" mov ah,0\r\n mov al,symtable[{0}]\r\n mul symtable[{1}] \r\n mov symtable[{2}],al\r\n"
                        , Arg1.ToString(), Arg2.ToString(), Result.ToString());
                case "/":
                    return string.Format(" mov ah,0\r\n mov al,symtable[{0}]\r\n mul symtable[{1}] \r\n mov symtable[{2}],al\r\n"
                        , Arg1.ToString(), Arg2.ToString(), Result.ToString());
                case ":=":
                    return string.Format(" mov al,symtable[{0}]\r\n mov symtable[{1}],al\r\n"
                        , Arg1.ToString(),  Result.ToString());

                case "not":
                    return string.Format(" mov al,symtable[{0}]\r\n xor al,01h \r\n mov symtable[{1}],al\r\n"
                        , Arg1.ToString(),  Result.ToString());
                case "and":
                    return string.Format(" mov al,symtable[{0}]\r\n and al,symtable[{1}] \r\n mov symtable[{2}],al\r\n"
                        , Arg1.ToString(), Arg2.ToString(), Result.ToString());
                case "or":
                    return string.Format(" mov al,symtable[{0}]\r\n add al,symtable[{1}] \r\n mov symtable[{2}],al\r\n"
                        , Arg1.ToString(), Arg2.ToString(), Result.ToString());

                case "j=":
                    return string.Format(" mov al,symtable[{0}]\r\n cmp al,symtable[{1}]\r\n je cmd{2}\r\n"
                        , Arg1.ToString(), Arg2.ToString(),Result.ToString());
                case "j>":
                    return string.Format(" mov al,symtable[{0}]\r\n cmp al,symtable[{1}]\r\n jg cmd{2}\r\n"
                        , Arg1.ToString(), Arg2.ToString(), Result.ToString());
                case "j>=":
                    return string.Format(" mov al,symtable[{0}]\r\n cmp al,symtable[{1}]\r\n jnl cmd{2}\r\n"
                        , Arg1.ToString(), Arg2.ToString(), Result.ToString());
                case "j<=":
                    return string.Format(" mov al,symtable[{0}]\r\n cmp al,symtable[{1}]\r\n jna cmd{2}\r\n"
                        , Arg1.ToString(), Arg2.ToString(), Result.ToString());
                case "j<":
                    return string.Format(" mov al,symtable[{0}]\r\n cmp al,symtable[{1}]\r\n jl cmd{2}\r\n"
                        , Arg1.ToString(), Arg2.ToString(), Result.ToString());
                case "j":
                    return string.Format(" jmp cmd{0}\r\n"
                        ,  Result.ToString());
                case "jz":
                    return string.Format(" mov al,symtable[{0}]\r\n cmp al,0\r\n je cmd{1}\r\n"
                        , Arg1.ToString(),  Result.ToString());

                case "out":
                    return string.Format(" mov al,symtable[{0}]\r\n mov ah,0\r\n call PrintNum\r\n"
                        ,  Result.ToString());


                default:
                    break;
            }
            throw new Exception("命令遗失");
            
        }
    }
}
