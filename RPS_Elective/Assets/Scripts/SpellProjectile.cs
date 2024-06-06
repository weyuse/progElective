using UnityEngine;

/// <summary>
/// The projectile that is fired.
/// Currently never leaves the scene once added
public class SpellProjectile : MonoBehaviour
{
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
        //default colour 
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

}
