using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Text;

using System.Threading;
using Open.Nat;
using System.Threading.Tasks;

public class Network : MonoBehaviour
{
    public bool isClient = false;
    public bool isServer = false;
    public Server server = null;
    public Client client = null;

    //public InternetAccess internetAccess = new InternetAccess();
    public InternetAccess internetAccess = null;

    public NetworkReachability networkReachability;
    public bool hasAccess = false;

    public System.Collections.Generic.List<string> localIpAddresses;

    public bool havingTroubleConnecting = false;

    public void Awake()
    {
        client = GetComponent<Client>();
        server = GetComponent<Server>();

        internetAccess = GetComponent<InternetAccess>();

        CheckAccess();
        GetLocalDnsData();

        client.address = localIpAddresses[1];
        server.address = localIpAddresses[1];

        // functions below write on different variables of "internetAccess"
        // There should not be any conflict in results.
        GetExternalIp(internetAccess);
        GetPortMaps(internetAccess);
    }

    public void Update()
    {
        CheckAccess();

        if (internetAccess.doneNatTesting)
        {
            // Echo out results

            for (int i = 0; i < internetAccess.openDotNatResult.Count; i++)
            {
                Debug.Log(internetAccess.openDotNatResult[i].ToString()); 
            }

            internetAccess.doneNatTesting = false;
        }

        if (isClient)
        {
            if (client.tcpClient != null
                && client.tcpClient.Connected == true)
            {
                client.Receive();
            }
        }

        if (isServer)
        {
            // If tcp listener (server is online)
            if (server.tcpListener != null)
            {
                // Is there new client connections pending?
                if (server.tcpListener.Pending())
                {
                    // Go through pending for the new clients.
                    server.Pending();
                }

                // See if any messages have been received from any clients.
                server.Receive();

                // Send out spam messages                
                // Send heart beat signal.
                if (server.connections != null
                    && server.sendHeartBeat == true)
                {
                    server.sendHeartBeat = false;

                    // Send a heart beat message to all clients connected.
                    for (int i = 0; i < Server.MAXIMUM_NUMBER_OF_CLIENTS; i++)
                    {
                        if (server.connections[i] != null)
                        {
                            server.Send(i, "Heart Beat");
                        }
                    }
                }
            }
        }
    }

    public void OnApplicationQuit()
    {
        Debug.Log("exit");

        if (server.tcpListener != null)
        {
            server.DisconnectClients();

            server.tcpListener.Stop(); 
        }

        if (client.tcpClient != null)
        {
            client.Disconnect();
        }
    }

    /// <summary>
    /// 
    /// </summary>

    public void CheckAccess()
    {
        // Get internet access state enum - NetworkReachability.

        if ((networkReachability = Application.internetReachability) != NetworkReachability.NotReachable)
            hasAccess = true;
        else
            hasAccess = false;
    }

    // ==========================================

    public static System.Threading.Tasks.Task GetExternalIp(InternetAccess internetAccess)
    {
        var natDiscoverer = new Open.Nat.NatDiscoverer();
        var cancellationTokenSource = new CancellationTokenSource();
        //cancellationTokenSource.CancelAfter(5000);

        NatDevice natDevice = null;

        return natDiscoverer.DiscoverDeviceAsync(Open.Nat.PortMapper.Upnp, cancellationTokenSource)
        .ContinueWith(task =>
        {
            natDevice = task.Result;
            return natDevice.GetExternalIPAsync();
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            internetAccess.publicIpAddress = task.Result.ToString();
        });
    }

