using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using UnityEngine;
using System.Threading;

public class Server : MonoBehaviour {

	private byte[] SendBuffer = new byte[1024];
	private byte[] RecvBuffer = new byte[100];
	private char InputType;
	public GameObject Myself;
	//private Thread SendThread;
	private Thread Listening;
	Socket tcpSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
	Socket mSocket;

	Vector3 temp;
	// Use this for initialization
	void Start () {

		/*绑定监听消息IP和端口号*/
		IPAddress ip = IPAddress.Parse("192.168.1.4");
		EndPoint endPoint = new IPEndPoint(ip, 6000);
		tcpSocket.Bind(endPoint);//向操作系统申请一个ip和端口号

		Listening = new Thread (ListenClient);
		Listening.IsBackground = true;
		Listening.Start ();

//		SendThread = new Thread (SendMessageToClient);
//		SendThread.IsBackground = true;
//		SendThread.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.LeftArrow)) {
			Myself.transform.position += new Vector3(0.5f,0.0f,0.0f);

		}if (Input.GetKey (KeyCode.RightArrow)) {
			Myself.transform.position -= new Vector3(0.5f,0.0f,0.0f);
		}
		if(mSocket != null)
		SendMessageToClient ();
	}
	
	#region SendMessageToClient
	public void SendMessageToClient(){
			InputType = 'D';
			byte[] inputType_Byte = BitConverter.GetBytes ((char)InputType);
		///Rotation
			temp = Myself.transform.eulerAngles;
			byte[] x_Byte = BitConverter.GetBytes ((float)temp.x);
			byte[] y_Byte = BitConverter.GetBytes ((float)temp.y);
			byte[] z_Byte = BitConverter.GetBytes ((float)temp.z);


		/// Position
			temp = Myself.transform.position;
			byte[] Px_Byte = BitConverter.GetBytes ((float)temp.x);
			byte[] Py_Byte = BitConverter.GetBytes ((float)temp.y);
			byte[] Pz_Byte = BitConverter.GetBytes ((float)temp.z);
		

			inputType_Byte.CopyTo (SendBuffer, 0);
			x_Byte.CopyTo (SendBuffer, inputType_Byte.Length);
			y_Byte.CopyTo (SendBuffer, inputType_Byte.Length+x_Byte.Length);
			z_Byte.CopyTo (SendBuffer, inputType_Byte.Length+x_Byte.Length + y_Byte.Length);

		    Px_Byte.CopyTo(SendBuffer, inputType_Byte.Length+x_Byte.Length + y_Byte.Length + z_Byte.Length);//position x
			Py_Byte.CopyTo(SendBuffer, inputType_Byte.Length+x_Byte.Length + y_Byte.Length + z_Byte.Length + Px_Byte.Length);//position y
		    Pz_Byte.CopyTo(SendBuffer, inputType_Byte.Length+x_Byte.Length + y_Byte.Length + z_Byte.Length + Px_Byte.Length + Px_Byte.Length);//position z
			
		    mSocket.Send (SendBuffer);


	}
	#endregion
	
	private void ListenClient(){
		/*开始监听客户端的连接请求*/
		tcpSocket.Listen(100);//最多可以接收100个客户端请求
		mSocket = tcpSocket.Accept();//暂停当前线程，知道接收到客户端发来的连接请求；当接收到客户端的连接请求后，在本地服务器创建一个socket与客户端连接，并返回出来

	}
	
}

