# 基于 Lucene 的桌面文件搜索

开源2010年，自己在学习 [Lucene](https://lucene.apache.org/ "") 时开发的一款桌面文件搜索工具，这么多年过去了，代码一直静静存放在自己的硬盘上，与其让其沉睡，不如分享出来。

这款工具带有明显的模仿 Everything 的痕迹。事实上这是当时某项课程的作业，而那个时候刚好发现了 Eveything 这款神奇的工具，出于想探究其原理，就着手做了一款与其类似的工具，但是最后的结果却是令人不满意的，因为差距仍然是很大。

就比如 Everything 能够实时监测 NTFS 文件的变化（据说是监测 NTFS 的日志）并自动更新索引，而我却需要手动来更新。

虽然这不是一款另我十分满意的作品，但希望其中某些部分能够帮到有需要的人。这里还要感谢共同完成的`杨一`和`江边串串香`。

**为什么选择 Lucene？**

1. Lucene 是最著名的全文检索引擎的核心库，使用 Lucene 可以让搜索出来的结果按匹配程度排序。
2. Lucene 有很多版本的实现，Java，C#，Python。
3. 用数据库的模糊搜索也可以实现，但是效果和速度跟 Lucene 还是有差距。

关于 Lucene 的一些资料可以参考[车东的笔记](http://www.chedong.com/tech/lucene.html "")。


## 功能

实现对桌面文件名、MP3 文件 Tag 信息（歌手，专辑，流派，...）的快速检索。

前提：需要先对文件建立索引。

![Lucene File Finder](http://oxygen.qiniudn.com/img2015053136.png "")

## 分词

默认情况下使用中文的分词是对词进行切分，比如：

`这是文件名` -> `这是|文件名`

为了实现对文件名的模糊搜索，需要自定义一个自己的分词，分词效果是对所有的字都进行切分：

`这是文件名` -> `这|是|文|件|名`


特别指出，Lucene 可以自定义分词，这里就不展开介绍。

在 Lucene.Net.Analysis 中添加一个自己的 MyAnalyzer

```c#
//文件：Lucene/Net/Analysis/MyAnalyzer/MyAnalyzer.cs
using System;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Analysis;
using System.IO;

namespace Lucene.Net.Analysis.MyAnalyzer
{
    public class MyAnalyzer : Analyzer
    {
        public override TokenStream TokenStream(string fieldName, System.IO.TextReader reader)
        {
            TokenStream result = new MyTokenizer(reader);
            return result;
        }

        public override TokenStream ReusableTokenStream(System.String fieldName, System.IO.TextReader reader)
        {
            Tokenizer tokenizer = (Tokenizer)GetPreviousTokenStream();
            if (tokenizer == null)
            {
                tokenizer = new MyTokenizer(reader);
                SetPreviousTokenStream(tokenizer);
            }
            else
                tokenizer.Reset(reader);
            return tokenizer;
        }
    }
}
```

```c#
//文件：Lucene/Net/Analysis/MyAnalyzer/MyTokenizer.cs
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Lucene.Net.Analysis;

using Token = Lucene.Net.Analysis.Token;
using Tokenizer = Lucene.Net.Analysis.Tokenizer;

namespace Lucene.Net.Analysis.MyAnalyzer
{
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

```

## 相关注意事项

1. 由于需要访问系统注册表，来根据文件扩展名获取图标，因此启动的时候需要管理员权限，同时在VS上开发的时候，也相应需要用管理员权限打开VS。
2. 建立索引的时候，使用了三个线程来处理文件索引操作：分扫描磁盘文件，处理扫描获取的文件数据，将文件信息加入到索引中。它们之间使用队列来传递文件信息，如果运行时线程的速度不一致，就会出现队列有大量未处理的数据，导致诡异的内存占用升到非常高。这在其他同学的电脑上测试时有出现过。（当时使用多线程，是为了在等待IO时，顺便创建索引）