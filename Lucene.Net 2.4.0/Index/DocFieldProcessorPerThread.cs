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

using Document = Lucene.Net.Documents.Document;
using Fieldable = Lucene.Net.Documents.Fieldable;

namespace Lucene.Net.Index
{
    internal sealed class DocFieldProcessorPerThread : DocConsumerPerThread
    {
        //internal float docBoost;
        internal int fieldGen;
        internal readonly DocFieldProcessor docFieldProcessor;
        internal readonly FieldInfos fieldInfos;
        internal readonly DocFieldConsumerPerThread consumer;

        // Holds all fields seen in current doc
        internal DocFieldProcessorPerField[] fields = new DocFieldProcessorPerField[1];
        internal int fieldCount;

        // Hash table for all fields ever seen
        internal DocFieldProcessorPerField[] fieldHash = new DocFieldProcessorPerField[2];
        internal int hashMask = 1;
        internal int totalFieldCount;

        internal readonly DocumentsWriter.DocState docState;

        public DocFieldProcessorPerThread(DocumentsWriterThreadState threadState, DocFieldProcessor docFieldProcessor)
        {
            this.docState = threadState.docState;
            this.docFieldProcessor = docFieldProcessor;
            this.fieldInfos = docFieldProcessor.fieldInfos;
            this.consumer = docFieldProcessor.consumer.addThread(this);
        }

        internal override void abort()
        {
            for (int i = 0; i < fieldHash.Length; i++)
            {
                DocFieldProcessorPerField field = fieldHash[i];
                while (field != null)
                {
                    DocFieldProcessorPerField next = field.next;
                    field.abort();
                    field = next;
                }
            }
            consumer.abort();
        }

        public ICollection<object> Fields()
        {
            IDictionary<object, object> fields = new Dictionary<object, object>();
            for (int i = 0; i < fieldHash.Length; i++)
            {
                DocFieldProcessorPerField field = fieldHash[i];
                while (field != null)
                {
                    fields[field.consumer] = field.consumer;
                    field = field.next;
                }
            }
            System.Diagnostics.Debug.Assert(fields.Count == totalFieldCount);
            return fields.Keys;
        }

        /// <summary>
        /// If there are fields we've seen but did not see again in the last run, then free them up.
        /// </summary>
        /// <param name="state"></param>
        internal void trimFields(DocumentsWriter.FlushState state)
        {

            for (int i = 0; i < fieldHash.Length; i++)
            {
                DocFieldProcessorPerField perField = fieldHash[i];
                DocFieldProcessorPerField lastPerField = null;

                while (perField != null)
                {

                    if (perField.lastGen == -1)
                    {

                        // This field was not seen since the previous
                        // flush, so, free up its resources now

                        // Unhash
                        if (lastPerField == null)
                            fieldHash[i] = perField.next;
                        else
                            lastPerField.next = perField.next;

                        if (state.docWriter.infoStream != null)
                            state.docWriter.infoStream.WriteLine("  purge field=" + perField.fieldInfo.name);

                        totalFieldCount--;

                    }
                    else
                    {
                        // Reset
                        perField.lastGen = -1;
                        lastPerField = perField;
                    }

                    perField = perField.next;
                }
            }
        }

        private void rehash()
        {
            int newHashSize = (int)(fieldHash.Length * 2);
            System.Diagnostics.Debug.Assert(newHashSize > fieldHash.Length);

            DocFieldProcessorPerField[] newHashArray = new DocFieldProcessorPerField[newHashSize];

            // Rehash
            int newHashMask = newHashSize - 1;
            for (int j = 0; j < fieldHash.Length; j++)
            {
                DocFieldProcessorPerField fp0 = fieldHash[j];
                while (fp0 != null)
                {
                    int hashPos2 = fp0.fieldInfo.name.GetHashCode() & newHashMask;
                    DocFieldProcessorPerField nextFP0 = fp0.next;
                    fp0.next = newHashArray[hashPos2];
                    newHashArray[hashPos2] = fp0;
                    fp0 = nextFP0;
                }
            }

            fieldHash = newHashArray;
            hashMask = newHashMask;
        }

