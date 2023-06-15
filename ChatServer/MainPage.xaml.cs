/// <summary>
/// Author:    Draeden Jensen and John Haraden
/// Date:      03-23-2023
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500, Draeden Jensen, and John Haraden - This work may not 
///            be copied for use in Academic Coursework.
///
/// We, Draeden Jensen and John Haraden, certify that this code was written from scratch and
/// we did not copy it in part or whole from another source.  All 
/// references used in the completion of the assignments are cited 
/// in the README file.
///
/// File Contents:
/// Contains a graphical user interface for a server window which represents a chat server. Remote
/// users can connect to the server and send messages. This GUI contains a text window to display
/// the chat in real time, as well as a list of all connected clients.
/// </summary>

using Communications;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using System.Net;
using System.Threading.Channels;

namespace ChatServer;

public partial class MainPage : ContentPage
{
    private bool serverConnected;

    private Networking server;
    private List<Networking> clients;

    private readonly ILogger<MainPage> logger;

    /// <summary>
    /// Constructor which builds the elements needed for the window.
    /// </summary>
    /// <param name="logger">Logger object is passed via dependency injection</param>
    public MainPage(ILogger<MainPage> logger)
    {
        serverConnected = false;
        clients = new();
        this.logger = logger;

        InitializeComponent();
        CreateUserInteractionGrid();

        logger.LogInformation("Server window started");
    }

    /// <summary>
    /// <para>
    /// Event handler method which listens for the user to click the connect button. When it
    /// is clicked, this method either starts or shuts down the server, depending on whether
    /// the server is already connected.
    private void OnClickConnectToServer(object sender, EventArgs e)
    {
        if (serverConnected)
        {
            ShutdownServer();
        } 
        else
        {
            StartServer();
        }
    }

    /// <summary>
    /// Private helper method which starts up a new server. It updates the GUI to show current 
    /// participants as well as the server's name/IP address for sharing. It also creates a new 
    /// Networking object which, on a new thread, begins listening for client connections.
    /// </summary>
    private void StartServer()
    {
        server = new(logger, OnConnect, OnDisconnect, OnMessage, '\n');

        CurrentParticipants.Text = "Current Participants:\n---------------------------------\n";

        string serverName = Dns.GetHostName();
        string IPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
        ServerNameEntry.Text = serverName;
        IPAddressEntry.Text = IPAddress;

        serverConnected = true;
        lock (logger)
        {
            logger.LogInformation($"Server connection established; host name: {serverName}; IP address: {IPAddress}");
        }
        ChatDisplayBox.Text += "Server Started\n";
        StartShutdownButton.Text = "Shutdown Server";

        Thread clientConnectionThread = new(WaitForClients);
        clientConnectionThread.Start();
    }

    /// <summary>
    /// Private helper method which shuts down the connected server. The server's Networking
    /// object stops awaiting new client connections, then each connected client is individually
    /// disconnected and removed from the clients list.
    /// </summary>
    private void ShutdownServer()
    {
        serverConnected = false;
        lock (logger)
        {
            logger.LogInformation("Server shut down");
        }
        ChatDisplayBox.Text += "Server Disconnected\n";
        StartShutdownButton.Text = "Start Server";
        server.StopWaitingForClients();

        lock (clients)
        {
            foreach (Networking client in clients)
            {
                client.Disconnect();
            }

            clients.Clear();
        }
    }

    /// <summary>
    /// Private helper method which is run by a new thread every time the the server is started.
    /// This thread continuously listens for new client connections until the server is shut down.
    /// </summary>
    private void WaitForClients()
    {
        server.WaitForClients(11000, true);
    }

    /// <summary>
    /// The OnConnect callback handles new client connections. It creates a new Networking object
    /// corresponding to the client that joined, and begins listening for messages from that client.
    /// This client is also added to the participants list.
    /// </summary>
    /// <param name="channel">The new client's Networking object</param>
    private void OnConnect(Networking channel)
    {
        SendMessageBack($"{channel.ID} joined the server\n");

        lock (clients)
        {
            clients.Add(channel);

            Thread clientMessageThread = new(() => AwaitMessages(channel));
            clientMessageThread.Start();
        }

        lock (logger)
        {
            logger.LogInformation($"{channel.ID} joined the server. Number of participants: {clients.Count}");
        }
    }

