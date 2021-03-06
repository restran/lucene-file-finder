/**
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Lucene.Net.Index
{
    /// <summary>
    /// Used by DocumentsWriter to implemented a StringReader
    /// that can be reset to a new string; we use this when
    /// tokenizing the string value from a Field.
    /// </summary>
    internal sealed class ReusableStringReader : System.IO.TextReader
    {
        int upto;
        int left;
        string s;

        internal void Init(string s)
        {
            this.s = s;
            left = s.Length;
            this.upto = 0;
        }

        public int Read(char[] c)
        {
            return Read(c, 0, c.Length);
        }

        public override int Read(char[] c, int off, int len)
        {
            if (left > len)
            {
                SupportClass.TextSupport.GetCharsFromString(s, upto, upto + len, c, off);
                upto += len;
                left -= len;
                return len;
            }
            else if (0 == left)
            {
                return -1;
            }
            else
            {
                SupportClass.TextSupport.GetCharsFromString(s, upto, upto + left, c, off);
                int r = left;
                left = 0;
                upto = s.Length;
                return r;
            }
        }

        public override void Close() { }

        public override int Read()
        {
            throw new System.NotImplementedException("ReusableStringReader.Read() is not implemented");
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            throw new System.NotImplementedException("ReusableStringReader.ReadBlock is not implemented");
        }

        public override string ReadLine()
        {
            throw new System.NotImplementedException("ReusableStringReader.ReadLine is not implemented");
        }

        public override int Peek()
        {
            throw new System.NotImplementedException("ReusableStringReader.Peek is not implemented");
        }

        public override string ReadToEnd()
        {
            if (left == 0) return null;
            string retStr =  s.Substring(s.Length-left);
            left = 0;
            return retStr;
        }
    }

    
}
