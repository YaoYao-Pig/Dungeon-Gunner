using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{
    [SerializeField] private MovementDetailsSO movementDetailsSO;

    private Enemy enemy;
    private Stack<Vector3> movePathSteps = new Stack<Vector3>();
    private Vector3 playerReferencePosition;
    private Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate waitForFixedUpdate;


    [HideInInspector] public float moveSpeed;

    private bool chasePlayer = false;
    [HideInInspector] public int updateFrameNumber = 1;
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        moveSpeed = movementDetailsSO.GetMoveSpeed();

    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
    }

    private void Update()
    {
        MoveEnemy();

    }

    private void MoveEnemy()
    {
        currentEnemyPathRebuildCooldown -= Time.deltaTime;

        if (!chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) < enemy.enemyDetails.chaseDistance)
        {
            chasePlayer = true;
        }
        if (!chasePlayer)
        {
            return;
        }
        if (Time.frameCount % Settings.targetFrameRateToSpreadPathFindingOver != updateFrameNumber) return;

        if(currentEnemyPathRebuildCooldown<=0f||(Vector3.Distance(playerReferencePosition,GameManager.Instance.GetPlayer().GetPlayerPosition())>
            Settings.playerMoveDistanceToRebuildPath))
        {
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;
            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

            CreatePath();

            if (movePathSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);

                }
                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movePathSteps));
            }
        }
    }

    public void SetUpdateFrameNumber(int updateFrameNumber)
    {
        this.updateFrameNumber = updateFrameNumber;
    }

    private void CreatePath()
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();
        Grid grid = currentRoom.instantiatedRoom.grid;
        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);
        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);
        movePathSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        if (movePathSteps != null)
        {
            movePathSteps.Pop();
        }
        else
        {
            enemy.idleEvent.CallIdleEvent();
        }
    }

    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
        Vector3Int playerCellPosition = currentRoom.instantiatedRoom.grid.WorldToCell(playerPosition);

        Vector2Int adjustedPlayerCellPosition = new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds.x,
            playerCellPosition.y - currentRoom.templateLowerBounds.y);
        int obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y];
        if (obstacle != 0)
        {
            return playerCellPosition;
        }
        
        for(int i = -1; i <= 1; ++i)
        {
            for(int j = -1; j <= 1; ++j)
            {
                if (i == 0 && j == 0) continue;
                try
                {
                    obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x + i, adjustedPlayerCellPosition.y + j];
                    if (obstacle != 0) return new Vector3Int(playerCellPosition.x + i, playerCellPosition.y + j, 0);
                }
                catch
                {
                    continue;
                }
            }
        }
        return playerCellPosition;
    }

    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movePathSteps)
    {
        while (movePathSteps.Count>0)
        {
            Vector3 nextPosition = movePathSteps.Pop();
            while (Vector3.Distance(nextPosition, transform.position) > 0.2f)
            {
                enemy.movementToPositionEvent.CallMovementToPositionEvent(nextPosition, 
                    transform.position, 
                    moveSpeed, 
                    (nextPosition - transform.position).normalized);
                yield return waitForFixedUpdate;
            }
            yield return waitForFixedUpdate;
        }
        enemy.idleEvent.CallIdleEvent();
    }
}
