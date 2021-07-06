using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Domain.AplicaAcl
{
    public class AplicaAclModel
    {
        [JsonProperty("ObjectId", Required = Required.Always)]
        public string ObjectId { get; set; }
    }
}