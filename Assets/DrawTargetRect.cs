using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DrawTargetRect : MonoBehaviour {

	public Material myMaterial;
	
	public bool showTargetRect;

	private GameObject myObject;

	private Vector3[] nodePositions;

	// Use this for initialization
	void Start () {
		nodePositions = new Vector3[4];
		
		nodePositions[0] = new Vector3(-10,0,0);
		nodePositions[1] = new Vector3(10,0,0);
		nodePositions[2] = new Vector3(10,1,0);
		nodePositions[3] = new Vector3(-10,1,0);
	}
	
	// Update is called once per frame
	void Update () {
		if(!showTargetRect){
			return;
		}
		updatePolygon();
	}

	public void SetTargetBox(AttentionEvent e){
		var lon = AngleHelperMethods.PixelCoordToLong(e.targetHorPixel - (e.width/2f));
		var lat = AngleHelperMethods.PixelCoordToLat(e.targetVerPixel - (e.height/2f));
		var pos = AngleHelperMethods.LonLatToPosition(lon, lat);
		nodePositions[0] = pos;

		lon = AngleHelperMethods.PixelCoordToLong(e.targetHorPixel - (e.width/2f));
		lat = AngleHelperMethods.PixelCoordToLat(e.targetVerPixel + (e.height/2f));
		pos = AngleHelperMethods.LonLatToPosition(lon, lat);
		nodePositions[1] = pos;

		lon = AngleHelperMethods.PixelCoordToLong(e.targetHorPixel + (e.width/2f));
		lat = AngleHelperMethods.PixelCoordToLat(e.targetVerPixel + (e.height/2f));
		pos = AngleHelperMethods.LonLatToPosition(lon, lat);
		nodePositions[2] = pos;

		lon = AngleHelperMethods.PixelCoordToLong(e.targetHorPixel + (e.width/2f));
		lat = AngleHelperMethods.PixelCoordToLat(e.targetVerPixel - (e.height/2f));
		pos = AngleHelperMethods.LonLatToPosition(lon, lat);
		nodePositions[3] = pos;
	}

	void updatePolygon()
	{
		for(int x = 0; x < 2; x++)
		{
			//Destroy old game object
			if(myObject != null){
				Destroy(myObject);
			}
	
			//New mesh and game object
			myObject = new GameObject();
			myObject.name = "TargetPoly";

			//Components
			MeshFilter MF = myObject.AddComponent<MeshFilter>();
			MeshRenderer MR = myObject.AddComponent<MeshRenderer>();
			//myObject[x].AddComponent();
		
			//Create mesh
			Mesh mesh = CreateMesh(x);
		
			//Assign materials
			MR.material = myMaterial;
		
			//Assign mesh to game object
			MF.mesh = mesh;
		}
	}

	Mesh CreateMesh(int num)
	{
		
		int x; //Counter
	
		//Create a new mesh
		Mesh mesh = new Mesh();
	
		//Vertices
		var vertex = new Vector3[nodePositions.Length];
	
		for(x = 0; x < nodePositions.Length; x++)
		{
			vertex[x] = nodePositions[x];
		}
	
		//UVs
		var uvs = new Vector2[vertex.Length];
	
		for(x = 0; x < vertex.Length; x++)
		{
			if((x%2) == 0)
			{
				uvs[x] = new Vector2(0,0);
			}
			else
			{
				uvs[x] = new Vector2(1,1);
			}
		}
	
		//Triangles
		var tris = new int[3 * (vertex.Length - 2)];    //3 verts per triangle * num triangles
		int C1;
		int C2;
		int C3;
	
		if(num == 0)
		{
			C1 = 0;
			C2 = 1;
			C3 = 2;
		
			for(x = 0; x < tris.Length; x+=3)
			{
				tris[x] = C1;
				tris[x+1] = C2;
				tris[x+2] = C3;
			
				C2++;
				C3++;
			}
		}
		else
		{
			C1 = 0;
			C2 = vertex.Length - 1;
			C3 = vertex.Length - 2;
		
			for(x = 0; x < tris.Length; x+=3)
			{
				tris[x] = C1;
				tris[x+1] = C2;
				tris[x+2] = C3;
			
				C2--;
				C3--;
			}  
		}
	
		//Assign data to mesh
		mesh.vertices = vertex;
		mesh.uv = uvs;
		mesh.triangles = tris;
	
		//Recalculations
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();  
	
		//Name the mesh
		mesh.name = "MyMesh";
	
		//Return the mesh
		return mesh;
	}
}
