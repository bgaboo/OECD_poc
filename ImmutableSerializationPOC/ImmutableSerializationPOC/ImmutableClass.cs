using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ImmutableSerializationPOC
{
    [DataContract]
    public class ImmutableClass
    {
        [DataMember(Name = "field1_public")]
        private string _property;
        public string Property { get { return _property; } set { _property = value; } }

        [DataMember(Name = "field2_noSetter")]
        private string _immutableProperty;
        // DEVNOTE: property has no setter, DataMember attribute should be on the private field
        //[DataMember(Name = "field2_noSetter")]
        public string ImmutableProperty { get { return _immutableProperty; } }

        [DataMember(Name = "field23_privateSetter")]
        public string ImmutablePrivateSetterProperty { get; private set; }

        private ImmutableClass()
        {
        }

        public ImmutableClass(string property, string immutableProperty, string immutablePrivateSetterProperty)
        {
            _property = property;
            _immutableProperty = immutableProperty;
            ImmutablePrivateSetterProperty = immutablePrivateSetterProperty;
        }
    }
}
