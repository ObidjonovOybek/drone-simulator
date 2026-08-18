using UnityEngine;

public class DroneMissionFollower : MonoBehaviour
{
    public Transform drone;
    public MissionPlanner missionPlanner;

    public float moveSpeed = 6f;
    public float rotateSpeed = 4f;
    public float stopDistance = 0.6f;

    private int currentIndex = 0;
    private bool isFlying = false;

    public void StartMission()
    {
        if (missionPlanner == null) return;
        if (missionPlanner.generatedRoute == null) return;
        if (missionPlanner.generatedRoute.Count == 0) return;

        currentIndex = 0;
        isFlying = true;
    }

    void Update()
    {
        if (!isFlying || drone == null || missionPlanner == null) return;
        if (currentIndex >= missionPlanner.generatedRoute.Count)
        {
            isFlying = false;
            return;
        }

        Vector3 target = missionPlanner.generatedRoute[currentIndex];
        Vector3 flatTarget = new Vector3(target.x, drone.position.y, target.z);

        Vector3 dir = flatTarget - drone.position;
        dir.y = 0f;

        if (dir.magnitude <= stopDistance)
        {
            currentIndex++;

            if (currentIndex >= missionPlanner.generatedRoute.Count)
            {
                isFlying = false;
                return;
            }

            target = missionPlanner.generatedRoute[currentIndex];
            flatTarget = new Vector3(target.x, drone.position.y, target.z);
            dir = flatTarget - drone.position;
            dir.y = 0f;
        }

        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            drone.rotation = Quaternion.Slerp(drone.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }

        drone.position = Vector3.MoveTowards(drone.position, flatTarget, moveSpeed * Time.deltaTime);
    }
}