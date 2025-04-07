using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    
    [SerializeField] Bullet bulletPrefab;
    public float speed = 500f;
    private float lifeTime = 10f;
    public int bounces = 0;
    public float homingRadius = 3;
    public bool homing;
    public int pierce = 0;
    bool targetFound = false;
    GameObject homingTarget = null;


    private Rigidbody2D rb;
    [SerializeField] TextMeshPro text;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Project(Vector2 direction, string bulletText)
{
    rb.velocity = direction.normalized * (speed * Time.fixedDeltaTime);
    Destroy(gameObject, lifeTime);
    text.text = bulletText;
}


    private void Update() {
        if(homing && text.text == "catch{}"){
            Homing();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bug"))
        {
            other.gameObject.GetComponent<Bug>().Death();

            if (bounces > 0 && text.text == "try{}")
            {
                GameObject nextBug = FindNearestBug(other.transform.position);
                if (nextBug != null)
                {
                    Ricochet(nextBug.transform.position,bounces-1);
                    Destroy(gameObject);
                }
            }
            if(pierce == 0){
                Destroy(this.gameObject);
            }else{
                pierce--;
                targetFound = false;
            }
        }
    }

    void Homing()
    {
        float rotationSpeed = 180f;

        if (!targetFound)
        {
            homingTarget = FindNearestBugInRadius(homingRadius);
            if (homingTarget != null) targetFound = true;
            else return;
        }

        if (homingTarget == null) return;

        Vector2 direction = (homingTarget.transform.position - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.MoveRotation(Mathf.LerpAngle(rb.rotation, angle, rotationSpeed * Time.deltaTime));

        rb.velocity = direction * (speed * Time.fixedDeltaTime);
    }


        
    void Ricochet(Vector2 newTarget, int _bounces)
    {
        Bullet newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Vector2 direction = (newTarget - (Vector2)newBullet.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        newBullet.transform.rotation = Quaternion.Euler(0, 0, angle);
        newBullet.Project(direction, "try{}");
        newBullet.bounces = _bounces;
    }


    GameObject FindNearestBugInRadius(float radius)
    {
        GameObject bug = FindNearestBug(transform.position);
        if (bug == null) return null;

        float distance = Vector3.Distance(transform.position, bug.transform.position);
        if (distance < radius)
        {
            return bug;
        }

        return null;
    }


    GameObject FindNearestBug(Vector2 currentBugPosition)
    {
        GameObject[] bugs = GameObject.FindGameObjectsWithTag("Bug");
        if (bugs == null || bugs.Length == 0) return null;

        return bugs
            .Where(b => (Vector2)b.transform.position != currentBugPosition)
            .OrderBy(b => Vector2.Distance(transform.position, b.transform.position))
            .FirstOrDefault();
    }  

     void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, homingRadius);
    }


}