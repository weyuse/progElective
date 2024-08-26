using UnityEngine;


public class SpellProjectile : MonoBehaviour

{
    public string magicType = "empty";
    private int baseDamage = 10;
    //Delay to not shoot itself
    private float collisionDetectionDelay = 0.05f; 
    private float elapsedTime = 0f;
    private audioControl audioController;

    void Start()
    {
        GameObject audioManager = GameObject.Find("Audio Source");
        if (audioManager != null)
        {
            audioController = audioManager.GetComponent<audioControl>();
        }
    }
       
    void FixedUpdate()
    {
        transform.Translate(new Vector3(0f, 0f, 500 * Time.fixedDeltaTime), Space.Self);
        transform.position = new Vector3(transform.position.x, 25f, transform.position.z);
        Destroy(gameObject, 2f);
        elapsedTime += Time.fixedDeltaTime;

    }

    public void SetColor(string magicType)
    {

        this.magicType = magicType;
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
        if (elapsedTime < collisionDetectionDelay) return;

        if (other.CompareTag("Boat"))
        {
            PirateShipController targetShip = other.GetComponent<PirateShipController>();
            if (targetShip != null)
            {
                HitTarget(targetShip);
            }   
        }
        else if (other.CompareTag("background")) 
        {
            Destroy(gameObject);
        }
    }

    //doubles or halves the damage depending on magic type
    public void HitTarget(PirateShipController target)
    {
        //grabs the targets magic type
        string targetMagicType = target.GetCurrentMagicType();
        int damage = baseDamage;
        bool isCritHit = false;

        // fire magic
        if (magicType == "Fire")
        {
            if (targetMagicType == "Leaf")
            {
                damage *= 2;
                isCritHit = true;
            }
            else if (targetMagicType == "Water")
            {
                damage /= 2;
            }
        }
        // leaf magic
        else if (magicType == "Leaf")
        {
            if (targetMagicType == "Water")
            {
                damage *= 2;
                isCritHit = true;
            }
            else if (targetMagicType == "Fire")
            {
                damage /= 2;
            }
        }
        //water magic type
        else if (magicType == "Water")
        {
            if (targetMagicType == "Fire")
            {
                damage *= 2;
                isCritHit = true;
            }
            else if (targetMagicType == "Leaf")
            {
                damage /= 2;
            }
        }

        if (isCritHit == true)
        {
            audioController.PlaySound(audioController.critHitSound);
        }
        else
        {
            audioController.PlaySound(audioController.normalHitSound);
        }

        target.hit(damage);
        //gets rid of the game object so it doesnt drift off forever
        Destroy(gameObject);
    }
}


