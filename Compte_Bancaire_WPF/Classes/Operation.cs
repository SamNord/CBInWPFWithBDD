using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compte_Bancaire_WPF.Classes
{
    public class Operation
    {
        private int id;
        private decimal montant;
        private DateTime date;
        private string type;
        private int idCompte;
        private decimal solde = 0;

        public decimal Montant { get => montant; set => montant = value; }
        public DateTime Date { get => date; set => date = value; }
        public string Type { get => type; set => type = value; }
        public int IdCompte { get => idCompte; set => idCompte = value; }
        public int Id { get => id; set => id = value; }

        public Operation()
        {

        }

        /**********************Ajout de l'opération************************************/
        public bool Save()
        {
            Configuration.Command = new SqlCommand("INSERT INTO operation (date, montant,type, id_compte) OUTPUT INSERTED.ID Values(@date, @montant, @type,@idCompte)", Configuration.Connection);
            Configuration.Command.Parameters.Add(new SqlParameter("@date", Date));
            Configuration.Command.Parameters.Add(new SqlParameter("@montant", Montant));
            Configuration.Command.Parameters.Add(new SqlParameter("@type", Type));
            Configuration.Command.Parameters.Add(new SqlParameter("@idCompte", IdCompte));
            Configuration.Connection.Open();
            Id = (int)Configuration.Command.ExecuteScalar();
            Configuration.Command.Dispose();
            Configuration.Connection.Close();
            return Id > 0;
        }

        /*****On peut utiliser cette méthode si on veut automatiquement mettre à jour le compte*/
        public bool AddOperation(decimal montant, int compteId, decimal s)
        {
            bool res = false;
            Configuration.Command = new SqlCommand("INSERT INTO operation (date, montant, type, id_compte) OUTPUT INSERTED.ID values(@date_operation, @montant, @type, @compteId)", Configuration.Connection);
            Configuration.Command.Parameters.Add(new SqlParameter("@montant", montant));
            Configuration.Command.Parameters.Add(new SqlParameter("@compteId", compteId));
            Configuration.Command.Parameters.Add(new SqlParameter("@type", Type));
            Configuration.Command.Parameters.Add(new SqlParameter("@date_operation", DateTime.Now));
            Configuration.Connection.Open();
            if ((int)Configuration.Command.ExecuteScalar() > 0)
            {
                Configuration.Command.Dispose();
                Configuration.Command = new SqlCommand("UPDATE compte SET solde = @solde  WHERE id = @id", Configuration.Connection);
                Configuration.Command.Parameters.Add(new SqlParameter("@montant", montant));
                Configuration.Command.Parameters.Add(new SqlParameter("@id", compteId));
                Configuration.Command.Parameters.Add(new SqlParameter("@solde", s));
                if (Configuration.Command.ExecuteNonQuery() > 0)
                    res = true;
                Configuration.Command.Dispose();
            }
            Configuration.Connection.Close();
            return res;
        }


        /******************Affichage de la liste des opérations de tous les comptes = historique**************
         * ****************************************************************************************************
         * --> pas utilisée ici car on veut que par compte --< voir ci-dessous***/
        public static List<Operation> SeeListOperation()
        {
            List<Operation> liste = new List<Operation>();
            Configuration.Command = new SqlCommand("SELECT * FROM operation", Configuration.Connection);
            Configuration.Connection.Open();
            Configuration.Reader = Configuration.Command.ExecuteReader();
            while (Configuration.Reader.Read())
            {
                Operation o = new Operation()
                {
                    Id = Configuration.Reader.GetInt32(0),
                    Date = Configuration.Reader.GetDateTime(1),
                    Montant = Configuration.Reader.GetDecimal(2),
                    Type = Configuration.Reader.GetString(3),
                    IdCompte = Configuration.Reader.GetInt32(4)
                };
                //on n'oublie pas d'ajouter l'opération dans la liste tant qu'on effectur l'opération
                liste.Add(o);
            }
            Configuration.Reader.Close();
            Configuration.Command.Dispose();
            Configuration.Connection.Close();
            return liste;
        }

        /********************Affichage de la liste des opération d'un seul compte***********
         *********************************************************************************/
        /*pour ne voir que la liste des opération d'un seul compte*/
        public static List<Operation> SeeListOperationCompte(int idC)
        {
            List<Operation> liste = new List<Operation>();
            Configuration.Command = new SqlCommand("SELECT id, date, montant, type FROM operation where id_compte = @id", Configuration.Connection);
            Configuration.Command.Parameters.Add(new SqlParameter("@id", idC));
            Configuration.Connection.Open();
            Configuration.Reader = Configuration.Command.ExecuteReader();
            while (Configuration.Reader.Read())
            {
                Operation o = new Operation()
                {
                    Id = Configuration.Reader.GetInt32(0),
                    Date = Configuration.Reader.GetDateTime(1),
                    Montant = Configuration.Reader.GetDecimal(2),
                    Type = Configuration.Reader.GetString(3),
                    IdCompte = idC
                };
                liste.Add(o);
            }
            Configuration.Reader.Close();
            Configuration.Command.Dispose();
            Configuration.Connection.Close();
            return liste;
        }


        public override string ToString()
        {
            return "Date operation : " + Date + " Montant : " + Montant;
        }

        /***********************Suppression de l'opération*******************
         ********************************************************************/
        public static bool DeleteTableOperation()
        {
            bool res = false;
            Configuration.Command = new SqlCommand("TRUNCATE Table operation", Configuration.Connection);
            Configuration.Connection.Open();
            if (Configuration.Command.ExecuteNonQuery() > 0)
            {
                res = true;
            }
            Configuration.Command.Dispose();
            Configuration.Connection.Close();
            return res;
        }
    }
}
