using UnityEngine;
using System.Collections;

public class FollowCameraController : MonoBehaviour
{
    // static fields
    private const float UpDistanceMinValue = 0.0f;
    private const float UpDistanceMaxValue = 20.0f;
    private const float AwayDistanceMinValue = 0.0f;
    private const float AwayDistanceMaxValue = 20.0f;
    private const float FollowSpeedMinValue = 1.0f;
    private const float FollowSpeedMaxValue = 10.0f;

    // public fields
    [Range(UpDistanceMinValue, UpDistanceMaxValue)]
    public float upDistance = 1.0f;

    [Range(AwayDistanceMinValue, AwayDistanceMaxValue)]
    public float awayDistance = 3.0f;

    [Range(FollowSpeedMinValue, FollowSpeedMaxValue)]
    public float followSpeed = 4.0f;

    public Transform targetToFollow;

    #region Unity Events

    private void LateUpdate()
    {
        Vector3 upOffset = this.upDistance * Vector3.up;
        Vector3 awayOffset = this.targetToFollow.forward * this.awayDistance;
        Vector3 newPosition = this.targetToFollow.position + upOffset - awayOffset;

        Debug.DrawLine(this.targetToFollow.position, this.targetToFollow.position + upOffset, Color.red);
        Debug.DrawLine(this.targetToFollow.position, this.targetToFollow.position - awayOffset, Color.green);
        Debug.DrawLine(this.targetToFollow.position, newPosition, Color.blue);

        this.transform.position = Vector3.Lerp(this.transform.position, newPosition, this.followSpeed * Time.deltaTime);
        this.transform.LookAt(this.targetToFollow);
    }

    #endregion Unity Events
}
