using System.Collections.Generic;

namespace ds.delmia.dsmfg.schema
{
    public static class SchemaUtils
    {
        //public static string GetFieldsQueryString(IList<IFields> _fields)
        //{
        //    if ((_fields == null) || (_fields.Count ==0)) return null;

        //    IFields[] schemaFieldsArray = new IFields[_fields.Count];
        //    _fields.CopyTo(schemaFieldsArray, 0);

        //    return string.Join(",", schemaFieldsArray.Select(p => p.Name));
        //}

        public static string GetFieldsQueryString(IList<string> _fieldsNames)
        {
            if ((_fieldsNames == null) || (_fieldsNames.Count == 0)) return null;
            
            return string.Join(",", _fieldsNames);
        }
    }
}
