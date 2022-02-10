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
    class ItemTypeSchemaAttributeRegistry : Dictionary<string, IDictionary<string, IList<Type>>>
    {

        public void Parse(Assembly assembly)
        {
            IDictionary<string, IDictionary<string, IList<Type>>> schemaTypeClasses = GetSchemaTypeClassesByTypeName(assembly);
            this.Merge(schemaTypeClasses);
        }

        private IDictionary<string, IDictionary<string, IList<Type>>> GetSchemaTypeClassesByTypeName(Assembly _assembly)
        {
            Dictionary<string, IDictionary<string, IList<Type>>> __output = new Dictionary<string, IDictionary<string, IList<Type>>>();

            Type[] assemblyTypes = _assembly.GetTypes();

            foreach (Type assemblyType in assemblyTypes)
            {
                IEnumerable<ItemTypeSchemaAttribute> schemaItemTypeAttsEnumerable = assemblyType.GetCustomAttributes<ItemTypeSchemaAttribute>();
                if (schemaItemTypeAttsEnumerable == null) continue;

                IEnumerator<ItemTypeSchemaAttribute> schemaItemTypeAttsEnumerator = schemaItemTypeAttsEnumerable.GetEnumerator();
                if (schemaItemTypeAttsEnumerator == null) continue;

                while (schemaItemTypeAttsEnumerator.MoveNext())
                {
                    ItemTypeSchemaAttribute schemaItemTypeAttribute = schemaItemTypeAttsEnumerator.Current;
                    string typePropertyName = schemaItemTypeAttribute.TypePropertyName;
                    string typeName = schemaItemTypeAttribute.TypeValue;

                    if (!__output.ContainsKey(typePropertyName))
                    {
                        __output.Add(typePropertyName, new Dictionary<string, IList<Type>>());
                    }

                    IDictionary<string, IList<Type>> typeNameDictionary = __output[typePropertyName];

                    if (!typeNameDictionary.ContainsKey(typeName))
                    {
                        typeNameDictionary[typeName] = new List<Type>();
                    }

                    typeNameDictionary[typeName].Add(assemblyType);
                }
            }

            return __output;
        }
    }
}
