using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using Newtonsoft.Json.Linq;
using Nagih;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1;
using System.IO;

public class WebsocketManager : MonoBehaviour
{
    string currentIP;
    int currentPort;

    TcpListener server;
    static X509Certificate2 serverCertificate = null;

    List<TcpClient> clients = new List<TcpClient>();
    Dictionary<TcpClient, NetworkStream> networkStreams = new Dictionary<TcpClient, NetworkStream>();
    Dictionary<TcpClient, SslStream> sslStreams = new Dictionary<TcpClient, SslStream>();

    [SerializeField] private Text connectionText;
    [SerializeField] private Text messageText;

    private void Start()
    {
        //serverCertificate = GenerateCertificate($"{GetLocalIPv4()}");
        //serverCertificate = new X509Certificate2("mycomputer.crt", "password");
        
        /*RSA rsa = serverCertificate.GetRSAPrivateKey();

        Debug.Log($"RSA: {rsa != null}");*/

        /*string path = "mycomputer.key";
        StreamReader reader = new StreamReader(path);
        string key = reader.ReadToEnd().Replace("-----BEGIN PRIVATE KEY-----", "");
        key = key.Replace("-----END PRIVATE KEY-----", "");
        key = key.Replace("\n", "");
        reader.Close();
        Debug.Log(key);

        int i;
        RSA rsa = RSA.Create();*/

        /*Debug.Log($"Certificate: {serverCertificate.HasPrivateKey}" +
            $"\nOid: {serverCertificate.PublicKey.Oid.Value}" +
            $"\nSubject: {serverCertificate.Subject}" +
            $"\nEffective: {serverCertificate.GetEffectiveDateString()}" +
            $"\nExpired: {serverCertificate.GetExpirationDateString()}" +
            $"\nFormat: {serverCertificate.GetFormat()}" +
            $"\nThumbprint: {serverCertificate.Thumbprint}");*/

        currentIP = GetLocalIPv4();
        //currentIP = "127.0.0.1";
        currentPort = 3000;
        server = new TcpListener(IPAddress.Parse(currentIP), currentPort);
        server.Start();

        connectionText.text = $"Connection: {currentIP}:{currentPort}";
        Debug.Log($"Server is listening on {currentIP}:{currentPort}...");

        CheckClient();
    }

    /*
    public X509Certificate2 GenerateSelfSignedCertificate()
    {
        string secp256r1Oid = "1.2.840.10045.3.1.7";  //oid for prime256v1(7)  other identifier: secp256r1

        string subjectName = "localhost";

        ECDsa ecdsa = ECDsa.Create(ECCurve.CreateFromValue(secp256r1Oid));

        //var certRequest = new CertificateRequest($"CN={subjectName}", ecdsa, HashAlgorithmName.SHA256);

        //add extensions to the request (just as an example)
        //add keyUsage
        //ertRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));

        //X509Certificate2 generatedCert = certRequest.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(10)); // generate the cert and sign!

        //X509Certificate2 pfxGeneratedCert = new X509Certificate2(generatedCert.Export(X509ContentType.Pfx)); //has to be turned into pfx or Windows at least throws a security credentials not found during sslStream.connectAsClient or HttpClient request...

        //return pfxGeneratedCert;
        return null;
    }
    */

