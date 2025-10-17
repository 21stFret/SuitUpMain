using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipideBossManager : MonoBehaviour
{
    public static CentipideBossManager instance;
    public List<CentapideHead> firstPhaseHeads, secondPhaseHeads, thirdPhaseHeads, bossHeads;
    public GameObject bossHead;
    public GameObject firstPhaseParents;
    public List<GameObject> secondPhaseParents;
    public FallingRocksController fallingRocksController;
    public float timeBetweenRockFalls = 5f; // Time interval between rock falls in phase two
    public float fallingDuration = 30f; // Duration for which rocks will fall in phase two
    private float fallingTimer = 0f;
    public AudioSource centipideRoarSound;
    public List<AudioClip> centipideRoarClips;
    private int finalHeadsIndex = 0;
    public float randomDropTimer = 0f;
    public bool rockfallingPhase = false;
    public bool phaseThreeStarted = false;
    public bool finalPhaseStarted = false;
    private int totalHeads;
    private int deadHeads;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartBossFight()
    {
        rockfallingPhase = false;
        phaseThreeStarted = false;
        finalPhaseStarted = false;
        centipideRoarSound.Play();
        firstPhaseParents.SetActive(true);
        finalHeadsIndex = 0;
        totalHeads = firstPhaseHeads.Count + secondPhaseHeads.Count + thirdPhaseHeads.Count + bossHeads.Count;
        deadHeads = 0;
    }

    void Update()
    {
        if (rockfallingPhase)
        {
            fallingTimer -= Time.deltaTime;
            if (fallingTimer <= 0f && fallingRocksController != null)
            {
                fallingTimer = fallingDuration;
                fallingRocksController.StopFalling();
                rockfallingPhase = false;
                if(phaseThreeStarted)
                {
                    StartFinalPhase();
                    return;
                }
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
            if (randomDropTimer >= timeBetweenRockFalls) // Drop rocks every 5 seconds
            {
                randomDropTimer = 0f;
                if (fallingRocksController != null)
                {
                    fallingRocksController.StartFalling();
                    StartCoroutine(StopFallingAfterDelay(1f)); // Let rocks fall for 1 second
                    centipideRoarSound.clip = centipideRoarClips[Random.Range(1, centipideRoarClips.Count)];
                    centipideRoarSound.Play();
                }
            }
        }
        if (finalPhaseStarted)
        {
            if (finalHeadsIndex < thirdPhaseHeads.Count)
            {
                randomDropTimer += Time.deltaTime;
                if (randomDropTimer >= 2f)
                {
                    thirdPhaseHeads[finalHeadsIndex].gameObject.SetActive(true);
                    finalHeadsIndex++;
                }
            }
        }
    }

    private void StartRockFalling()
    {
        rockfallingPhase = true;
        if (fallingRocksController != null)
        {
            fallingRocksController.StartFalling();
        }
        centipideRoarSound.Play();
    }

    private void StartFinalPhase()
    {
        phaseThreeStarted = false;
        finalPhaseStarted = true;
        bossHead.SetActive(true);
        fallingRocksController.StartFalling();
        centipideRoarSound.clip = centipideRoarClips[0];
        centipideRoarSound.Play();
    }

    public void CheckDeadHeads()
    {
        deadHeads++;
        GameManager.instance.gameUI.objectiveUI.UpdateBar(1-(float)deadHeads / totalHeads);
        if (finalPhaseStarted)
        {
            foreach (var head in bossHeads)
            {
                if (!head.dead) return; // If any head is alive, exit
            }
            foreach (var head in thirdPhaseHeads)
            {
                if (!head.dead) return; // If any head is alive, exit
            }
            // All heads are dead, boss defeated
            Debug.Log("Boss defeated!");
            fallingRocksController.StopFalling();
            BattleManager.instance.ObjectiveComplete();
            return;
        }
        if(phaseThreeStarted)
        {
            foreach (var head in secondPhaseHeads)
            {
                if (!head.dead) return; // If any head is alive, exit
            }
            StartRockFalling();
            return; // If phase two has already started, do nothing
        }
        foreach (var head in firstPhaseHeads)
        {
            if (!head.dead) return; // If any head is alive, exit
        }
        StartRockFalling();
    }

    private IEnumerator StopFallingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (fallingRocksController != null)
        {
            if(finalPhaseStarted)
            {
                yield break;
            }
            fallingRocksController.StopFalling();
        }
    }
}
