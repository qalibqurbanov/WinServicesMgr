using System.Runtime.Serialization;
using System.ServiceProcess;

namespace WinServicesMgr.Entities
{
    /// <summary>
    /// Servisin veziyyetini temsil edir.
    /// </summary>
    [DataContract]
    internal class ServiceStateEntity
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
    }
}