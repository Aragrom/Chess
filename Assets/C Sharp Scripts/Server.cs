using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using UnityEngine;


// TCP guarantees delivery of data, and also guarantees that 
// packets will be delivered in the same order in which they
// were sent. In order to guarantee that packets are delivered
// in the right order, TCP uses acknowledgement (ACK) packets 
// and sequence numbers to create a "full duplex reliable 
// stream connection between two endpoints", with the endpoints
// referring to the communicating hosts. The connection between
// the client and the server begins with a 3-way handshake.

// After the handshake, it is just a matter of sending packets
// and incrementing the sequence number to verify that the packets
// are getting sent and received.

// 1. Physical layer
// 2. Data layer
// 3. Network layer (IP Protocol)
// 4. Transmission layer (TCP layer)
// 5. Session layer
// 6. Presentation layer
// 7. Application layer

public class Server : MonoBehaviour
{
    public int port = 13000;
    public string address = "127.0.0.1";

    public const int MAXIMUM_NUMBER_OF_CLIENTS = 100;
    public const int MAXIMUM_CLIENT_QUEUE_SIZE = 1000;
    public int numberOfClients = 0;

    public TcpListener tcpListener = null;
    public Connection[] connections = null;

    NetworkStream networkStream = null;
    Byte[] bytes = new Byte[256];
    const int MAX_MESSAGE_SIZE = 256;
    int messageLength = 0;
    String message = null;

    static System.Timers.Timer timer = new System.Timers.Timer();

    public bool sendHeartBeat = false;

    public enum MessageType
    {

    }

    private void SetTimer()
    {
        // Create a timer with a two second interval.
        timer = new System.Timers.Timer(1000);
        // Hook up the Elapsed event for the timer. 
        timer.Elapsed += OnTimedEvent;
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        sendHeartBeat = true;
    }

    public void Initiate()
    {
        try
        {
            SetTimer();

            connections = new Connection[MAXIMUM_NUMBER_OF_CLIENTS];
            for (int i = 0; i < MAXIMUM_NUMBER_OF_CLIENTS; i++)
            {
                connections[i] = null;
            }

            // Set the TcpListener on port 13000.
            Int32 tempPort = port;
            IPAddress tempAddress = IPAddress.Parse(address);

            // TcpListener server = new TcpListener(port);
            tcpListener = new TcpListener(tempAddress, tempPort);

            // Start listening for client requests.
            tcpListener.Start();

            Debug.Log("Started Server successfully.");
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e);

            tcpListener = null;
        }
    }

    private bool IsServerFull()
    {
        // Loop through all available client space.
        for (int i = 0; i < MAXIMUM_NUMBER_OF_CLIENTS; i++)
        {
            if (connections[i] == null)
            {
                return false;
            }
        }

        return true;
    }

    private int GetNewClientID()
    {
        // Loop through to find the first null client.
        for (int i = 0; i < MAXIMUM_NUMBER_OF_CLIENTS; i++)
        {
            if (connections[i] == null)
            {
                // Found an empty client.
                // Return id

                return i;
            }
        }

        Debug.Log("GetNewClientID error - The server is full. No free client IDs. Should have been caught by IsServerFull.");

        return 0;
    }

    private void AddClient(TcpClient tcpClient)
    {
        // Process the connection here. (Add the client to a
        // server table, read data, etc.)
        Debug.Log("Client connection completed");

        // Get new client ID - Search through Client array 
        // for an empty client.
        int id = GetNewClientID();

        // Create new client using constructor and add it to the clients list.
        connections[id] = new Connection(id, tcpClient);

        numberOfClients++;

        Debug.Log("Added client");
    }

    public void Pending()
    {
        while (tcpListener.Pending())
        {
            TcpClient tcpClient = tcpListener.AcceptTcpClient();

            // Check if server is full.
            if (IsServerFull())
            {
                // TO DO ADAPT TO HAVE QUEUE TO GET IN.

                // Server is full

                message = tcpClient.Client.RemoteEndPoint.ToString() + " Server is full.";

                try
                {
                    bytes = System.Text.Encoding.ASCII.GetBytes(message);

                    networkStream = tcpClient.GetStream();

                    // Send message.
                    networkStream.Write(bytes, 0, bytes.Length);

                    networkStream.Close();
                    tcpClient.Close();

                    Debug.Log("Sent: " + message + " Disconnected them from server.");
                }
                catch (Exception e)
                {
                    Debug.Log("Exception: " + e);
                }
            }
            else
            {
                // Server is not full

                AddClient(tcpClient);
            }
        }
    }


    public void Receive()
    {
        // Only process receive when there is some clients.
        if (connections == null) return;

        // Loop through all clients
        for (int i = 0; i < MAXIMUM_NUMBER_OF_CLIENTS; i++)
        {
            // Is there a connected client on this ID.
            if (connections[i] != null
                && connections[i].tcpClient.GetStream().DataAvailable) // will break loop if entered without data
            {
                while (connections[i].tcpClient.GetStream().DataAvailable)
                {
                    messageLength = connections[i].tcpClient.GetStream().Read(bytes, 0, bytes.Length);

                    // Translate data bytes to a ASCII string.
                    message = System.Text.Encoding.ASCII.GetString(bytes, 0, messageLength);

                    Debug.Log("Received: " + message);

                    Parse(message);
                }
            }
        }
    }

    /// <summary>
    /// Weakest part of the game server design.
    /// Reverse engineering of the game server
    /// could be performed through here.
    /// </summary>
    /// <param name="data"></param>

    public void Parse(string data)
    {

    }

    public void Send(int id, string message)
    {
        bytes = Encoding.ASCII.GetBytes(message);

        try
        {
            // Send message.
            connections[id].tcpClient.GetStream().Write(bytes, 0, bytes.Length);

            Debug.Log("Sent: " + message);
        }
        catch (Exception)
        {
            // Cant write to the stream? Maybe disconnected?
            // Disconnect them from sever.
            // Stream will be already closed so just close client.

            connections[id].tcpClient.Close();

            connections[id] = null;

            numberOfClients--;

            Debug.Log("Client " + id + ": connection has ended. Disconnected. Clients left = " + numberOfClients);

            return;
        }
    }

    public void DisconnectClients()
    {
        // Disconnect all connected clients
        for (int i = 0; i < Server.MAXIMUM_NUMBER_OF_CLIENTS; i++)
        {
            if (connections[i] != null)
            {
                DisconnectClient(i);
            }
        }
    }

    public void DisconnectClient(int id)
    {
        // Get and close the network stream for the client.
        // Closing the TCPClient will not do this
        //NetworkStream networkStream = networkState.clients[i].tcpClient.GetStream();

        // Prepare disconnect message
        bytes = Encoding.ASCII.GetBytes("Manually disconnected");

        // Send message that they have been manually disconnected.
        connections[id].tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        Debug.Log("Manually disconnected Client - " + id + " from server");

        connections[id].tcpClient.GetStream().Close();

        // Request that the underlying TCP connection be closed.
        connections[id].tcpClient.Close();

        connections[id] = null;

        numberOfClients--;
    }
}

