## Description
This project is an asynchronous chat application implemented in C#. It features a server capable of handling multiple client connections simultaneously, using TCP sockets for communication.

## How to Run

### Server
1. Navigate to the server project directory.
2. Run `dotnet run` to start the server.

### Client
1. Navigate to the client project directory.
2. Run `dotnet run` to start a client instance.
3. To start multiple clients, repeat step 2 in a new terminal for each client.

## Features
- TCP/IP socket communication.
- Asynchronous message handling.
- Supports multiple client connections.

## Commands
- Clients can send any text message to the server.
- Type `/exit` to disconnect a client.

## Notes
- This application is for demonstration purposes and does not include advanced features like encryption, authentication, or persistent chat history.
- The server listens on `localhost:8080`.

## Future Improvements
- Implement user authentication.
- Add chat rooms and private messaging.
- Include message encryption for security.
