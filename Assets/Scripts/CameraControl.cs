using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject target;
    public bool FollowTargetSmooth = false, followTarget = true, followPosition = false;
    public float followSpeed = .1f,followPositionDistanceX=10, followPositionDistanceY=8,ownFollowSpeed=.1f;
    public Vector2 position = Vector2.zero;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void ResetCamera()
    {
        target = null;
        FollowTargetSmooth = false; followTarget = true; followPosition = false;
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        //Debug.Log("Say to follow target");
        if(target != null&&followTarget)
        {
            //Debug.Log("Follow target");
            MoveTowards(new(target.transform.position.x, target.transform.position.y));
        }
        else if (followPosition){
            MoveTowards(new(position.x, position.y));
        }
    }
    public void FollowPosition(Vector2 position)
    {
        followPosition = true;
        FollowTargetSmooth = true;
        followTarget = false;
        this.position = position;
    }
    private void MoveTowards(Vector2 pos)
    {
        var curDistance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(pos.x, pos.y));
        if (curDistance < followSpeed)
        {
            FollowTargetSmooth = false;
        }
        if (!FollowTargetSmooth)
        {
            transform.position = new(pos.x, pos.y, transform.position.z);
        }
        else
        {
            if (followPosition)
            {
                //Debug.Log("Yes follow posiotn");
                var tempPosition = transform.position;
                if (Mathf.Abs(transform.position.x - pos.x) > followPositionDistanceX)
                {
                    if (transform.position.x > pos.x)
                    {
                        tempPosition.x -= ownFollowSpeed;
                    }
                    else
                    {
                        tempPosition.x += ownFollowSpeed;
                    }
                }
                if (Mathf.Abs(transform.position.y - pos.y) > followPositionDistanceY)
                {
                    if (transform.position.y > pos.y)
                    {
                        tempPosition.y -= ownFollowSpeed;
                    }
                    else
                    {
                        tempPosition.y += ownFollowSpeed;
                    }
                }
                transform.position = tempPosition;
            }
            else
            {
                transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(pos.x, pos.y, transform.position.z), followSpeed+curDistance/12);
            }
            
        }
    }
    public void IncreasePosition(Vector3 changes)
    {
        followPosition = false;
        followTarget=false;
        transform.position += changes;
    }
    public void SetTarget(GameObject cameraTarget)
    {
        target = cameraTarget;
    }
}
