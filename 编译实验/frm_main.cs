using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 编译实验
{
    public partial class frm_main : Form
    {
        public frm_main()
        {
            InitializeComponent();
        }

        private void frm_main_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(Char.IsSymbol('>').ToString());
            text_main.AcceptsTab = true;

            //文本框颜色
            SetColor(text_main, "while", Color.Blue);
            SetColor(text_main, "if", Color.Blue);
            SetColor(text_main, "var", Color.Blue);
            SetColor(text_main, "real", Color.Green);
            SetColor(text_main, "program", Color.DarkRed);
            SetColor(text_main, "begin", Color.Chocolate);
            SetColor(text_main, "end", Color.Brown);

            this.ProgramStart2Code();




        }





  


        private void text_tree_TextChanged(object sender, EventArgs e)
        {

        }

 

        private void tree_view_DoubleClick(object sender, EventArgs e)
        {
            tree_view.ExpandAll();
        }

 

        private void button1_Click_1(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
            MessageBox.Show("成功将代码复制到剪贴板");
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

            RichBoxSetColor();//设置关键字高亮 会导致显示的问题 因为文本框需要选中时才能改变颜色 而选中时会导致位置的改变 而使文本框上下移动
            this.ProgramStart2Code();
            //this.ProgramStart2CodeDebug();

        }
        private void RichBoxSetColor()
        {



            int start = text_main.SelectionStart;
            //文本框颜色
            SetColorBack(text_main);

            SetColor(text_main, "while", Color.Blue);
            SetColor(text_main, "do", Color.Blue);
            SetColor(text_main, "if", Color.Blue);
            SetColor(text_main, "var", Color.Blue);
            SetColor(text_main, "real", Color.Green);
            SetColor(text_main, "program", Color.DarkRed);
            SetColor(text_main, "begin", Color.Chocolate);
            SetColor(text_main, "end", Color.Brown);
            SetColor(text_main, "output", Color.MediumVioletRed);

            SetColorFinsh(text_main);

            text_main.SelectionStart = start;





        }

        private void ProgramStart2Code()
        {
            GrammarTree gt;
            WordList wl = null;
            VarTable vt;
            MLList mll;

            text_ex.Text = "";
            try
            {
                //词法分析
                wl = new WordList(text_main.Text);
                list_word_n.Items.Clear();
                int ii = 0;
                foreach (Word wd in wl.ListOfWord)
                {
                    ListViewItem lvi = new ListViewItem(ii++.ToString());
                    lvi.SubItems.Add(wd.Type.ToString());
                    lvi.SubItems.Add(wd.Name.ToString());
                    lvi.SubItems.Add(wd.Value != null ? wd.Value.ToString() : "null");
                    lvi.SubItems.Add(wd.PosStr.ToString());
                    list_word_n.Items.Add(lvi);
                }


                //语法分析
                gt = new GrammarTree();
                foreach (Word wd in wl.ListOfWord)
                {
                    gt.InsertWord(wd);
                }

                tree_view.Nodes.Clear();
                tree_view.Nodes.Add(gt.AdvToShow());


                //语义及四元式生成
                mll = new MLList();
                vt = new VarTable();
                gt.Head.Start(vt, mll);

                listView1.Items.Clear();
                int i = 0;
                foreach (VarTable.VarItem vi in vt.VarTableA)
                {

                    ListViewItem lvi = new ListViewItem(i++.ToString());
                    lvi.SubItems.Add(vi.Type.ToString());
                    lvi.SubItems.Add(vi.Name.ToString());
                    lvi.SubItems.Add(vi.Value != null ? vi.Value.ToString() : "null");

                    listView1.Items.Add(lvi);
                }
                listView2.Items.Clear();
                i = 0;
                foreach (MLanguage ml in mll.List)
                {
                    ListViewItem lvi = new ListViewItem(i++.ToString());
                    lvi.SubItems.Add(ml.OP.ToString());
                    lvi.SubItems.Add(ml.Arg1.ToString());
                    lvi.SubItems.Add(ml.Arg2.ToString());
                    lvi.SubItems.Add(ml.Result.ToString());

                    listView2.Items.Add(lvi);

                }
                //目标代码生成
                textBox1.Text = mll.To8086Code(vt);

            }
            catch (Exception ex)
            {
                //throw ex;
                if (ex.Message.Split(':')[0] == "语法错误")
                {
                    try
                    {
                        string pos = wl.ListOfWord.ElementAt(int.Parse(ex.Message.Split(':')[1])).PosStr.ToString();
                        text_ex.Text =
                            string.Format("语法错误 错误位置为:{0}行{1}列", pos.Split(':'));

                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
                else
                {
                    text_ex.Text = ex.Message;
                }

                //throw;
            }
        }
        private void ProgramStart2CodeDebug()
        {
            GrammarTree gt;
            WordList wl = null;
            VarTable vt;
            MLList mll;

            text_ex.Text = "";
            
            {
                //词法分析
                wl = new WordList(text_main.Text);
                list_word_n.Items.Clear();
                int ii = 0;
                foreach (Word wd in wl.ListOfWord)
                {
                    ListViewItem lvi = new ListViewItem(ii++.ToString());
                    lvi.SubItems.Add(wd.Type.ToString());
                    lvi.SubItems.Add(wd.Name.ToString());
                    lvi.SubItems.Add(wd.Value != null ? wd.Value.ToString() : "null");
                    lvi.SubItems.Add(wd.PosStr.ToString());
                    list_word_n.Items.Add(lvi);
                }


                //语法分析
                gt = new GrammarTree();
                foreach (Word wd in wl.ListOfWord)
                {
                    gt.InsertWord(wd);
                }

                tree_view.Nodes.Clear();
                tree_view.Nodes.Add(gt.AdvToShow());


                //语义及四元式生成
                mll = new MLList();
                vt = new VarTable();
                gt.Head.Start(vt, mll);

                listView1.Items.Clear();
                int i = 0;
                foreach (VarTable.VarItem vi in vt.VarTableA)
                {

                    ListViewItem lvi = new ListViewItem(i++.ToString());
                    lvi.SubItems.Add(vi.Type.ToString());
                    lvi.SubItems.Add(vi.Name.ToString());
                    lvi.SubItems.Add(vi.Value != null ? vi.Value.ToString() : "null");

                    listView1.Items.Add(lvi);
                }
                listView2.Items.Clear();
                i = 0;
                foreach (MLanguage ml in mll.List)
                {
                    ListViewItem lvi = new ListViewItem(i++.ToString());
                    lvi.SubItems.Add(ml.OP.ToString());
                    lvi.SubItems.Add(ml.Arg1.ToString());
                    lvi.SubItems.Add(ml.Arg2.ToString());
                    lvi.SubItems.Add(ml.Result.ToString());

                    listView2.Items.Add(lvi);

                }
                //目标代码生成
                textBox1.Text = mll.To8086Code(vt);

            }

        }

        private void SetColor(RichTextBox richBox,string word,Color color)
        {
            string find = word;
            
            if (richBox.Text.Contains(find))
            {
                var matchString = Regex.Escape(find);
                foreach (Match match in Regex.Matches(richBox.Text, matchString))
                {
                    richBox.Select(match.Index, find.Length);
                    richBox.SelectionColor = color;

                    //richBox.Select(match.Index + find.Length, 1);
                    //richBox.SelectionColor = Color.Black;

                };
            }
            
        }
        private void SetColorFinsh(RichTextBox richBox)
        {
            richBox.Select(richBox.TextLength, 0);
            richBox.SelectionColor = richBox.ForeColor;

        }

        private void SetColorBack(RichTextBox richBox)
        {
            richBox.Select(0, richBox.TextLength);
            richBox.SelectionColor = Color.Black;
            //richBox.Select(richBox.TextLength / 2, richBox.TextLength);
            //richBox.SelectionColor = Color.Black;


            //richBox.Select(richBox.TextLength, 0);
            //richBox.SelectionColor = Color.Black;
            //richBox.ForeColor = Color.Black;
        }

        private void btn_undo_Click(object sender, EventArgs e)
        {
            
        }

        private void text_main_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar == )
            //{
            //    MessageBox.Show();
            //}
            
        }

        private void list_word_n_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tree_view_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tree_view.SelectedNode == null) return;
            TreeNode tn = tree_view.SelectedNode;

            //MessageBox.Show(((GrammarTree.GrammarItem)tn.Tag).Num.ToString());


            GrammarTree.GrammarItem.TreeNodeTag tnt = ((GrammarTree.GrammarItem.TreeNodeTag)tn.Tag);
            if (tnt == null) return;
            if (tnt.Type < 1000)
            {
                text_main.Select(((Word)tnt.Item).Pos, ((Word)tnt.Item).Name.Length);
            }
            else
            {
                List<Word> lwd = new List<Word>();
                lwd = ((GrammarTree.GrammarItem)tnt.Item).GetAllWord();
                if (lwd.Count == 0) return;
                text_main.Select(lwd.First().Pos, lwd.Last().Pos+ lwd.Last().Name.Length - lwd.First().Pos);
            }


            //List<Word> lwd = ((GrammarTree.GrammarItem)tn.Tag).GetAllWord();
            //MessageBox.Show(lwd.First().Pos.ToString());
            //MessageBox.Show(lwd.Last().Pos.ToString());

            
           
        }

        private void tree_view_Click(object sender, EventArgs e)
        {

        }
    }
}
