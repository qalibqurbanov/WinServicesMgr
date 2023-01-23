using Microsoft.Win32;
using WinServicesMgr.Entities;
using System.Collections.Generic;

namespace WinServicesMgr.Helpers
{
    /// <summary>
    /// Registry ile elaqeli komekci sinif.
    /// </summary>
    static class RegistryHelper
    {
        /// <summary>
        /// Servisin Start tipini deyiwir.
        /// </summary>
        /// <param name="Entities">Start tipi deyiwecek olan servisleri temsil eden entityler.</param>
        /// <returns>Geriye start tipi deyiwen servis sayini qaytarir.</returns>
        public static int ChangeServiceStartMode(List<ServiceStateEntity> Entities)
        {
            int changedStateCount = 0;

            foreach (ServiceStateEntity entity in Entities)
            {
                if (GetKeyValue(entity.ServiceName) != null) /* GetKeyValue() icerisinde dediyim sebebe gore yoxlayiram */
                {
                    if (GetKeyValue(entity.ServiceName) != (int)entity.ServiceStartMode) /* Movcud start tipi elimdeki entity-de olan start tipinden ferqlidirse, demeli burada start tipini deyiwecem, bu sebeble count-u bir artiriram */
                    {
                        Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Services\\{entity.ServiceName}").SetValue("Start", entity.ServiceStartMode);
                        changedStateCount++;
                    }
                }
            }

            return changedStateCount;
        }

        /// <summary>
        /// Servisin start tipini oxuyur.
        /// </summary>
        /// <param name="ServiceName">Icerisinden start tipini oxumaq istediyimiz servisin adi.</param>
        /// <returns>Geriye servisin start tipini dondurur, start tipini saxlayan subkey yoxdursa geriye 'null' dondururuk.
        /// <para/>
        /// + Bezi servislerin adi/Key reyestrda oz-ozune deyiwe bilir, eger hemin bezi servislerin start tipini 'services.msc'-den deyiwsek hemin servisin reyestrdaki qeydinin reyestrda eyni pathde kopyasi yaradilaraq kohne qeyd silinecek ve ya silinmeyede biler, bu zaman axtarmaga caliwdigim qeyd/servis silinmiw olur, ona gore de eger qeyd tapilmasa(demeli, hec Start adli subkeyide yoxdur) 'null' ve ya daha deqiq desem 'null ala bilen int' dondururem.</returns>
        private static int? GetKeyValue(string ServiceName)
        {
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Services\\{ServiceName}");
            if (regKey != null)
            {
                return (int)regKey.GetValue("Start");
            }

            return null;

            /*
                * 
             */
        }
    }
}