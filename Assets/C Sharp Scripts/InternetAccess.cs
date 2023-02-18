///===================================================================================
/// Copyright © 2017 Graham Alexander MacDonald.
///===================================================================================

using System.Collections.Generic;
using UnityEngine;      // Monobehaviour, Network, Time, ConnectionTesterStatus

///==================================================================================
/// <summary>
/// Test this machines network connection.
/// </summary>
/// 
/// <remark> 
/// Two types of tests are performed depending on if the machine has a public IP address 
/// present or if it only has a private address(or addresses).
/// 
/// The public IP address test is primarily for when running a server as no tests are 
/// needed for clients with public addresses.In order for the public IP test to succeed 
/// a server instance must be started.A test server will try to connect to the IP and 
/// port of the local server and thus it is shown in the server is connectable.If not 
/// then a firewall is most likely blocking the server port. A server instance needs to
/// be running so that the test server has something to connect to.
/// 
/// The other test is for checking NAT punchthrough capabilities. This is a valid test
/// for both servers and clients and can be performed without any prior setup. There
/// are 4 types of NAT test results (see ConnectionTesterStatus):
/// 
///     Full Cone
///     Address Restricted Cone
///     Port restricted
///     Symmetric
/// 
/// First two types offer full NAT punchthrough support and can connect to any 
/// type.Port restricted type cannot connect to or receive a connection from symmetric
/// type. Symmetric if worst and cannot connect to other symmetric types or port 
/// restricted type. The latter two are labelled as offering limited NAT punchthrough
/// support.
/// 
/// This function is asynchronous and might not return a valid result right away 
/// because the tests needs some time to complete (1-2 seconds). After test completion
/// the test result is only returned when the function is called again, a full network
/// test is not redone.That way it is safe to poll the function frequently. If another
/// test is desired, like if the network connection has been altered, then the 
/// forceTest parameter should be passed as true.
/// 
/// The function returns a ConnectionTesterStatus enum.
/// </remark>
///==================================================================================

public class InternetAccess : MonoBehaviour
{
    public string publicIpAddress = null;
    public List<string> openDotNatResult = new List<string>();
    public string openDotNatStatus = null;
    public bool doneNatTesting = false;
}

