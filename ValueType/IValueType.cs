using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueType
{
    public interface IValueType
    {
        void AddValueType(string value, ref List<object> convertValues);
    }
}
