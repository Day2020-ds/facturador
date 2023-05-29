using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IntelliLock.Licensing;

namespace Facturador.GHO.Controllers
{
    public class License
    {
        public List<Parametro> parametros = null;

        public License()
        {
            this.ReadLicenseInformation();
        }

        public License(string rutaLicencia)
        {
            this.LoadLicense(rutaLicencia);
            this.ReadLicenseInformation();
        }

        public bool IsValidLicenseAvailable()
        {
#if DEBUG
            return (EvaluationMonitor.CurrentLicense.LicenseStatus == IntelliLock.Licensing.LicenseStatus.Licensed);
#else
            return true;
            
#endif
        }

        public int ObtenerEmpresas()
        {
#if DEBUG
            int value = 200;
#else
            int value = 0;
#endif
            try
            {
                if (parametros != null)
                {
                    var emp = (from e in parametros where e.key == "Parameter1" select e.value).FirstOrDefault();
                    if (emp != null)
                    {
                        value = Convert.ToInt32(emp);
                    }
                }
            }
            catch { }
            return value;
        }

        public string ObtenerCompania()
        {
            string value = string.Empty;
            try
            {
                if (parametros != null)
                {
                    var comp = (from e in parametros where e.key == "Company" select e.value).FirstOrDefault();
                    if (comp != null)
                    {
                        value = comp;
                    }
                }
            }
            catch { }
            return value;
        }

        public void CheckExpirationDaysLock()
        {
            bool lock_enabled = EvaluationMonitor.CurrentLicense.ExpirationDays_Enabled;
            int days = EvaluationMonitor.CurrentLicense.ExpirationDays;
            int days_current = EvaluationMonitor.CurrentLicense.ExpirationDays_Current;
        }

        public string GetHardwareID()
        {
            return HardwareID.GetHardwareID(false, true, false, true, false, false);
        }

        public bool CompareHardwareID()
        {
            if (HardwareID.GetHardwareID(false, true, false, true, false, false) == EvaluationMonitor.CurrentLicense.HardwareID)
                return true;
            else
                return false;
        }

        public void LoadLicense(string filename)
        {
            try
            {
                EvaluationMonitor.LoadLicense(filename);
            }
            catch { }
        }

        public void ReadLicenseInformation()
        {
            parametros = new List<Parametro>();
            try
            {
                /* Check first if a valid license file is found */
                if (this.IsValidLicenseAvailable())
                {
                    /* Read additional license information */
                    for (int i = 0; i < EvaluationMonitor.CurrentLicense.LicenseInformation.Count; i++)
                    {
                        string key = EvaluationMonitor.CurrentLicense.LicenseInformation.GetKey(i).ToString();
                        string value = EvaluationMonitor.CurrentLicense.LicenseInformation.GetByIndex(i).ToString();
                        Parametro parametro = new Parametro();
                        parametro.key = key;
                        parametro.value = value;
                        parametros.Add(parametro);
                    }
                }
            }
            catch { }
        }

        public class Parametro
        {
            public string key { get; set; }
            public string value { get; set; }
        }
    }
}