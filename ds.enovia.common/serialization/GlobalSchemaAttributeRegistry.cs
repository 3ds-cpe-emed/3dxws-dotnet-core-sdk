using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ds.enovia.common.serialization
{
    class GlobalSchemaAttributeRegistry
    {
        private static object m_lock = new object();

        private static MaskSchemaAttributeRegistry     m_glbSchemaMaskRegistry       = null;
        private static FieldsSchemaAttributeRegistry   m_glbSchemaFieldsRegistry     = null;
        private static ItemTypeSchemaAttributeRegistry m_glbSchemaCustomTypeRegistry = null;
        
        private static bool m_isInitialized = false;

        private static void Initialize()
        {
            IList<Assembly> assemblies = GetAllRelevantAssemblies();
            
            lock (m_lock)
            {
                m_glbSchemaMaskRegistry       = new MaskSchemaAttributeRegistry();
                m_glbSchemaFieldsRegistry     = new FieldsSchemaAttributeRegistry();
                m_glbSchemaCustomTypeRegistry = new ItemTypeSchemaAttributeRegistry();

                foreach (Assembly assembly in assemblies)
                {
                    m_glbSchemaMaskRegistry.Parse(assembly);

                    m_glbSchemaCustomTypeRegistry.Parse(assembly);

                    m_glbSchemaFieldsRegistry.Parse(assembly);
                }

                IsInitialized = true;
            }
        }

        private static bool IsInitialized { get => m_isInitialized; set => m_isInitialized = value; }

        private static IList<Assembly> GetAllRelevantAssemblies()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return new List<Assembly>(assemblies);
        }

        public static Type GetTypeFromPropertyNameAndMaskAttributes(string _maskName, string _typePropName, string _typeValue)
        {
            if (!IsInitialized)
            {
                Initialize();
            }

            try
            {
                // Do we need a catchall type?
                // Filter by mask first
                if (!m_glbSchemaMaskRegistry.ContainsKey(_maskName))
                {
                    //TODO: return a catchall or throw exception
                    return null;
                }

                IList<Type> maskedFilteredTypes = m_glbSchemaMaskRegistry[_maskName];

                //Filter mask results by Custom Type
                if (!m_glbSchemaCustomTypeRegistry.ContainsKey(_typePropName))
                {
                    //TODO: return a catchall or throw exception
                    return null;
                }

                IDictionary<string, IList<Type>> schemaTypeDict = m_glbSchemaCustomTypeRegistry[_typePropName];
                if (!schemaTypeDict.ContainsKey(_typeValue))
                {
                    //TODO: return a catchall or throw exception
                    return null;
                }

                IList<Type> customTypesFilteredTypes = schemaTypeDict[_typeValue];

                //The intersection of both Type Lists should be (in an ideal world) just one.

                IList<Type> typesIntersection = customTypesFilteredTypes.Intersect(maskedFilteredTypes).ToList();

                if ((typesIntersection != null) && (typesIntersection.Count > 0))
                {
                    return typesIntersection[0];
                }
            }
            catch (Exception _ex)
            {

            }

            //TODO: return a catchall or throw exception
            return null;
        }


        public static bool InterfaceFilterByName(Type typeObj, Object criteriaObj)
        {   
            if (typeObj.ToString() == criteriaObj.ToString())
                return true;
            else
                return false;
        }

        public static Type GetFieldsSchemaType(string _fieldsSchema)
        {
            if (!IsInitialized)
            {
                Initialize();
            }

            Type __returnType = null;

            try
            {
                // Do we need a catchall type?
                // Filter by mask first
                if (!m_glbSchemaFieldsRegistry.ContainsKey(_fieldsSchema))
                {
                    //TODO: return a catchall or throw exception
                    return null;
                }

                IList<Type> fieldsSchemaNameTypes = m_glbSchemaFieldsRegistry[_fieldsSchema];

                List<Type> fieldsInterfaceFilteredTypes = new List<Type>();

                foreach (Type fieldsMarked in fieldsSchemaNameTypes)
                {
                   Type[] fieldsSchemaInterfaces = fieldsMarked.FindInterfaces(InterfaceFilterByName, "ds.enovia.common.serialization.IFieldsSchema");

                    if (fieldsSchemaInterfaces.Length == 1)
                    {
                        fieldsInterfaceFilteredTypes.Add(fieldsMarked);
                    }
                }

                int fieldsInterfaceFilteredTypesCount = fieldsInterfaceFilteredTypes.Count;

                if (fieldsInterfaceFilteredTypesCount > 0)
                {
                    __returnType = fieldsInterfaceFilteredTypes[0];
                }
            }
            catch (Exception _ex)
            {

            }

            //TODO: return a catchall or throw exception
            return __returnType;
        }
    }
}
