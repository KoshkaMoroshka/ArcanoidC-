using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeBlock : Block
{
    [SerializeField] private LayerMask wall;
    private float moveVector = -1;

    // Start is called before the first frame update

    public override void Move()
    {
        if (moveVector == -1) moveVector = Random.Range(0, 100);
        BlockHP.transform.position = Camera.main.WorldToScreenPoint(new Vector2(transform.position.x + moveVector / 100 * Time.deltaTime, transform.position.y));
        transform.position = new Vector3(transform.position.x + moveVector/100 * Time.deltaTime, transform.position.y, 0);
    }

    public override void CollisionHit(Collision2D collision)
    {
        if ((wall.value & (1 << collision.gameObject.layer)) == 0)
        {
            base.CollisionHit(collision);
        } else
            moveVector *= -1;
    }
}
