using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


namespace edu.jhu.co
{

    public class Lobby : MonoBehaviourPunCallbacks
    {

        [Tooltip("The Ui Text to inform the user PlayerList")]
        [SerializeField]
        private Text RoomList;

        public Sprite tileSprite;
        public float[,] Grid;
        int Vertical, Horizontal, Columns, Rows;
        float tileBoundsX = 0;
        float tileBoundsY = 0;

        #region MonoBehaviour CallBacks
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (RoomInfo roomInfo in roomList)
            {
                if (roomInfo.RemovedFromList)
                {
                    //find in the list and update 
                    /*  int index = _listings.FindIndex(x >= x.RoomInfo.Name == roomInfo.Name);
                    if (indexer != -1) { Destroy(_listing[index].gameObject); } 
                    _listings.RemoveAt(index);*/
                }
                else
                {
                    RoomList.text += roomInfo.Name + System.Environment.NewLine;
                }
            }
        }

        void Start()
        {

        }


        private void InstantiateGrid()
        {

            

            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    Grid[i, j] = Random.Range(0.0f, 1.0f);
                    SpawnTile(i, j, Grid[i, j]);
                }
            }
        }

        private void SpawnTile(int x, int y, float value)
        {
            GameObject g = new GameObject(x + ":" + y);
            g.transform.position = new Vector3(x - (Horizontal - tileBoundsX), y - (Vertical - tileBoundsY));
            Debug.Log("TILE:" + g.transform.position);
            var tile = g.AddComponent<SpriteRenderer>();
            tile.sprite = tileSprite;
            tile.color = new Color(value, value, value);
        }

        private void Update()
            {
                if (Input.GetKey("escape"))
                {
                    Application.Quit();
                }
            }


            #endregion


            #region Public Methods
            /// <summary>
            /// Creates a new game room
            /// </summary>
            /// <param name="value">The name of the Player</param>
            public void AddNewRoomName(string value)
            {
                // #Important
                if (string.IsNullOrEmpty(value))
                {
                    Debug.LogError("Room Name is null or empty");
                    return;
                }

                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = Login.MaxPlayersPerRoom;
                roomOptions.PlayerTtl = 20000; //time in the game
                PhotonNetwork.CreateRoom(value, roomOptions, null);


            //create new room
        }


        /// <summary>
        /// Go back to Login screen
        /// </summary>
        public void LeaveRoom()
            {
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene("Login");
            }

            /// <summary>
            /// 
            /// </summary>
            public void ExitApplication()
            {
                Application.Quit();
            }
            #endregion
        }
    }
