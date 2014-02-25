using UnityEngine;
using System.Collections;

[System.Serializable]
public class TerrainMap{
	public int width;
	public int height;

	public TerrainMap(int pWidth, int pHeight){
		width = pWidth;
		height = pHeight;
	}
}

public class TerrainGenerator : MonoBehaviour {
	
	Terrain terrain;
	float minTileSize = 5.0f;
	float maxTileSize = 50.0f;
	
	float maxX = 0.1f;
	float maxY = 0.05f;
	
	private float [,] heightmapBackup;
	private float [, ,] alphamapBackup;
	private TreeInstance[] backupTreeInstances;
	
	TerrainMap heightMap;
	TerrainMap alphaMap;
	TerrainMap detailMap;
	
	float splatMap;
	
	int detailCountPerDetailPixel = 16;
	
	float[,] heights;
	
	void Start(){
		terrain = GetComponent<Terrain>();
		
		RefreshTerrainMapVars();
		
		if (Debug.isDebugBuild){
            heightmapBackup = terrain.terrainData.GetHeights(0, 0, heightMap.width, heightMap.height);
            alphamapBackup = terrain.terrainData.GetAlphamaps(0, 0, alphaMap.width, alphaMap.height);  
			//backupTreeInstances = terrain.terrainData.treeInstances;
        }

		GenerateTerrain();
	}

	void OnApplicationQuit(){
        if (Debug.isDebugBuild){
            terrain.terrainData.SetHeights(0, 0, heightmapBackup);
            terrain.terrainData.SetAlphamaps(0, 0, alphamapBackup);
			//terrain.terrainData.treeInstances = backupTreeInstances;
        }
    }
	void Update(){
		if(Input.GetKeyDown(KeyCode.G)){
			GenerateTerrain();
		}
	}
	void OnGUI(){
		GUI.Label(new Rect(Screen.width*0.5f-100,0,200,25),"Press G to generate new Terrain");
	}

    void GenerateTerrain(){
		RefreshTerrainMapVars();
		heights = new float[terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight];
		
		float tempTileSize = Random.Range(minTileSize,maxTileSize);
 
        for (int i = 0; i < terrain.terrainData.heightmapWidth; i++){
            for (int j = 0; j < terrain.terrainData.heightmapHeight; j++){
				float x = ((float)i / (float)heightMap.width) * tempTileSize + Random.Range(0.0f,maxX);
				float y = ((float)j / (float)heightMap.height) * tempTileSize + Random.Range(0.0f,maxY);
                heights[i, j] = Mathf.PerlinNoise(x,y) / 10.0f;
            }
        }
		
        terrain.terrainData.SetHeights(0, 0, heights);
		//float [, ,] currAlphamap = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);
		//terrain.terrainData.SetAlphamaps(0, 0, terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight));
		
		//GenerateDetails();
		//GenerateTrees();
	}
	
	private void GenerateDetails(){
		int[,] newDetailLayer = new int[detailMap.width,detailMap.height];
		float resolutionDiffFactor = (float)alphaMap.width/detailMap.width;
		
		for(int i=0;i<detailMap.width;i++){
			for(int j=0;j<detailMap.height;j++){
				float [, ,] splatMap = terrain.terrainData.GetAlphamaps(0, 0, alphaMap.width, alphaMap.height);
				//float alphaValue = splatMap[(int)(resolutionDiffFactor*i),(int)(resolutionDiffFactor*j),0];
				//newDetailLayer[i,j] = (int)Mathf.Round(alphaValue * ((float)detailCountPerDetailPixel)) + newDetailLayer[i,j];

			}
		}
		terrain.terrainData.SetDetailLayer(0,0,0,newDetailLayer);
	}
	private void GenerateTrees(){
		float threshold = 0.8f;
        float variation = 0.1f;
		int step = 4;
		
		//Texture2D treeSplatMap = ;
		ArrayList instances = new ArrayList();
		for(int i=0;i<100;i++){
			for(int j=0;j<100;j++){
				TreeInstance instance = new TreeInstance();
				Vector3 pos = new Vector3(i,0.5f,j);
				
				instance.position = pos;
				instance.color = Color.white;
				instance.prototypeIndex=0;
				
				instance.widthScale = 1.0f + Random.Range(-variation,variation);
				instance.heightScale = 1.0f + Random.Range(-variation,variation);
				
				instances.Add(instance);
			}
		}
		// td= terrain.terrainData;
		///terrain.terrainData.treeInstances = (TreeInstance[])instances.ToArray(typeof(TreeInstance));
		//td.RefreshPrototypes();
		//terrain.terrainData.RecalculateTreePositions();
		
	}
	
	private void RefreshTerrainMapVars(){
		heightMap = GetHeightMap();
		alphaMap = GetAlphaMap();
		detailMap = GetDetailMap();
	}
	
	private TerrainMap GetHeightMap(){
		return new TerrainMap(terrain.terrainData.heightmapWidth,terrain.terrainData.heightmapHeight);
	}
	
	private TerrainMap GetAlphaMap(){
		return new TerrainMap(terrain.terrainData.alphamapWidth,terrain.terrainData.alphamapHeight);
	}
	
	private TerrainMap GetDetailMap(){
		return new TerrainMap(terrain.terrainData.detailResolution,terrain.terrainData.detailResolution);
	}
}