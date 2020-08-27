using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class PlayerNetwork : MonoBehaviour
{
    public static PlayerNetwork instance;
    public string playerName { get; private set; }
    public int CharacterID;
    private PhotonView view;
    private int PlayersInGame = 0;
    Vector3 redSpawn = new Vector3(6.5f, 1, 9.5f);
    Vector3 blueSpawn = new Vector3(6.5f, 1, -9.5f);
    string compressed = "";
    public int numB = -1;
    public int numR = -1;
    bool spawned;
    bool spo;
    int amountFin;
    public string playersSerialized = "";
    GameCanvas scoreboard;
    GameObject player;
    int prevRed = -1;
    int prevBlue = -1;
    public Text swapButton;
    bool inStages;
    public int stageID;
    public GameObject css;
    public GameObject stageSelect;
    public GameObject startbutton;
    public GameObject setopenbutton;
    public GameObject stagebutton;
    public List<PhotonPlayer> redPlayers = new List<PhotonPlayer>();
    public List<PhotonPlayer> bluePlayers = new List<PhotonPlayer>();
    public List<Stage> stages = new List<Stage>();
    public Image stageImage;
    public Text stageNameText;
    [System.Serializable]
    public class Stage
    {
        public int stageID;
        public string stageName;
        public Vector3 redSpawn = new Vector3(6.5f, 1, 9.5f);
        public Vector3 blueSpawn = new Vector3(6.5f, 1, -9.5f);
        public Sprite image;
    }
    private void Awake()
    {
        instance = this;
        playerName = "Player#" + Random.Range(1000, 9999);
        view = GetComponent<PhotonView>();
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }
    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Dodgeball")
        {
            if (PhotonNetwork.isMasterClient)
            {
                MasterLoadedGame();
            } else
            {
                NonMasterLoadedGame();
            }
        }
    }
    void OnJoinedRoom()
    {
        if (PhotonNetwork.isMasterClient)
        {

        }else
        {
            setopenbutton.SetActive(false);
            startbutton.SetActive(false);
            stagebutton.SetActive(false);

        }
    }
    public void OnClick_SwapStage()
    {
        if (!inStages)
        {
            inStages = true;
            swapButton.text = "Back to Characters";
            stageSelect.SetActive(true);
            css.SetActive(false);
        }
        else
        {
            inStages = false;
            swapButton.text = "Choose Map";
            stageSelect.SetActive(false);
            css.SetActive(true);
        }
    }
    [PunRPC]
    private void RPC_SetPlayers(int b, int r)
    {
        numB += b;
        numR += r;
    }
    private void MasterLoadedGame()
    {
        view.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
        view.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
    }
    private void NonMasterLoadedGame()
    {
        view.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
    }
    public void leaveGame()
    {
        PhotonNetwork.LoadLevel(0);
        PhotonNetwork.LeaveLobby();
        Application.Quit();
    }
    [PunRPC]
    private void RPC_LoadGameOthers()
    {
        PhotonNetwork.LoadLevel(1);
    }
    [PunRPC]
    void RPC_SetFinished()
    {
        amountFin++;
    }
    IEnumerator finishStartGame()
    {
        while (amountFin < PhotonNetwork.playerList.Length)
        {
            yield return null;
        }
        view.RPC("RPC_CreatePlayer", PhotonTargets.All);
    }
    [PunRPC]
    public void RPC_SetUpJSON(string addition)
    {
        playersSerialized = addition + playersSerialized;
        Debug.Log(playersSerialized);
    }

    [PunRPC]
    public void RPC_SetMap(int stageID)
    {
        stageImage.sprite = stages[stageID].image;
        stageNameText.text = stages[stageID].stageName;
    }
    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        PlayersInGame++;
        if (PlayersInGame == PhotonNetwork.playerList.Length && PhotonNetwork.isMasterClient)
        {
            if (PunTeams.PlayersPerTeam.TryGetValue(PunTeams.Team.blue, out bluePlayers))
            {
                for (int i = 0; i < bluePlayers.Count; i++)
                {
                    string addition = bluePlayers[i].ID.ToString() + "B";
                    compressed = addition + compressed;
                }
            }
            if (PunTeams.PlayersPerTeam.TryGetValue(PunTeams.Team.red, out redPlayers))
            {
                for (int i = 0; i < redPlayers.Count; i++)
                {
                    string addition = redPlayers[i].ID.ToString() + "R";
                    compressed = addition + compressed;
                }
            }
            amountFin++;
            view.RPC("RPC_SyncLists", PhotonTargets.All, compressed);
            StartCoroutine(finishStartGame());
            StartCoroutine(delay());
        }
    }
    [PunRPC]
    void RPC_SyncLists(string c)
    {
        bluePlayers.Clear();
        redPlayers.Clear();
        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            char play = c[i * 2];
            int id = play - '0';
            string team = c[(i * 2) + 1].ToString();
            PhotonPlayer p = PhotonNetwork.player.Get(id);
            if (team == "B")
            {
                bluePlayers.Add(p);
            } else
            {
                redPlayers.Add(p);
            }
        }
        view.RPC("RPC_SetFinished", PhotonTargets.MasterClient);
    }
    public void OnClick_SetID(int ID)
    {
        CharacterID = ID;
    }
    [PunRPC]
    public void RPC_ChangeList(PhotonPlayer p)
    {
        //if (p.GetTeam() == PunTeams.Team.red)
        //{
        //    bluePlayers.Remove(p);
        //    redPlayers.Add(p);
        //}
        //else
        //{
        //    bluePlayers.Add(p);
        //    redPlayers.Remove(p);
        //}
    }
    [PunRPC]
    public void RPC_SetPlayerTeam(bool onBlue, PhotonPlayer player)
    {
        if (onBlue && !bluePlayers.Contains(player))
        {
            bluePlayers.Add(player);
        }
        else if (!redPlayers.Contains(player))
        {
            redPlayers.Add(player);
        }
    }
    [PunRPC]
    private void RPC_CreatePlayer()
    {
        if (PhotonNetwork.player.GetTeam() == PunTeams.Team.blue)
        {
            int index = bluePlayers.FindIndex(x => x == PhotonNetwork.player);
            Vector3 pos = new Vector3(index * -2.5f, 0, 0);
            player = PhotonNetwork.Instantiate("Character" + CharacterID, stages[stageID].blueSpawn + pos, Quaternion.identity, 0);
            player.GetComponent<PhotonView>().RPC("RPC_SetTeam", PhotonTargets.All, PhotonNetwork.player.GetTeam(), index, stages[stageID].blueSpawn + pos);
            view.RPC("RPC_SetUpJSON", PhotonTargets.MasterClient, CharacterID.ToString() + "B");
        }
        else
        {
            int index = redPlayers.FindIndex(x => x == PhotonNetwork.player);
            Vector3 pos = new Vector3(index * -2.5f, 0, 0);
            player = PhotonNetwork.Instantiate("Character" + CharacterID, stages[stageID].redSpawn + pos, Quaternion.Euler(0, 180, 0), 0);
            player.GetComponent<PhotonView>().RPC("RPC_SetTeam", PhotonTargets.All, PhotonNetwork.player.GetTeam(), index, stages[stageID].redSpawn + pos);
            view.RPC("RPC_SetUpJSON", PhotonTargets.MasterClient, CharacterID.ToString() + "R");
        }
    }
    [PunRPC]
    private void RPC_Reset()
    {
        foreach (PlayerMovement p in GameObject.FindObjectsOfType<PlayerMovement>())
        {
            p.GetComponent<PhotonView>().RPC("RPC_StartRound", PhotonTargets.All);
        }
        if (PhotonNetwork.isMasterClient)
        {
            foreach (Dodgeball d in FindObjectsOfType<Dodgeball>())
            {
                d.GetComponent<PhotonView>().RPC("RPC_SetVelocity", PhotonTargets.All, Vector3.zero);
                d.GetComponent<PhotonView>().RPC("RPC_Kill", PhotonTargets.All);
            }
            GameCanvas can = GameObject.FindGameObjectWithTag("MenuCanvas").GetComponent<GameCanvas>();
            PrefabPool pool = GameObject.FindGameObjectWithTag("PrefabPool").GetComponent<PrefabPool>();
            foreach (Transform t in can.dodgeballSpawns)
            {
                pool.spawnObject(t.position, Quaternion.identity, 2);
            }
        }
    }

    IEnumerator delay()
    {
        while (playersSerialized.Length != PhotonNetwork.playerList.Length * 2)
        {
            yield return new WaitForEndOfFrame();
        }
        GameCanvas can = GameObject.FindGameObjectWithTag("MenuCanvas").GetComponent<GameCanvas>();
        PrefabPool pool = GameObject.FindGameObjectWithTag("PrefabPool").GetComponent<PrefabPool>();
        can.GetComponent<PhotonView>().RPC("RPC_SetEverything", PhotonTargets.All, playersSerialized);
        can.GetComponent<PhotonView>().RPC("RPC_StartGame", PhotonTargets.All);
        can.GetComponent<PhotonView>().RPC("RPC_SetStage", PhotonTargets.All, stageID);
    }
}
