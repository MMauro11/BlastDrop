using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class AimControl : MonoBehaviour
{
    [SerializeField] private GameObject sight;
    private GameObject enemy,oldEnemy;
    private Transform followThis;

    //Position of the target relative to the screen
    public Vector2 screenTarget;

    //TARGET
    [SerializeField] private Camera displayCamera;

    private GameObject[] Targets;
    public GameObject nearestEnemy;
    private float bestAngle;
    private bool enemyFound = false;

    //boundary variables
    private bool isOutOfBounds;
    [SerializeField] private int borderLeft;
    [SerializeField] private int borderRight;
    [SerializeField] private int borderTop;
    [SerializeField] private int borderBottom;

    private void Start()
    {
        //Don't show on load.
        NoTarget();

        //TARGET
        if (displayCamera == null)
        {
            displayCamera = Camera.main;
        }
        Targets = GameObject.FindGameObjectsWithTag("Enemy");

        nearestEnemy = null;
    }

    void Update()
    {
        oldEnemy = enemy;
        enemy = NearestEnemy();
        //If the player currently has a target
        if (enemy != null)//check this
        {
            //set the enemy's target to the transform to track
            followThis = enemy.transform;
            
            //if there is a target.
            if (followThis == null)
            {
                return;
            }

            //calculate position relative to the screen space
            screenTarget = displayCamera.WorldToScreenPoint(followThis.position);

            if(!IsOutOfBounds())
            {
                
                //Get the vector 2 of the position of the enemy via world space
                //edit the transform to match the world postion
                sight.transform.position = screenTarget; 
                
                //enable the sight
                ActivateSight();
               
                if(enemy != oldEnemy)
                {
                    //draw new sight
                    SightDrawer.instance.ActivateSight();
                }

            }
            else
            {
                NoTarget();
            }
        }
    }

    private void ActivateSight()
    {
        sight.SetActive(true);
    }

    //sight disabler
    private void NoTarget()
    {
        sight.SetActive(false);
    }

    public GameObject NearestEnemy()
    {
            enemyFound = false;
            bestAngle = -1f;
            Targets = GameObject.FindGameObjectsWithTag("Enemy");
            if (nearestEnemy == null || nearestEnemy.IsDestroyed() == true || nearestEnemy.activeSelf == false )
            {
                NoTarget();
            }
            foreach (GameObject target in Targets)
            {
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(displayCamera);
                if (GeometryUtility.TestPlanesAABB(planes, target.GetComponent<Collider>().bounds))
                {
                    Vector3 vectorToEnemy = target.transform.position - transform.position;
                    vectorToEnemy.Normalize();
                    float angleToEnemy = Vector3.Dot(transform.forward, vectorToEnemy);
                    if (angleToEnemy > bestAngle)
                    {
                        nearestEnemy = target;
                        bestAngle = angleToEnemy;
                        enemyFound = true;
                    }
                }
            } 

            return nearestEnemy; 
    }

    public bool IsOutOfBounds()
    {
        bool outOfBoundary;
        //caclulate if enemy is behind the player
        Vector3 toTarget = (nearestEnemy.transform.position - transform.position).normalized;
        if (Vector3.Dot(toTarget, transform.forward) < 0 || screenTarget.x <= borderLeft || screenTarget.x >= Screen.width - borderRight || screenTarget.y <= borderBottom || screenTarget.y >= Screen.height - borderTop)
        {
            outOfBoundary = true;
        }
        else
        {
            outOfBoundary = false;
        }
        isOutOfBounds = outOfBoundary;
        return isOutOfBounds;
    }
}
