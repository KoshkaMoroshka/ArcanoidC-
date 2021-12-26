using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected GameObject textPrefab;

    protected Text BlockHP;
    [SerializeField] protected int CountToDestroy;
    [SerializeField] protected int Points;
    protected PlayerScript playerScript;
    
    // Start is called before the first frame update
    void Start()
    {
        CreateBlock();
    }


    public virtual void CreateBlock()
    {
        if (canvas == null) canvas = FindObjectOfType<Canvas>().GetComponentInChildren<Canvas>();
        BlockHP = Instantiate(textPrefab, canvas.transform).GetComponent<Text>();
        BlockHP.text = CountToDestroy.ToString();
        BlockHP.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        CollisionHit(collision);
    }
    public virtual void CollisionHit(Collision2D collision)
    {
        var ballBool = collision.gameObject.TryGetComponent<BallScript>(out var ball);
        int damage = ballBool ? ball.damage : 0;
        if (gameObject.layer != collision.gameObject.layer)
        {
            CountToDestroy -= damage;
            BlockHP.text = CountToDestroy.ToString();
            if (CountToDestroy <= 0)
            {
                Destroy(gameObject);
                Destroy(BlockHP.gameObject);
                playerScript.BlockDestroyed(Points);
            }
        }
    }

    public virtual void Move()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        changePoint();
    }

    public virtual void changePoint()
    {
        
    }
}
