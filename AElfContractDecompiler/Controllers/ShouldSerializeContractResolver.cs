using System.Collections.Generic;
using System.Reflection;
using AElfContractDecompiler.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AElfContractDecompiler.Controllers
{
    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(List<SingleDirectory>))
            {
                property.ShouldSerialize =
                    instance =>
                    {
                        var e = (List<SingleDirectory>) instance;
                        return e.Count > 0;
                    };
            }

            if (property.DeclaringType == typeof(List<SingleFile>))
            {
                property.ShouldSerialize =
                    instance =>
                    {
                        var e = (List<SingleFile>) instance;
                        return e.Count > 0;
                    };
            }

            return property;
        }
    }
}