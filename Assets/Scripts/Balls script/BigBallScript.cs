using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBallScript : BallScript
{
    
    public override void OnCollisionBall()
    {
        damage = 4;
        Destroy(gameObject);
    }
}
