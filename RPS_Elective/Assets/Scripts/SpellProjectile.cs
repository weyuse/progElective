using UnityEngine;

/// <summary>
/// The projectile that is fired.
/// Currently never leaves the scene once added
public class SpellProjectile : MonoBehaviour

{
    private string magicType = "";
    private int baseDamage = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Makes the bullet fly
    
    void FixedUpdate()
    {
        transform.Translate(new Vector3(0f, 0f, 500 * Time.fixedDeltaTime), Space.Self);
    }

    public void SetColor(string magicType)
    {
        //default colour no u
        Color color = Color.white;



        //changing per type
        switch (magicType)
        {
            case "Fire":
                color = Color.red;
                break;
            case "Water":
                color = Color.blue;
                break;
            case "Leaf":
                color = Color.green;
                break;
        }

        GetComponent<Renderer>().material.color = color;
    }

    // Checking if the missile hits a wizard still tagged as boat
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            PirateShipController targetShip = other.GetComponent<PirateShipController>();
            if (targetShip != null)
            {
                HitTarget(targetShip);
            }
        }
    }

    //doubles or halves the damage depending on magic type
    private void HitTarget(PirateShipController target)
    {
        int damage = baseDamage;

        if (magicType == "Fire" && target.GetCurrentMagicType() == "Leaf")
        {
            damage *= 2; 
        }
        else if (magicType == "Fire" && target.GetCurrentMagicType() == "Water")
        {
            damage /= 2; 
        }

        target.hit(damage);
        //gets rid off the game object so it doesnt drift off forever
        Destroy(gameObject);
    }
}


