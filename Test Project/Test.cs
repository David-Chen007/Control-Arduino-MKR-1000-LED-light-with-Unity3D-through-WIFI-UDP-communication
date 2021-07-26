using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
public class Test : MonoBehaviour
{
    public struct UdpState
    {
        public UdpClient u;
        public IPEndPoint e;
    }
    //public static bool messageReceived = false;
    public long recievedCounter;
    public float delayTime;
    public long sentCounter;
    public byte[] receiveBytes;
    public static float[] valve_data;
    public string remote_ip_address ;// wifi address
    public string local_ip_address;//game running platform IP  LAB: 192.168.150.101
    public int port ;
    private bool lightState = false;
    public GameObject light = null;
    public AudioClip clip;
    IPEndPoint remote_endpoint;
    UdpClient client;
    IPEndPoint RemoteIpEndPoint;
    // Start is called before the first frame update

    void Start()
    {
        valve_data = new float[5];
        valve_data[1] = 2;
        valve_data[2] = 3;
        valve_data[3] = 4;
        recievedCounter = 0;
        sentCounter = 0;
        remote_endpoint = new IPEndPoint(IPAddress.Parse(remote_ip_address), port);
        client = new UdpClient();
        ReceiveMessages();
        InvokeRepeating("SendMessages", 0, delayTime);
        //    RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed");
            valve_data[1] = 1;
            
            SendMessages(); //Invoke("SendMessages", delayTime);
            // SendMessages();
            // this.transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
        }

    }
    public void OnMouseUpAsButton()
    {

        //if (serial.IsOpen == false)
        //				serial.Open ();
        if (lightState == false)
        {
            valve_data[0] = 1;
            SendMessages();
            //client.Send(on, 1, remote_endpoint);
            //Debug.Log("Sending: " + on[0]);
            lightState = true;
            if (light != null && light.GetComponent<Light>() != null)
            {
                light.GetComponent<Light>().enabled = lightState;
                light.GetComponent<AudioSource>().PlayOneShot(clip);
            }
        }
        else
        {
            valve_data[0] = 0;
            SendMessages();
            //client.Send(off, 1, remote_endpoint);
            //Debug.Log("Sending: " + off[0]);
            lightState = false;
            if (light != null && light.GetComponent<Light>() != null)
            {
                light.GetComponent<Light>().enabled = lightState;
                light.GetComponent<AudioSource>().PlayOneShot(clip);
            }
        }
    }

    public void ReceiveCallback(IAsyncResult ar)
    {
        UdpClient u = ((UdpState)(ar.AsyncState)).u;
        IPEndPoint e = ((UdpState)(ar.AsyncState)).e;
        recievedCounter++;
        receiveBytes = u.EndReceive(ar, ref e);
        string receiveString = Encoding.ASCII.GetString(receiveBytes);
        Debug.Log("revievedMessage" + recievedCounter + $". Received: {receiveString}");
        //messageReceived = true;
        // needed to handle new messages
        UdpState s = new UdpState();
        s.e = e;
        s.u = u;
        // Debug.Log("listening for messages");
        u.BeginReceive(new AsyncCallback(ReceiveCallback), s);
    }
    public void SendMessages()
    {
        try
        {
              valve_data[4] = sentCounter++;
           // sentCounter++;
            byte[] data = Encoding.UTF8.GetBytes(Covert2CSV(valve_data));
            client.Send(data, data.Length, remote_endpoint);
            Debug.Log("Sending: " + Covert2CSV(valve_data));
        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
    }
    public void ReceiveMessages()
    {
        // Receive a message and write it to the console.
        IPEndPoint e = new IPEndPoint(IPAddress.Parse(local_ip_address), port);
        UdpClient u = new UdpClient(e);
        UdpState s = new UdpState();
        s.e = e;
        s.u = u;
        //Debug.Log("listening for messages");
        u.BeginReceive(new AsyncCallback(ReceiveCallback), s);
    }
    string Covert2CSV(float[] data)
    {
        string tmp = "";
        for (int i = 0; i < data.Length; ++i) tmp += data[i].ToString() + ",";
        return tmp;
    }
}