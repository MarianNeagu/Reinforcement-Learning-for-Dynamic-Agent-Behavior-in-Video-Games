using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFieldOfView : MonoBehaviour
{
    [SerializeField] private float viewRadius;
	[Range(0, 360)]
    [SerializeField] private float viewAngle;

	[SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

	[SerializeField] private Transform visibleTarget;

	private bool targetObstructedByWall;

    public bool TargetInFOV()
    {
        return visibleTarget != null;
    }

	public bool TargetObstructedByWall()
	{
		return targetObstructedByWall;
	}

	public float GetViewRadius()
	{
		return viewRadius;
	}

	public float GetViewAngle()
	{
		return viewAngle;
	}

	public Transform GetVisibleTarget() 
	{ 
		return visibleTarget; 
	}

    private void Update()
    {
		FindVisibleTargets();
    }

    private void FindVisibleTargets()
	{
		targetObstructedByWall = false;
        visibleTarget = null;
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
		{
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
			{
				float dstToTarget = Vector3.Distance(transform.position, target.position);

				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
				{
					visibleTarget = target;
				}
				else
				{
                    visibleTarget = null; // De test
                    targetObstructedByWall = true;
				}

			}
		}
        
    }

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}
} 
