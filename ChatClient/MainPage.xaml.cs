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
/// Contains a graphical user interface for a client window which can connect to a remote chat
/// server. Once connected, the GUI allows for sending and receiving messages from the networking,
/// as well as receiving a list of all connected participants. The GUI contains a text window to 
/// display the chat in real time.
/// </summary>

using Microsoft.Extensions.Logging;
using Communications;

namespace ChatClient;

public partial class MainPage : ContentPage
{
    Button connectButton;
    Editor currentParticipants;
    Entry host;
    Entry name;
    Entry message;

    private bool connected;

    private Networking client;
    private readonly ILogger<MainPage> logger;

    /// <summary>
    /// Constructor which builds the elements needed for the window.
    /// </summary>
    /// <param name="logger">Logger object is passed via dependency injection</param>
    public MainPage(ILogger<MainPage> logger)
    {
        connected = false;
        this.logger = logger;

		InitializeComponent();
        CreateUserInteractionGrid();
    
        logger.LogInformation("Client window started");
    }

    /// <summary>
    /// Event handler method which listens for a user clicking the connect button. When a user
    /// clicks the button, it creates a new Networking object, tries to connect to the server
    /// given the credentials entered by the user, and, upon successful connection, begins
    /// awaiting messages from the server.
    /// </summary>
    private void OnClickConnectToServer()
    {
        if (!connected)
        {
            client = new(logger, OnConnect, OnDisconnect, OnMessage, '\n');
            client.ID = name.Text;

            try
            {
                client.Connect(host.Text, 11000);
                connected = true;
                connectButton.Text = "Connected to Server";
                connectButton.BackgroundColor = Colors.SlateGray;

                logger.LogInformation($"Client successfully connected to server; host: {host.Text}");
                ChatDisplayBox.Text += "Connected to Server!\n";


                Thread clientMessageThread = new(AwaitMessages);
                clientMessageThread.Start();
            }
            catch (Exception)
            {
                logger.LogInformation($"Client tried to connect to server, connection failed; host: {host.Text}");
                ChatDisplayBox.Text += "Couldn't connect to server\n";
            }
        }
    }

    /// <summary>
    /// Private helper method which is run by a new thread every time the user clicks connect.
    /// This thread endlessly awaits messages from the server.
    /// </summary>
    private void AwaitMessages()
    {
        client.Send($"Command Name {name.Text}");
        client.AwaitMessagesAsync();
    }

    /// <summary>
    /// Event handler method which listens for the user to complete a message to send in the
    /// corresponding entry element. The method uses the Networking object's send method to 
    /// send the message to the server, and clears the message entry.
    /// </summary>
    private void OnMessageCompleted()
    {
        string text = message.Text;
        logger.LogDebug($"{client.ID} sent message to server: {text}");
        client.Send(text);
        message.Text = "";
    }

    /// <summary>
    /// Event handler method which listens for the user to click the "Retrieve participants" button.
    /// Sends a message to the server, which should send back a list of participants.
    /// </summary>
    private void OnClickRetrieveParticipants()
    {
        client.Send("Command Participants");
    }

    /// <summary>
    /// The OnConnect callback does not need to do anything on the client side.
    /// </summary>
    private void OnConnect(Networking channel) { }

    /// <summary>
    /// The OnDisconnect callback disconnects the underlying Networking object from the server.
    /// </summary>
    /// <param name="channel">This client's Networking object</param>
    private void OnDisconnect(Networking channel) 
    {
        logger.LogInformation($"{client.ID} disconnected from server");
        connected = false;
        lock (client)
        {
            client.Disconnect();
        }
    }

    /// <summary>
    /// The OnMessage callback is ran every time a client receives a message from the server.
    /// In the normal case, this method simply updates the chat display editor with the message
    /// received. In the case where the message contains the phrase "command participants" then
    /// the following message will be parsed and turned into an updated list of all clients
    /// connected to the server, which is then displayed in the current participants display
    /// editor.
    /// </summary>
    /// <param name="channel">This client's Networking object</param>
    /// <param name="message">The message received from the server</param>
    private void OnMessage(Networking channel, string message)
    {
        Dispatcher.Dispatch(() => 
        {
            string lower = message.ToLower();
            if (lower.Contains("command participants,"))
            {
                currentParticipants.Text = "";

                string[] split = message.Split(',');
                split[0] = "Current Participants:\n---------------------------------";
                foreach (string name in split)
                {
                    currentParticipants.Text += $"{name}\n";
                }
                logger.LogDebug($"{client.ID} retrieved list of participants");
                ChatDisplayBox.Text += "Updated list of participants\n";
            }
            else
            {
                ChatDisplayBox.Text += $"{message}";
            }
        });
    }

