// ------------------------------------------------------------------------------------------------------------------------------------
// Copyright 2022 Dassault Systèmes - CPE EMED
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished
// to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ------------------------------------------------------------------------------------------------------------------------------------

using ds.enovia.common.helper;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ds.enovia.common.serialization
{
    class FieldsSchemaAttributeRegistry : Dictionary<string, IList<Type>>
    {
        public void Parse(Assembly assembly)
        {
            IDictionary<string, IList<Type>> schemaFieldsClasses = GetSchemaFieldsClassesByFieldsName(assembly);
            this.Merge(schemaFieldsClasses);
        }

        private IDictionary<string, IList<Type>> GetSchemaFieldsClassesByFieldsName(Assembly _assembly)
        {
            Dictionary<string, IList<Type>> __output = new Dictionary<string, IList<Type>>();

            Type[] assemblyTypes = _assembly.GetTypes();

            foreach (Type assemblyType in assemblyTypes)
            {
                IEnumerable<FieldsSchemaAttribute> schemaFieldsAttsEnumerable = assemblyType.GetCustomAttributes<FieldsSchemaAttribute>();
                if (schemaFieldsAttsEnumerable == null) continue;

                IEnumerator<FieldsSchemaAttribute> schemaFieldsAttsEnumerator = schemaFieldsAttsEnumerable.GetEnumerator();
                if (schemaFieldsAttsEnumerator == null) continue;

                while (schemaFieldsAttsEnumerator.MoveNext())
                {
                    FieldsSchemaAttribute schemaFieldsAttribute = schemaFieldsAttsEnumerator.Current;
                    string fieldsName = schemaFieldsAttribute.FieldsName;

                    if (!__output.ContainsKey(fieldsName))
                    {
                        __output.Add(fieldsName, new List<Type>());
                    }
                    __output[fieldsName].Add(assemblyType);
                }
            }

            return __output;
        }
    }
}
