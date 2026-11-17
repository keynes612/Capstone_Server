using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SimpleTcpServer : MonoBehaviour
{
    public int port = 8888; // Netcode(7777)와 겹치지 않는 포트

    private TcpListener tcpListener;
    private Thread listenerThread;
    private bool isRunning = false;
    private int connectionCount = 0; // 연결 횟수 카운트

    void Start()
    {
        StartServer();
    }

    public void StartServer()
    {
        if (isRunning) return;

        isRunning = true;
        listenerThread = new Thread(ListenForClients);
        listenerThread.IsBackground = true;
        listenerThread.Start();

        Debug.Log($"[Unity Server] 서버 시작 (포트: {port}) - 다중 연결 대기 중...");
    }

    private void ListenForClients()
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Loopback, port);
            tcpListener.Start();

            while (isRunning)
            {
                // 새로운 클라이언트 접속 수락
                TcpClient client = tcpListener.AcceptTcpClient();

                // 스레드 안전한 카운트 증가
                int currentNum = Interlocked.Increment(ref connectionCount);
                string clientEndPoint = client.Client.RemoteEndPoint.ToString();

                Debug.Log($"<color=green>[Unity Server] #{currentNum}번째 외부 프로그램 연결 성공! ({clientEndPoint})</color>");

                // 각 클라이언트를 별도 스레드에서 처리 (다음 연결을 즉시 받기 위함)
                ThreadPool.QueueUserWorkItem(HandleClient, new ClientInfo(client, currentNum));
            }
        }
        catch (SocketException e)
        {
            if (isRunning)
                Debug.LogError($"[Unity Server] 소켓 오류: {e.Message}");
        }
    }

    private void HandleClient(object state)
    {
        ClientInfo info = (ClientInfo)state;
        TcpClient client = info.Client;
        int id = info.Id;

        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log($"[Unity Server] #{id}번 클라이언트 메시지: {receivedMessage}");
            }
        }
        catch (Exception)
        {
            // 연결 종료 시 예외 무시
        }
        finally
        {
            Debug.Log($"[Unity Server] #{id}번 클라이언트 연결 종료");
            client.Close();
        }
    }

    void OnDisable() => StopServer();
    void OnApplicationQuit() => StopServer();

    public void StopServer()
    {
        isRunning = false;
        tcpListener?.Stop();
        if (listenerThread != null && listenerThread.IsAlive)
            listenerThread.Abort();
    }

    private class ClientInfo
    {
        public TcpClient Client;
        public int Id;
        public ClientInfo(TcpClient client, int id) { Client = client; Id = id; }
    }
}