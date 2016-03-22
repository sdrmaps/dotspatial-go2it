using System;
using System.Collections.Generic;
using System.Text;

namespace SDR.Network
{
    /// <summary>
    /// Command used to interact between the AliServer and Client
    /// </summary>
    public enum Command
    {
        Login,      // Login to the server
        Logout,     // Logout of the server
        Message,    // Send a message to all logged in clients
        List,       // Get a list of clients logged in from the server
        Ping,       // Check if the server is up and listening
        Null        // No command
    }

    /// <summary>
    /// Data packet used to commnicate between sdr aliserver and client
    /// </summary>
    public class AliServerDataPacket
    {
        public AliServerDataPacket()
        {
            Command = Command.Null;
            Message = null;
            Name = null;
        }

        // Convert bytes into an object of type Data
        public AliServerDataPacket(byte[] data)
        {
            // The first four bytes are for the Command
            Command = (Command)BitConverter.ToInt32(data, 0);
            // The next four store the length of the name
            int nameLen = BitConverter.ToInt32(data, 4);
            // The next four store the length of the message
            int msgLen = BitConverter.ToInt32(data, 8);
            // This check makes sure that strName has been passed in the array of bytes
            Name = nameLen > 0 ? Encoding.UTF8.GetString(data, 12, nameLen) : null;
            // This checks for a null message field
            Message = msgLen > 0 ? Encoding.UTF8.GetString(data, 12 + nameLen, msgLen) : null;
        }

        // Converts the Data structure into an array of bytes
        public byte[] ToByte()
        {
            var result = new List<byte>();
            // First four are for the Command
            result.AddRange(BitConverter.GetBytes((int)Command));
            // Add the length of the name
            result.AddRange(Name != null ? BitConverter.GetBytes(Name.Length) : BitConverter.GetBytes(0));
            // Length of the message
            result.AddRange(Message != null ? BitConverter.GetBytes(Message.Length) : BitConverter.GetBytes(0));
            // Add the name
            if (Name != null)
            {
                result.AddRange(Encoding.UTF8.GetBytes(Name));
            }
            // Last we add the message text to our array of bytes
            if (Message != null)
            {
                result.AddRange(Encoding.UTF8.GetBytes(Message));
            }
            return result.ToArray();
        }

        public string Name;      // Name the client logs on with
        public string Message;   // Message text
        public Command Command;  // Command type (login, logout, message, ping, etcetera)
    }
}
