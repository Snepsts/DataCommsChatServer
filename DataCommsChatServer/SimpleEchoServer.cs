/******************************************************************************
    SimpleEchoServer.cs - Simple TCP echo server using sockets

  This program demonstrates the use of socket APIs to echo back the
  client sentence.  The user interface is via a MS Dos window.

  This program has been compiled and tested under Microsoft Visual Studio 2010.

  Copyright 2012 by Ziping Liu for VS2010
  Prepared for CS480, Southeast Missouri State University

******************************************************************************/
/*-----------------------------------------------------------------------
 *
 * Program: SimpleEchoServer
 * Purpose: wait for a connection from an echo client and echo data
 * Usage:   SimpleEchoServer <portnum>
 *
 *-----------------------------------------------------------------------
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class SimpleEchoServer
{
	public static void Main(string[] args)
	{
		int recv;
		byte[] data = new byte[1024];

		if(args.Length > 1) // Test for correct # of args
			throw new ArgumentException("Parameters: [<Port>]");

		IPEndPoint ipep = new IPEndPoint(IPAddress.Any, Int32.Parse(args[0]));
		Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		server.Bind(ipep);
		server.Listen(10);

		DateTime currTime = DateTime.Now;
		string genMessage = "SimpleEchoServer log generated at: " + currTime + Environment.NewLine;
		//System.IO.StreamWriter log = new System.IO.StreamWriter(@"C:\Users\Public\EchoServerLog.txt"); did not work, but for some reason the using statement does
		using (System.IO.StreamWriter log = new System.IO.StreamWriter(@"C:\Users\Public\EchoServerLog.txt"))
		{
			log.WriteLine(genMessage);
			log.WriteLine(Environment.NewLine); //one more line to make it clear that the log starts below here

			for (; ; )
			{
				string prompt = "Do you need to shut down server? Yes or No";
				Console.WriteLine(prompt);
				log.WriteLine(prompt);
				string choice = Console.ReadLine();
				log.WriteLine(choice);
				if (choice.Contains("Y") || choice.Contains("y"))
				{
					string shutdownMsg = "The server is shutting down...";
					Console.WriteLine(shutdownMsg);
					log.WriteLine(shutdownMsg);
					break;
				}
				string waitMsg = "Waiting for a client...";
				Console.WriteLine(waitMsg);
				log.WriteLine(waitMsg);
				Socket client = server.Accept();
				IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
				string connectMsg = "Connected with " + clientep.Address + " at port " + clientep.Port;
				Console.WriteLine(connectMsg);
				log.WriteLine(connectMsg);
				string welcome = "Welcome to my test server";
				data = Encoding.ASCII.GetBytes(welcome);
				client.Send(data, data.Length, SocketFlags.None);

				while (true)
				{
					data = new byte[1024];
					recv = client.Receive(data);
					if (recv == 0)
						break;
					currTime = DateTime.Now;
					string msg = "[" + currTime + "] client: " + Encoding.ASCII.GetString(data, 0, recv);
					Console.WriteLine(msg);
					log.WriteLine(msg);
					client.Send(data, recv, SocketFlags.None);
				}
				string disconnectMsg = "Disconnected from " + clientep.Address;
				Console.WriteLine(disconnectMsg);
				log.WriteLine(disconnectMsg);
				client.Close();
			}
		}

		server.Close();
	}
}