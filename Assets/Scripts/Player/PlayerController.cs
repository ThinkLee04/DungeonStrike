using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 vector2;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
    }

    void FixedUpdate()
    {
        PlayerFacingDirector();
        Move();
    }

    private void PlayerInput()
    {
        vector2.x = Input.GetAxis("Horizontal");
        vector2.y = Input.GetAxis("Vertical");

        animator.SetFloat("moveX", vector2.x);
        animator.SetFloat("moveY", vector2.y);
    }

    private void Move()
    {
        rb.MovePosition(rb.position + vector2 * (speed * Time.fixedDeltaTime));
    }

    private void PlayerFacingDirector()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        print(mousePos);

        if (mousePos.x < rb.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("coin"))
    //    {
    //        Destroy(collision.gameObject);
    //    }
    //}
}
