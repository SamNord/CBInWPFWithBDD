using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compte_Bancaire_WPF.Classes
{
    public class Configuration
    {
        public static SqlConnection Connection = new SqlConnection(@"Data Source=(LocalDb)\TPCompteBancaire;Integrated Security=True");
        public static SqlCommand Command;
        public static SqlDataReader Reader;
    }
}
