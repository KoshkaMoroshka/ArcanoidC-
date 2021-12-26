using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlueBlock : Block
{

    private bool point = true;
    [SerializeField] private float speed;
    [SerializeField] private LayerMask wall;
    protected Vector3 point1;
    protected Vector3 point2;
    public GameObject points;
    private GameObject obj1;
    private GameObject obj2;

    public override void Move()
    {
        if (point)
            transform.Translate(Vector3.Normalize(obj1.transform.position - transform.position) * speed * Time.deltaTime);
        else
            transform.Translate(Vector3.Normalize(obj2.transform.position - transform.position) * speed * Time.deltaTime);
        BlockHP.transform.position = Camera.main.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y));
    }

    public override void CollisionHit(Collision2D collision)
    {
        if ((wall.value & (1 << collision.gameObject.layer)) != 0)
        {
            changePoint();
        }
        if ((wall.value & (1 << collision.gameObject.layer)) == 0)
        {
            base.CollisionHit(collision);
        }
    }

    public override void CreateBlock()
    {
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        BlockHP = Instantiate(textPrefab, canvas.transform).GetComponent<Text>();
        BlockHP.text = CountToDestroy.ToString();
        BlockHP.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        var x = (Random.value * 6 - 3);
        var y = (Random.value * 6 - 3);
        point1 = new Vector3(x + transform.position.x, y + transform.position.y, 0);
        point2 = new Vector3(-x + transform.position.x, -y + transform.position.y, 0);
        obj1 = Instantiate(points, point1, Quaternion.identity);
        obj2 = Instantiate(points, point2, Quaternion.identity);
    }

    public override void changePoint()
    {
        point = !point;
    }
}
