using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header DUNGEON LEVELS
    [Space(10)]
    [Header("DUNGEON LEVELS")]
    #endregion Header DUNGEON LEVELS

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    [SerializeField] private int currentDungeonLevelListIndex = 0;

    private GameState _gameState;
    public GameState gameState
    {
        get { return _gameState; }
        set { _gameState = value; }
    }



    private void Start()
    {
        gameState = GameState.gameStarted;
    }

    private void Update()
    {
        HandleGameState();
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameState = GameState.gameStarted;
        }
    }


    private void HandleGameState()
    {
        switch (gameState)
        {
            case GameState.gameStarted:
                PlayeDungeonLevel(currentDungeonLevelListIndex);
                gameState = GameState.playingLevel;
                break;


        }
    }

    private void PlayeDungeonLevel(int dungeonLevelListIndex)
    {

    }


    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion Validation
}
