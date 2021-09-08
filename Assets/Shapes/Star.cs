using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Star : MonoBehaviour
{
	public Vector3 point = Vector3.up;
	public Vector3[] points;
	public int frequency = 5;

	private Mesh mesh;
	private Vector3[] vertices;
	private int[] triangles;

	void Start()
	{
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Star Mesh";
		points = new Vector3[] { new Vector3(0, 1, 0), new Vector3(0, (float)0.5, 0) };

		if (frequency < 1)
		{
			frequency = 1;
		}
		if (points == null)
		{
			points = new Vector3[0];
		}
		int numberOfPoints = frequency * points.Length;
		vertices = new Vector3[numberOfPoints + 1];
		triangles = new int[numberOfPoints * 3];

		if (numberOfPoints >= 3)
		{
			float angle = -360f / numberOfPoints;
			for (int repetitions = 0, v = 1, t = 1; repetitions < frequency; repetitions++)
			{
				for (int p = 0; p < points.Length; p += 1, v += 1, t += 3)
				{
					vertices[v] = Quaternion.Euler(0f, 0f, angle * (v - 1)) * points[p];
					triangles[t] = v;
					triangles[t + 1] = v + 1;
				}
			}
			triangles[triangles.Length - 1] = 1;
		}
		mesh.vertices = vertices;
		mesh.triangles = triangles;
	}
}
