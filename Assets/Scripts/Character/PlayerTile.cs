using UnityEngine;

public class PlayerTile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public SpriteRenderer sr;

    // Optional: expose for inspector preview
    private int currentPlayerTile = 0;
    void Start()
    {
        sr.sprite = GameManager.Instance.playerSpriteManager.Get(currentPlayerTile);
    }
    public void SetClassSprite(int index)
    {
        currentPlayerTile = index;
        sr.sprite = GameManager.Instance.playerSpriteManager.Get(currentPlayerTile);
        //sr.sprite = classSprites[index];
    }

    public void SetCustomSprite(Sprite newSprite)
    {
        sr.sprite = newSprite;
    }
}