    //TlsException: Handshake failed - error code: UNITYTLS_INTERNAL_ERROR, verify result: 4294936704
    static X509Certificate2 GenerateCertificate(string certName)
    {
        RsaKeyPairGenerator keypairgen = new RsaKeyPairGenerator();
        keypairgen.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), 1024));

        var keypair = keypairgen.GenerateKeyPair();

        var gen = new X509V3CertificateGenerator();

        var CN = new X509Name("CN=" + certName);
        var SN = BigInteger.ProbablePrime(120, new System.Random());

        gen.SetSerialNumber(SN);
        gen.SetSubjectDN(CN);
        gen.SetIssuerDN(CN);
        gen.SetNotAfter(DateTime.MaxValue);
        gen.SetNotBefore(DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)));
        //gen.SetSignatureAlgorithm("MD5WithRSA");
        gen.SetPublicKey(keypair.Public);

        gen.AddExtension(X509Extensions.BasicConstraints.Id, true, new BasicConstraints(true));

        var newCert = gen.Generate(new Asn1SignatureFactory(PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(), keypair.Private));

        System.Security.Cryptography.X509Certificates.X509Certificate x509Certificate = DotNetUtilities.ToX509Certificate(newCert);
        X509Certificate2 certificate = new X509Certificate2(x509Certificate);

        //--> Find/Generate RSA in keypair
        var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keypair.Private);
        var asn1Seq = (Asn1Sequence)Asn1Object.FromByteArray(privateKeyInfo.ParsePrivateKey().GetDerEncoded());
        var rsaPrivateKeyStruct = RsaPrivateKeyStructure.GetInstance(asn1Seq);
        var rsa = DotNetUtilities.ToRSA(rsaPrivateKeyStruct);

        return certificate.CopyWithPrivateKey(rsa);
    }


    private void OnApplicationQuit()
    {
        if (server != null)
        {
            foreach (TcpClient client in clients)
            {
                client.Close();
            }
            server.Stop();
        }
    }

    private void FixedUpdate()
    {
        foreach (TcpClient client in networkStreams.Keys)
        {
            if (networkStreams[client] != null && networkStreams[client].DataAvailable && client.Connected)
            {
                GetMessage(client);
            }
        }
    }

    private async void CheckClient()
    {
        TcpClient client = await server.AcceptTcpClientAsync();

        if (client != null)
        {
            Debug.Log("Client accepted");
            if (client.Connected) Debug.Log($"Client {client.Client.RemoteEndPoint} connected");

            if (!clients.Contains(client))
            {
                clients.Add(client);
                networkStreams.Add(client, client.GetStream());

                //try
                //{
                    /*SslStream sslStream = new SslStream(client.GetStream(), true, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);

                    await sslStream.AuthenticateAsServerAsync(serverCertificate, false, SslProtocols.Tls12, true);

                    sslStream.ReadTimeout = 5000;
                    sslStream.WriteTimeout = 5000;

                    sslStreams.Add(client, sslStream);*/
                /*}
                catch (Exception e)
                {
                    Debug.Log("Exception: " + e.Message);
                }*/
            }
        }
    }

    private bool ValidateServerCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        Debug.Log("Inside callback");
        if (sslPolicyErrors == SslPolicyErrors.None)
            return true;

        Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

        // Do not allow this client to communicate with unauthenticated servers.
        return false;
    }

    private async void GetMessage(TcpClient client)
    {
        //Debug.Log($"Client Available: " + client.Available);
        byte[] bytes = new byte[client.Available];
        await GetStream(client).ReadAsync(bytes, 0, bytes.Length);

        string data = Encoding.UTF8.GetString(bytes);
        Debug.Log($"Get a Message : {data}\nBytes Length: {bytes.Length}");

        if (new Regex("^GET").IsMatch(data))
        {
            Debug.Log($"Get a Connect Handshake Request message : {data}");
            const string eol = "\r\n"; // HTTP/1.1 defines the sequence CR LF as the end-of-line marker

            byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + eol
                + "Connection: Upgrade" + eol
                + "Upgrade: websocket" + eol
                + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                    System.Security.Cryptography.SHA1.Create().ComputeHash(
                        Encoding.UTF8.GetBytes(
                            new Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                        )
                    )
                ) + eol
                + eol);

            GetStream(client).Write(response, 0, response.Length);
            Debug.Log("Send a message: " + Encoding.UTF8.GetString(response));
        }
        else
        {
            bool fin = (bytes[0] & 0b10000000) != 0,
                mask = (bytes[1] & 0b10000000) != 0; // must be true, "All messages from the client to the server have this bit set"
            int opcode = bytes[0] & 0b00001111, // expecting 1 - text message
                offset = 2;
            int msglen = bytes[1] & 127;

            if (opcode == 8) //Close handshake by Client
            {
                CloseClient(client);
                return;
            }

            if (msglen == 126)
            {
                msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                offset = 4;
            }
            else if (msglen == 127)
            {
                msglen = BitConverter.ToInt32(new byte[] { bytes[9], bytes[8], bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2] }, 0);
                offset = 10;
            }

            if (msglen == 0)
            {
                Debug.Log("msglen == 0");
            }
            else if (mask)
            {
                if (bytes[1] == 130) bytes[1] = 200;
                byte[] decoded = new byte[msglen];
                byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                offset += 4;

                for (int i = 0; i < msglen; ++i)
                {
                    decoded[i] = (byte)(bytes[offset + i] ^ masks[i % 4]);
                }

                string text = Encoding.UTF8.GetString(decoded);
                messageText.text += $"\n{client.Client.RemoteEndPoint}: {text}";
                Debug.Log($"Get a message: {text}\nBytes Length: {bytes.Length}");
                HandleMessage(client, text);
            }
            else
            {
                Debug.Log("mask bit not set");
            }
        }
    }
    public string GetLocalIPv4()
    {
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress address in hostEntry.AddressList)
        {
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                return address.ToString();
            }
        }
        return "127.0.0.1";
    }

    public void HandleMessage(TcpClient client, string message)
    {
        try
        {
            var json = JObject.Parse(message);
            var type = json["type"].ToObject<string>();
            switch (type)
            {
                case Const.TYPE_FROM_CONTROLLER:
                    HandleJsonMessage(client, json["data"].ToObject<ControllerInputMessage>());
                    break;
                default:
                    break;
            }
        }
        catch
        {
            HandleIntMessage(client, message);
        }
    }

    private void HandleJsonMessage(TcpClient client, ControllerInputMessage message)
    {
        if (string.Compare(message.input, "ping") == 0)
        {
            SendWebsocketMessage(client, "{\"type\":\"4001\",\"data\":{\"input\":\"joystick\",\"condition\":\"down\",\"content\":{\"x\":1,\"y\":-0.1}}}");
        }
    }

    private void HandleIntMessage(TcpClient client, string message)
    {
        if (message == "31010102009")
        {
            SendWebsocketMessage(client, "41010102009");
        }
    }

    private void SendWebsocketMessage(TcpClient client, string message)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        byte[] sendMessage = new byte[128];
        int indexStartRawData;

        sendMessage[0] = 129;

        if (bytes.Length <= 125)
        {
            sendMessage[1] = (byte)bytes.Length;
            indexStartRawData = 2;
        }
        else if (bytes.Length >= 126 && bytes.Length <= 65535)
        {
            sendMessage[1] = 126;
            sendMessage[2] = (byte)((byte)(bytes.Length >> 8) & (255));
            sendMessage[3] = (byte)((byte)(bytes.Length) & (255));
            indexStartRawData = 4;
        }
        else
        {
            sendMessage[1] = 127;
            sendMessage[2] = (byte)((byte)(bytes.Length >> 56) & (255));
            sendMessage[3] = (byte)((byte)(bytes.Length >> 48) & (255));
            sendMessage[4] = (byte)((byte)(bytes.Length >> 40) & (255));
            sendMessage[5] = (byte)((byte)(bytes.Length >> 32) & (255));
            sendMessage[6] = (byte)((byte)(bytes.Length >> 24) & (255));
            sendMessage[7] = (byte)((byte)(bytes.Length >> 16) & (255));
            sendMessage[8] = (byte)((byte)(bytes.Length >> 8) & (255));
            sendMessage[9] = (byte)((byte)(bytes.Length) & (255));
            indexStartRawData = 10;
        }

        //Copy to-be-sended message (bytes) to sendMessage array
        Buffer.BlockCopy(bytes, 0, sendMessage, indexStartRawData, bytes.Length);

        GetStream(client).Write(sendMessage, 0, indexStartRawData + bytes.Length);

        Debug.Log("Send a message: " + Encoding.UTF8.GetString(bytes));
    }

    private void CloseClient(TcpClient client)
    {
        if (client.Connected)
        {
            Debug.Log($"Close Client {client.Client.RemoteEndPoint}");
            if (sslStreams.ContainsKey(client))
            {
                sslStreams[client].Close();
            }
            networkStreams[client].Close();
            client.Close();

            if (sslStreams.ContainsKey(client))
            {
                sslStreams.Remove(client);
            }
            networkStreams.Remove(client);
            clients.Remove(client);
        }
    }

    private Stream GetStream(TcpClient client)
    {
        if (sslStreams.ContainsKey(client))
        {
            return sslStreams[client];
        }
        else
        {
            return networkStreams[client];
        }
    }
}