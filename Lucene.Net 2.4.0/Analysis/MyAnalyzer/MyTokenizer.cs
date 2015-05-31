using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Lucene.Net.Analysis;

using Token = Lucene.Net.Analysis.Token;
using Tokenizer = Lucene.Net.Analysis.Tokenizer;

namespace Lucene.Net.Analysis.MyAnalyzer
{
    /// <summary>
    /// 单字分词
    /// </summary>
    public class MyTokenizer : Tokenizer
    {
        public MyTokenizer(System.IO.TextReader input) : base(input)
        { }

        private int start = 0;
        private int length = 0;
        private const int IO_BUFFER_SIZE = 256;
        private char[] ioBuffer = new char[IO_BUFFER_SIZE];

        public override Token Next(Token token)
        {
            token.Clear();
            if (start == 0)
            {
                length = input.Read((System.Char[])ioBuffer, 0, ioBuffer.Length);
                if (length <= 0)
                    return null;
            }
           
            if (start == length)
                return null;
            token.SetTermBuffer(ioBuffer, start, 1);
           
            start++;
            token.termBuffer[0] = System.Char.ToLower(token.termBuffer[0]);
            return token;
        }

        public override void Reset(System.IO.TextReader input)
        {
            start = 0;
            length = 0;
        }
    }
}
