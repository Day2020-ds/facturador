using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Configuration;

namespace Facturador.GHO.CFDRIP
{
    public class CFDIRIPConexionBD
    {
        MySqlConnection conectar;
        public CFDIRIPConexionBD()
        {
            conectar = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }
        public MySqlConnection ObtenerConexion()
        {
            try
            {
                conectar.Open();
                return conectar;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return conectar;
            }
        }
        public void CerrarConexion()
        {
            conectar.Close();
        }
    }
}