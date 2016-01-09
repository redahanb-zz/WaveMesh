/// <summary>
/// Wave Mesh - Ocean surface generator
/// 2016 - Ben Redahan
/// </summary>

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class WaveMesh : MonoBehaviour {

	public 	int 	xSize, 
					zSize;

	public float waveStrength;

	public bool flatShading = false;

	private Mesh waveMesh;
	private Vector3[] vertices;


	void Awake()
	{
		if (!flatShading) {
			Generate ();
		} else {
			Generate2 ();
		}
	}

	void Start () 
	{
		

	}

	void Update () 
	{
		if (!flatShading) {
			Wave ();
		} else {
			Wave2 ();
		}
	}

	// offset the vertices using Perlin Noise to get a wave effect
	private void Wave()
	{
		for (int q = 0; q < vertices.Length; q++) {
			float tempY = 	transform.position.y + Mathf.PerlinNoise (q + Time.time, 2.1f*(q) + Time.time) * waveStrength;
			Vector3 tempV = new Vector3 (vertices [q].x, tempY, vertices [q].z);
			vertices [q] = tempV;
		}
		waveMesh.vertices = vertices;
		waveMesh.RecalculateNormals ();
	}

	// using extra vertices for flat shading
	private void Wave2()
	{
		for (int q = 0; q < vertices.Length-5; q+=6) {
			float tempY = 	transform.position.y + Mathf.PerlinNoise (q + Time.time, 2.1f*(q) + Time.time) * waveStrength;
			Vector3 tempV = new Vector3 (vertices [q].x, tempY, vertices [q].z);
			for(int t = 0; t < 6; t++){
				vertices [q+t] = tempV;
			}
		}
		waveMesh.vertices = vertices;
		waveMesh.RecalculateNormals ();
	}

	// Generate the mesh surface according to the correct dimensions
	private void Generate()
	{
		GetComponent<MeshFilter>().mesh = waveMesh = new Mesh ();
		waveMesh.name = "Ocean Surface";

		vertices = new Vector3[(xSize + 1) * (zSize + 1)];

		for (int i = 0, y = 0; i <= xSize; i++) 
		{
			for (int j = 0; j <= zSize; j++, y++) 
			{
				vertices [y] = new Vector3 (j, 0, i);
			}
		}

		waveMesh.vertices = vertices;

		int[] triangles = new int[xSize * zSize * 6];
		for(int ti = 0, ve = 0, z = 0; z < zSize; z++, ve++){
			for(int x = 0; x < xSize; x++, ti+=6, ve++){
				triangles [ti] = ve;
				triangles [ti+1] = triangles[ti+4] = ve+ xSize+1;
				triangles [ti+2] = triangles[ti+3] = ve+ 1;
				triangles [ti+5] = ve + xSize + 2;
				waveMesh.triangles = triangles;
				waveMesh.RecalculateNormals ();
			}
		}
	} // end Generate

	// multiple vertices, unique vertices per triangle, for flat shading effect
	private void Generate2()
	{
		GetComponent<MeshFilter>().mesh = waveMesh = new Mesh ();
		waveMesh.name = "Ocean Surface";

		vertices = new Vector3[(xSize + 1) * (zSize + 1) * 6];

		for (int i = 0, y = 0; i <= xSize; i++) 
		{
			for (int j = 0; j <= zSize; j++, y+=6) 
			{
				for (int t = 0; t < 6; t++) {
					vertices [y+t] = new Vector3 (j, 0, i);
					//vertices [y+t] = new Vector3 (j, -t, i);
				}
			}
		}

		waveMesh.vertices = vertices;

		int[] triangles = new int[xSize * zSize * 6];
		for(int ti = 0, ve = 0, z = 0; z < zSize; z++, ve+=6){
			for(int x = 0; x < xSize; x++, ti+=6, ve+=6){
				triangles [ti] = ve;
				triangles [ti+1] = ve + 6*(xSize+1) + 1;
				triangles [ti+2] = ve + 6 + 2;
				triangles [ti+3] = ve + 6 + 3;
				triangles [ti+4] = ve + 6*(xSize+1) + 4;
				triangles [ti+5] = ve + 6*(xSize+1) + 11;
				waveMesh.triangles = triangles;
				waveMesh.RecalculateNormals ();
			}
		}
	} // end Generate

	// visualise the vertices
	private void OnDrawGizmos()
	{
		if (vertices == null) {
			return;
		}

		Gizmos.color = Color.black;
		for (int i = 0; i < vertices.Length; i++) 
		{
			Gizmos.DrawSphere (vertices [i], 0.1f);
		}
	}
}
