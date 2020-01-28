using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compte_Bancaire_WPF.Classes
{
    public class Compte
    {
        private int id;
        private decimal solde;
        private int idClient;


        public int Id { get => id; set => id = value; }
        public decimal Solde { get => solde; set => solde = value; }
        public int IdClient { get => idClient; set => idClient = value; }


        public Compte()
        {
            Solde = 0;
        }

        /************************Ajout du compte*********************
         --> pas eu besoin car elle y est avec l'ajout du client********/
        public bool Save()
        {
            Configuration.Command = new SqlCommand("INSERT INTO compte (solde, id_client) OUTPUT INSERTED.ID Values(@solde, @idC)", Configuration.Connection);
            Configuration.Command.Parameters.Add(new SqlParameter("@solde", Solde));
            Configuration.Command.Parameters.Add(new SqlParameter("@idC", IdClient));
            Configuration.Connection.Open();
            Id = (int)Configuration.Command.ExecuteScalar();
            Configuration.Command.Dispose();
            Configuration.Connection.Close();
            return Id > 0;
        }

        /********************Suppression du compte****************
         --> pas demandée*/
        public bool Delete()
        {
            bool res = false;
            Configuration.Command = new SqlCommand("DELETE  from compte where id= @id", Configuration.Connection);
            Configuration.Command.Parameters.Add(new SqlParameter("@id", Id));
            Configuration.Connection.Open();
            if (Configuration.Command.ExecuteNonQuery() > 0)
            {
                res = true;
            }
            Configuration.Command.Dispose();
            Configuration.Connection.Close();
            return res;
        }

        /*******************Recherche du compte du client par son id*************
         --> besoin pour effectuer l'opération */
        public static Compte GetCompteByClient(int idClient)
        {
            Compte compte = null;
            Configuration.Command = new SqlCommand("SELECT id, solde FROM compte WHERE id=@idC", Configuration.Connection);
            Configuration.Command.Parameters.Add(new SqlParameter("@idC", idClient));
            Configuration.Connection.Open();
            Configuration.Reader = Configuration.Command.ExecuteReader();
            while (Configuration.Reader.Read())
            {
                compte = new Compte
                {
                    Id = Configuration.Reader.GetInt32(0),
                    Solde = Configuration.Reader.GetDecimal(1),
                    IdClient = idClient
                };
            }
            Configuration.Reader.Close();
            Configuration.Command.Dispose();
            Configuration.Connection.Close();
            return compte;
        }

      //mis à jour du compte en cas de retrait ou de dépot
        public bool Update()
        {
            bool result = false;
            string request = "UPDATE compte set solde=@solde where id=@id";
            Configuration.Command = new SqlCommand(request, Configuration.Connection);
            Configuration.Command.Parameters.Add(new SqlParameter("@id", Id));
            Configuration.Command.Parameters.Add(new SqlParameter("@solde", Solde));
            Configuration.Connection.Open();
            if (Configuration.Command.ExecuteNonQuery() > 0)
            {
                result = true;
            }
            Configuration.Command.Dispose();
            Configuration.Connection.Close();
            return result;
        }

        public static bool DeleteTableCompte()
        {
            bool res = false;
            Configuration.Command = new SqlCommand("TRUNCATE Table compte", Configuration.Connection);
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
