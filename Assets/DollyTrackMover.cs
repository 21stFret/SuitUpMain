using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyTrackMover : MonoBehaviour
{
    public Cinemachine.CinemachinePathBase Path;
    public Cinemachine.CinemachineVirtualCamera virtualCamera;
    public Cinemachine.CinemachineDollyCart dollyCart;
    public float speed = 1.0f;
    public bool enableAutoMove = false;

    public void MoveOnPath()
    {
        StartCoroutine(Move());
    }

    public void MoveCart()
    {
        dollyCart.m_Speed = speed;
    }

    public void MoveCartBack()
    {
        dollyCart.m_Speed = -speed;
    }

    private IEnumerator Move()
    {
        float distance = 0.0f;
        while (distance < Path.PathLength)
        {
            distance += speed * Time.deltaTime;
            Vector3 newPosition = Path.EvaluatePositionAtUnit(distance / Path.PathLength, Cinemachine.CinemachinePathBase.PositionUnits.Distance);
            print(newPosition);
            transform.position = newPosition;
            yield return null;
        }
    }

}
