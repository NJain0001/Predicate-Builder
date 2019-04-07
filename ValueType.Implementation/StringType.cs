using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueType.Implementation
{
    public class StringType : IValueType
    {
        public void AddValueType(string value, ref List<object> convertValues)
        {
            convertValues.Add(value);
        }
    }
}
