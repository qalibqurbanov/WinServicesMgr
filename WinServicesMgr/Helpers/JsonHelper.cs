using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

namespace WinServicesMgr.Helpers
{
    /// <summary>
    /// Json Serialization/Deserialization etmek ucun komekci sinif.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class JsonHelper<T> where T : class, new()
    {
        /// <summary>
        /// Verilen datalari 'json' fayla yazir(serializasiya edir).
        /// </summary>
        /// <param name="Entity">'json' fayla yazacagimiz datalarin qaynagi.</param>
        /// <param name="FilePath">Hansi pathdeki hansi fayla yazilsin datalar.</param>
        public static void Serialize(List<T> Entity, string FilePath)
        {
            using (FileStream FS = new FileStream(FilePath, FileMode.Append))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<T>));
                serializer.WriteObject(FS, Entity);
            }
        }

        /// <summary>
        /// Verdiyimiz yoldaki 'json' fayldan datalari oxuyur(deserializasiya edir).
        /// </summary>
        /// <param name="FilePath">Hansi pathdeki hansi fayldan oxuyacayiq datalari.</param>
        /// <returns>Oxunmuw datalari tek bir kolleksiya daxilinde geri dondurur, oxunasi bir wey ve ya umumiyyetle fayl movcud deyilse null dondurur.</returns>
        public static List<T> Deserialize(string FilePath)
        {
            using (FileStream FS = new FileStream(FilePath, FileMode.Open))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<T>));

                if (File.Exists(FilePath))
                {
                    List<T> datas = serializer.ReadObject(FS) as List<T>;

                    return datas;
                }

                return null;
            }
        }
    }
}