    /// <summary>
    /// Private helper method which is run by a new thread every time a new client joins the
    /// server. As long as they are connected, this thread will be listening for messages from
    /// them.
    /// </summary>
    /// <param name="thisClient">The new client's Networking object</param>
    private void AwaitMessages(Networking thisClient)
    {
        thisClient.AwaitMessagesAsync();
    }

    /// <summary>
    /// The OnDisconnect callback handles clients leaving the server. It removes their Networking
    /// object from the clients list and updates the participants list.
    /// </summary>
    /// <param name="channel">The exiting client's Networking object</param>
    private void OnDisconnect(Networking channel)
    {
        lock (clients)
        {
            clients.Remove(channel);
        }

        lock (logger)
        {
            logger.LogInformation($"{channel.ID} left the server. Number of participants: {clients.Count}");
        }
        SendMessageBack($"{channel.ID} left the server");

        UpdateParticipants();
    }

    /// <summary>
    /// The OnMessage callback is called every time the server receives a message. If the message
    /// contains "command name" the server updates this client's name. If the message contains
    /// "command participants" the server sends them an up-to-date list of all connected clients.
    /// Otherwise, the server sends the message received back to every client connected to the
    /// server.
    /// </summary>
    /// <param name="channel">The client sending the message</param>
    /// <param name="message">The message received</param>
    private void OnMessage(Networking channel, string message)
    {
        Dispatcher.Dispatch(() =>
        {
            string lower = message.ToLower();
            if (lower.Contains("command name "))
            {
                ChangeName(channel, message);
                UpdateParticipants();
            }
            else if (lower.Contains("command participants"))
            {
                SendParticipants(channel);
            }
            else
            {
                message = $"{channel.ID}: {message}";

                lock (logger)
                {
                    logger.LogDebug($"{channel.ID} sent message to server: {message}");
                }
                SendMessageBack(message);
            }
        });
    }

    /// <summary>
    /// Private helper method which changes a client's ID, and updates the server accordingly.
    /// </summary>
    /// <param name="channel">Client wishing to change their name</param>
    /// <param name="message">Message received (which contains "command name")</param>
    private void ChangeName(Networking channel, string message)
    {
        string oldID = channel.ID;
        channel.ID = message.Substring(message.ToLower().IndexOf("command name ") + 13).Replace("\n", "");

        string returnMessage = $"{oldID} changed name to {channel.ID}\n";
        SendMessageBack(returnMessage);
        lock (logger)
        {
            logger.LogDebug(returnMessage);
        }
    }

    /// <summary>
    /// Private helper method which sends a list of participants back to a client, via the
    /// "command participants," syntax. Obtains every client's ID and appends that to the
    /// return message.
    /// </summary>
    /// <param name="channel"></param>
    private void SendParticipants(Networking channel)
    {
        string returnMessage = "command participants";
        string requestInfo = $"{channel.ID} requested a list of participants\n";

        ChatDisplayBox.Text += requestInfo;
        lock (logger)
        {
            logger.LogDebug(requestInfo);
        }

        lock (clients)
        {
            foreach (Networking client in clients)
            {
                returnMessage += $",{client.ID}";
            }
        }
        channel.Send(returnMessage);
    }

    /// <summary>
    /// Private helper method which is called every time a user joins, leaves, or changes their
    /// name. Updates the current participants window to reflect this change.
    /// </summary>
    private void UpdateParticipants()
    {
        Dispatcher.Dispatch(() =>
        {
            CurrentParticipants.Text = "Current Participants:\n---------------------------------\n";
            lock (clients)
            {
                foreach (Networking client in clients)
                {
                    CurrentParticipants.Text += $"{client.ID}\n";
                }
            }
        });
    }

    /// <summary>
    /// Private helper method which allows the server to send a message it has received back out
    /// to every client connected to it.
    /// </summary>
    /// <param name="message">Message to be sent</param>
    private void SendMessageBack(string message)
    {
        Dispatcher.Dispatch(() =>
        {
            ChatDisplayBox.Text += $"{message}";

            List<Networking> toSendTo = new();

            lock (clients)
            {
                foreach (Networking client in clients)
                {
                    toSendTo.Add(client);
                }
            }

            foreach (Networking client in toSendTo)
            {
                client.Send(message);
            }
        });
    }

    /// <summary>
    /// Private helper method called by the constructor, which uses C# syntax to create the
    /// GUI elements.
    /// </summary>
    private void CreateUserInteractionGrid()
    {
        ServerNameLabel.Text = "\tServer Name:\t\t";
        IPAddressLabel.Text = "\tIP Address:\t\t";
    }
}

