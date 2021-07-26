using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Singletons/MasterManger")]
public class MasterManager : SingletonScriptableObject<MasterManager>
{
    // Start is called before the first frame update
    [SerializeField] private GameSettings _gameSettings;

    public static GameSettings GameSettings { get { return Instance._gameSettings; } }




}
