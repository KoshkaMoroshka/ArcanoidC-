using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private GameObject player;

    [SerializeField] private GameObject miniBall;

    private Rigidbody2D rb;
    public Vector2 ballInitialForce;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        var vector3 = player.transform.position;
        vector3.y += 1.0f;
        transform.position = vector3;
        if (Input.GetButtonDown("Fire1"))
        {
            vector3.y += 1.0f;
            var obj = Instantiate(miniBall, vector3, Quaternion.identity);
            rb = obj.GetComponent<Rigidbody2D>();
            rb.isKinematic = false;
            rb.AddForce(ballInitialForce);
        }
    }
}
