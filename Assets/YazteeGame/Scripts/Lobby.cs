using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace edu.jhu.co
{

    public class Lobby : MonoBehaviour
    {
        /// <summary>
        /// Player name input field. Let the user input his name, will appear above the player in the game.
        /// </summary>
        [RequireComponent(typeof(InputField))]
        public class YazteePlayer : MonoBehaviour
        {
            #region Private Constants


            #endregion


            #region MonoBehaviour CallBacks


            /// <summary>
            /// MonoBehaviour method called on GameObject by Unity during initialization phase.
            /// </summary>
            void Start()
            {


                string defaultName = string.Empty;
                InputField _inputField = this.GetComponent<InputField>();
                if (_inputField != null)
                {
                    //list the rooms
                }
                /* if (PlayerPrefs.HasKey(playerNamePrefKey))
                 {
                     defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                     _inputField.text = defaultName;
                 }*/

                


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

                //create new room
            }


            #endregion
        }
    }
}
