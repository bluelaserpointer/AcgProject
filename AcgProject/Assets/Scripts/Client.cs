using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client : MonoBehaviour
{
    [SerializeField]
    private string host = "127.0.0.1";
    [SerializeField]
    private int port = 10086;

    private string message;
    private Socket client;
    private byte[] messTmp;
    void Start()
    {
        messTmp = new byte[1024 * 32];
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Debug.Log("Connecting...");
        try
        {
            client.Connect(new IPEndPoint(IPAddress.Parse(host), port));
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return;
        }
        //client.Close();
    }
    Data ReadToObject(string json)
    {
        Data deserializedUser = new Data();
        print("json: " + json);
        deserializedUser = (Data)JsonUtility.FromJson(json, deserializedUser.GetType());
        return deserializedUser;
    }
    void FixedUpdate()
    {
        var count = client.Receive(messTmp);
        if (count != 0)
        {
            string str = Encoding.UTF8.GetString(messTmp, 1, count - 2);
            int overIndex = str.IndexOf("'");
            if(overIndex != -1)
                str = str.Substring(0, str.IndexOf("'"));
            Data frame = ReadToObject(str);
            message = frame.ToString();
            Array.Clear(messTmp, 0, count);
        }
        Debug.Log(message);
    }
}

[Serializable]
class Data
{
    public List<Frame> infolist;
    public override string ToString()
    {
        string tmp = "";
        foreach (Frame one in infolist)
        {
            tmp += one + "\n";
        }
        return tmp;
    }
}

[Serializable]
class Frame
{
    public string x;
    public string y;
    public string z;
    public override string ToString()
    {
        return "x: " + x + " y: " + y + " z: " + z;
    }
}