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

using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ds.delmia.dsmfg.fields
{
    public class ManufacturingItemFields
    {
        public ManufacturingItemFields()
        {
            CustomerAttributes = false;
            SupportedTypes = false;
            ScopeEngItem = false;
            ResultingEngItems = false;
        }

        [DefaultValue("dsmveno:CustomerAttributes")]
        public bool CustomerAttributes { get; set; }

        [DefaultValue("dsmveno:SupportedTypes")]
        public bool SupportedTypes { get; set; }
        
        [DefaultValue("dsmfg:ScopeEngItem.referencedObject")]
        public bool ScopeEngItem { get; set; }

        [DefaultValue("dsmfg:ResultingEngItems")]
        public bool ResultingEngItems { get; set; }

        public override string ToString()
        {
            string __fields = "";

            foreach (PropertyInfo property in this.GetType().GetProperties())
            {
                if ((bool)property.GetValue(this))
                {
                    DefaultValueAttribute[] attributes = property.GetCustomAttributes(typeof(DefaultValueAttribute), false) as DefaultValueAttribute[];

                    if (attributes != null && attributes.Any())
                    {
                        __fields += string.Format("{0},", (string)attributes.First().Value);
                    }
                }
            }

            if (__fields.EndsWith(","))
            {
                __fields = __fields.Substring(0, __fields.Length - 1);
            }

            return __fields;

        }

    }
}
