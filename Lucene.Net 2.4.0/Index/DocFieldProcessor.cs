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

using System.Collections.Generic;

namespace Lucene.Net.Index
{
    /// <summary>
    /// This is a DocConsumer that gathers all fields under the
    /// same name, and calls per-field consumers to process field
    /// by field.  This class doesn't doesn't do any "real" work
    /// of its own: it just forwards the fields to a
    /// DocFieldConsumer.
    /// </summary>
    internal sealed class DocFieldProcessor : DocConsumer
    {

        internal readonly DocumentsWriter docWriter;
        internal readonly FieldInfos fieldInfos = new FieldInfos();
        internal readonly DocFieldConsumer consumer;

        public DocFieldProcessor(DocumentsWriter docWriter, DocFieldConsumer consumer)
        {
            this.docWriter = docWriter;
            this.consumer = consumer;
            consumer.setFieldInfos(fieldInfos);
        }

        internal override void closeDocStore(DocumentsWriter.FlushState state)
        {
            consumer.closeDocStore(state);
        }

        internal override void Flush(ICollection<object> threads, DocumentsWriter.FlushState state)
        {

            IDictionary<object, ICollection<object>> childThreadsAndFields = new Dictionary<object, ICollection<object>>();
            IEnumerator<object> it = threads.GetEnumerator();
            while (it.MoveNext())
            {
                DocFieldProcessorPerThread perThread = (DocFieldProcessorPerThread)it.Current;
                childThreadsAndFields[perThread.consumer] = perThread.Fields();
                perThread.trimFields(state);
            }

            consumer.flush(childThreadsAndFields, state);

            // Important to save after asking consumer to flush so
            // consumer can alter the FieldInfo* if necessary.  EG,
            // FreqProxTermsWriter does this with
            // FieldInfo.storePayload.
            fieldInfos.Write(state.directory, state.segmentName + ".fnm");
        }

        internal override void abort()
        {
            consumer.Abort();
        }

        internal override bool freeRAM()
        {
            return consumer.freeRAM();
        }

        internal override DocConsumerPerThread addThread(DocumentsWriterThreadState threadState)
        {
            return new DocFieldProcessorPerThread(threadState, this);
        }
    }
}