    /// <summary>
    /// Private helper method called by the constructor, which uses C# syntax to create the
    /// GUI elements.
    /// </summary>
	private void CreateUserInteractionGrid()
	{
		VerticalStackLayout leftLabelEntryColumnStack = new();
        HorizontalStackLayout topRowStack = new(); 
        HorizontalStackLayout secondRowStack = new(); 
        HorizontalStackLayout thirdRowStack = new();
        HorizontalStackLayout bottomRowStack = new();

        topRowStack.Add(
                new Label
                {
                    Text = "Server Name/Address: \t",
                    Padding = new Thickness(5),
                    VerticalOptions = LayoutOptions.Center
                });
        
        host = new Entry
        {
            Placeholder = "localhost",
            WidthRequest = 200,
            HeightRequest = 40,
            HorizontalOptions = LayoutOptions.End
        };
        topRowStack.Add(host);
        topRowStack.HorizontalOptions = LayoutOptions.Fill;
        UserEntryGrid.Children.Add(topRowStack);
        UserEntryGrid.SetRow(topRowStack, 1);

        secondRowStack.Add(
                new Label
                {
                    Text = "Your Name: \t\t",
                    Padding = new Thickness(5),
                    VerticalOptions = LayoutOptions.Center,
                });

        name = new Entry
        {
            Placeholder = "",
            WidthRequest = 200,
            HeightRequest = 40,
            HorizontalOptions = LayoutOptions.End
        };
        secondRowStack.Add(name);
        secondRowStack.HorizontalOptions = LayoutOptions.Fill;
        UserEntryGrid.Children.Add(secondRowStack);
        UserEntryGrid.SetRow(secondRowStack, 2);

        connectButton = new();
        connectButton.Text = "Connect to Server";
        connectButton.WidthRequest = 400;
        connectButton.HeightRequest = 40;
        connectButton.Clicked += (sender, e) => OnClickConnectToServer();

        thirdRowStack.Add(connectButton);
        thirdRowStack.HorizontalOptions = LayoutOptions.Fill;
        UserEntryGrid.Children.Add(thirdRowStack);
        UserEntryGrid.SetRow(thirdRowStack, 3);

        bottomRowStack.Add(
                new Label
                {
                    Text = "Your Message: \t\t",
                    Padding = new Thickness(5),
                    VerticalOptions = LayoutOptions.Center,
                });

        message = new Entry
        {
            WidthRequest = 200,
            HeightRequest = 40,
            HorizontalOptions = LayoutOptions.End
        };
        message.Completed += (sender, e) => OnMessageCompleted();
        bottomRowStack.Add(message);
        bottomRowStack.HorizontalOptions = LayoutOptions.Fill;
        UserEntryGrid.Children.Add(bottomRowStack);
        UserEntryGrid.SetRow(bottomRowStack, 4);


        VerticalStackLayout participantsColumnStack = new();

        currentParticipants = new();

        currentParticipants.IsReadOnly = true;
        currentParticipants.WidthRequest = 400;
        currentParticipants.HeightRequest = 180;
        participantsColumnStack.Add(currentParticipants);

        Button retrieveParticipants = new();
        retrieveParticipants.Text = "Retrieve Participants";
        retrieveParticipants.Clicked += (sender, e) => OnClickRetrieveParticipants();
        participantsColumnStack.Add(retrieveParticipants);
        
        UserEntryGrid.Children.Add(participantsColumnStack);
        UserEntryGrid.SetColumn(participantsColumnStack, 1);
        UserEntryGrid.SetRow(participantsColumnStack, 1);
        UserEntryGrid.SetRowSpan(participantsColumnStack, 4);

        ChatDisplayBox.HeightRequest = 200;
    }
}

