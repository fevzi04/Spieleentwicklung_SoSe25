using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public int health = 3;
    [SerializeField] public int score = 0;
    [SerializeField] private float immunitySeconds = 1f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float fireRate = 2f;
    private float nextShotTime = 0f;

    [Header("Items")]
    public int ricochetBounces = 0;
    public int regeneration = 0;
    public bool homing = false;
    public int pierce = 0;

    [SerializeField] private Bullet bulletPrefab;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private int shotCount;
    private bool isImmune = false;
    private GameController gameController;

    private int previousScore = 0;

    void Start()
    {
        gameController = GameObject.FindAnyObjectByType<GameController>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(gameController.isPaused) return;
        ScreenWrap();

        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            Shoot();
        }

        if(health <= 0) gameController.PlayerDeath();

        Regeneration();
    }

    void FixedUpdate()
    {
        HandleRotation();
        HandleMovement();
    }

    void HandleRotation()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        rb.angularVelocity = -horizontalInput * rotationSpeed;
    }

    void HandleMovement()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetMouseButton(1))
        {
            rb.AddForce(transform.up * moveSpeed, ForceMode2D.Force);
        }
    }

    void Shoot()
    {
        if (Time.time < nextShotTime) return;

        nextShotTime = Time.time + (1f / fireRate);

        anim.SetTrigger("Shoot");

        shotCount++;
        string bulletText = (shotCount % 2 == 0) ? "try{}" : "catch{}";
        Bullet bullet = Instantiate(bulletPrefab, transform.position, transform.rotation * Quaternion.Euler(0, 0, 90));
        bullet.bounces = ricochetBounces;
        bullet.homing = homing;
        bullet.pierce = pierce;
        bullet.Project(transform.up, bulletText);
    }

    void Regeneration()
    {
        if (score / 500 > previousScore / 500)
        {
            health += regeneration;
            previousScore = score;
        }
    }

    void ScreenWrap()
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);

        if (viewportPosition.x > 1)
            viewportPosition.x = 0;
        else if (viewportPosition.x < 0)
            viewportPosition.x = 1;
        if (viewportPosition.y > 1)
            viewportPosition.y = 0;
        else if (viewportPosition.y < 0)
            viewportPosition.y = 1;

        transform.position = Camera.main.ViewportToWorldPoint(viewportPosition);
    }

    public void AddScore(int _score){
        score += _score;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bug") && !isImmune){
            isImmune = true;
            health--;
            StartCoroutine("FlashRed");
            StartCoroutine("Immunity");
        } 
    }

    IEnumerator Immunity(){
        yield return new WaitForSeconds(immunitySeconds);
        isImmune = false;
    }

    private IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }
}
