using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    [SerializeField]
    private Text roomNameText;
    private Text RoomNameText { get { return roomNameText; } }
    public string RoomName { get; private set; }
    public bool Updated { get; set; }
    private void Start()
    {
        GameObject lobbyCanvasObj = MainCanvasManager.Instance.lobbyCanvas.gameObject;
        if (lobbyCanvasObj == null)
        {
            return;
        }
        LobbyCanvas lobbyCanvas = lobbyCanvasObj.GetComponent<LobbyCanvas>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => lobbyCanvas.OnClick_JoinRoom(roomNameText.text));
    }

    private void OnDestroy()
    {
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
    }

    public void SetRoomNameText(string text)
    {
        RoomName = text;
        roomNameText.text = RoomName;
    }
}
