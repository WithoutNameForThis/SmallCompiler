using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace 编译实验
{

    /// <summary>
    /// 词语类 用于保存输入词语的类型、名称、和值。
    /// </summary>
    public class Word
    {
        public int Type = -1;
        public string Name;
        public string Value;
        public string PosStr="";
        public int Pos = 0;

        public override string ToString()
        {
            return string.Format("Word<{1}>[{0}],Name[{1}],Value[{2}]", Type, (Name != null ? Name : "null"), (Value != null ? Value : "null"));
        }
    }



    
    /// <summary>
    /// 词语表 保存传入的参数字符串转为的词语
    /// </summary>
    public partial class WordList
    {
        /// <summary>
        /// 字符缓冲区
        /// </summary>
        public class CharBuffer
        {
            char[] _Buffer = new char[200];
            int _Ptr = 0;

            public void Insert(char c)
            {
                _Buffer[_Ptr] = c;
                _Ptr++;
            }
            public string Read()
            {
                return new string(_Buffer).Substring(0, _Ptr);
            }
            public void Clear()
            {
                _Buffer = new char[200];
                _Ptr = 0;
            }

        }


        CharBuffer _CharBuffer = new CharBuffer();
        List<Word> _ListOfWord;

        public List<Word> ListOfWord
        {
            get { return _ListOfWord; }
        }

        int crtf_count = 0;//行号
        int start_count = 0; //当前行位置
        int char_count = 0;//词语列数
        public WordList(string ProgramString)
        {
            _ListOfWord = new List<Word>();     

            int str_ptr = 0;
            for(str_ptr = 0; str_ptr < ProgramString.Length; str_ptr++)
            {
                char now_char = ProgramString[str_ptr];

                if (now_char == '\n')
                {
                    crtf_count ++;
                    start_count = str_ptr;
                }
                char_count = str_ptr - start_count;

                if(Char.IsDigit(now_char))
                {
                    str_ptr = RecogReal(ProgramString, str_ptr);
                }
                if (Char.IsLetter(now_char))
                {
                    str_ptr = RecogWord(ProgramString, str_ptr);
                }
                if (Char.IsSymbol(now_char) || Char.IsPunctuation(now_char))
                {
                    str_ptr = RecogSign(ProgramString, str_ptr);
                }

            }
            Word end_wd = new Word();
            end_wd.Type = 99;
            end_wd.Name = "#";
            end_wd.PosStr = crtf_count.ToString() + ":" + char_count.ToString();
            end_wd.Pos = ProgramString.Length;
            _ListOfWord.Add(end_wd);
        }


        /// <summary>
        /// 符号识别子程序
        /// </summary>
        /// <param name="str">当前字符串引用</param>
        /// <param name="start_index">当前位置</param>
        /// <returns>结束位置</returns>
        private int RecogSign(string str,int start_index)
        {
            int pos = start_index;
            for (int i = start_index; i < str.Length; i++)
            {
                char c = str[i];
                if (Char.IsSymbol(c)||Char.IsPunctuation(c))
                {
                    _CharBuffer.Insert(c);

                }
                else
                {
                    string temp = _CharBuffer.Read();
                    _CharBuffer.Clear();

                    _ListOfWord.Add(KeyWordTable.GetSignWord(temp, crtf_count.ToString() + ":" + char_count.ToString(), pos));

                    i--;
                    return i;
                }


            }
            throw new Exception(string.Format("未知的末尾:{0}行{1}列",crtf_count, char_count)); 
        }
        private int RecogWord(string str, int start_index)
        {
            int pos = start_index;
            for (int i = start_index; i < str.Length; i++)
            {
                char c = str[i];
                if (Char.IsLetter(c))
                {
                    _CharBuffer.Insert(c);
                }
                else
                {
                    string temp = _CharBuffer.Read();
                    _CharBuffer.Clear();

                    _ListOfWord.Add(KeyWordTable.GetWord(temp, crtf_count.ToString() + ":" + char_count.ToString(), pos));

                    i--;
                    return i;
                }
            }
            throw new Exception(string.Format("未知的末尾:{0}行{1}列", crtf_count, char_count));

        }
        private int RecogReal(string str, int start_index)
        {
            int _State = 0;
            int pos = start_index;
            for (int i = start_index; i < str.Length; i++)
            {
                char c = str[i];
                if (_State == 0)
                {
                    if (Char.IsDigit(c))
                    {
                        _State = 1;
                        _CharBuffer.Insert(c);
                        continue;
                    }
                    if (c == '.')
                    {
                        _State = 6;
                        _CharBuffer.Insert(c);
                        continue;
                    }

                    throw new Exception(string.Format("数字识别错误:{0}行{1}列", crtf_count, char_count));
                }
                if (_State == 1)
                {
                    if (Char.IsDigit(c))
                    {
                        _State = 1;
                        _CharBuffer.Insert(c);
                        continue;
                    }
                    if (c == '.')
                    {
                        _State = 2;
                        _CharBuffer.Insert(c);
                        continue;
                    }
                    if (c == 'E')
                    {
                        _State = 4;
                        _CharBuffer.Insert(c);
                        continue;
                    }
                    _State = 7;
                }
                if (_State == 2)
                {
                    if (Char.IsDigit(c))
                    {
                        _State = 2;
                        _CharBuffer.Insert(c);
                        continue;
                    }
                    if (c == 'E')
                    {
                        _State = 3;
                        _CharBuffer.Insert(c);
                        continue;
                    }
                    _State = 7;
                }
                if (_State == 3)
                {
                    if (c == '+' || c == '-')
                    {
                        _State = 4;
                        _CharBuffer.Insert(c);
                        continue;
                    }
                    if (Char.IsDigit(c))
                    {
                        _State = 5;
                        _CharBuffer.Insert(c);
                        continue;
                    }
                    throw new Exception(string.Format("数字识别错误:{0}行{1}列", crtf_count, char_count));
                }
                if (_State == 4)
                {
                    if (Char.IsDigit(c))
                    {
                        _State = 5;
                        _CharBuffer.Insert(c);
                        continue;
                    }
                    throw new Exception(string.Format("数字识别错误:{0}行{1}列", crtf_count, char_count));
                }
                if (_State == 5)
                {
                    if (Char.IsDigit(c))
                    {
                        _State = 5;
                        _CharBuffer.Insert(c);
                        continue;
                    }
                    _State = 7;
                }
                if (_State == 6)
                {
                    if (Char.IsDigit(c))
                    {
                        _State = 2;
                        _CharBuffer.Insert(c);
                        continue;
                    }
                    throw new Exception(string.Format("数字识别错误:{0}行{1}列", crtf_count, char_count));
                }
                if (_State == 7)
                {

                    Word wd = new Word();
                    wd.Name = "REAL";
                    wd.Type = 31;
                    wd.Value = _CharBuffer.Read();
                    wd.PosStr = crtf_count.ToString() + ":" + char_count.ToString();
                    wd.Pos = pos;
                    _ListOfWord.Add(wd);
                    
                    _CharBuffer.Clear();
                    i--;
                    return i;

                }
            }
            throw new Exception(string.Format("数字识别错误:{0}行{1}列", crtf_count, char_count));
        }

    }


}
