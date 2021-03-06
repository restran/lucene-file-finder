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

//using System.Collections;
using System.Collections.Generic;

using ArrayUtil = Lucene.Net.Util.ArrayUtil;

namespace Lucene.Net.Index
{
    internal class DocFieldConsumers : DocFieldConsumer
    {
        internal readonly DocFieldConsumer one;
        internal readonly DocFieldConsumer two;

        public DocFieldConsumers(DocFieldConsumer one, DocFieldConsumer two)
        {
            this.one = one;
            this.two = two;
        }

        internal override void setFieldInfos(FieldInfos fieldInfos)
        {
            base.setFieldInfos(fieldInfos);
            one.setFieldInfos(fieldInfos);
            two.setFieldInfos(fieldInfos);
        }

        //internal override void flush(/*IDictionary*/ IDictionary<DocFieldConsumerPerThread, ICollection<DocFieldConsumerPerField>> threadsAndFields, DocumentsWriter.FlushState state)
        internal override void flush(/*IDictionary*/ IDictionary<object, ICollection<object>> threadsAndFields, DocumentsWriter.FlushState state)
        {
            //IDictionary oneThreadsAndFields = new Dictionary();
            //IDictionary twoThreadsAndFields = new Dictionary();
            //IDictionary<DocFieldConsumerPerThread, ICollection<DocFieldConsumerPerField>> oneThreadsAndFields = new Dictionary<DocFieldConsumerPerThread, ICollection<DocFieldConsumerPerField>>();
            //IDictionary<DocFieldConsumerPerThread, ICollection<DocFieldConsumerPerField>> twoThreadsAndFields = new Dictionary<DocFieldConsumerPerThread, ICollection<DocFieldConsumerPerField>>();
            IDictionary<object, ICollection<object>> oneThreadsAndFields = new Dictionary<object, ICollection<object>>();
            IDictionary<object, ICollection<object>> twoThreadsAndFields = new Dictionary<object, ICollection<object>>();

            //IEnumerator it = threadsAndFields.GetEnumerator();
            //IEnumerator<KeyValuePair<DocFieldConsumerPerThread, ICollection<DocFieldConsumerPerField>>> it = threadsAndFields.GetEnumerator();
            IEnumerator<KeyValuePair<object, ICollection<object>>> it = threadsAndFields.GetEnumerator();
            while (it.MoveNext())
            {
                //KeyValuePair entry = it.Current;
                //KeyValuePair<DocFieldConsumerPerThread, ICollection<DocFieldConsumerPerField>> entry = it.Current;
                KeyValuePair<object, ICollection<object>> entry = it.Current;

                //DocFieldConsumersPerThread perThread = (DocFieldConsumersPerThread)entry.Key;
                DocFieldConsumersPerThread perThread = (DocFieldConsumersPerThread)entry.Key;

                //ICollection fields = (ICollection)entry.Value;
                //ICollection<DocFieldConsumerPerField> fields = entry.Value;
                ICollection<object> fields = entry.Value;

                //IEnumerator fieldsIt = fields.GetEnumerator();
                //IEnumerator<DocFieldConsumerPerField> fieldsIt = fields.GetEnumerator();
                IEnumerator<object> fieldsIt = fields.GetEnumerator();

                //IDictionary oneFields = new Dictionary();
                //IDictionary twoFields = new Dictionary();
                //IDictionary<DocFieldConsumerPerField, DocFieldConsumerPerField> oneFields = new Dictionary<DocFieldConsumerPerField, DocFieldConsumerPerField>();
                //IDictionary<DocFieldConsumerPerField, DocFieldConsumerPerField> twoFields = new Dictionary<DocFieldConsumerPerField, DocFieldConsumerPerField>();
                IDictionary<object, object> oneFields = new Dictionary<object, object>();
                IDictionary<object, object> twoFields = new Dictionary<object, object>();
                
                while (fieldsIt.MoveNext())
                {
                    DocFieldConsumersPerField perField = (DocFieldConsumersPerField)fieldsIt.Current;
                    oneFields[perField.one] = perField.one;
                    twoFields[perField.two] = perField.two;
                }

                oneThreadsAndFields[perThread.one] = oneFields.Keys;
                twoThreadsAndFields[perThread.two] = twoFields.Keys;
            }

            one.flush(oneThreadsAndFields, state);
            two.flush(twoThreadsAndFields, state);
        }

        internal override void closeDocStore(DocumentsWriter.FlushState state)
        {
            try
            {
                one.closeDocStore(state);
            }
            finally
            {
                two.closeDocStore(state);
            }
        }

        internal override void Abort()
        {
            try
            {
                one.Abort();
            }
            finally
            {
                two.Abort();
            }
        }

        internal override bool freeRAM()
        {
            bool any = one.freeRAM();
            any |= two.freeRAM();
            return any;
        }

        internal override DocFieldConsumerPerThread addThread(DocFieldProcessorPerThread docFieldProcessorPerThread)
        {
            return new DocFieldConsumersPerThread(docFieldProcessorPerThread, this, one.addThread(docFieldProcessorPerThread), two.addThread(docFieldProcessorPerThread));
        }

        internal PerDoc[] docFreeList = new PerDoc[1];
        internal int freeCount;
        internal int allocCount;

        internal PerDoc getPerDoc()
        {
            lock (this)
            {
                if (freeCount == 0)
                {
                    allocCount++;
                    if (allocCount > docFreeList.Length)
                    {
                        // Grow our free list up front to make sure we have
                        // enough space to recycle all outstanding PerDoc
                        // instances
                        System.Diagnostics.Debug.Assert(allocCount == 1 + docFreeList.Length);
                        docFreeList = new PerDoc[ArrayUtil.GetNextSize(allocCount)];
                    }
                    return new PerDoc(this);
                }
                else
                    return docFreeList[--freeCount];
            }
        }

        internal void freePerDoc(PerDoc perDoc)
        {
            lock (this)
            {
                System.Diagnostics.Debug.Assert(freeCount < docFreeList.Length);
                docFreeList[freeCount++] = perDoc;
            }
        }

        internal class PerDoc : DocumentsWriter.DocWriter
        {
            internal DocumentsWriter.DocWriter one;
            internal DocumentsWriter.DocWriter two;

            private DocFieldConsumers enclosing_instance;

            internal PerDoc(DocFieldConsumers enclosing_instance)
            {
                this.enclosing_instance = enclosing_instance;
            }

            internal override long SizeInBytes()
            {
                return one.SizeInBytes() + two.SizeInBytes();
            }

            internal override void Finish()
            {
                try
                {
                    try
                    {
                        one.Finish();
                    }
                    finally
                    {
                        two.Finish();
                    }
                }
                finally
                {
                    enclosing_instance.freePerDoc(this);
                }
            }

            internal override void Abort()
            {
                try
                {
                    try
                    {
                        one.Abort();
                    }
                    finally
                    {
                        two.Abort();
                    }
                }
                finally
                {
                    enclosing_instance.freePerDoc(this);
                }
            }
        }
    }
}
