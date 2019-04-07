using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateGeneratorLibrary
{
    public class RoutingOptionRequest
    {
        public string TargetName { get; set; }
        public string Operator { get; set; }
        public IEnumerable<string> Values { get; set; }
        public string Condition { get; set; }
        public IEnumerable<RoutingOptionRequest> NestedRequest { get; set; }
    }
}
