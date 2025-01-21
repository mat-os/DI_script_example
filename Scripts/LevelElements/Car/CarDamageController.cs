using System.Linq;
using UnityEngine;

namespace Game.Scripts.LevelElements.Car
{
    public class CarDamageController : MonoBehaviour
    {
        [SerializeField] private float deformRadius = 2f; // Radius of influence for the damage on mesh
        [SerializeField] private float deformMultiplier = 0.3f; // Deformation intensity multiplier
        [SerializeField] private AnimationCurve damageDistanceCurve; // Curve to control deformation falloff

        private MeshFilter[] meshFilters;
        private Vector3[][] originalVertices;
        private Vector3[][] deformedVertices;

        private void Start()
        {
            // Get all MeshFilters in the car and store the original vertices
            meshFilters = GetComponentsInChildren<MeshFilter>().Where(m => m.mesh.isReadable).ToArray();

            originalVertices = new Vector3[meshFilters.Length][];
            deformedVertices = new Vector3[meshFilters.Length][];

            for (int i = 0; i < meshFilters.Length; i++)
            {
                // Cache original and deformed vertices
                originalVertices[i] = meshFilters[i].mesh.vertices;
                deformedVertices[i] = meshFilters[i].mesh.vertices;
            }
        }

        // This method should be called when the car collides with an object
        private void OnCollisionEnter(Collision collision)
        {
            // Get the contact point of the collision
            foreach (ContactPoint contact in collision.contacts)
            {
                // Apply damage based on contact point
                ApplyDamage(contact.point, collision.relativeVelocity.magnitude);
            }
        }

        // Apply damage at the contact point and spread it to nearby vertices
        private void ApplyDamage(Vector3 contactPoint, float collisionForce)
        {
            // Iterate over all meshes in the car
            for (int i = 0; i < meshFilters.Length; i++)
            {
                Mesh mesh = meshFilters[i].mesh;
                Vector3[] vertices = mesh.vertices;

                // Iterate over all vertices in the mesh
                for (int j = 0; j < vertices.Length; j++)
                {
                    Vector3 worldVertexPosition = meshFilters[i].transform.TransformPoint(vertices[j]);

                    // Calculate the distance from the collision point to the vertex
                    float distanceToContact = (worldVertexPosition - contactPoint).sqrMagnitude;

                    // Apply damage if within deform radius
                    if (distanceToContact < deformRadius * deformRadius)
                    {
                        // Calculate deformation intensity based on distance (with falloff)
                        float distanceFactor = damageDistanceCurve.Evaluate(Mathf.Sqrt(distanceToContact) / deformRadius);
                        Vector3 deformation = collisionForce * deformMultiplier * distanceFactor * Random.insideUnitSphere;

                        // Apply deformation to the vertex
                        vertices[j] += deformation;
                    }
                }

                // Update the mesh with new vertices
                mesh.vertices = vertices;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
            }

            Debug.Log("Damage applied at contact point: " + contactPoint);
        }
    }
}
