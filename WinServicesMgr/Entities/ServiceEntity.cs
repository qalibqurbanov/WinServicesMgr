﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Runtime.Serialization;

namespace WinServicesMgr.Entities
{
    [DataContract]
    internal class ServiceEntity
    {
        [DataMember]
        public string ServiceName { get; set; }

        [DataMember]
        public ServiceStartMode ServiceStartMode { get; set; }

        [DataMember]
        public string DisplayName { get; set; }
    }
}
