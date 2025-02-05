using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnHolo : MonoBehaviour
{
    private Vector3 startPos;
    public float maxDistance = 10f;
    private bool triggered = false;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (triggered)
        {
            return;
        }
        if(Vector3.Distance(transform.position, startPos)>maxDistance)
        {
            StartCoroutine(Respawn());
            triggered = true;
        }
    }

    private IEnumerator Respawn()
    {
        var crawler = GetComponent<Crawler>();
        crawler.Die(WeaponType.Default);
        transform.position = startPos;
        yield return new WaitForSeconds(1f);
        crawler.gameObject.SetActive(true);
        crawler.Spawn();
        triggered = false;

    }
}
