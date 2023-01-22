using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WinServicesMgr.Entities
{
    [DataContract]
    internal class ServiceStateEntity
    {
        [DataMember]
        public string ServiceName { get; set; }

        [DataMember]
        public ServiceStartMode ServiceStartMode { get; set; }
    }
}
