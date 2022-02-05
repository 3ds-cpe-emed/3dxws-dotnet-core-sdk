using ds.enovia.common.helper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ds.enovia.common.serialization
{
    class MaskSchemaAttributeRegistry : Dictionary<string, IList<Type>>
    {
        public void Parse(Assembly assembly)
        {
            IDictionary<string, IList<Type>> schemaMaskClasses = GetSchemaMaskClassesByMaskName(assembly);
            this.Merge(schemaMaskClasses);
        }

        private IDictionary<string, IList<Type>> GetSchemaMaskClassesByMaskName(Assembly _assembly)
        {
            Dictionary<string, IList<Type>> __output = new Dictionary<string, IList<Type>>();

            Type[] assemblyTypes = _assembly.GetTypes();

            foreach (Type assemblyType in assemblyTypes)
            {
                IEnumerable<MaskSchemaAttribute> schemaMaskAttsEnumerable = assemblyType.GetCustomAttributes<MaskSchemaAttribute>();
                if (schemaMaskAttsEnumerable == null) continue;

                IEnumerator<MaskSchemaAttribute> schemaMaskAttsEnumerator = schemaMaskAttsEnumerable.GetEnumerator();
                if (schemaMaskAttsEnumerator == null) continue;

                while (schemaMaskAttsEnumerator.MoveNext())
                {
                    MaskSchemaAttribute schemaMaskAttribute = schemaMaskAttsEnumerator.Current;
                    string maskName = schemaMaskAttribute.MaskName;

                    if (!__output.ContainsKey(maskName))
                    {
                        __output.Add(maskName, new List<Type>());
                    }
                    __output[maskName].Add(assemblyType);
                }
            }

            return __output;
        }
    }
}
