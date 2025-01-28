using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header DUNGEON LEVELS
    [Space(10)]
    [Header("DUNGEON LEVELS")]
    #endregion Header DUNGEON LEVELS

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    [SerializeField] private int currentDungeonLevelListIndex = 0;
    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;

    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI messageTextTMP;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("GameScore")]
    private long gameScore;
    private int multiplierCount;

    private InstantiatedRoom bossRoom;


    protected override void Awake()
    {
        base.Awake();

        playerDetails = GameResources.Instance.CurrentPlayerSO.playerDetails;

        InitiatePlayer();
    }



    private void InitiatePlayer()
    {
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);
        player = playerGameObject.GetComponent<Player>();

       

        player.Initialise(playerDetails);
    }


    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;
        StaticEventHandler.OnMultiplier += StaticEventHandler_OnMultiplier;
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
        player.destroyEvent.OnDestroyed += DestroyEvent_OnDestroyed;
    }



    private void Start()
    {
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;
        gameScore = 0;
        multiplierCount = 1;

        StartCoroutine(Fade(0f, 1f, 0f, Color.black));
    }



    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;
        StaticEventHandler.OnMultiplier -= StaticEventHandler_OnMultiplier;
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
        player.destroyEvent.OnDestroyed -= DestroyEvent_OnDestroyed;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }



    private void Update()
    {
        HandleGameState();
        if (Input.GetKeyDown(KeyCode.P))
        {
            gameState = GameState.gameStarted;
        }
    }


    private void HandleGameState()
    {
        switch (gameState)
        {
            case GameState.gameStarted:
                PlayDungeonLevel(currentDungeonLevelListIndex); 
                gameState = GameState.playingLevel;
                RoomEnemiesDefeated();
                break;
            case GameState.levelCompleted:
                StartCoroutine(LevelCompleted());
                break;
            case GameState.gameWon:
                if (previousGameState != GameState.gameWon)
                    StartCoroutine(GameWon());
                break;
            case GameState.gameLost:
                if (previousGameState != GameState.gameLost)
                {
                    StopAllCoroutines();
                    StartCoroutine(GameLost());
                }
                break;
            case GameState.restartGame:
                RestartGame();
                break;

        }
    }


    private IEnumerator LevelCompleted()
    {
        gameState = GameState.playingLevel;
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0.4f)));

        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE !\n" +
            "YOU'VE SURVIVED THIS DUNGEON LEVEL!PRESS RETURN TO CONTINUE!",Color.white,5f));

        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0.4f)));
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;

        }
        yield return null;
        currentDungeonLevelListIndex++;
        PlayDungeonLevel(currentDungeonLevelListIndex);
    }
    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;
        GetPlayer().playerControl.DisablePlayer();
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        Enemy[] enemyArray = GameObject.FindObjectsOfType<Enemy>();

        foreach(Enemy enemy in enemyArray)
        {
            enemy.gameObject.SetActive(false);
        }

        yield return StartCoroutine(DisplayMessageRoutine("YOU ARE DEAD !\n" +
            "YOU SCORED "+gameScore.ToString("###,###0"), Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO RESTART", Color.white, 4f));
        gameState = GameState.restartGame;
    }

    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;

        GetPlayer().playerControl.DisablePlayer();
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE !\n" +
             "YOU SCORED " + gameScore.ToString("###,###0"), Color.white, 4f));
        gameState = GameState.restartGame;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSuccessfully)
        {
            Debug.LogError("Can't Build the dungeon");
        }

        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        player.gameObject.transform.position = HelperUtilities.GetSwapnPositionNearestToPlayer(
            new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f));

        StartCoroutine(DisplayDungeonLevelText());
    }


    public Room GetCurrentRoom()
    {
        return currentRoom;
    }
    public void SetCurrentRoom(Room room)
    {
        currentRoom= room;
    }


    public Player GetPlayer()
    {
        return player;
    }

    public Sprite GetPlayerMiniMapIcon()
    {
        return playerDetails.playerMiniMapIcon;
    }
    public DungeonLevelSO GetCurrentDungeonLevel()
    {
        return dungeonLevelList[currentDungeonLevelListIndex];
    }

    private void StaticEventHandler_OnMultiplier(MultiplierArgs multiplierArgs)
    {
        if (multiplierArgs.multiplier)
        {
            multiplierCount++;
        }
        else
        {
            multiplierCount--;
        }
        multiplierCount = Mathf.Clamp(multiplierCount, 1, 30);
        StaticEventHandler.CallScoreChangedEvent(gameScore, multiplierCount);
    }




    private void StaticEventHandler_OnPointsScored(PointScoredArgs pointScoredArgs)
    {
        gameScore += pointScoredArgs.point * multiplierCount;
        StaticEventHandler.CallScoreChangedEvent(gameScore, multiplierCount);
    }


    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        RoomEnemiesDefeated();
    }

    private void RoomEnemiesDefeated()
    {
        bool isDungeonCloearOfRegularEnemies = true;
        bossRoom = null;
        foreach(var keyValue in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            if (keyValue.Value.roomNodeType.isBossRoom)
            {
                bossRoom = keyValue.Value.instantiatedRoom;
                continue;
            }
            if (!keyValue.Value.isClearedOfEnemies)
            {
                isDungeonCloearOfRegularEnemies = false;
                break;
            }

        }

        if ((isDungeonCloearOfRegularEnemies&&bossRoom==null)||(isDungeonCloearOfRegularEnemies&&bossRoom.room.isClearedOfEnemies))
        {
            if (currentDungeonLevelListIndex < dungeonLevelList.Count - 1)
            {
                gameState = GameState.levelCompleted;
            }
            else
            {
                gameState = GameState.gameWon;
            }
        }
        else if (isDungeonCloearOfRegularEnemies)
        {

            gameState = GameState.bossStage;
            StartCoroutine(BossStage());

        }

    }

    private IEnumerator BossStage()
    {
        bossRoom.gameObject.SetActive(true);
        bossRoom.UnlockDoors(0);
        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0.4f)));

        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE " + GameResources.Instance.CurrentPlayerSO.playerName +
            "! THE WAY TO BOSS HAS BEEN OPENED\n" +
            "\nNOW FIND AND DEAFAT THE BOSS ...... GOOD LUCK!", Color.white, 5f));
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0.4f)));
    }

    private void DestroyEvent_OnDestroyed(DestroyEvent destroyEvent, DestroyEventArgs destroyEventArgs)
    {
        previousGameState = gameState;
        gameState = GameState.gameLost;
    }

    private IEnumerator Fade(float startFadeAlpha ,float targetFadeAlpha, float fadeSeconds, Color backgroundColor)
    {
        Image image = canvasGroup.GetComponent<Image>();
        image.color = backgroundColor;
        float time = 0f;
        while (time <= fadeSeconds)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }
        canvasGroup.alpha = targetFadeAlpha;

    }


    private IEnumerator DisplayDungeonLevelText()
    {
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));
        GetPlayer().playerControl.DisablePlayer();
        string messageText = "LEVEL " + (currentDungeonLevelListIndex+1).ToString() + "\n\n"
            + dungeonLevelList[currentDungeonLevelListIndex].levelName.ToUpper();
        yield return StartCoroutine(DisplayMessageRoutine(messageText,Color.white,2f));
        GetPlayer().playerControl.EnablePlayer();
        yield return StartCoroutine(Fade(1f, 0f, 2f, Color.black));
    }

    private IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds)
    {
        messageTextTMP.SetText(text);
        messageTextTMP.color = textColor;

        if (displaySeconds > 0f)
        {
            float timer = displaySeconds;
            while (timer > 0f && !Input.GetKeyDown(KeyCode.Return))
            {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            while ( !Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }
        yield return null;

        messageTextTMP.SetText("");
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
