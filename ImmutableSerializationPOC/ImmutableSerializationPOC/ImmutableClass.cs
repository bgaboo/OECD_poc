using ProtoBuf;
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

    [DataContract]
    public class ImmutableClassSS : ImmutableClass
    {
        private string _property;
        [DataMember(Name = "field1_public")]
        public string Property { get { return _property; } set { _property = value; } }

        
        private string _immutableProperty;
        [DataMember(Name = "field2_noSetter")]
        public string ImmutableProperty { get { return _immutableProperty; } }

        [DataMember(Name = "field23_privateSetter")]
        public string ImmutablePrivateSetterProperty { get; private set; }
        
        public ImmutableClassSS(string property, string immutableProperty, string immutablePrivateSetterProperty) : base (property, immutableProperty, immutablePrivateSetterProperty)
        {
            _property = property;
            _immutableProperty = immutableProperty;
            ImmutablePrivateSetterProperty = immutablePrivateSetterProperty;
        }
    }

    [ProtoContract]
    public class ImmutableClassProto// : ImmutableClass
    {
        [ProtoMember(1)]
        private string _property;
        public string Property { get { return _property; } set { _property = value; } }

        [ProtoMember(2)]
        private string _immutableProperty;
        public string ImmutableProperty { get { return _immutableProperty; } }

        [ProtoMember(3)]
        public string ImmutablePrivateSetterProperty { get; private set; }

        private ImmutableClassProto()
        { 
        }

        public ImmutableClassProto(string property, string immutableProperty, string immutablePrivateSetterProperty)
            //: base(property, immutableProperty, immutablePrivateSetterProperty)
        {
            _property = property;
            _immutableProperty = immutableProperty;
            ImmutablePrivateSetterProperty = immutablePrivateSetterProperty;
        }
    }
}
