using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compte_Bancaire_WPF.Classes
{
    public class Client
    {
        private int id;
        private string nom;
        private string prenom;
        private string telephone;
        private decimal solde;


        public string Nom { get => nom; set => nom = value; }
        public string Prenom { get => prenom; set => prenom = value; }
        public string Telephone { get => telephone; set => telephone = value; }
        public int Id { get => id; set => id = value; }
        public decimal Solde { get => solde; set => solde = value; }

        public Client()
        {

        }

        /******************Ajouter un client ainsi que son compte dans la base de données*******/
        public bool Save()
        {
            bool res = false;
            //par défaut le solde est à zéro
            Solde = 0;
            Configuration.Command = new SqlCommand("INSERT INTO client (nom, prenom, telephone) OUTPUT INSERTED.ID VALUES(@nom,@prenom,@telephone)", Configuration.Connection);
            //on n'oublie pas d'ajouter les paramètres
            Configuration.Command.Parameters.Add(new SqlParameter("@nom", Nom));
            Configuration.Command.Parameters.Add(new SqlParameter("@prenom", Prenom));
            Configuration.Command.Parameters.Add(new SqlParameter("@telephone", Telephone));
            //on ouvre la connection
            Configuration.Connection.Open();
            //éxécution de la commande en récupérant l'id de la table client
            Id = (int)Configuration.Command.ExecuteScalar();
            //on libère cette première commande
            Configuration.Command.Dispose();
            /*si le client a bien été ajouté alors on ouvre une autre commande pour ajouter son compte
            dont  le solde vaut zéro au départ*/
            if (Id > 0)
            {
                Configuration.Command = new SqlCommand("INSERT INTO compte (solde, id_client) OUTPUT INSERTED.ID Values(@solde, @idC)", Configuration.Connection);
                Configuration.Command.Parameters.Add(new SqlParameter("@solde", Solde));
                Configuration.Command.Parameters.Add(new SqlParameter("@idC", Id));
                /*si le compte s'est bien ajouté
                (en vérifiant que l'id du compte = (int)Configuration.Command.ExecuteScalar() >0
                on met le booléen à true*/
                if ((int)Configuration.Command.ExecuteScalar() > 0)
                {
                    res = true;
                }
                //on n'oublie pas de libérer la deuxième commande lié à l'ajout du compte
                Configuration.Command.Dispose();
            }
            //on n'oublie pas de fermer la connection comme certains l'ont fait, je dis pas c'est qui
            Configuration.Connection.Close();
            return res;
        }

        /***************Modification du client**************************/
        public bool Update()
        {
            bool result = false;
            Configuration.Command = new SqlCommand("UPDATE client  set nom = @nom, prenom= @prenom, telephone = @telephone where id=@id", Configuration.Connection);
            Configuration.Command.Parameters.Add(new SqlParameter("@nom", Nom));
            Configuration.Command.Parameters.Add(new SqlParameter("@prenom", Prenom));
            Configuration.Command.Parameters.Add(new SqlParameter("@telephone", Telephone));
            Configuration.Command.Parameters.Add(new SqlParameter("@id", Id));
            Configuration.Connection.Open();
            if (Configuration.Command.ExecuteNonQuery() > 0)
            {
                result = true;
            }
            Configuration.Command.Dispose();
            Configuration.Connection.Close();
            return result;
        }

        /************Suppression du client*****************************/
        //public bool Delete()
        //{
        //    bool result = false;
        //    Configuration.Command = new SqlCommand("DELETE from client where id=@id", Configuration.Connection);
        //    Configuration.Command.Parameters.Add(new SqlParameter("@id", Id));
        //    Configuration.Connection.Open();
        //    if (Configuration.Command.ExecuteNonQuery() > 0)
        //    {
        //        result = true;
        //    }
        //    Configuration.Command.Dispose();
        //    Configuration.Connection.Close();
        //    return result;
        //}

        /************************Recherche du client par son numéro et son téléphone*****/
        public static Client GetClients(string tel, int id)
        {
            Client client = null;
            Configuration.Command = new SqlCommand("SELECT nom, prenom FROM client WHERE id = @id AND telephone=@telephone", Configuration.Connection);
            Configuration.Command.Parameters.Add(new SqlParameter("@id", id));
            Configuration.Command.Parameters.Add(new SqlParameter("@telephone", tel));
            Configuration.Connection.Open();
            //ici on fait un ExecuteReader car on ne fait que lire les données de la table client
            Configuration.Reader = Configuration.Command.ExecuteReader();
            //tant qu'il y a une ligne, on lit les colonnes ou données
            while (Configuration.Reader.Read())
            {
                client = new Client()
                {
                    //l'id et le téléphone on le connait déjà car on recherche par rapport à ça
                    Id = id,
                    //dans la requête : select nom, prenom --> 0(nom), 1(prenom)
                    //on compte à partir de zéro
                    //si c'étatit étoile dans ce cas c'est dans l'ordre des colonnes de la table
                    Nom = Configuration.Reader.GetString(0),
                    Prenom = Configuration.Reader.GetString(1),
                    Telephone = tel
                    //Telephone = Configuration.Reader.GetString(2) --> au cas où on met telephone dans la requête
                };
            }
            //avec Reader on ferme la lecture
            Configuration.Reader.Close();
            Configuration.Command.Dispose();
            Configuration.Connection.Close();
            //on retourne le client
            return client;
        }

        /************************Affichage de tous les clients****************
         **********************************************************************/
        //Pour avoir la liste de tous les clients en cas de besoin
        public static List<Client> SeeListClients()
        {
            List<Client> liste = new List<Client>();
            Configuration.Command = new SqlCommand("SELECT * FROM client", Configuration.Connection);
            Configuration.Connection.Open();
            Configuration.Reader = Configuration.Command.ExecuteReader();
            while (Configuration.Reader.Read())
            {
                Client client = new Client()
                {
                    Id = Configuration.Reader.GetInt32(0),
                    Nom = Configuration.Reader.GetString(1),
                    Prenom = Configuration.Reader.GetString(2),
                    Telephone = Configuration.Reader.GetString(3),
                };
                liste.Add(client);
            }
            Configuration.Reader.Close();
            Configuration.Command.Dispose();
            Configuration.Connection.Close();
            return liste;
        }

        /*********************************Affichage d'un seul client sous forme de liste******/
        public static List<Client> SeeClient(int id, decimal solde)
        {
            List<Client> liste = new List<Client>();
            Configuration.Command = new SqlCommand("SELECT nom, prenom, telephone FROM client WHERE id=@id", Configuration.Connection);
            Configuration.Command.Parameters.Add(new SqlParameter("@id", id));
            Configuration.Connection.Open();
            Configuration.Reader = Configuration.Command.ExecuteReader();
            while (Configuration.Reader.Read())
            {
                Client client = new Client()
                {
                    Id = id,
                    Nom = Configuration.Reader.GetString(0),
                    Prenom = Configuration.Reader.GetString(1),
                    Telephone = Configuration.Reader.GetString(2),
                    Solde = solde
                };
                liste.Add(client);
            }
            Configuration.Reader.Close();
            Configuration.Command.Dispose();
            Configuration.Connection.Close();
            return liste;
        }

        /*méthode ToString générée avec le raccourci ALT+ENTREE dans le code */
        public override string ToString()
        {
            return $"Nom : {Nom}, Prénom : {Prenom}, Telephone : {Telephone}";
        }


        //Méthode de réinitialisation des données e la table client en réinitialisant l'incrémentation
        public static bool DeleteTableClient()
        {
            bool res = false;
            Configuration.Command = new SqlCommand("TRUNCATE Table client", Configuration.Connection);
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