        internal override DocumentsWriter.DocWriter processDocument()
        {

            consumer.startDocument();
            Document doc = docState.doc;

            System.Diagnostics.Debug.Assert(docFieldProcessor.docWriter.writer.TestPoint("DocumentsWriter.ThreadState.init start"));

            fieldCount = 0;

            int thisFieldGen = fieldGen++;

            System.Collections.IList docFields = doc.GetFields();
            int numDocFields = docFields.Count;

            // Absorb any new fields first seen in this document.
            // Also absorb any changes to fields we had already
            // seen before (eg suddenly turning on norms or
            // vectors, etc.):

            for (int i = 0; i < numDocFields; i++)
            {
                Fieldable field = (Fieldable)docFields[i];
                string fieldName = field.Name();

                // Make sure we have a PerField allocated
                int hashPos = fieldName.GetHashCode() & hashMask;
                DocFieldProcessorPerField fp = fieldHash[hashPos];
                while (fp != null && !fp.fieldInfo.name.Equals(fieldName))
                    fp = fp.next;

                if (fp == null)
                {

                    // TODO FI: we need to genericize the "flags" that a
                    // field holds, and, how these flags are merged; it
                    // needs to be more "pluggable" such that if I want
                    // to have a new "thing" my Fields can do, I can
                    // easily add it
                    FieldInfo fi = fieldInfos.Add(fieldName, field.IsIndexed(), field.IsTermVectorStored(),
                                                  field.IsStorePositionWithTermVector(), field.IsStoreOffsetWithTermVector(),
                                                  field.GetOmitNorms(), false, field.GetOmitTf());

                    fp = new DocFieldProcessorPerField(this, fi);
                    fp.next = fieldHash[hashPos];
                    fieldHash[hashPos] = fp;
                    totalFieldCount++;

                    if (totalFieldCount >= fieldHash.Length / 2)
                        rehash();
                }
                else
                    fp.fieldInfo.update(field.IsIndexed(), field.IsTermVectorStored(),
                                        field.IsStorePositionWithTermVector(), field.IsStoreOffsetWithTermVector(),
                                        field.GetOmitNorms(), false, field.GetOmitTf());

                if (thisFieldGen != fp.lastGen)
                {

                    // First time we're seeing this field for this doc
                    fp.fieldCount = 0;

                    if (fieldCount == fields.Length)
                    {
                        int newSize = fields.Length * 2;
                        DocFieldProcessorPerField[] newArray = new DocFieldProcessorPerField[newSize];
                        System.Array.Copy(fields, 0, newArray, 0, fieldCount);
                        fields = newArray;
                    }

                    fields[fieldCount++] = fp;
                    fp.lastGen = thisFieldGen;
                }

                if (fp.fieldCount == fp.fields.Length)
                {
                    Fieldable[] newArray = new Fieldable[fp.fields.Length * 2];
                    System.Array.Copy(fp.fields, 0, newArray, 0, fp.fieldCount);
                    fp.fields = newArray;
                }

                fp.fields[fp.fieldCount++] = field;
            }

            // If we are writing vectors then we must visit
            // fields in sorted order so they are written in
            // sorted order.  TODO: we actually only need to
            // sort the subset of fields that have vectors
            // enabled; we could save [small amount of] CPU
            // here.
            quickSort(fields, 0, fieldCount - 1);

            for (int i = 0; i < fieldCount; i++)
                fields[i].consumer.processFields(fields[i].fields, fields[i].fieldCount);

            if (docState.maxTermPrefix != null && docState.infoStream != null)
                docState.infoStream.WriteLine("WARNING: document contains at least one immense term (longer than the max length " + DocumentsWriter.MAX_TERM_LENGTH + "), all of which were skipped.  Please correct the analyzer to not produce such terms.  The prefix of the first immense term is: '" + docState.maxTermPrefix + "...'");

            return consumer.finishDocument();
        }

        void quickSort(DocFieldProcessorPerField[] array, int lo, int hi)
        {
            if (lo >= hi)
                return;
            else if (hi == 1 + lo)
            {
                if (string.CompareOrdinal(array[lo].fieldInfo.name, array[hi].fieldInfo.name) > 0)
                {
                    DocFieldProcessorPerField tmp = array[lo];
                    array[lo] = array[hi];
                    array[hi] = tmp;
                }
                return;
            }

            int mid = (int)((uint)(lo + hi) >> 1);

            if (string.CompareOrdinal(array[lo].fieldInfo.name, array[mid].fieldInfo.name) > 0)
            {
                DocFieldProcessorPerField tmp = array[lo];
                array[lo] = array[mid];
                array[mid] = tmp;
            }

            if (string.CompareOrdinal(array[mid].fieldInfo.name, array[hi].fieldInfo.name) > 0)
            {
                DocFieldProcessorPerField tmp = array[mid];
                array[mid] = array[hi];
                array[hi] = tmp;

                if (string.CompareOrdinal(array[lo].fieldInfo.name, array[mid].fieldInfo.name) > 0)
                {
                    DocFieldProcessorPerField tmp2 = array[lo];
                    array[lo] = array[mid];
                    array[mid] = tmp2;
                }
            }

            int left = lo + 1;
            int right = hi - 1;

            if (left >= right)
                return;

            DocFieldProcessorPerField partition = array[mid];

            for (; ; )
            {
                while (string.CompareOrdinal(array[right].fieldInfo.name, partition.fieldInfo.name) > 0)
                    --right;

                while (left < right && string.CompareOrdinal(array[left].fieldInfo.name, partition.fieldInfo.name) <= 0)
                    ++left;

                if (left < right)
                {
                    DocFieldProcessorPerField tmp = array[left];
                    array[left] = array[right];
                    array[right] = tmp;
                    --right;
                }
                else
                {
                    break;
                }
            }

            quickSort(array, lo, left);
            quickSort(array, left + 1, hi);
        }
    }
}
