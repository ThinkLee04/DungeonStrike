using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State { Roaming, ChasingPlayer }
    [Header("State Management")]
    private State state;
    private EnemyController enemyController;

    [Header("Roaming Settings")]
    [SerializeField] private BoxCollider2D roamArea;
    [SerializeField] private float minRoamWaitTime = 2f;
    [SerializeField] private float maxRoamWaitTime = 4f;
    private Vector2 lastRoamPosition;

    [Header("Stuck Prevention")]
    [Tooltip("Thời gian tối đa để Enemy đi đến một điểm trước khi coi là bị kẹt.")]
    [SerializeField] private float pathfindingTimeout = 5f;
    private float timeSinceStartedMoving;

    [Header("Chasing Settings")]
    private Transform playerTransform;
    [SerializeField] private float chaseRadius = 7f;
    [SerializeField] private float loseTargetRadius = 10f;
    private bool playerExists = false;

    // --- BIẾN MỚI QUAN TRỌNG ---
    // Biến để lưu trữ tham chiếu đến Coroutine đang chạy
    private Coroutine aiBehaviorCoroutine;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        state = State.Roaming;
    }

    private void Start()
    {
        if (roamArea == null) { Debug.LogError("Roam Area chưa được gán..."); }
        FindPlayer();
        lastRoamPosition = transform.position;
        // --- THAY ĐỔI: Lưu lại Coroutine ---
        aiBehaviorCoroutine = StartCoroutine(AIBehaviorRoutine());
    }

    private void Update()
    {
        // Kiểm tra xem người chơi có còn tồn tại không
        if (playerTransform == null)
        {
            if (playerExists) // Nếu trước đó người chơi tồn tại
            {
                playerExists = false;
                state = State.Roaming;
                RestartAIBehavior(); // Khởi động lại AI để nó đi lang thang
            }
            return; // Dừng xử lý ở đây nếu không có người chơi
        }

        // Đảm bảo playerExists là true nếu playerTransform hợp lệ
        if (!playerExists)
        {
            playerExists = true;
        }

        // Logic chính
        HandleStateTransitions();

        if (state == State.ChasingPlayer)
        {
            timeSinceStartedMoving = 0f;
            enemyController.MoveTo(playerTransform.position);
        }

        // Kiểm tra bị kẹt
        if (enemyController.IsMoving())
        {
            timeSinceStartedMoving += Time.deltaTime;
            if (timeSinceStartedMoving > pathfindingTimeout)
            {
                Debug.LogWarning(gameObject.name + " is stuck. Forcing a new path.");
                enemyController.ForceStop();
                timeSinceStartedMoving = 0f;
            }
        }
    }

    // --- HÀM MỚI: RestartAIBehavior ---
    // Hàm này có thể được gọi từ các script khác (như EnemyHealth)
    public void RestartAIBehavior()
    {
        // Dừng coroutine cũ nếu nó đang chạy
        if (aiBehaviorCoroutine != null)
        {
            StopCoroutine(aiBehaviorCoroutine);
        }
        // Buộc enemy dừng di chuyển từ đường đi cũ
        if (enemyController != null)
        {
            enemyController.ForceStop();
        }
        // Bắt đầu một coroutine mới, làm mới hoàn toàn hành vi của AI
        aiBehaviorCoroutine = StartCoroutine(AIBehaviorRoutine());
    }
    // --- KẾT THÚC HÀM MỚI ---

    private IEnumerator AIBehaviorRoutine()
    {
        while (true)
        {
            if (state == State.Roaming)
            {
                // Chỉ tìm đường đi mới khi không di chuyển
                if (!enemyController.IsMoving())
                {
                    Vector2 roamPosition = GetNewRoamingPosition();
                    enemyController.MoveTo(roamPosition);
                    timeSinceStartedMoving = 0f;

                    // Chờ một khoảng ngẫu nhiên sau khi đã có mục tiêu mới
                    yield return new WaitForSeconds(Random.Range(minRoamWaitTime, maxRoamWaitTime));
                }
            }
            // Luôn chờ một chút trước khi lặp lại vòng lặp chính để tránh quá tải CPU
            yield return new WaitForSeconds(0.2f);
        }
    }

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
            playerTransform = null;
            playerExists = false;
        }
    }

    private void HandleStateTransitions()
    {
        // Đã kiểm tra playerTransform != null ở Update() nên ở đây an toàn
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        switch (state)
        {
            case State.Roaming:
                if (distanceToPlayer <= chaseRadius)
                {
                    state = State.ChasingPlayer;
                    // Khi chuyển sang truy đuổi, nên dừng coroutine lang thang
                    if (aiBehaviorCoroutine != null)
                    {
                        StopCoroutine(aiBehaviorCoroutine);
                    }
                }
                break;

            case State.ChasingPlayer:
                if (distanceToPlayer > loseTargetRadius)
                {
                    state = State.Roaming;
                    // Khi mất dấu, khởi động lại AI để nó đi lang thang ngay lập tức
                    RestartAIBehavior();
                }
                break;
        }
    }

    #region Unchanged Methods
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
            float randomX = Random.Range(roamBounds.min.x, roamBounds.max.y); // Lỗi nhỏ ở đây, phải là max.x
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