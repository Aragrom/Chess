using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using System.Threading;

public class Client : MonoBehaviour
{
    public int connectAttempt = 0;

    public String address = "127.0.0.1";
    public Int32 port = 13000;

    public TcpClient tcpClient = null;                          // Functionality for clients to Connect - Receive and Send messages.
    public bool sendMessage = false;
    public string message = "yo";                               // Input/message sent to the Game Server.
    private int messageLength = 0;                              // Used in checking and storing the length of messages.
    const int MAX_MESSAGE_SIZE = 256;                           // In Bytes.
    private Byte[] bytes = new Byte[MAX_MESSAGE_SIZE];          // Buffer to store the response bytes.
    private String data = String.Empty;                         // String to store the response ASCII representation.

    public void Connect()
    {
        try
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer 
            // connected to the same address as specified by the server, port
            // combination.

            Debug.Log("Attempting to connect to Server: " + address + " on Port: " + port);
            connectAttempt++;

            tcpClient = new TcpClient(address, port);

            if (tcpClient.Connected)
            {
                Debug.Log("Success connecting to the server. Client IP Address: " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString() + " Port: " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port.ToString());
            }
            else
            {
                tcpClient.GetStream().Close();
                tcpClient.Close();
                tcpClient = null;

                Debug.Log("Failure connecting to the server");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Client.Connect - Exception: " + e);
        }
    }

    public void Receive()
    {
        try
        {
            // Loop to receive all the data sent by the server.
            while (tcpClient.GetStream().DataAvailable)
            {
                // Read the first batch of the TcpServer response bytes.
                messageLength = tcpClient.GetStream().Read(bytes, 0, MAX_MESSAGE_SIZE);

                data = Encoding.ASCII.GetString(bytes, 0, messageLength);

                Debug.Log("Received: " + data);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Client.Receive - Exception: " + e);
        }
    }

    public void Send(string message)
    {
        try
        {
            // Translate the passed message into ASCII and store it as a Byte array.
            byte[] bytes = Encoding.ASCII.GetBytes(message);

            if (bytes.Length <= MAX_MESSAGE_SIZE)
            {
                tcpClient.GetStream().Write(bytes, 0, bytes.Length);

                Debug.Log("Sent: " + message);
            }
            else
            {
                Debug.Log("Message size to large... (" + bytes.Length + "/" + MAX_MESSAGE_SIZE + ")");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Client.Send - Exception: " + e);
        }
    }

    public void Disconnect()
    {
        tcpClient.GetStream().Close();
        tcpClient.Close();
        tcpClient = null;
    }
}
