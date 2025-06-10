using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Animator animator;
    private EnemyAttack enemyAttack;

    private Vector2 targetPosition;
    private bool isMoving = false;
    private bool isAttacking = false;

    // THÊM: Tham chiếu đến mục tiêu đang bị tấn công
    private GameObject currentAttackTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyAttack = GetComponent<EnemyAttack>();
        targetPosition = rb.position;
    }

    // Nên dùng Update cho các logic không liên quan đến vật lý như kiểm tra input hoặc trạng thái
    private void Update()
    {
        // THÊM: Logic kiểm tra nếu mục tiêu biến mất khi đang tấn công
        if (isAttacking && currentAttackTarget == null)
        {
            // Mục tiêu đã chết hoặc biến mất, hủy hành động tấn công ngay lập tức
            CancelAttack();
        }
    }

    private void FixedUpdate()
    {
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (!isMoving) return;

        Vector2 moveDirection = (targetPosition - rb.position).normalized;
        float distance = Vector2.Distance(targetPosition, rb.position);

        if (distance > 0.1f)
        {
            rb.MovePosition(rb.position + moveDirection * (moveSpeed * Time.fixedDeltaTime));

            if (animator != null)
            {
                animator.SetBool("isWalking", true);
                animator.SetFloat("PosX", moveDirection.x);
                animator.SetFloat("PosY", moveDirection.y);
            }
            if (enemyAttack != null)
            {
                enemyAttack.SetMoveDirection(moveDirection);
            }
        }
        else
        {
            StopMoving();
        }
    }

    public void MoveTo(Vector2 newTargetPosition)
    {
        if (isAttacking) return;
        targetPosition = newTargetPosition;
        isMoving = true;
    }

    private void StopMoving()
    {
        isMoving = false;
        rb.linearVelocity = Vector2.zero;
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }
    }

    public void ForceStop()
    {
        StopMoving();
        targetPosition = rb.position;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    // SỬA ĐỔI: Thêm tham số GameObject để lưu lại mục tiêu
    public void InitiateAttack(Vector2 attackDirection, GameObject target)
    {
        if (isAttacking) return;

        isAttacking = true;
        currentAttackTarget = target; // Lưu lại mục tiêu
        StopMoving();

        if (animator != null)
        {
            animator.SetFloat("PosX", attackDirection.x);
            animator.SetFloat("PosY", attackDirection.y);
            animator.SetTrigger("Attack");
        }
    }

    // Được gọi bởi Animation Event khi animation tấn công kết thúc bình thường
    public void AttackAnimationFinished()
    {
        isAttacking = false;
        currentAttackTarget = null; // Xóa tham chiếu mục tiêu khi xong việc
    }

    // HÀM MỚI: Hủy bỏ hành động tấn công hiện tại
    private void CancelAttack()
    {
        isAttacking = false;
        currentAttackTarget = null;

        if (animator != null)
        {
            // Sử dụng một trigger đặc biệt để buộc animator quay về trạng thái Idle
            // từ bất kỳ trạng thái nào.
            animator.SetTrigger("ForceIdle");
        }

        // Đảm bảo Enemy dừng di chuyển sau khi hủy tấn công
        StopMoving();
    }
}