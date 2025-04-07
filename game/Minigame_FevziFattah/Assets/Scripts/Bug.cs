using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bug : MonoBehaviour
{
    [SerializeField] Bug standardBug;
    private Rigidbody2D rb;
    Vector2 direction;
    float speed;
    PlayerController playerController;
    SpriteRenderer sr;

    [Header("Bug Stats")]
    [SerializeField] public string bugName;
    [SerializeField] public string description;
    [SerializeField] private float minSize = 0.5f;
    [SerializeField] private float maxSize = 1.5f;
    [SerializeField] float maxSpeed = 1;
    [SerializeField] float minSpeed = 2;
    [SerializeField] private int score;    

    [Header("Bug Type")]
    [SerializeField] public bool mitosis;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController= GameObject.FindAnyObjectByType<PlayerController>();
        sr = GetComponent<SpriteRenderer>();
        transform.localScale *= Random.Range(minSize,maxSize);
    }


    public void Project(Vector2 _direction)
    {
        direction = _direction.normalized;
        speed = Random.Range(minSpeed,maxSpeed);
        rb.velocity = direction * speed;
    }

    void Update()
    {
        ScreenWrap();
    }

    public void Death()
    {
        if (mitosis)
        {
            Mitosis();
        }
        playerController.AddScore(score);
        Destroy(gameObject);
    }

    private void Mitosis(){
        for (int i = 0; i < 2; i++)
        {
            float randomAngle = Random.Range(0f, 360f);
            Vector2 randomDirection = RotateVector(Vector2.up, randomAngle);
            Bug newBug = Instantiate(standardBug, transform.position, Quaternion.Euler(0, 0, randomAngle));
            newBug.Project(randomDirection);
            newBug.transform.localScale *= 0.7f;
        }
    }

    private Vector2 RotateVector(Vector2 v, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    void ScreenWrap()
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        float buffer = 0.1f; 

        if (viewportPosition.x > 1 + buffer)
            viewportPosition.x = 0 - buffer;
        else if (viewportPosition.x < 0 - buffer)
            viewportPosition.x = 1 + buffer;

        if (viewportPosition.y > 1 + buffer)
            viewportPosition.y = 0 - buffer;
        else if (viewportPosition.y < 0 - buffer)
            viewportPosition.y = 1 + buffer;

        transform.position = Camera.main.ViewportToWorldPoint(viewportPosition);
    }

}