    public static System.Threading.Tasks.Task GetPortMaps(InternetAccess internetAccess)
    {
        internetAccess.openDotNatResult = new List<string>();

        var natDiscoverer = new Open.Nat.NatDiscoverer();
        var cancellationTokenSource = new CancellationTokenSource();
        //cancellationTokenSource.CancelAfter(5000);

        NatDevice natDevice = null;
        var stringBuilder = new StringBuilder();

        internetAccess.openDotNatStatus = "Discovering Device...";

        return natDiscoverer.DiscoverDeviceAsync(Open.Nat.PortMapper.Upnp, cancellationTokenSource)
        .ContinueWith(task =>
        {
            natDevice = task.Result;
            internetAccess.openDotNatStatus = "Getting all port maps...";
            return natDevice.GetAllMappingsAsync();
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            internetAccess.openDotNatStatus = "Looping all maps...";

            foreach (var mapping in task.Result)
            {
                stringBuilder.AppendFormat("{0} {1} {2} {3} {4} {5}",
                    mapping.PublicPort,
                    mapping.PrivateIP,
                    mapping.PrivatePort,
                    mapping.Description,
                    mapping.Protocol == Open.Nat.Protocol.Tcp ? "TCP" : "UDP",
                    mapping.Expiration.ToLocalTime());

                internetAccess.openDotNatResult.Add(stringBuilder.ToString());
                stringBuilder = new StringBuilder();
            }

            internetAccess.openDotNatStatus = "Done getting maps.";

            internetAccess.doneNatTesting = true;
        });
    }

    public static System.Threading.Tasks.Task CreatePortMap(int port, string internalIpAddress, InternetAccess internetAccess)
    {
        internetAccess.openDotNatResult = new List<string>();

        internetAccess.openDotNatStatus = "Initializing...";

        IPAddress ip;

        IPAddress.TryParse(internalIpAddress, out ip);

        var natDiscoverer = new Open.Nat.NatDiscoverer();
        var cancellationTokenSource = new CancellationTokenSource();
        //cancellationTokenSource.CancelAfter(5000);

        NatDevice natDevice = null;
        var stringBuilder = new StringBuilder();

        bool tcpOpened = false;
        bool udpOpened = false;

        internetAccess.openDotNatStatus = "Discovering Device...";

        return natDiscoverer.DiscoverDeviceAsync(Open.Nat.PortMapper.Upnp, cancellationTokenSource)
        .ContinueWith(task =>
        {
            natDevice = task.Result;
            internetAccess.openDotNatStatus = "Creating new port map - TCP...";
            return natDevice.CreatePortMapAsync(new Open.Nat.Mapping(Open.Nat.Protocol.Tcp, ip, port, port, 0, "Chess - TCP"));
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            internetAccess.openDotNatStatus = "Creating new port map - UDP...";
            return natDevice.CreatePortMapAsync(new Open.Nat.Mapping(Open.Nat.Protocol.Udp, ip, port, port, 0, "Chess - UDP"));
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            internetAccess.openDotNatStatus = "Getting all port maps...";
            return natDevice.GetAllMappingsAsync();
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            // NAT devices' mapping table is a scarce resource and that
            // means we cannot have an infinite number of mappings.
            // These tables have a limit, some have 10, 16, 32 and even
            // more entries and then, the question is: how should the 
            // NAT response once the its limit is reached? Well, that's
            // the problem, the specs do not say anything about this so,
            // some NATs just accept the addNewPortMapping request and
            // response with status 200 OK and for that reason the clients,
            // like Open.NAT, have no cheap way to know about that fact.

            // Workaround:

            // Clients can verify that the new requested port mappings
            // have been append to the mapping table by requesting the full
            // list of port mappings and check those ports are listed.

            internetAccess.openDotNatStatus = "Looping all maps...";

            foreach (var mapping in task.Result)
            {
                stringBuilder.AppendFormat("{0} {1} {2} {3} {4} {5}",
                    mapping.PublicPort,
                    mapping.PrivateIP,
                    mapping.PrivatePort,
                    mapping.Description,
                    mapping.Protocol == Open.Nat.Protocol.Tcp ? "TCP" : "UDP",
                    mapping.Expiration.ToLocalTime());

                internetAccess.openDotNatResult.Add(stringBuilder.ToString());
                stringBuilder = new StringBuilder();

                if (mapping.PublicPort == port)
                {
                    if (mapping.Protocol == Open.Nat.Protocol.Tcp)
                    {
                        tcpOpened = true;
                    }
                    else
                    {
                        udpOpened = true;
                    }
                }
            }


            if (tcpOpened && udpOpened)
            {
                internetAccess.openDotNatStatus = "Success Opening Port.";
            }
            else
            {
                internetAccess.openDotNatStatus = "Failed Opening Port.";
            }

            internetAccess.doneNatTesting = true;
        });
    }

