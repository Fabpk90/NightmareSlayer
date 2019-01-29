using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float xMax;
    public float xMin;
    public float yMax;
    public float yMin;
    public float yOffset = 0;

    public bool isFollowingPlayer = true;

    public Transform playerPosition;
    public Vector3 bossRoomPosition;
    public float cameraMovementDuration;

    void LateUpdate()
    {

        if (isFollowingPlayer && playerPosition != null)
        {
            transform.position = new Vector3(Mathf.Clamp(playerPosition.position.x, xMin, xMax), Mathf.Clamp(playerPosition.position.y + yOffset, yMin, yMax), transform.position.z);
        }
        //else if (isMovingBackToPlayer)
        //{
        //    transform.position = new Vector3(Mathf.Clamp(Mathf.Lerp(transform.position.x, playerPosition.position.x, lerpIncrement), xMin, xMax), Mathf.Clamp(Mathf.Lerp(transform.position.y + yOffset, playerPosition.position.y, lerpIncrement), yMin, yMax), transform.position.z);
        //    if (lerpIncrement < 1)
        //    {
        //        lerpIncrement += Time.deltaTime / 2f;
        //    }
        //}
    }

    public void FixPositionForBossFight()
    {
        StartCoroutine(FixPosition());
    }

    private IEnumerator FixPosition()
    {
        GameManager.instance.GiveControls(false);

        float xStart = transform.position.x;
        float startTime = Time.time;
        float time = 0;
        isFollowingPlayer = false;
        while (time < 1)
        {
            transform.position = new Vector3(Mathf.SmoothStep(xStart, bossRoomPosition.x, time), bossRoomPosition.y, bossRoomPosition.z);
            time = (Time.time - startTime) / cameraMovementDuration;
            yield return null;
        }
    }
}
