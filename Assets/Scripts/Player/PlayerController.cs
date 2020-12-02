using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float fireForce;
    [SerializeField] private float stickRadius;
    [SerializeField] private LayerMask stickMask;
    [SerializeField] private float stickCooldown;

    private bool isSticking;
    private float stickTimer;
    private Vector2 aimDir;

    private Rigidbody2D rb;
    private LineRenderer lr;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
	{
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = Vector2.zero;

        lr = GetComponent<LineRenderer>();
        lr.enabled = false;

        isSticking = false;
        aimDir = Vector2.zero;
    }

    private void Update()
    {
        if (!LevelManager.Instance.IsPlaying) {
            return;
        }

        StickToWall();

        if (isSticking) {
            if (!LevelManager.Instance.MoveManager.HasMovesLeft) {
                // the game is only over when the player has stopped moving
                LevelManager.Instance.GameOver();
            } else {
                GetPlayerAim();
            }
        }

        UpdateTimers();
    }

    private void StickToWall()
    {
        if (isSticking || stickTimer > 0f) {
            return;
        }

        if (Physics2D.OverlapCircle(transform.position, stickRadius, stickMask)) {
            isSticking = true;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    private void GetPlayerAim()
    {
        if (Input.GetButton("Fire1")) {
            Aim();
        } else if (Input.GetButtonUp("Fire1")) {
            Fire();
            LevelManager.Instance.MoveManager.DecreaseCounter();
        }
    }

    private void Aim()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimDir = (Vector2)transform.position - mousePos;

        lr.enabled = true;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + (Vector3)aimDir);
    }

    private void Fire()
    {
        isSticking = false;
        stickTimer = stickCooldown;
        rb.bodyType = RigidbodyType2D.Dynamic;
        lr.enabled = false;

        rb.velocity = fireForce * aimDir;
    }

    private void UpdateTimers()
    {
        if (stickTimer > 0f) {
            stickTimer -= Time.deltaTime;
        }
    }

    public void AddToStickTimer(float val)
	{
        stickTimer += val;
	}

	private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!LevelManager.Instance.IsPlaying) {
            return;
        }

        if (collision.CompareTag("Obstacle")) {
            LevelManager.Instance.GameOver();
		} else if (collision.CompareTag("LevelEnd")) {
            LevelManager.Instance.LevelComplete();
		}
	}
}
