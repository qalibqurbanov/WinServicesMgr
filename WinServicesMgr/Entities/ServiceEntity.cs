using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Runtime.Serialization;

namespace WinServicesMgr.Entities
{
    /// <summary>
    /// ('ListView'-da gostereceyim ve s.) Servisi temsil edir.
    /// </summary>
    [DataContract]
    internal class ServiceEntity
    {
        /// <summary>
        /// Servisin adi.
        /// </summary>
        [DataMember]
        public string ServiceName { get; set; }

        /// <summary>
        /// Servisin start tipi.
        /// </summary>
        [DataMember]
        public ServiceStartMode ServiceStartMode { get; set; }

        /// <summary>
        /// Servisin istifadeciye ('services.msc'-de ve s.) gorsendiyi adi.
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }
    }
}