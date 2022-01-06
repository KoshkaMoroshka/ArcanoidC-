using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBallScript : BallScript
{
    public override void OnCollisionBall()
    {
        Destroy(this.gameObject);
    }
}

