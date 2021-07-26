#include <WiFi101.h>
#include <WiFiUdp.h>

// Set WiFi credentials
#define WIFI_SSID "david"            //wifi name "TP-LINK_65C1BC"
#define WIFI_PASS "88888888"         //wifi password "21971013"
#define RECIVER_IP "192.168.43.101" //sender IP address "192.168.150.101"
#define UDP_PORT 6666
WiFiUDP Udp;
char packetBuffer[255];              //buffer to hold incoming packet
char command[5];                     //[lightControl,none,none,none,sentNumber]
char ReplyBuffer[] = "acknowledged"; // a string to send back
void setup()
{
  // Setup serial port
  Serial.begin(9600);
  pinMode(LED_BUILTIN, OUTPUT);

  // Begin connectiong WiFi
  WiFi.begin(WIFI_SSID, WIFI_PASS);

  // Connecting to WiFi...
  Serial.print("Connecting to ");
  Serial.print(WIFI_SSID);
  // Loop continuously while WiFi is not connected
  while (WiFi.status() != WL_CONNECTED)
  {
    delay(100);
    Serial.print(".");
  }
  // Connected to WiFi
  Serial.println();
  IPaddress(1);
  Udp.begin(UDP_PORT);
}
int input;
void loop()
{
  if (Serial.available())
  {
    input = Serial.read();
    if (input == 'A')
    {
      IPaddress(1);
    }
  }
  // If packet received...
  int packetSize = Udp.parsePacket();
  if (packetSize)
  {
  //  IPaddress(2);

    // read the packet into packetBufffer
    int len = Udp.read(packetBuffer, 255);
    if (len > 0)
      packetBuffer[len] = 0;
    int k = 0;
    for (int i = 0; i < len; i++)
    {
      if (packetBuffer[i] == ',')
      {
      }
      else
      {
        command[k] = packetBuffer[i];
        //Serial.println(packetBuffer[i]);
        //Serial.println(command[k]);
        k++;
      }
    }

    /*Below will be the control command*/
    // send a reply, to the IP address and port that sent us the packet we received
    Udp.beginPacket(RECIVER_IP, UDP_PORT);
    int lightControl = command[0];
    if (lightControl == '1')
    {
      digitalWrite(LED_BUILTIN, HIGH);
      Udp.write("LED ON"); // Reply message to Unity Game
   //   Serial.println("LED ON");
    }
    if (lightControl == '0')
    {
      digitalWrite(LED_BUILTIN, LOW);
      Udp.write("LED  OFF");
    }
    print(command);
    Udp.endPacket();
  }
}
void print(char command[])
{
  // Serial.print("sentNumber: ");
  // int sentNumber=packetBuffer[4];
  // Serial.println(sentNumber);
  Serial.println("The input command are: ");
  for (int i = 0; i <= sizeof(command); i++)
  {
    Serial.print(command[i]);
    Serial.print(",");
  }
   Serial.println("================================");
}

void IPaddress(int type) //"1" for localIP. "2" for remoteIP
{
  // Serial.println(type);
  if (type == 1)
  {
    IPAddress ip = WiFi.localIP();
    Serial.print("Connected! IP address: ");
    Serial.println(ip);
  }
  else if (type == 2)
  { //Serial.print("Received packet of size ");
    //Serial.println( Udp.parsePacket());
    Serial.print("From ");
    IPAddress remoteIp = Udp.remoteIP();
    Serial.print(remoteIp);
    Serial.print(", port ");
    Serial.println(Udp.remotePort());
  }
}
