// using Steamworks;
// using UnityEngine;
// using Mirror;
// using System.Collections.Generic;
// public class SteamP2PTransport : Transport
// {
//  private readonly Dictionary<CSteamID, P2PConnectionState> connections = new();

//     public override bool Available()
//     {
//         return SteamAPI.IsSteamRunning();
//     }

//     public override void ClientConnect(string address)
//     {
//         CSteamID hostId = new CSteamID(ulong.Parse(address));
//         Debug.Log($"Attempting to connect to host: {hostId}");
//         connections[hostId] = P2PConnectionState.Connecting;
//     }

//     public override bool ClientConnected()
//     {
//         foreach (var state in connections.Values)
//         {
//             if (state == P2PConnectionState.Connected)
//                 return true;
//         }
//         return false;
//     }

//     public override void ClientDisconnect()
//     {
//         foreach (var connection in connections.Keys)
//         {
//             SteamNetworking.CloseP2PSessionWithUser(connection);
//         }
//         connections.Clear();
//         Debug.Log("Disconnected from Steam P2P.");
//     }

//     public override void ServerStart()
//     {
//         Debug.Log("Server started on Steam P2P.");
//     }

//     public override void ServerStop()
//     {
//         foreach (var connection in connections.Keys)
//         {
//             SteamNetworking.CloseP2PSessionWithUser(connection);
//         }
//         connections.Clear();
//         Debug.Log("Server stopped on Steam P2P.");
//     }

//     public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId)
//     {
//         CSteamID target = new CSteamID((ulong)connectionId);
//         SteamNetworking.SendP2PPacket(target, segment.Array, (uint)segment.Count, channelId == Channels.Reliable ? EP2PSend.k_EP2PSendReliable : EP2PSend.k_EP2PSendUnreliable);
//     }

//     public override bool ServerActive()
//     {
//         return connections.Count > 0;
//     }

//     public override void ClientSend(ArraySegment<byte> segment, int channelId)
//     {
//         foreach (var connection in connections.Keys)
//         {
//             SteamNetworking.SendP2PPacket(connection, segment.Array, (uint)segment.Count, channelId == Channels.Reliable ? EP2PSend.k_EP2PSendReliable : EP2PSend.k_EP2PSendUnreliable);
//         }
//     }

//     private void Update()
//     {
//         while (SteamNetworking.IsP2PPacketAvailable(out uint packetSize))
//         {
//             byte[] buffer = new byte[packetSize];
//             if (SteamNetworking.ReadP2PPacket(buffer, packetSize, out uint bytesRead, out CSteamID sender))
//             {
//                 if (!connections.ContainsKey(sender))
//                 {
//                     connections[sender] = P2PConnectionState.Connected;
//                 }

//                 OnClientDataReceived.Invoke(buffer, 0);
//                 OnServerDataReceived.Invoke((int)sender.m_SteamID, buffer, 0);
//             }
//         }
//     }
// }   



