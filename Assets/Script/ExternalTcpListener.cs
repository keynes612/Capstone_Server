using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ExternalTcpListener : MonoBehaviour
{
    [Header("외부 프로그램 전용 포트 (Netcode와 겹치지 않게 설정)")]
    public int externalPort = 8888;

    private TcpListener tcpListener;
    private Thread listenerThread;
    private bool isRunning = false;

    void Start()
    {
        StartExternalServer();
    }

    public void StartExternalServer()
    {
        if (isRunning) return;

        isRunning = true;
        listenerThread = new Thread(ListenForExternalClients);
        listenerThread.IsBackground = true;
        listenerThread.Start();

        Debug.Log($"[External Server] 외부 프로그램 대기 서버 시작 (TCP 포트: {externalPort})");
    }

    private void ListenForExternalClients()
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Loopback, externalPort);
            tcpListener.Start();

            while (isRunning)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                string clientInfo = client.Client.RemoteEndPoint.ToString();

                // 유니티 콘솔에 연결 확인 로그 출력
                Debug.Log($"<color=cyan>[External Server] 외부 실행 프로그램 연결 성공! ({clientInfo})</color>");

                // 데이터 수신 처리
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.Log($"[External Server] 외부에서 보낸 메시지: {msg}");
                }
            }
        }
        catch (SocketException)
        {
            // 서버 종료 시 발생하는 예외 무시
        }
    }

    void OnDisable()
    {
        StopExternalServer();
    }

    public void StopExternalServer()
    {
        isRunning = false;
        tcpListener?.Stop();
        if (listenerThread != null && listenerThread.IsAlive)
            listenerThread.Abort();
    }
}