using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBlock : Block
{
    public GameObject bonus;
    [SerializeField] private GameObject[] bonuses;
    public override void CollisionHit(Collision2D collision)
    {
        base.CollisionHit(collision);
        if (CountToDestroy == 0)
        {

            var obj = Instantiate(bonuses[Random.Range(0, bonuses.Length-1)]);
            var bonusBase = obj.GetComponent<BonusBase>();
            bonusBase.transform.position = collision.transform.position;
        }
    }
}
