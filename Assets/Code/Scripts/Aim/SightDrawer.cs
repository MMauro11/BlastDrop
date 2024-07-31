using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SightDrawer : MonoBehaviour
{
    //List to manage sight dots
    List<GameObject> SightPieces = new List<GameObject>();

    private GameObject sightDot;
    [SerializeField] private GameObject sightPiece;
    [SerializeField] private AimControl aimControl;
    private Enemy enemy;
    public static SightDrawer instance;

    void Awake()
    { instance = this; }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void DrawAroundPoint(int num, Vector3 point, float radius)
    {
        //randomize starting position
        float rand = UnityEngine.Random.Range(0, 1);
        
        for (int i = 0; i < num; i++)
        {
            /* Distance around the circle */
            var radians = 2 * Mathf.PI / num * i + rand;

            /* Get the vector direction */
            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians);

            var spawnDir = new Vector3(horizontal, vertical, 0);

            /* Get the spawn position */
            var spawnPos = point + spawnDir * radius; // Radius is just the distance away from the point

            /* Now spawn */
            sightDot = (GameObject)Instantiate(sightPiece, this.transform);
            sightDot.transform.position = spawnPos;

            //Add every sight dot to a list that manage them
            SightPieces.Add(sightDot);

            /* Adjust height */
            sightDot.transform.Translate(new Vector3(0, sightDot.transform.localScale.y / 2, 0));

        }
    }


    // Update is called once per frame
    void Update()
    {
    }

    public void ActivateSight()
    {
        //if (aimControl.nearestEnemy != null && !aimControl.IsOutOfBounds() )
        {
            enemy = aimControl.nearestEnemy.GetComponent<Enemy>();
            DrawNewSight();
        }
    }

    private void CleanSight()
    {
        foreach (GameObject piece in SightPieces)
        {
            Destroy(piece);
        }
    }

    public void RemoveDot()
    {
        SightPieces.Remove(sightPiece);
        DrawNewSight();
    }

    private void OnEnable()
    {
        ActivateSight();
    }

    private void DrawNewSight()
    {
        CleanSight();
        DrawAroundPoint(enemy.Life, new Vector3(aimControl.screenTarget.x, aimControl.screenTarget.y, 0), 30f);
    }
}
