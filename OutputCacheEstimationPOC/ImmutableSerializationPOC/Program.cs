using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ImmutableSerializationPOC
{
    class Program
    {
        static void Main(string[] args)
        {
            ResponseSize.DoTest();
        }
    }
}
