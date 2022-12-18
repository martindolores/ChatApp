using ChatServer.Net.IO;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _packerReader;
        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packerReader = new PacketReader(ClientSocket.GetStream());
            var opcode = _packerReader.ReadByte();
            Username = _packerReader.ReadMessage();
            Console.WriteLine($"{DateTime.Now}: Client has connected with Username: {Username}");
            Task.Run(() => Process());
        }

        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = _packerReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = _packerReader.ReadMessage();
                            Console.WriteLine($"{DateTime.Now}: Message Received! {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: [{Username}]: {msg}");
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"{UID.ToString()}: Disconnected!");
                    Program.BroadcastDisconnected(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
