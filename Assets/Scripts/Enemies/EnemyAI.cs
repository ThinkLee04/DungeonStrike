using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // ... (các biến enum, Header, ... giữ nguyên như cũ) ...
    private enum State { Roaming, ChasingPlayer }
    [Header("State Management")]
    private State state;
    private EnemyController enemyController;

    [Header("Roaming Settings")]
    [SerializeField] private BoxCollider2D roamArea;
    [SerializeField] private float minRoamWaitTime = 2f;
    [SerializeField] private float maxRoamWaitTime = 4f;
    private Vector2 lastRoamPosition;

    // --- BIẾN MỚI ---
    [Header("Stuck Prevention")]
    [Tooltip("Thời gian tối đa để Enemy đi đến một điểm trước khi coi là bị kẹt.")]
    [SerializeField] private float pathfindingTimeout = 5f;
    private float timeSinceStartedMoving;
    // --- HẾT BIẾN MỚI ---

    [Header("Chasing Settings")]
    private Transform playerTransform;
    [SerializeField] private float chaseRadius = 7f;
    [SerializeField] private float loseTargetRadius = 10f;
    private bool playerExists = false;


    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        state = State.Roaming;
    }

    private void Start()
    {
        // ... (hàm Start giữ nguyên) ...
        if (roamArea == null) { Debug.LogError("Roam Area chưa được gán..."); }
        FindPlayer();
        lastRoamPosition = transform.position;
        StartCoroutine(AIBehaviorRoutine()); // Đổi tên Coroutine cho rõ nghĩa hơn
    }

    private void Update()
    {
        // ... (hàm Update logic truy đuổi giữ nguyên) ...
        if (!playerExists)
        {
            if (state != State.Roaming) state = State.Roaming;
            return;
        }

        HandleStateTransitions();

        if (state == State.ChasingPlayer)
        {
            // Khi truy đuổi, ta cũng cần reset bộ đếm thời gian liên tục
            // vì mục tiêu (player) luôn thay đổi
            timeSinceStartedMoving = 0f;
            enemyController.MoveTo(playerTransform.position);
        }

        // --- LOGIC MỚI: KIỂM TRA BỊ KẸT ---
        if (enemyController.IsMoving())
        {
            timeSinceStartedMoving += Time.deltaTime;
            if (timeSinceStartedMoving > pathfindingTimeout)
            {
                Debug.LogWarning(gameObject.name + " is stuck. Forcing a new path.");
                enemyController.ForceStop();
                timeSinceStartedMoving = 0f; // Reset bộ đếm
            }
        }
    }

    // Đổi tên RoamingRoutine thành AIBehaviorRoutine để bao quát hơn
    private IEnumerator AIBehaviorRoutine()
    {
        while (true)
        {
            if (state == State.Roaming)
            {
                if (!enemyController.IsMoving())
                {
                    // Lấy vị trí mới và di chuyển
                    Vector2 roamPosition = GetNewRoamingPosition();
                    enemyController.MoveTo(roamPosition);

                    // Reset bộ đếm thời gian mỗi khi bắt đầu một lộ trình mới
                    timeSinceStartedMoving = 0f;

                    // Chờ một khoảng thời gian ngẫu nhiên
                    yield return new WaitForSeconds(Random.Range(minRoamWaitTime, maxRoamWaitTime));
                }
            }
            // Chờ một chút trước khi lặp lại vòng lặp chính
            yield return new WaitForSeconds(0.2f);
        }
    }

    // ... (Các hàm còn lại: FindPlayer, HandleStateTransitions, GetNewRoamingPosition, GetQuadrant, OnDrawGizmosSelected giữ nguyên)
    #region Unchanged Methods
    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            playerExists = true;
        }
        else
        {
            playerExists = false;
        }
    }

    private void HandleStateTransitions()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        switch (state)
        {
            case State.Roaming:
                if (distanceToPlayer <= chaseRadius)
                {
                    state = State.ChasingPlayer;
                }
                break;

            case State.ChasingPlayer:
                if (distanceToPlayer > loseTargetRadius)
                {
                    state = State.Roaming;
                }
                break;
        }
    }

    private Vector2 GetNewRoamingPosition()
    {
        if (roamArea == null) return transform.position;

        Bounds roamBounds = roamArea.bounds;
        Vector2 center = roamBounds.center;
        int lastQuadrant = GetQuadrant(lastRoamPosition, center);
        Vector2 newPosition;
        int newQuadrant;
        int safetyBreak = 0;

        do
        {
            float randomX = Random.Range(roamBounds.min.x, roamBounds.max.x);
            float randomY = Random.Range(roamBounds.min.y, roamBounds.max.y);
            newPosition = new Vector2(randomX, randomY);
            newQuadrant = GetQuadrant(newPosition, center);
            safetyBreak++;
        }
        while (newQuadrant == lastQuadrant && safetyBreak < 20);

        lastRoamPosition = newPosition;
        return newPosition;
    }

    private int GetQuadrant(Vector2 point, Vector2 center)
    {
        bool isRight = point.x >= center.x;
        bool isTop = point.y >= center.y;

        if (isTop && isRight) return 1;
        if (isTop && !isRight) return 2;
        if (!isTop && !isRight) return 3;
        if (!isTop && isRight) return 4;

        return 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseTargetRadius);
    }
    #endregion
}