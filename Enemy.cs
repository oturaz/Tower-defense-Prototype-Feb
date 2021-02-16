using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject pathGO;
    private Transform targetPathNode;
    private int pathNodeIndex = 0;

    public float speed = 1f;
    public float turningSpeed = 5f;
    public float distError = 0.1f;
    
    public float health = 1;
    protected int goldWorth = 1;

    private int pathChildCount;

    public void Awake()
    {
        health = 30;
    }

    void Start()
    {
        //pathGO = GameObject.Find("Path");
        pathChildCount = pathGO.transform.childCount;
    }

    
    void Update()
    {
        if(targetPathNode == null)
        {
            GetNextPathNode();
            if(targetPathNode == null)
            {
                //reached the end of path
                ReachedGoal();
            }
        }

        if(targetPathNode != null)
        {
            Vector3 dir = targetPathNode.position - this.transform.localPosition;

            float distThisFrame = speed * Time.deltaTime;

            if (Vector3.Distance(this.transform.position, targetPathNode.position) < distError)
            {
                //reached current node
                targetPathNode = null;
            }
            else
            {
                //move towards node
                transform.Translate(transform.forward * distThisFrame, Space.World);
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, turningSpeed * Time.deltaTime);
            }
        }
    }

    public void GetNextPathNode()
    {
        if(pathChildCount > pathNodeIndex)
        {
            targetPathNode = pathGO.transform.GetChild(pathNodeIndex);
        }
        pathNodeIndex++;
    }

    public void ReachedGoal()
    {
        GameObject.FindObjectOfType<GameManager>().LoseHp();
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        health -=damage;
        if(health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GameObject.FindObjectOfType<GameManager>().GainGold(goldWorth);
        Destroy(gameObject);
    }
}
