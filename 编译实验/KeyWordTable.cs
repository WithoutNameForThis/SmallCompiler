using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 编译实验
{
    public partial class WordList { 
        public static class KeyWordTable
        {
            //按照书中的关键字建立的
            private static string[] KeyWords = {
                "begin", "end", "integer","char","bool","real","input","output","program","read","write","for","to","while","do","repeat","until","if","then","else","true","false","var","const", //关键字
                "+","-","*","/","=","<",">","and","or","not","<=",">=","<>",":=",//算符
                "ID", //标识符
                "INT","REAL","CHAR","BOOL", //常数
                "(",")",":",".",";",",","_","'","","/*","*/" }; //界符
        
            //供词法分析器使用
            public static Word GetWord(string Word,string PosStr,int Pos)
            {
                if ("true" == Word|| "false" == Word)
                {
                    Word wd = new Word();
                    wd.Type = 42;
                    wd.Name = "BOOL";
                    wd.Value = Word;
                    wd.PosStr = PosStr;
                    wd.Pos = Pos;
                    //wd.Ref = null;
                    return wd;
                }

                for (int i = 0; i < KeyWords.Length; i++)
                {
                    if (KeyWords[i] == Word)
                    {
                        Word wd = new Word();
                        wd.Type = i;
                        wd.Name = Word;
                        wd.PosStr = PosStr;
                        wd.Pos = Pos;
                        //wd.Ref = null;
                        return wd;
                    }
                }
                Word wd_id = new Word();
                wd_id.Type = 38;
                wd_id.Name = Word;
                wd_id.PosStr = PosStr;
                wd_id.Pos = Pos;
                return wd_id;
            }
            public static Word GetSignWord(string Word,string PosStr, int Pos)
            {
                for (int i = 0; i < KeyWords.Length; i++)
                {
                    if (KeyWords[i] == Word)
                    {
                        Word wd = new Word();
                        wd.Type = i;
                        wd.Name = Word;
                        wd.PosStr = PosStr;
                        wd.Pos = Pos;
                        return wd;
                    }
                }
                throw new Exception("符号错误");
            }
            public static bool IsSign(string Word)
            {
                for (int i = 0; i < KeyWords.Length; i++)
                {
                    if (KeyWords[i] == Word)
                    {
                        Word wd = new Word();
                        wd.Type = i;

                        return true;
                    }
                }
                return false;
            }

        }
    }




}
