using Compte_Bancaire_WPF.Classes;
using Compte_Bancaire_WPF.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Compte_Bancaire_WPF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Client client;
        private Compte compte;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AjouterClient(object sender, RoutedEventArgs e)
        {
            Client c = new Client() { Nom = lastname.Text, Prenom = firstname.Text, Telephone = phone.Text };
            if (c.Save())
            {
                //si le client a bien été ajouté on l'ajoute dans la listeClient
                //listeClient.Add(c);
                MessageBox.Show("client ajouté avec le numéro de compte : " + c.Id);
                //on vide le contenu des textbox après l'ajout
                lastname.Text = "";
                firstname.Text = "";
                phone.Text = "";
            }
            else
            {
                MessageBox.Show("erreur");
            }
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32(numero.Text);
            client = Client.GetClients(tel.Text, id);
            compte = Compte.GetCompteByClient(id);
            if (client != null)
            {
                //si le compte n'est pas null on affiche la liste des opérations par rapport à ce compte en cliquant sur rechercher
                if (compte != null)
                {
                    //on met à null la liste dans listView
                    listView.ItemsSource = null;
                    //Grâce à la méthode SeeListOperationCompte on affiche la liste des opération du compte
                    listView.ItemsSource = Operation.SeeListOperationCompte(compte.Id);
                    //affichage des détails du client par message
                    MessageBox.Show($"Nom : {client.Nom}, Prénom : {client.Prenom}, Solde : {compte.Solde}€");
                    //Le solde qu'on a ajouté en propriété dans la classe client, on le fait correspndre à celui du compte
                    client.Solde = compte.Solde;

                    /***on peut aussi ajouter le compte dans une liste mais on s'en servira pas ici*/
                    //listeCompte.Add(compte);
                    //on met à null l'itemsource coorrespondant à la listView des détails du clients
                    detailClient.ItemsSource = null;
                    //on affiche les détails du client dans la listView située à côte de la zone de recherche
                    //grâce à méthode SeeClient qui renvoit le client dans une liste par rapport à son id
                    // méthode définit dans la classe client
                    detailClient.ItemsSource = Client.SeeClient(client.Id, compte.Solde);
                }
                //on vide les contenus des textbox après avoir recherché le client
                numero.Text = "";
                tel.Text = "";
            }
            else
            {
                MessageBox.Show("Erreur");
            }

        }

        private void Depot(object sender, RoutedEventArgs e)
        {
            //on récupère le numéro de compte de la textbox et on le convertit en int
            int idN = Convert.ToInt32(numero.Text);
            /*int idN = Int32.Parse(numero.Text); --> avec un Parse*/
            //on garde quand même ce numéro en string
            string n = numero.Text;
            //on recherche avec ce numéro et le téléphone le client grâce à la méthode GetClients définit dans la classe client

            client = Client.GetClients(tel.Text, idN);
            //client = Client.GetClients(tel.Text, i);
            //on définit le type de l'opération à "depot"
            string typ = "depot";
            /*grâce au constructeur avec paramètre de l'autre fenêtre qu'on va afficher,
             on récupère le téléphone et le type de l'opération sous forme de chaine de caractère*/
            PerformOperation op = new PerformOperation(n, typ);
            //on affiche la fenêtre
            op.Show();

        }

        private void Retrait(object sender, RoutedEventArgs e)
        {
            //même chose mais en définissant le type à "retrait"
            int idNR = Convert.ToInt32(numero.Text);
            /*int idNR = Int32.Parse(numero.Text); --> avec un Parse*/
            //client = Client.GetClients(tel.Text, idNR);
            client = Client.GetClients(tel.Text, idNR);
            string n = numero.Text;
            string typ = "retrait";
            PerformOperation op = new PerformOperation(n, typ);
            op.Show();

        }


        /********************Réinitialisation des données des tables******************/
        /***************************************************************************/
        /*Commande pour réinitialiser les données de la table operation en appelant
        la méthode DeleteTableOperation définit dans la classe operation
        --> j'ai fais la même chose pour les autres tables dans leur propres classes
        --> voir ci-dessous
        --> bouton en rouge dans la fenêtre principale*/
        private void DeleteTableOp(object sender, RoutedEventArgs e)
        {
            if (Operation.DeleteTableOperation())
            {
                MessageBox.Show("table opération réinitialisée");
            }
        }

        private void DeleteTableCpt(object sender, RoutedEventArgs e)
        {
            if (Compte.DeleteTableCompte())
                MessageBox.Show("Données de la table compte réinitialisées");

        }

        private void DeleteTableClient(object sender, RoutedEventArgs e)
        {
            if (Client.DeleteTableClient())
            {

                MessageBox.Show("Données de la table client réinitialisées");
            }
        }

    }
}
