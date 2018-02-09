using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 编译实验
{
    //包装有根节点的语法树节点
    partial class GrammarTree
    {

        public GrammarItem Head = GrammarTable.GetGrammarItem(1000,8, null) ;
        

        int counter = 0;
        public GrammarTree()
        {

        }
        public void InsertWord(Word wd)
        {
            if (!Head.Filled())
            {
                
                Head.InsertWord(wd,ref counter);
                //MessageBox.Show("");
            }

        }
        public override string ToString()
        {
            return Head.ToString();
        }

        //public string AdvToString()
        //{
        //    string str = Head.ToString() + Environment.NewLine;
        //    str += Head.AdvToString();
        //    return str;
        //}

        public TreeNode AdvToShow()
        {
            return Head.AdvToShow();
        }


        //语法树节点 为了调试上的方便 将语法和语义分开处理了
        public class GrammarItem
        {
            public int[] Sign;
            public object[] Items;
            private int _Index=0;

            public int Num;
            public string ItemStr="";
            public string GrammarStr = "";//便于在语义分析中进行定位

            public GrammarItem _Father;//因为预测分析表的follow集求的不完全，导致插入的词语可能陷入为空的节点中，以该引用进行回溯。如果有完善的follow集的化，则不需回溯。
            public GrammarItem(GrammarItem father)
            {
                _Father = father;
            }


            

            public bool Filled()
            {
                if (Sign == null) return true;
                return _Index == Sign.Length;
            }

            //语法树构建
            public void InsertWord(Word wd,ref int counter)
            {

                //如果是终结符 则向引用表中添加词语 号码表位置值加一
                if (Sign[_Index] < 1000)
                {
                    if (Sign[_Index] == wd.Type)
                    {
                        Items[_Index] = wd;
                        _Index++;
                        counter++;
                    
                    }
                    else
                    {
                        throw new Exception("语法错误:"+(counter-1).ToString());
                    }
                }
                //如果是终结符 则向引用表中添加从GrammarTable获取的非终结符 号码表位置值不变
                else
                {

                    if (Items[_Index] == null)
                    {
                        GrammarItem temp_gi = GrammarTable.GetGrammarItem(Sign[_Index], wd.Type, this);

                        if (temp_gi == null)
                        {
                            throw new Exception("语法错误:" + (counter - 1).ToString());
                        }
                        else
                        {
                            Items[_Index] = temp_gi;

                        }
                    }


                    GrammarItem git = ((GrammarItem)Items[_Index]);
                    if (git.Filled())
                    {
                        _Index++;
                        if (this._Father == null)
                        {
                        
                            if (wd.Type == 99)
                            {
                                this.InsertWord(wd,ref counter);
                                return;//程序结束
                            }
                            else
                            {
                                throw new Exception("错误的结尾");
                            }

                        }
                        this._Father.InsertWord(wd,ref counter);//由于Follow集求的不完全 导致需要回溯寻找
                    
                    }
                    else
                    {
                        git.InsertWord(wd,ref counter);
                    
                    }
                }

            }

            public override string ToString()
            {
                string str = "Node<"+GrammarTable.GetName(Num)+">[" + Num.ToString()+"]:[" + ItemStr+"]";
                //if (Sign == null)
                //{
                //    str += "null]";
                //    return str;
                //}
                //for(int i=0;i< Sign.Length; i++)
                //{
                //    //if (Items == null)
                //    //{
                //    //    str = "null" + Environment.NewLine;
                //    //}
                //    //else
                //    //{
                //    //     str = Items[i].ToString()+Environment.NewLine;
                //    //}
                //    str +=  Sign[i].ToString()+ (i== Sign.Length-1?"": ",");



                //}
                return str;
            }

            ////以文本的形式显示树
            //public string AdvToString(int left=0)
            //{
            //    string str = "";

            //    if (Sign == null)
            //    {
            //        return str;
            //    }

            //    for(int i = 0; i < Items.Length; i++)
            //    {
            //        if (Sign[i] < 1000)
            //        {
            //            str += GetSpace(left + 2) + ((Word)Items[i]).ToString() + Environment.NewLine;
            //        }
            //        else
            //        {
            //            str += GetSpace(left + 2) + ((GrammarItem)Items[i]).ToString()+Environment.NewLine;
            //            str +=((GrammarItem)Items[i]).AdvToString(left+2) + Environment.NewLine;
            //        }
            //    }
            //    return str;

            //}
            //private string GetSpace(int Left)
            //{
            //    string str="";
            //    for(int i = 0; i < Left; i++)
            //    {
            //        str += ' ';
            //    }
            //    return str;
            //}


            //利用递归建立用于控件可以显示的树

            
            


            public TreeNode AdvToShow()
            {
                TreeNode tn = new TreeNode(this.ToString());
                if (tn != null)
                {
                    TreeNodeTag tnt = new TreeNodeTag();
                    tnt.Type = this.Num;
                    tnt.Item = this;
                    tn.Tag = tnt;
                }


                tn.ForeColor = System.Drawing.Color.DarkRed;
                if (Sign == null)
                {
                    return null;
                }

                for (int i = 0; i < Items.Length; i++)
                {
                    if (Sign[i] < 1000)
                    {
                        if (Items[i] != null)
                        {
                            TreeNode tnn = new TreeNode(((Word)Items[i]).ToString());
                            tnn.ForeColor = System.Drawing.Color.DarkBlue;

                            TreeNodeTag tnt = new TreeNodeTag();
                            tnt.Type = ((Word)Items[i]).Type;
                            tnt.Item = ((Word)Items[i]);
                            tnn.Tag = tnt;


                            tn.Nodes.Add(tnn) ;
                        }


                    }
                    else
                    {

                        if (((GrammarItem)Items[i]) != null)
                        {

                            TreeNode tnn = ((GrammarItem)Items[i]).AdvToShow();
                            //if(tnn!=null) tnn.Tag = (GrammarItem)Items[i];

                            if (tnn != null)
                            {
                        
                                tn.Nodes.Add(tnn);
                            }
                        }

                    }
                }
                return tn;

            }

            private void GetTreeNodePr(int num, List<GrammarItem> result)
            {
                if (Sign == null)
                {
                    return;
                }

                for (int i = 0; i < Items.Length; i++)
                {
                    if (Sign[i] < 1000)
                    {

                    }
                    else
                    {
                        if (Sign[i] == num)
                        {
                            result.Add((GrammarItem)Items[i]);
                        }
                        ((GrammarItem)Items[i]).GetTreeNodePr(num, result);
                    }

                }
            } 

            //Debug用 返回指定非终结符号的树
            public List<GrammarItem> GetTreeNode(int num)
            {
                List<GrammarItem> list = new List<GrammarItem>();
                this.GetTreeNodePr(num,list);
                return list;
            }

            private void GetAllWordPr(List<Word> lwd)
            {
                if (Sign == null) return;

                for (int i = 0; i < Sign.Length; i++)
                {
                    if (Sign[i] < 1000)
                    {
                        lwd.Add((Word)Items[i]);
                    }
                    else
                    {
                        ((GrammarItem)Items[i]).GetAllWordPr(lwd);
                    }
                }

            }
            public List<Word> GetAllWord()
            {
                List<Word> lwd = new List<Word>();
                this.GetAllWordPr(lwd);
                return lwd;
            }

            public class TreeNodeTag
            {
                public int Type;
                public object Item;
            }



            //其是否为空节点
            private bool IsEmpty()
            {
                return (this.ItemStr == "9000");
            }

            //数学表达式相关语义分析
            //获取指定树节点的出口地址值
            private int GetAddr(VarTable vt,MLList mlList)
            {
                if (this.GrammarStr == "1015>1100")
                {
                    return this.GetNode(0).GetAddr(vt,mlList);
                }
                if (this.GrammarStr == "1104>31")
                {
                    int result = vt.AddTemp(this.GetWord(0).Value);
                    return result;
                }
                if (this.GrammarStr == "1104>38")
                {
                    int result = vt.GetVar(this.GetWord(0).Name);
                    if (result == -1) throw new Exception("未定义的变量");

                    return result;
                }
                if (this.GrammarStr == "1102>1104")
                {
                    return this.GetNode(0).GetAddr(vt,mlList);
                }
                if (this.GrammarStr == "1102>43,1100,44")
                {
                    return GetNode(1).GetAddr(vt,mlList);
                }

                if (this.GrammarStr == "1101>1102,1103")
                {
                    int Left = GetNode(0).GetAddr(vt,mlList);
                    if (this.GetNode(1).IsEmpty())
                    {
                        return Left;
                    }

                    int Main = vt.AddTemp();

                
                    GetNode(1).Make(Main,Left, vt,mlList);
                    return Main;
                }
                if (this.GrammarStr == "1100>1101,1107")
                {
                    int Left = GetNode(0).GetAddr(vt,mlList);
                    if (GetNode(1).IsEmpty())
                    {
                        return Left;
                    }
                    else
                    {
                        int Now = vt.AddTemp();

                        this.GetNode(1).Make(Now, Left, vt,mlList);
                        return Now;
                    }
                
                }

                throw new Exception("程序错误");
            }

            //对指定出口地址值及其左节点地址进行处理 用于处理消除了左递归+*法的无法主节点无法找到右节点符号后右操作数的问题 包括四元式的生成。
            private void Make(int Main,int LeftT,VarTable vt,MLList mlList)
            {
                if (this.GrammarStr == "1103>9000")
                {
                    return;
                }
                if(this.GrammarStr== "1103>26,1102,1103"|| this.GrammarStr == "1103>27,1102,1103")
                {

                    if (this.GetNode(2).IsEmpty())
                    {
                    
                        mlList.Add(this.GetWord(0).Name, LeftT, this.GetNode(1).GetAddr(vt, mlList), Main);

                        return;
                    }
                    var now = vt.AddTemp();

                    mlList.Add(this.GetWord(0).Name, LeftT, now, Main);

                    this.GetNode(2).Make(now, GetNode(1).GetAddr(vt,mlList), vt,mlList);

                    return;
                }
                if(this.GrammarStr== "1107>24,1101,1107"|| this.GrammarStr == "1107>25,1101,1107")
                {
                    //int now = vt.AddTemp();
                    if ((GetNode(2)).IsEmpty())
                    {

                        mlList.Add(GetWord(0).Name, LeftT, GetNode(1).GetAddr(vt, mlList), Main);
                        return;
                    }
                    var now = vt.AddTemp();

                    mlList.Add(GetWord(0).Name, LeftT, now, Main);
                    GetNode(2).Make(now, GetNode(1).GetAddr(vt,mlList), vt,mlList);
                    return;
                }
                throw new Exception("程序错误");
            }

            //Bool相关语义分析 与数学表达式的方法类似
            private int BoolGetAddr(VarTable vt, MLList mlList,MLList subList)
            {
                if (this.GrammarStr == "1206>38,1207")
                {
                    if (GetNode(1).IsEmpty())
                    {
                        throw new Exception("未完成");
                        //return ((GrammarItem)this.Items[0]).GetAddr(vt, mlList);
                    }
                    int left = vt.GetVar(GetWord(0).Name);
                    int main = vt.AddTemp();

                    GetNode(1).BoolMake(main,left,vt,mlList, subList);
                    return main;

                }
                if(this.GrammarStr== "1206>43,1200,44")
                {
                    return GetNode(1).BoolGetAddr(vt, mlList, subList);
                }
                if (this.GrammarStr == "1205>1206")
                {
                    return GetNode(0).BoolGetAddr(vt, mlList, subList);
                }
                if (this.GrammarStr == "1205>33,1205")
                {
                    int past = GetNode(1).BoolGetAddr(vt, mlList,subList);
                    int now = vt.AddTemp();
                    mlList.Add("not", past, -1, now);
                    return now;
                }
                if (this.GrammarStr == "1201>1205,1204")
                {
                    int left = GetNode(0).BoolGetAddr(vt, mlList, subList);
                    if (GetNode(1).IsEmpty())
                    {
                        return left;
                    }
                    int main = vt.AddTemp();
                    GetNode(1).BoolMake(main, left, vt, mlList, subList);

                    return main;
                }
                if (this.GrammarStr == "1200>1201,1203")
                {
                    int left = GetNode(0).BoolGetAddr(vt,mlList,subList);
                    if (GetNode(1).IsEmpty())
                    {
                        return left;
                    }
                    int main = vt.AddTemp();
                    GetNode(1).BoolMake(main,left,vt,mlList,subList);

                    return main;



                }

                throw new Exception("程序错误");
            }
            private void BoolMake(int Main, int LeftT, VarTable vt, MLList mlList, MLList subList)
            {
                if (this.GrammarStr == "1207>1208,38")
                {
                    string op = ((Word)((GrammarItem)this.Items[0]).Items[0]).Name;
                    int right = vt.GetVar(((Word)this.Items[1]).Name);

                    //MessageBox.Show("Make J.." + "OP:" + op + ";Left:" + LeftT + ";Right:" + right+";Result:"+Main);

                    mlList.Add("j" + op, LeftT, right, mlList.WillBe() + 3);
                    mlList.Add(":=", 0, -1, Main);
                    mlList.Add("j", LeftT, right, mlList.WillBe() + 2);
                    mlList.Add(":=", 1, -1, Main);

                    //mlList.Add(op, LeftT, right, Main);

                    return;
                }
                if(this.GrammarStr== "1204>31,1205,1204")
                {
                    int right = ((GrammarItem)this.Items[1]).BoolGetAddr(vt, mlList, subList);
                    string op = ((Word)this.Items[0]).Name;

                    if (((GrammarItem)this.Items[2]).IsEmpty())
                    {
                        subList.Add(op, LeftT, right, Main);//up
                        return;
                    }
                    int now = vt.AddTemp();
                    subList.Add(op, LeftT, now, Main);//up
                    ((GrammarItem)this.Items[2]).BoolMake(now, right, vt, mlList, subList);
                    return;


                }
                if(this.GrammarStr == "1204>9000")
                {
                    return;
                }
                if(this.GrammarStr == "1203>32,1201,1203")
                {
                    int right = ((GrammarItem)this.Items[1]).BoolGetAddr(vt, mlList, subList);
                    string op = ((Word)this.Items[0]).Name;

                    if (((GrammarItem)this.Items[2]).IsEmpty())
                    {
                        subList.Add(op, LeftT, right, Main);//up
                        return;
                    }
                    int now = vt.AddTemp();
                    subList.Add(op, LeftT, now, Main);//up
                    ((GrammarItem)this.Items[2]).BoolMake(now, right, vt, mlList, subList);
                    return;
                }
                throw new Exception("程序错误");
            }

            //变量声明处理 用于定义True False常量
            public void DeclearVar(VarTable varTable)
            {
                varTable.AddVar("false", "0", 4);
                varTable.AddVar("true", "1", 4);

                DeclearPartPr(varTable);
            }
            private void DeclearPartPr(VarTable varTable)
            {
            
                if (Sign == null)
                {
                    return ;
                }

                for (int i = 0; i < Items.Length; i++)
                {
                    if (Sign[i] < 1000)
                    {
                   
                    }
                    else
                    {
                        if (Sign[i] == 1004)
                        {
                            DeclearBlock.ProcessDeclearBlock((GrammarItem)Items[i], varTable);
                        }
                        ((GrammarItem)Items[i]).DeclearPartPr(varTable);
                    }
                }
                return ;

            }


            //语句处理函数
            //赋值语句处理
            public void Assignment(VarTable vt,MLList mlList)
            {
                if (this.GrammarStr == "1013>38,37,1015")
                {
                    string VarName = ((Word)this.Items[0]).Name;

                    int Addr = vt.GetVar(VarName);

                    if (Addr == -1)
                    {
                        throw new Exception("没有定义该变量");
                    }
                    MLList temp_ml_list = new MLList();
                    int ResultAddr = ((GrammarItem)this.Items[2]).GetAddr(vt, temp_ml_list);
                    temp_ml_list.Sort(ResultAddr);

                    mlList.List.AddRange(temp_ml_list.List);
                    mlList.Add(":=", ResultAddr, -1, Addr);
                }
            }
            //if语句处理
            public void CondCommand(VarTable vt, MLList mlList)
            {
                if(this.GrammarStr == "1017>17,1200,18,1011,1022")
                {
                    MLList mll_temp = new MLList();
                    int boolAddr = GetNode(1).BoolGetAddr(vt, mlList,mll_temp );
                

                    mll_temp.Sort(boolAddr);
                    mlList.List.AddRange(mll_temp.List);

                    int insert_jp_false_cmd_ptr = mlList.WillBe();
                    mlList.Add("jz", boolAddr, -1, 0);

                    

                    this.GetNode(3).Command(vt, mlList);

                    

                    if (this.GetNode(4).IsEmpty())
                    {
                        int jp2_ptr = mlList.WillBe();
                        //mlList.Add("jmp_label", -1, -1, jp2_ptr);
                        
                        mlList.List.ElementAt(insert_jp_false_cmd_ptr).Result = jp2_ptr;
                        
                    }
                    else
                    {
                        int insert_jp_true_cmd_ptr = mlList.WillBe();
                        mlList.Add("j", boolAddr, -1, 0);
                        
                        int jp2_ptr = mlList.WillBe();
                        //mlList.Add("jmp_label", -1, -1, jp2_ptr);

                        mlList.List.ElementAt(insert_jp_false_cmd_ptr).Result = jp2_ptr;

                        //mlList.Add("reg_inti", -1, -1, -1);
                        this.GetNode(4).GetNode(1).Command(vt, mlList);
                        //mlList.Add("block_end", -1, -1, -1);

                        int jp_end_ptr = mlList.WillBe();
                        //mlList.Add("jmp_label", -1, -1, jp_end_ptr);
                        mlList.List.ElementAt(insert_jp_true_cmd_ptr).Result = jp_end_ptr;

                    }



                    return;
                    
                }
                throw new Exception("cant hap");
            
            }
            //while语句处理
            public void WhileCommand(VarTable vt,MLList mlList)
            {
                if(this.GrammarStr== "1018>13,1200,14,1011")
                {
                    int jp_cmd_head = mlList.WillBe();
                    //mlList.Add("jmp_label", -1, -1, jp_cmd_head);

                    MLList mll_temp = new MLList();
                    int boolAddr = GetNode(1).BoolGetAddr(vt, mlList, mll_temp);


                    mll_temp.Sort(boolAddr);
                    mlList.List.AddRange(mll_temp.List);


                    int insert_jp_false_cmd_ptr = mlList.WillBe();
                    mlList.Add("jz", boolAddr, -1, 0);//等待回填

                    this.GetNode(3).Command(vt, mlList);

                    mlList.Add("j", -1, -1, jp_cmd_head);
                    int after = mlList.WillBe();
                    //mlList.Add("jmp_label", -1, -1, after);

                    mlList.List.ElementAt(insert_jp_false_cmd_ptr).Result = after;
                    return;

                


                }
                throw new Exception("ex");


            }
            //显示语句处理
            public void OutputCommand(VarTable vt,MLList mlList)
            {
                if(this.GrammarStr== "1019>7,43,38,44")
                {
                    int targetAddr = vt.GetVar( this.GetWord(2).Name);
                    mlList.Add("out", -1, -1, targetAddr);
                }
            }

            //一般命令处理
            public void Command(VarTable vt,MLList mlList)
            {
                if(this.GrammarStr== "1011>1013")
                {
                    this.GetNode(0).Assignment(vt, mlList);
                    return;
                }
                if(this.GrammarStr== "1011>1014")
                {
                    this.GetNode(0).Command(vt, mlList);
                    return;
                }
                if(this.GrammarStr== "1014>1017")
                {
                    this.GetNode(0).CondCommand(vt, mlList);
                    return;
                }
                if (this.GrammarStr == "1014>1018")
                {
                    this.GetNode(0).WhileCommand(vt, mlList);
                    return;
                }
                if (this.GrammarStr== "1014>1003")
                {
                    this.GetNode(0).CommandBlock(vt, mlList);
                    return;
                }
                if(this.GrammarStr== "1014>1019")
                {
                    this.GetNode(0).OutputCommand(vt,mlList);
                    return;
                }

                throw new Exception("error");
                //MessageBox.Show("a command");
            }

            //复数命令处理
            public void Commandes(VarTable vt,MLList mlList)
            {
                if(this.GrammarStr== "1010>1011,47,1012")
                {
                    GetNode(0).Command(vt,mlList);
                    if (GetNode(2).IsEmpty())
                    {
                        return;
                    }
                    GetNode(2).Commandes(vt, mlList);
                    return;
                }
                if(this.GrammarStr == "1012>1010")
                {
                    GetNode(0).Commandes(vt,mlList);
                    return;
                }
                throw new Exception("cw");
            }
            //语句块处理
            public void CommandBlock(VarTable vt,MLList mlList)
            {
                if(this.GrammarStr== "1003>0,1010,1")
                {
                    GetNode(1).Commandes(vt, mlList);
                    return;
                }
                throw new Exception("程序错误");
            }
        
            //程序开始
            public void Start(VarTable vt,MLList mlList)
            {
                if(this.GrammarStr == "1000>8,38,1001,99")
                {
                    GetNode(2).Start(vt, mlList);
                    return;
                }
                if(this.GrammarStr== "1001>1002,1003")
                {
                    GetNode(0).DeclearVar(vt);
                    GetNode(1).CommandBlock(vt, mlList);
                    return;
                }
                throw new Exception("程序错误");
            }

            //强制类型转换
            private GrammarItem GetNode(int Index)
            {
                return ((GrammarItem)this.Items[Index]);
            }
            private Word GetWord(int Index)
            {
                return ((Word)this.Items[Index]);
            }

        
        }
    }


   

    

}
