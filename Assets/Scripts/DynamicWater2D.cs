using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicWater2D : MonoBehaviour
{
    [System.Serializable]
    public struct Bound
    {
        public float top;
        public float right;
        public float bottom;
        public float left;
    }
    [Header("WaterSettings")]
    public Bound bound;
    public int quality;
    public Material waterMAterial;
    public GameObject splash;
    [Header("PhysicsSettings")]
    public float spring = 0.02f;
    public float damping = 0.1f;
    public float spread = 0.1f;
    public float collisionVelocityFactor = 0.04f;

    private Vector3[] verticies;
    private Mesh mesh;

    private float[] velocities;
    private float[] accelerations;
    private float[] leftDeltas;
    private float[] rightDeltas;

    private float timer;

    private void GenerateMesh()
    {
        float range = (bound.right - bound.left) / (quality - 1);
        verticies = new Vector3[quality * 2];
        velocities = new float[quality];
        accelerations = new float[quality];
        leftDeltas = new float[quality];
        rightDeltas = new float[quality];
        //Generate verts 
        //top vert
        for (int i = 0; i < quality; i++)
        {
            verticies[i] = new Vector3(bound.left + (i * range), bound.top, 0);
            verticies[i + quality] = new Vector3(bound.left + (i * range), bound.bottom, 0);
        }
        //generate triangles
        int[] template = new int[6];
        template[0] = quality;
        template[1] = 0;
        template[2] = quality + 1;
        template[3] = 0;
        template[4] = 1;
        template[5] = quality + 1;

        int marker = 0;
        int[] tris = new int[((quality - 1) * 2) * 3];
        for (int i = 0; i < tris.Length; i++)
        {
            tris[i] = template[marker++]++;
            if (marker >= 6)
            {
                marker = 0;
            }
        }
        //Generate Mesh
        MeshRenderer meshRenderer = GetOrAddComponent<MeshRenderer>();
        if (waterMAterial != null) meshRenderer.sharedMaterial = waterMAterial;
        MeshFilter filter = GetOrAddComponent<MeshFilter>();
        if (mesh == null)
        {
            mesh = new Mesh();
        }
        else
        {
            mesh.Clear();
        }
        mesh.vertices = verticies;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.name = "WaterName";

        filter.mesh = mesh;
    }
    private T GetOrAddComponent<T>() where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }
    private void OnValidate()
    {
        GenerateMesh();
    }

    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D col = GetOrAddComponent<BoxCollider2D>();
        col.isTrigger = true;

        GenerateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0)
            return;

        timer -= Time.deltaTime;
        // Update physics
        for (int i = 0; i < quality; i++)
        {
            float force = spring * (verticies[i].y - bound.top) + velocities[i] * damping;
            accelerations[i] = -force;
            verticies[i].y += velocities[i];
            velocities[i] += accelerations[i];

        }
        //Affecting neighbour velocities
        for (int i = 0; i < quality; i++)
        {
            if (i > 0)
            {
                leftDeltas[i] = spread * (verticies[i].y - verticies[i - 1].y);
                velocities[i - 1] += leftDeltas[i];
            }
            if (i < quality - 1)
            {
                rightDeltas[i] = spread * (verticies[i].y - verticies[i + 1].y);
                velocities[i + 1] += rightDeltas[i];
            }
        }
        //update the mesh verts 
        mesh.vertices = verticies;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb2D = collision.GetComponent<Rigidbody2D>();
        Splash(collision, rb2D.velocity.y * collisionVelocityFactor);
    }
    public void Splash(Collider2D _col, float _force)
    {
        timer = 3;
        float radius = _col.bounds.center.x - _col.bounds.min.x;
        Vector2 center = new Vector2(_col.bounds.center.x, bound.top);
        // instatiating splach particles 
        if (splash != null)
        {
            GameObject newSplash = Instantiate(splash, new Vector3(center.x, center.y, 0), Quaternion.Euler(0, 0, 60));
            Destroy(newSplash, 2f);
        }
        for (int i = 0; i < quality; i++)
        {
            if (PointIsInsideCircle(verticies[i], center, radius))
            {
                velocities[i] = _force;
            }
        }
    }
    bool PointIsInsideCircle(Vector2 _point, Vector2 _center, float _radius)
    {
        return Vector2.Distance(_point, _center) < _radius;
    }
}
