using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipideBossManager : MonoBehaviour
{
    public List<CentapideHead> firstPhaseHeads, secondPhaseHeads;
    public List<GameObject> secondPhaseParents;
    public FallingRocksController fallingRocksController;
    private bool phaseTwoStarted = false;
    public float fallingDuration = 30f; // Duration for which rocks will fall in phase two
    public AudioSource centipideRoarSound;
    public List<AudioClip> centipideRoarClips;
    public float randomDropTimer = 0f;
    private bool phaseThreeStarted = false;

    void Start()
    {
        centipideRoarSound.Play();
    }

    void Update()
    {
        if (phaseTwoStarted)
        {
            fallingDuration -= Time.deltaTime;
            if (fallingDuration <= 0f && fallingRocksController != null)
            {
                fallingRocksController.StopFalling();
                phaseTwoStarted = false; // End phase two
                phaseThreeStarted = true;
                foreach (var parent in secondPhaseParents)
                {
                    parent.SetActive(true);
                }
            }
        }
        if (phaseThreeStarted)
        {
            randomDropTimer += Time.deltaTime;
            if (randomDropTimer >= 5f) // Drop rocks every 5 seconds
            {
                randomDropTimer = 0f;
                if (fallingRocksController != null)
                {
                    fallingRocksController.StartFalling();
                    StartCoroutine(StopFallingAfterDelay(3f)); // Let rocks fall for 3 seconds
                    centipideRoarSound.clip = centipideRoarClips[Random.Range(1, centipideRoarClips.Count)];
                }
            }
        }
    }

    private void StartPhaseTwo()
    {
        phaseTwoStarted = true;
        if (fallingRocksController != null)
        {
            fallingRocksController.StartFalling();
        }
        centipideRoarSound.Play();
    }

    public void CheckDeadHeads()
    {
        foreach (var head in firstPhaseHeads)
        {
            if (!head.dead) return; // If any head is alive, exit
        }
        StartPhaseTwo();
    }

    private IEnumerator StopFallingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (fallingRocksController != null)
        {
            fallingRocksController.StopFalling();
        }
    }
}
