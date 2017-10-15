/******************************************************************************
            SimpleEchoServer.cs - Simple TCP echo server using sockets

  Copyright 2012 by Ziping Liu for VS2010
  Prepared for CS480, Southeast Missouri State University

            SimpleChatServer.cs - Simple TCP chat server using sockets

  This program demonstrates the use of socket APIs to chat with the client.
  This includes sending and receiving/printing messages. The user interface is
  via a MS Dos window.

  This program has been compiled and tested under Microsoft Visual Studio 2017.

  Copyright 2017 by Michael Ranciglio for VS2017
  Prepared for CS480, Southeast Missouri State University

******************************************************************************/
/*-----------------------------------------------------------------------
 *
 * Program: SimpleChatServer
 * Purpose: wait for a connection from a chat client and send/receive data
 * Usage:   SimpleChatServer <portnum>
 *
 *-----------------------------------------------------------------------
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class SimpleChatServer
{
	private static async Task<string> GetInputAsync() //method to grab input async (type while receiving messages)
	{
		return await Task.Run(() => Console.ReadLine());
	}

	public static void Main(string[] args)
	{
		int recv;
		byte[] data = new byte[1024];
		string genMsg, input, msg;

		if (args.Length > 1) // Test for correct # of args
			throw new ArgumentException("Parameters: [<Port>]");

		IPEndPoint ipep = new IPEndPoint(IPAddress.Any, Int32.Parse(args[0]));
		Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		server.Bind(ipep);
		server.Listen(10);

		DateTime currTime = DateTime.Now;
		genMsg = "SimpleChatServer log generated at: " + currTime + Environment.NewLine;
		using (System.IO.StreamWriter log = new System.IO.StreamWriter(@"C:\Users\Public\ChatServerLog.txt"))
		{
			log.WriteLine(genMsg);
			log.WriteLine(Environment.NewLine); //more lines to make it clear that the log starts below here

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
				bool exit = false;

				while (true)
				{
					Task<string> T = GetInputAsync();

					while (!T.IsCompleted) //waiting for input
					{
						if (client.Available > 0)
						{
							Console.WriteLine();
							data = new byte[1024];
							recv = client.Receive(data);

							if (recv == 0) //server non-reponsive
							{
								exit = true;
								break;
							}
							
							msg = Encoding.ASCII.GetString(data, 0, recv);

							if (msg == "exit")
							{
								string exitMsg = "Exit received, initiating disconnect.";
								Console.WriteLine(exitMsg);
								log.WriteLine(exitMsg);

								exit = true;
								break;
							}

							Console.WriteLine(msg);
							log.WriteLine(msg);
						}

						System.Threading.Thread.Sleep(50); //Wait for .05 seconds before checking again
					}

					if (exit)
						break;
					//when the while loop finishes, we know we have something to send

					input = T.Result;

					if (input.Length == 0) //no input was been entered
						continue;

					if (input == "exit")
					{
						client.Send(Encoding.ASCII.GetBytes(input));
						log.WriteLine(input);
						string exitMsg = "Exit sent, initiating disconnect.";
						Console.WriteLine(exitMsg);
						log.WriteLine(exitMsg);

						break;
					}

					currTime = DateTime.Now;
					string prefix = "[" + currTime + "] server: "; //prepare our prefix to our message (time and name)
					msg = prefix + input; //create message
					client.Send(Encoding.ASCII.GetBytes(msg)); //send message
					Console.WriteLine(msg);
					log.WriteLine(msg); //record message
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
