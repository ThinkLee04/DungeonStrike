using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    [SerializeField] private string normalTag = "EnemyTag"; // default tag
    [SerializeField] private string attackingTag = "AttackedTag";
    private string originTag;

    private Rigidbody2D rb;
    private Animator animator;
    private EnemyAttack enemyAttack;
    private EnemyHealth enemyHealth;

    private Vector2 targetPosition;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool canMove = true;

    // THÊM: Tham chiếu đến mục tiêu đang bị tấn công
    private GameObject playerTarget;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyHealth = GetComponent<EnemyHealth>();
        targetPosition = rb.position;

        if(gameObject.tag != normalTag)
        {
            originTag = gameObject.tag; // Lưu tag hiện tại
        } 
        else
        {
            originTag = normalTag; // Gán tag mặc định nếu chưa có
        }
    }

    // Nên dùng Update cho các logic không liên quan đến vật lý như kiểm tra input hoặc trạng thái
    private void Update()
    {
        // THÊM: Logic kiểm tra nếu mục tiêu biến mất khi đang tấn công
        if (isAttacking && playerTarget == null)
        {
            // Mục tiêu đã chết hoặc biến mất, hủy hành động tấn công ngay lập tức
            CancelAttack();
        }
    }

    private void FixedUpdate()
    {
        if (isAttacking || !canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (!isMoving) return;

        Vector2 moveDirection = (targetPosition - rb.position).normalized;
        float distance = Vector2.Distance(targetPosition, rb.position) ;

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
        playerTarget = target; // Lưu lại mục tiêu
        StopMoving();

        gameObject.tag = attackingTag; // Đổi tag khi đang tấn công

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
        playerTarget = null; // Xóa tham chiếu mục tiêu khi xong việc

        originTag = normalTag; // Đổi lại tag về bình thường
    }
    // HÀM MỚI: Hủy bỏ hành động tấn công hiện tại
    private void CancelAttack()
    {
        gameObject.tag = normalTag; // Đổi lại tag về bình thường

        isAttacking = false;
        playerTarget = null;

        if (animator != null)
        {
            // Sử dụng một trigger đặc biệt để buộc animator quay về trạng thái Idle
            // từ bất kỳ trạng thái nào.
            animator.SetTrigger("ForceIdle");
        }

        // Đảm bảo Enemy dừng di chuyển sau khi hủy tấn công
        StopMoving();
    }
    public void StopAttack()
    {
        // Chỉ hủy tấn công nếu đang thực sự trong trạng thái tấn công.
        if (isAttacking)
        {
            // Chúng ta có thể gọi thẳng hàm CancelAttack đã có sẵn.
            CancelAttack();
        }
    }
    public void OnHurt()
    {
        canMove = false; 
        StopMoving();
    }
    public void AfterHurt()
    {
            canMove = true;
    }
    public void OnDead()
    {
        canMove = false;
        StopMoving();
        this.enabled = false;
        GetComponent<Collider2D>().enabled = false; 
        GetComponent<Rigidbody2D>().simulated = false; 
    }
}