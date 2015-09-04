using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	// 게임 타입 이름.
	const string GameTypeName = "UNIDRA"; // 원하는 타이틀 이름으로 바꿔 쓰자.
	
	// 로컬 IP 주소와 포트.
	const string LocalServerIP = "127.0.0.1"; // 개발용.
	const int ServerPort = 25000;
	
	string playerName;
	string gameServerName;
	
	void Start()
	{
		playerName = "Player"+Random.Range(0,99999999).ToString();
		gameServerName = "Server"+Random.Range(0,99999999).ToString();
		UpdateHostList();
	}
	
	
	// 상태.
	public enum Status {
		NoError,				// 오류 없음.
		
		LaunchingServer,		// 서버 실행 중.
		ServerLaunched,			// 서버 실행 성공.
		LaunchServerFailed,		// 서버 실행 실패.
		
		ConnectingToServer,		// 서버에 접속 중.
		ConnectedToServer,		// 서버에 접속 성공.
		ConnectToServerFailed,	// 서버에 접속 실패.
		
		DisconnectedFromServer, // 서버에서 연결 끊김.
	};
	
	
	Status _status = Status.NoError;
	public Status status {get{return _status;} private set{_status = value;}}
	
	// 서버를 실행한다.
	public void LaunchServer(string roomName)
	{
		status = Status.LaunchingServer;
		StartCoroutine(LaunchServerCoroutine(gameServerName));
	}
	
	bool useNat = false; // NAT 펀치스루를 사용하는가.
	IEnumerator CheckNat()
	{
		bool doneTesting = false; // 접속 테스트가 끝났는가.
		bool probingPublicIP = false;
		float timer = 0;
		useNat = false;
		
		// 접속 테스트를 하고 NAT 펀치스루가 필요한지 조사한다.
		while (!doneTesting) {
			ConnectionTesterStatus connectionTestResult = Network.TestConnection();
			switch (connectionTestResult) {
			case ConnectionTesterStatus.Error:
				// 문제가 발생했다.
				doneTesting = true;
				break;
				
			case ConnectionTesterStatus.Undetermined: 
				// 조사 중.
				doneTesting = false;
				break;
				
			case ConnectionTesterStatus.PublicIPIsConnectable:
				// 공인 IP 주소를 가지고 있으므로 NAT 펀치스루는 사용하지 않아도 된다.
				useNat = false;
				doneTesting = true;
				break;
				
				
			case ConnectionTesterStatus.PublicIPPortBlocked:
				// 공인 IP 주소인 것 같지만 포트가 막혀 접속할 수 없다.
				useNat = false;
				if (!probingPublicIP) {
					connectionTestResult = Network.TestConnectionNAT();
					probingPublicIP = true;
					timer = Time.time + 10;
				}
				
				else if (Time.time > timer) {
					probingPublicIP = false; 		// reset
					useNat = true;
					doneTesting = true;
				}
				break;
			case ConnectionTesterStatus.PublicIPNoServerStarted:
				// 공인 IP 주소를 가지고 있지만 서버가 실행되지 않았다.
				break;
				
			case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
			case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
				// NAT 펀치스루에 제한이 있다.
				// 서버에 접속할 수 없는 클라이언트가 있을지도 모른다.
				useNat = true;
				doneTesting = true;
				break;
			case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
			case ConnectionTesterStatus.NATpunchthroughFullCone:
				// 서버와 클라이언트는 NAT 펀치스루로 문제 없이 접속할 수 있다.
				useNat = true;
				doneTesting = true;
				break;
				
			default: 
				Debug.Log ( "Error in test routine, got " + connectionTestResult);
				break;
			}
			yield return null;
		}
	}

	
	
	// 서버를 실행하는 코루틴.
	IEnumerator LaunchServerCoroutine(string roomName)
	{
		yield return  StartCoroutine(CheckNat());
		
		// 서버를 실행한다.
		NetworkConnectionError error = Network.InitializeServer(32,ServerPort,useNat);
		if (error !=  NetworkConnectionError.NoError) {
			Debug.Log("Can't Launch Server");
			status = Status.LaunchServerFailed;
		} else {
			// 마스터 서버에 게임 서버를 등록한다.
			MasterServer.RegisterHost(GameTypeName, gameServerName);
		}
		
	}
	
	// 서버에 접속한다.
	public void ConnectToServer(string serverGuid,bool connectLocalServer)
	{
		status = Status.ConnectingToServer;
		if (connectLocalServer)
			Network.Connect(LocalServerIP,ServerPort);
		else 
			Network.Connect(serverGuid);
	}
	
	
	// 서버가 실행되었다.
	void OnServerInitialized()
	{
		status = Status.ServerLaunched;
	}
	
	// 서버에 접속했다.
	void OnConnectedToServer()
	{
		status = Status.ConnectedToServer;
	}
	
	// 서버 접속에 실패했다.
	void OnFailedToConnect(NetworkConnectionError error) {
		Debug.Log("FailedToConnect: " + error.ToString());
		status = Status.ConnectToServerFailed;
	}
	
	// 플레이어가 끊었다.
	// (서버가 동작하는 컴퓨터에서 호출된다).
	void OnPlayerDisconnected(NetworkPlayer player) {
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	
	// 서버에서 끊어졌다.
	void OnDisconnectedFromServer(NetworkDisconnection info) {
		Debug.Log("DisconnectedFromServer: " + info.ToString());
		status = Status.DisconnectedFromServer;
		Application.LoadLevel(0);
	}	
	
	// 스테이터스를 얻는다.
	public Status GetStatus()
	{
		return status;
	}

	// 플레이어 이름을 얻는다.
	public string GetPlayerName()
	{
		return playerName;
	}


	void OnDestroy()
	{
		if (Network.isServer) {
			MasterServer.UnregisterHost ();
			Network.Disconnect ();
		}
	}

	//-------------------- 로비 관련. --------------------
	// 마스터 서버에 등록된 게임 서버 목록을 갱신한다.
	public void UpdateHostList()
	{
		MasterServer.ClearHostList();
		MasterServer.RequestHostList(GameTypeName);
	}
	
	// 마스터 서버에 등록된 게임 서버 목록을 가져온다.
	public HostData[] GetHostList()
	{
		return MasterServer.PollHostList();
	}
	
	// 마스터 서버와 NAT 퍼실리테이터의 IP 주소를 설정한다.
	void SetMasterServerAndNatFacilitatorIP(string masterServerAddress,string facilitatorAddress)
	{
		MasterServer.ipAddress = masterServerAddress;
		Network.natFacilitatorIP = facilitatorAddress;
	}
	
	// 마스터 서버에 등록한 서버를 삭제한다.
	public void UnregisterHost()
	{
		MasterServer.UnregisterHost();
	}
	
	//-------------------- 설정 GUI.　-------------------
	void OnGUI()
	{
		if ((Network.isServer || Network.isClient))
			return;
		
		// 높이 480의 중심(0,0)을 기준으로 한다.
		float scale = Screen.height / 480.0f;
		GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width * 0.5f,Screen.height * 0.5f,0),Quaternion.identity,new Vector3(scale,scale,1.0f));
		
		GUI.Window(0, new Rect(-200,-200,400,400), NetworkSettingWindow, "Network Setting");
	}
	
	Vector2 scrollPosition;
	
	void NetworkSettingWindow(int windowID) {
		// 플레이어 이름을 설정한다.
		GUILayout.BeginHorizontal();
		GUILayout.Label("Player Name: ");
		playerName = GUILayout.TextField(playerName,32);
		GUILayout.EndHorizontal();
		
		// 게임 서버 이름을 설정한다.
		GUILayout.BeginHorizontal();
		GUILayout.Label("Game Server Name: ");
		gameServerName = GUILayout.TextField(gameServerName,32);
		GUILayout.EndHorizontal();
		
		// 게임 서버를 실행한다.
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button ("Launch"))
			LaunchServer(gameServerName);
		GUILayout.EndHorizontal();
		GUILayout.Space(20);
		
		// 게임 서버 목록.
		GUILayout.BeginHorizontal();
		GUILayout.Label("Game Server List (Click To Connect):");
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Refresh"))
			UpdateHostList();
		GUILayout.EndHorizontal();
		
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(380), GUILayout.Height(200));
		// 서버 목록 가져오기.
		HostData[] hosts = GetHostList();  
		if (hosts.Length > 0) {
			foreach (HostData host in hosts)
				if (GUILayout.Button(host.gameName,GUI.skin.box,GUILayout.Width(360)))
					ConnectToServer(host.guid,false);
		} else
			GUILayout.Label("No Server");
		GUILayout.EndScrollView();
		
		// 로컬 서버에 접속한다.
		if (GUILayout.Button("Connect Local Server")) {
			ConnectToServer("",true);
		}
		
		// 스테이터스를 표시한다.
		GUILayout.Label("Status: "+status.ToString());
		
	}
}
