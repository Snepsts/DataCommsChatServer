# Chat Server
#### By: Michael Ranciglio and Will Mertz

## How to use
### Building
This project was built in Microsoft Visual Studio, so you'll want to open it in
Visual Studio and compile it there. It is assumed you can figure that out
yourself.

NOTE: This project was designed to be used in conjunction with the
DataCommsChatClient project, so you'll want to have that too. You'll also want
to run this before running that one.

### Running
* Open the Windows command prompt. (Search for cmd)
* Move to the directory with the chat server binary.
* The program takes an argument for the port/application number you run it on.
	* We recommend using something around 2000x (x being 0-9).
		* i.e. DataCommsChatServer.exe 20003
* Now you'll want to launch the client with the same port/application number as
the server.
* After you launch the client, you're good to receive messages!!!
* You can exit by having the client exit and responding to the prompt with
"yes".
* A server log is written in C:\Public\Users\EchoServerLog.txt
