using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueType;

namespace ValueType.Implementation
{
    public class NullableInteger64 : IValueType
    {
        public void AddValueType(string value, ref List<object> convertValues)
        {
            convertValues.Add(Convert.ToInt64(value));
        }
    }
}
