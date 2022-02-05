
using System;
using System.Collections.Generic;
using System.Reflection;

using ds.enovia.common.helper;

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
