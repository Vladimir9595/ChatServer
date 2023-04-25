using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace ProjForms
{
    class Programme
    {
        static void Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            try
            {
                // Création du socket serveur
                TcpListener serverSocket = new TcpListener(IPAddress.Any, 8888);
                int counter = 0;
                serverSocket.Start();
                Console.WriteLine("Serveur démarré. Adresse IP : " + IPAddress.Any + " Port : 8888");

                // Boucle infinie pour accepter les connexions client
                while (true)
                {
                    counter++;
                    TcpClient clientSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Client " + counter + " connecté");

                    // Création d'un thread pour gérer la communication avec le client
                    Thread clientThread = new Thread(() => HandleClient(clientSocket, counter));
                    clientThread.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur : " + e.Message);
            }
        }

        // Fonction pour gérer la communication avec un client sur un thread séparé
        static void HandleClient(TcpClient clientSocket, int clientNumber)
        {
            try
            {
                // Récupération du flux de données entrant et sortant du socket client
                NetworkStream stream = clientSocket.GetStream();

                // Envoi d'un message de bienvenue au client
                byte[] welcomeMessage = System.Text.Encoding.ASCII.GetBytes("Bienvenue sur le serveur de chat. Vous êtes le client numéro " + clientNumber + "\n");
                stream.Write(welcomeMessage, 0, welcomeMessage.Length);

                // Boucle infinie pour recevoir et traiter les messages du client
                while (true)
                {
                    byte[] message = new byte[1024];
                    int bytesRead = stream.Read(message, 0, message.Length);

                    // Si le client se déconnecte, on sort de la boucle de traitement
                    if (bytesRead == 0)
                        break;

                    // Affichage du message reçu
                    string receivedMessage = System.Text.Encoding.ASCII.GetString(message, 0, bytesRead);
                    Console.WriteLine("Message reçu du client " + clientNumber + " : " + receivedMessage);
                }

                // Fermeture de la connexion avec le client
                clientSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur : " + e.Message);
            }
        }
    }
}