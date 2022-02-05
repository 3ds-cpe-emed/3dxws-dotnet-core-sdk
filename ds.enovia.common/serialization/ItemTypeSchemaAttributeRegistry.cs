using ds.enovia.common.helper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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
