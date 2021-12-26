using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusSteel : BonusBase
{
    public override void CreateBonus()
    {
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        BlockHP = Instantiate(textPrefab, canvas.transform).GetComponent<Text>();
        ColorText = Color.black;
        BlockHP.color = ColorText;
        BlockHP.text = textBonus;
        BlockHP.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        gameObject.GetComponent<SpriteRenderer>().color = ColorBonus;
    }

    public override void Bonus()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Ball"))
        {
            obj.GetComponent<SpriteRenderer>().color = ColorBonus;
            obj.GetComponent<BallScript>().damage = 40;
        }
    }
}
