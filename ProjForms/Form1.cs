using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjForms
{
    public partial class Form1 : Form
    {
        private TcpListener _listener;
        private CancellationTokenSource _cts;
        private List<Task<TcpClient>> _clientTasks = new List<Task<TcpClient>>();
        private int _clientCount = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private async void btnStartServer_Click(object sender, EventArgs e)
        {
            try
            {
                // Cr�ation du socket serveur
                _listener = new TcpListener(IPAddress.Any, 8888);
                _listener.Start();
                txtLogs.AppendText("Serveur d�marr�. Adresse IP : " + IPAddress.Any + " Port : 8888\n");

                _cts = new CancellationTokenSource();

                while (!_cts.Token.IsCancellationRequested)
                {
                    // Attendre la connexion d'un client
                    TcpClient client = await _listener.AcceptTcpClientAsync();

                    // Cr�er une t�che pour g�rer la connexion du client
                    Task<TcpClient> clientTask = HandleClientAsync(client, ++_clientCount, _cts.Token);

                    // Ajouter la t�che � la liste des t�ches clients
                    _clientTasks.Add(clientTask);
                }
            }
            catch (Exception ex)
            {
                txtLogs.AppendText("Erreur : " + ex.Message + "\n");
            }
        }

        private async Task<TcpClient> HandleClientAsync(TcpClient client, int clientNumber, CancellationToken token)
        {
            try
            {
                // Afficher un message pour signaler la connexion du client
                txtLogs.AppendText("Client " + clientNumber + " connect�.\n");

                // R�cup�rer le flux de donn�es entrant et sortant du socket client
                NetworkStream stream = client.GetStream();

                // Envoyer un message de bienvenue au client
                byte[] welcomeMessage = Encoding.ASCII.GetBytes("Bienvenue sur le serveur de chat. Vous �tes le client num�ro " + clientNumber + "\n");
                await stream.WriteAsync(welcomeMessage, 0, welcomeMessage.Length, token);

                // Boucle de r�ception des messages du client
                while (!token.IsCancellationRequested)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);

                    // Si le client se d�connecte, quitter la boucle de r�ception
                    if (bytesRead == 0)
                        break;

                    // Convertir les donn�es re�ues en cha�ne de caract�res
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    // Afficher le message re�u
                    txtLogs.AppendText("Client " + clientNumber + ": " + message + "\n");

                    // Envoyer le message re�u � tous les autres clients connect�s
                    byte[] messageBytes = Encoding.ASCII.GetBytes("Client " + clientNumber + ": " + message);
                    for (int i = 0; i < _clientTasks.Count; i++)
                    {
                        Task clientTask = _clientTasks[i];
                        if (clientTask != Task.CurrentId)
                        {
                            TcpClient connectedClient = await clientTask;
                            await connectedClient.GetStream().WriteAsync(messageBytes, 0, messageBytes.Length, token)
                                }

                 catch (Exception ex)
            {
                txtLogs.AppendText("Erreur : " + ex.Message + "\n");
            }
        }
            
    }
}