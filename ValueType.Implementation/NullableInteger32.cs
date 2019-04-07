using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueType.Implementation
{
    public class NullableInteger32 : IValueType
    {
        public void AddValueType(string value, ref List<object> convertValues)
        {
            convertValues.Add(Convert.ToInt32(value));
        }
    }
}
