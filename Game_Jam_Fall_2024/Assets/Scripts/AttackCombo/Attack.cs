using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{   

    public GameObject projectile;
    public float speed = 10f; //publicly accessible speed variable
    float horizontalInput;
    float verticalInput;


    private bool activeCombo = false;
    public GameObject fireCast;

    public GameObject windCast;
    
    public GameObject lightningCast;

    public GameObject iceCast;

    public GameObject fireWind;

    public GameObject fireLightning;

    public GameObject fireIce;

    public GameObject windLightning;

    public GameObject windIce;

    public GameObject lightningIce;


    public class Spell
    {
        public GameObject fireCombo;
        public GameObject windCombo;
        public GameObject lightningCombo;
        public GameObject iceCombo;
    }

    public Spell fire;
    public Spell wind;
    public Spell lightning;
    public Spell ice;

    private float timeSinceLastAttack;
    public float attackCooldown = 1.0f; // Cooldown time in seconds
    public float comboTimer = 2.0f; // Time window for combos
    private Spell lastSpell;

    private Dictionary<string, GameObject> baseSpells;
    private Dictionary<string, Dictionary<Spell, GameObject>> comboSpells;

    // Start is called before the first frame update
    void Start()
    {
        fire = new Spell() {
            fireCombo = fireCast,
            windCombo = fireWind,   
            lightningCombo = fireLightning,
            iceCombo = fireIce
        };

        wind = new Spell() {
            fireCombo = fireWind,
            windCombo = windCast,
            lightningCombo = windLightning,
            iceCombo = windIce
        };

        lightning = new Spell() {
            fireCombo = fireLightning,
            windCombo = windLightning,
            lightningCombo = lightningCast,
            iceCombo = lightningIce
        };

        ice = new Spell() {
            fireCombo = fireIce,
            windCombo = windIce,
            lightningCombo = lightningIce,
            iceCombo = iceCast
        };

        timeSinceLastAttack = attackCooldown; // Initialize to allow immediate attack

        baseSpells = new Dictionary<string, GameObject>
        {
            { "1", fire.fireCombo },
            { "2", wind.windCombo },
            { "3", lightning.lightningCombo },
            { "4", ice.iceCombo }
        };

        comboSpells = new Dictionary<string, Dictionary<Spell, GameObject>>
        {
            { "1", new Dictionary<Spell, GameObject> { { fire, fire.fireCombo }, { wind, wind.fireCombo }, { lightning, lightning.fireCombo }, { ice, ice.fireCombo } } },
            { "2", new Dictionary<Spell, GameObject> { { fire, fire.windCombo }, { wind, wind.windCombo }, { lightning, lightning.windCombo }, { ice, ice.windCombo } } },
            { "3", new Dictionary<Spell, GameObject> { { fire, fire.lightningCombo }, { wind, wind.lightningCombo }, { lightning, lightning.lightningCombo }, { ice, ice.lightningCombo } } },
            { "4", new Dictionary<Spell, GameObject> { { fire, fire.iceCombo }, { wind, wind.iceCombo }, { lightning, lightning.iceCombo }, { ice, ice.iceCombo } } }
        };
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * speed * Time.deltaTime);

        // Update the time since the last attack
        timeSinceLastAttack += Time.deltaTime;

        foreach (var key in baseSpells.Keys)
        {
            if (Input.GetKeyDown(key) && timeSinceLastAttack >= comboTimer)
            {
                Instantiate(baseSpells[key], transform.position, transform.rotation);
                baseSpells[key].GetComponent<Rigidbody2D>().velocity = transform.right * -10;

                timeSinceLastAttack = 0f;
                lastSpell = GetSpellByKey(key);
                activeCombo = true;
            }
            else if (Input.GetKeyDown(key) && timeSinceLastAttack <= comboTimer && lastSpell != null)
            {
                Instantiate(comboSpells[key][lastSpell], transform.position, transform.rotation);
                comboSpells[key][lastSpell].GetComponent<Rigidbody2D>().velocity = transform.right * -10;

                timeSinceLastAttack = 0f;
                lastSpell = null;
                activeCombo = false;
            }
        }
        if(timeSinceLastAttack >= comboTimer)
        {
            activeCombo = false;
        }
    }

    private Spell GetSpellByKey(string key)
    {
        switch (key)
        {
            case "1": return fire;
            case "2": return wind;
            case "3": return lightning;
            case "4": return ice;
            default: return null;
        }
    }
}