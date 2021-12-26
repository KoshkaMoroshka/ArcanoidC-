using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusBase : MonoBehaviour
{
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected GameObject textPrefab;
    [SerializeField] protected string textBonus;
    protected Color ColorText;
    [SerializeField] protected Color ColorBonus;
    protected Text BlockHP;
    public GameDataScript gameData;
    // Start is called before the first frame update
    public virtual void CreateBonus()
    {
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        BlockHP = Instantiate(textPrefab, canvas.transform).GetComponent<Text>();
        ColorText = Color.black;
        BlockHP.color = ColorText;
        BlockHP.text = textBonus;
        BlockHP.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        gameObject.GetComponent<SpriteRenderer>().color = ColorBonus;
    }
    void Start()
    {
        CreateBonus();
    }

    // Update is called once per frame
    void Update()
    {
        BlockHP.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Bonus();
        Destroy(BlockHP);
        Destroy(gameObject);
    }

    public virtual void Bonus() {
        gameData.points += 100;
    }
}
