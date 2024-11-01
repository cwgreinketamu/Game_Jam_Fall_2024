using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] private Image healthBar; 
    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseBar(float amount)
    {
        healthBar.fillAmount += amount;
    }

    public void DecreaseBar(float amount)
    {
        healthBar.fillAmount -= amount;
    }

    public void SetBar(float amount)
    {
        healthBar.fillAmount = amount;
    }
}
