using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BonusBomb : BonusBase
{
    public override void CreateBonus()
    {
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        BlockHP = Instantiate(textPrefab, canvas.transform).GetComponent<Text>();
        ColorText = Color.white;
        BlockHP.color = ColorText;
        BlockHP.text = textBonus;
        BlockHP.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        gameObject.GetComponent<SpriteRenderer>().color = ColorBonus;
    }

    public override void Bonus()
    {
        SceneManager.LoadScene("SampleScene");
    }
}