    public static System.Threading.Tasks.Task DeletePortMap(int port, InternetAccess internetAccess)
    {
        internetAccess.openDotNatResult = new List<string>();

        internetAccess.openDotNatStatus = "Initializing...";

        var natDiscoverer = new Open.Nat.NatDiscoverer();
        var cancellationTokenSource = new CancellationTokenSource();
        //cancellationTokenSource.CancelAfter(5000);

        NatDevice natDevice = null;
        var stringBuilder = new StringBuilder();

        bool tcpOpened = false;
        bool udpOpened = false;

        internetAccess.openDotNatStatus = "Discovering Device...";

        return natDiscoverer.DiscoverDeviceAsync(Open.Nat.PortMapper.Upnp, cancellationTokenSource)
        .ContinueWith(task =>
        {
            natDevice = task.Result;
            internetAccess.openDotNatStatus = "Deleting port map - TCP...";
            return natDevice.DeletePortMapAsync(new Open.Nat.Mapping(Open.Nat.Protocol.Tcp, port, port));
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            internetAccess.openDotNatStatus = "Deleting port map - UDP...";
            return natDevice.DeletePortMapAsync(new Open.Nat.Mapping(Open.Nat.Protocol.Udp, port, port));
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            internetAccess.openDotNatStatus = "Getting all port maps...";
            return natDevice.GetAllMappingsAsync();
        })
        .Unwrap()
        .ContinueWith(task =>
        {
            internetAccess.openDotNatStatus = "Looping all maps...";

            foreach (var mapping in task.Result)
            {
                stringBuilder.AppendFormat("{0} {1} {2} {3} {4} {5}",
                    mapping.PublicPort,
                    mapping.PrivateIP,
                    mapping.PrivatePort,
                    mapping.Description,
                    mapping.Protocol == Open.Nat.Protocol.Tcp ? "TCP" : "UDP",
                    mapping.Expiration.ToLocalTime());

                internetAccess.openDotNatResult.Add(stringBuilder.ToString());
                stringBuilder = new StringBuilder();

                if (mapping.PublicPort == port)
                {
                    if (mapping.Protocol == Open.Nat.Protocol.Tcp)
                    {
                        tcpOpened = true;
                    }
                    else
                    {
                        udpOpened = true;
                    }
                }
            }

            if (!tcpOpened && !udpOpened)
            {
                internetAccess.openDotNatStatus = "Success Closing Port.";
            }
            else
            {
                internetAccess.openDotNatStatus = "Failed Closing Port.";
            }

            internetAccess.doneNatTesting = true;
        });
    }

    void GetLocalDnsData()
    {
        // Find host by name (name of device) - 3rd element is IPv4 ip address

        System.Net.IPHostEntry ipHostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

        for (int index = 0; index < ipHostEntry.AddressList.Length; index++)
        {
            localIpAddresses.Add(ipHostEntry.AddressList[index].ToString());
        }

        // Debug Read out ==============================================

        string internetPropetiesReadOut = "";

        internetPropetiesReadOut += "Host Name - " + ipHostEntry.HostName + " / ";

        foreach (System.Net.IPAddress ipAddress in ipHostEntry.AddressList)
        {
            internetPropetiesReadOut += "IpAddress - " + ipAddress.ToString() + " / ";
        }

        foreach (string alias in ipHostEntry.Aliases)
        {
            internetPropetiesReadOut += "Alias - " + alias + " / ";
        }

        UnityEngine.Debug.Log("Internet Properties - " + internetPropetiesReadOut);

        // ============================================================
    }
}
