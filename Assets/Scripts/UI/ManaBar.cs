using UnityEngine;

public class ManaBar : MonoBehaviour
{
    public GameObject slider;

    // public SpellData sc;
    public PlayerSpellController psc;
    float old_perc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (psc == null) return;
        float perc = psc.mana * 1.0f / psc.max_mana;
        if (Mathf.Abs(old_perc - perc) > 0.01f)
        {
            slider.transform.localScale = new Vector3(perc, 1, 1);
            slider.transform.localPosition = new Vector3(-(1 - perc) / 2, 0, 0);
            old_perc = perc;
        }
    }
    
    // public void SetSpellCaster(SpellData sc)
    // {
    //     this.psc = sc;
    //     old_perc = 0;
    // }

    public void SetManaBar(PlayerSpellController psc)
    {
        
        this.psc = psc;
        old_perc = 0;
      
    }
}
