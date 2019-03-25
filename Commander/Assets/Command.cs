using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using UnityEngine.UI;
using System.Threading;

public class Command : MonoBehaviour {
	public Camera Cam;


	private IPAddress ip;
	private EndPoint ep;

	private Thread SendThread;
	private Thread RecvThread;

	private byte[] SendBuffer = new byte[100];
	private byte[] RecvBuffer = new byte[1024];

	//Create Socket
	private  Socket client = new Socket (AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

	#region Start
	void Start () {
		//Start a thread for sending message
		//SendThread = new Thread (SendMessageToServer);

		//Prepare the IP+Port for connecting
		ip=IPAddress.Parse("192.168.1.4");
		ep = new IPEndPoint (ip, 6000);

		//Connect to the server
		client.Connect(ep);

		//Send message to server
		string message = "This is Client!";
		SendBuffer = Encoding.UTF8.GetBytes (message);			
		client.Send(SendBuffer);


		//Start a thread for Receiving message
		RecvThread = new Thread (RecvMessageFromServer);
		RecvThread.IsBackground =true;
		RecvThread.Start ();

	}
	#endregion

	#region Update
	void Update(){
		//shutdown, close socket, close thread
		//RecvMessageFromServer()
		if (RecvBuffer != null) {
			char ID = BitConverter.ToChar (RecvBuffer, 0);
			if (ID == 'D') {
				
				/// Rotation
				float Rx = BitConverter.ToSingle (RecvBuffer, sizeof(char));
				float Ry = BitConverter.ToSingle (RecvBuffer, sizeof(char)+1 * sizeof(float));
				float Rz = BitConverter.ToSingle (RecvBuffer, sizeof(char)+2 * sizeof(float));
				/// Position
				float Tx = BitConverter.ToSingle (RecvBuffer, sizeof(char)+3 * sizeof(float));
				float Ty = BitConverter.ToSingle (RecvBuffer, sizeof(char)+4 * sizeof(float));
				float Tz = BitConverter.ToSingle (RecvBuffer, sizeof(char)+5 * sizeof(float));

				Cam.transform.rotation = Quaternion.Euler (Rx, Ry, Rz);
				Cam.transform.position = new Vector3 (Tx, Ty, Tz);
				print(Tx);
			}
		}
	}

	#endregion

	#region RecvMessageFromServer
	public  void RecvMessageFromServer(){

		client.Receive (RecvBuffer);

	}
	#endregion



}
