using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BallScript : MonoBehaviour
{
    public Vector2 ballInitialForce;
    protected Rigidbody2D rb;
    protected GameObject playerObj;
    protected float deltaX;
    AudioSource audioSrc;
    public AudioClip hitSound;
    public AudioClip loseSound;
    public GameDataScript gameData;
    public int damage = 1;
    
    [SerializeField] private UnityEvent<Block> onCollisionWithBlock;
    [SerializeField] private UnityEvent<Border> onCollisionWithBorder;
    [SerializeField] private UnityEvent<PlayerScript> onCollisionWithPlayer;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerObj = GameObject.FindGameObjectWithTag("Player");
        deltaX = transform.position.x;
        audioSrc = Camera.main.GetComponent<AudioSource>();
    }

    public virtual void AddForceAndCreateBalls()
    {
        if (rb.isKinematic)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                rb.isKinematic = false;
                rb.AddForce(ballInitialForce);
            }
            else
            {
                var pos = transform.position;
                pos.x = playerObj.transform.position.x + deltaX;
                transform.position = pos;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        AddForceAndCreateBalls();
        
        if (!rb.isKinematic && Input.GetKeyDown(KeyCode.J))
        {
            var v = rb.velocity;
            if (Random.Range(0, 2) == 0)
                v.Set(v.x - 0.1f, v.y + 0.1f);
            else
                v.Set(v.x + 0.1f, v.y - 0.1f);
            rb.velocity = v;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //audioSrc.PlayOneShot(hitSound);
        
        OnCollisionBall();
        if (collision.gameObject.TryGetComponent(out Block block))
        {
            onCollisionWithBlock?.Invoke(block);
        }
        else if (collision.gameObject.TryGetComponent(out Border border))
        {
            onCollisionWithBorder?.Invoke(border);
        }
        else if (collision.gameObject.TryGetComponent(out PlayerScript player))
        {
            onCollisionWithPlayer?.Invoke(player);
        }
    }

    public virtual void OnCollisionBall()
    {
        if (gameData.sound)
            audioSrc.PlayOneShot(hitSound, 5);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //audioSrc.PlayOneShot(loseSound);
        if (gameData.sound)
            audioSrc.PlayOneShot(loseSound, 5);
        Destroy(gameObject);
        playerObj.GetComponent<PlayerScript>().BallDestroyed();
    }
}