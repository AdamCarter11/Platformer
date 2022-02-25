using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private LayerMask grappleMask;
    [SerializeField] private float grappleSpeed;
    [SerializeField] private float grappleLength;
    [SerializeField] private int maxPoints;

    private Rigidbody2D rb;
    private List<Vector2> points = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = (mousePos-(Vector2)transform.position).normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, grappleLength, grappleMask);
            if(hit.collider != null){
                Vector2 hitPoint = hit.point;
                points.Add(hitPoint);
                rb.velocity = new Vector2(0,0);
                if(points.Count > maxPoints){
                    points.RemoveAt(0);
                }
            }
        }
        if(points.Count>0){
            Vector2 moveTo = centroid(points.ToArray());
            rb.MovePosition(Vector2.MoveTowards(transform.position, moveTo, Time.deltaTime * grappleSpeed));
            lr.positionCount = 0;
            lr.positionCount = points.Count*2;
            for(int n = 0, j = 0; n < points.Count*2; n+=2, j++){
                lr.SetPosition(n,transform.position);
                lr.SetPosition(n+1, points[j]);
            }
        }
        if(Input.GetKeyDown(KeyCode.Space)){
            Detatch();
        }
    }

    void Detatch(){
        lr.positionCount = 0;
        points.Clear();
    }

    Vector2 centroid(Vector2[] points){
        Vector2 center = Vector2.zero;
        foreach(Vector2 point in points){
            center += point;
        }
        center /= points.Length;
        return center;
    }
}
