//------------------------------------------------------------------------------------------------------------------------------------
// Copyright 2020 Dassault Systèmes - CPE EMED
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
//------------------------------------------------------------------------------------------------------------------------------------

using ds.enovia.common.exception;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;

namespace ds.enovia.dslib.exception
{
    public class ClassifiedItemLocationResponseException : ResponseException
    {
        JsonDocument m_internalObject;
        //JObject m_internalObject;
        public ClassifiedItemLocationResponseException(HttpResponseMessage _response) : base(_response)
        {
            m_internalObject = JsonDocument.Parse(_response.Content.ReadAsStringAsync().Result);

            PropertyInfo[] propertyInfoArray  = this.GetType().GetProperties(BindingFlags.Public |
                                                          BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (PropertyInfo propInfo in propertyInfoArray)
            {
                bool found = false;
                JsonElement.ObjectEnumerator propertiesIterator = m_internalObject.RootElement.EnumerateObject();

                while (propertiesIterator.MoveNext())
                {
                    string propName = propertiesIterator.Current.Name;

                    if (propName.Equals(propInfo.Name))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    throw new System.Exception("missing " + propInfo.Name);
                }
            }
        }

        public string GetString(string _propertyName)
        {
            JsonElement _element;

            if ((m_internalObject == null) || (!m_internalObject.RootElement.TryGetProperty(_propertyName, out _element)))
            {
                return default(string);
            }

            if (_element.ValueKind == JsonValueKind.String)
                return _element.GetString();

            return default(string);
        }
        public long GetLong(string _propertyName)
        {
            JsonElement _element;

            if ((m_internalObject == null) || (!m_internalObject.RootElement.TryGetProperty(_propertyName, out _element)))
            {
                return default(long);
            }

            if (_element.ValueKind == JsonValueKind.Number)
                return _element.GetInt64();

            return default(long);
        }

        public long status { get { return GetLong(nameof(this.status)); } }
        public string message { get { return GetString(nameof(this.message)); } }

    }
}
