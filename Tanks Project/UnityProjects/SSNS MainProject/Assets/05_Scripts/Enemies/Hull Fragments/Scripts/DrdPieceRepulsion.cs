using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrdPieceRepulsion : MonoBehaviour
{
    [SerializeField] Transform dreadnova;
    [SerializeField] GameObject pieceToThrow;
    [SerializeField] GameObject explosionToOn;
    public GameObject fireToOff;
    [SerializeField] GameObject replaceScar;
    [SerializeField] GameObject turnOffCrack;
    [SerializeField] float propulsionForce = 100f;
    [SerializeField] bool yFire = true;
    float spinnyAmount = 1.0f;
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.GetComponent<HealthAndShields>().onDeath += Repulse;
        pieceToThrow.SetActive(false);
        explosionToOn.SetActive(false);
        replaceScar.SetActive(false);
        fireToOff.SetActive(false);
        Physics.IgnoreLayerCollision(10, 15);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Repulse()
    {
        DrdRandomShooty.manager.AddShoot();

        fireToOff.SetActive(false);
        pieceToThrow.TryGetComponent(out Rigidbody rb);
        pieceToThrow.SetActive(true);
        explosionToOn.SetActive(true);
        replaceScar.SetActive(true);
        turnOffCrack.SetActive(false);

        if (rb != null)
        {
            Vector3 dir = gameObject.transform.position - dreadnova.position;
            dir = dir.normalized;
            if(yFire)
                dir = new Vector3(dir.x, dir.y * 2, 0);
            else
                dir = new Vector3(dir.x * 2, dir.y, 0);

            rb.AddForce(dir * propulsionForce, ForceMode.Impulse);
            rb.AddTorque( spinnyAmount * Random.Range(0.0f, 1.0f), spinnyAmount * Random.Range(0.0f, 1.0f), spinnyAmount * Random.Range(0.0f, 1.0f), ForceMode.Impulse);
        }

        //gameObject.SetActive(false);
    }
}
