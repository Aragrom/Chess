using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Connection
{
    public int id = 0;
    public TcpClient tcpClient = null;

    public Connection(int _id, TcpClient _tcpClient)
    {
        id = _id;
        tcpClient = _tcpClient;
    }
}
