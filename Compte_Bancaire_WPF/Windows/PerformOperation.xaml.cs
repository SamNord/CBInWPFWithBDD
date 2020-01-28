using Compte_Bancaire_WPF.Classes;
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
using System.Windows.Shapes;

namespace Compte_Bancaire_WPF.Windows
{
    /// <summary>
    /// Logique d'interaction pour PerformOperation.xaml
    /// </summary>
    public partial class PerformOperation : Window
    {
        private Compte compte;
        private Operation operation;

        public PerformOperation()
        {
            InitializeComponent();
        }

     
        public PerformOperation(string num, string type) : this()
        {
            numero.Text = num;
            typeO.Text = type;
        }

        /********************Validation de l'opération******************/
        private void Valider(object sender, RoutedEventArgs e)
        {
            //on convertit le numéro de compte en int qu'on récupère grâce à la textBox
            int id = Convert.ToInt32(numero.Text);
            //on convertit le montant en decimal 
            decimal mont = Convert.ToDecimal(montant.Text);
            //on récupère le type de l'opération grâce à la textBox
            string t = typeO.Text;
            //on recherche le compte avec l'id du client
            compte = Compte.GetCompteByClient(id);
            //si ce compte existe alors on effectue les opération selon le type
            if (compte != null)
            {
                //on définit l'operation
                operation = new Operation()
                {
                    Date = DateTime.Now,
                    Montant = mont,
                    Type = t,
                    IdCompte = compte.Id
                };

                //si c'est un dépot....
                if (operation.Type == "depot")
                {
                    //on ajoute le montant au solde
                    compte.Solde += mont;
                    if (operation.Save())
                        MessageBox.Show($"dépot effectué");
                }
                //sinon on le soustrait
                else
                {
                    //si le montant que l'on veut retirer est supérieur au compte solde
                    if (compte.Solde > mont)
                    {
                        //on procède au retrait
                        compte.Solde -= mont;
                        if (operation.Save())
                            MessageBox.Show("retrait effectué");
                    }
                    //sinon on affiche un message
                    else
                    {
                        MessageBox.Show("Le retrait est supérieur à votre compte");
                    }
                }

                //une fois qu'on a effectué l'opération, on n'oublie pas de mettre à jour le solde avec Update
                if (compte.Update())
                {
                    //si la mise à jour est faite on affiche un message avec le solde
                    MessageBox.Show($"solde mis à jour : votre solde est de : {compte.Solde}€");
                }
            }
        }
    }
}
