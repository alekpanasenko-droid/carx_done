using UnityEngine;

/// <summary>Creates a small forest-and-cabin scene entirely from Unity primitives.</summary>
public sealed class ForestSceneBuilder : MonoBehaviour
{
    private Transform environmentRoot;
    private Material grassMaterial;
    private Material barkMaterial;
    private Material leafMaterial;
    private Material wallMaterial;
    private Material roofMaterial;
    private Material stoneMaterial;

    private void Awake()
    {
        if (Application.isPlaying)
            BuildScene();
    }

    [ContextMenu("Build Forest Scene")]
    public void BuildScene()
    {
        if (GameObject.Find("EnvironmentRoot") != null)
            return;

        environmentRoot = new GameObject("EnvironmentRoot").transform;
        CreateMaterials();
        CreateLightingAndCamera();
        CreateGround();
        CreateCabin();
        CreateForest();
        CreateGrass();
        CreateRocksAndPath();
    }

    private void CreateMaterials()
    {
        grassMaterial = MakeMaterial("Grass", new Color(0.22f, 0.55f, 0.18f), new Color(0.30f, 0.66f, 0.23f));
        barkMaterial = MakeMaterial("Bark", new Color(0.24f, 0.12f, 0.05f), new Color(0.38f, 0.20f, 0.08f));
        leafMaterial = MakeMaterial("Leaves", new Color(0.08f, 0.34f, 0.12f), new Color(0.16f, 0.48f, 0.16f));
        wallMaterial = MakeMaterial("Plaster", new Color(0.72f, 0.53f, 0.32f), new Color(0.86f, 0.68f, 0.42f));
        roofMaterial = MakeMaterial("Roof", new Color(0.32f, 0.08f, 0.04f), new Color(0.48f, 0.13f, 0.06f));
        stoneMaterial = MakeMaterial("Stone", new Color(0.28f, 0.29f, 0.27f), new Color(0.42f, 0.43f, 0.40f));
    }

    private Material MakeMaterial(string name, Color a, Color b)
    {
        var shader = Shader.Find("Standard") ?? Shader.Find("Diffuse");
        var material = new Material(shader) { name = name };
        var texture = new Texture2D(8, 8) { name = name + " Texture", filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Repeat };
        for (var y = 0; y < 8; y++)
        for (var x = 0; x < 8; x++)
            texture.SetPixel(x, y, (x + y) % 2 == 0 ? a : b);
        texture.Apply();
        material.mainTexture = texture;
        material.color = Color.white;
        return material;
    }

    private void CreateLightingAndCamera()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.64f, 0.78f, 0.72f);
        RenderSettings.fogDensity = 0.012f;
        RenderSettings.ambientLight = new Color(0.53f, 0.58f, 0.49f);

        var lightObject = new GameObject("Sun");
        lightObject.transform.SetParent(environmentRoot);
        lightObject.transform.rotation = Quaternion.Euler(48f, -30f, 0f);
        var sun = lightObject.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.15f;
        sun.color = new Color(1f, 0.93f, 0.77f);

        var cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.SetParent(environmentRoot);
        cameraObject.transform.position = new Vector3(13f, 8f, -16f);
        cameraObject.transform.LookAt(new Vector3(0f, 2f, 0f));
        var camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.Skybox;
        camera.fieldOfView = 58f;
    }

    private void CreateGround()
    {
        var ground = CreatePrimitive(PrimitiveType.Plane, "Meadow", Vector3.zero, new Vector3(4f, 1f, 4f), grassMaterial, environmentRoot);
        ground.transform.localScale = new Vector3(4f, 1f, 4f);
    }

    private void CreateCabin()
    {
        var cabin = new GameObject("Cabin").transform;
        cabin.SetParent(environmentRoot);
        CreatePrimitive(PrimitiveType.Cube, "Walls", new Vector3(0, 1.6f, 0), new Vector3(5.5f, 3.2f, 4.2f), wallMaterial, cabin);
        CreatePrimitive(PrimitiveType.Cube, "Door", new Vector3(0, 1.15f, -2.13f), new Vector3(1.05f, 2.25f, 0.12f), barkMaterial, cabin);
        CreatePrimitive(PrimitiveType.Cube, "WindowLeft", new Vector3(-1.65f, 1.9f, -2.13f), new Vector3(1.05f, 1.0f, 0.12f), leafMaterial, cabin);
        CreatePrimitive(PrimitiveType.Cube, "WindowRight", new Vector3(1.65f, 1.9f, -2.13f), new Vector3(1.05f, 1.0f, 0.12f), leafMaterial, cabin);
        var leftRoof = CreatePrimitive(PrimitiveType.Cube, "RoofLeft", new Vector3(-1.43f, 3.95f, 0), new Vector3(3.65f, 0.32f, 5.0f), roofMaterial, cabin);
        leftRoof.transform.rotation = Quaternion.Euler(0, 0, -31f);
        var rightRoof = CreatePrimitive(PrimitiveType.Cube, "RoofRight", new Vector3(1.43f, 3.95f, 0), new Vector3(3.65f, 0.32f, 5.0f), roofMaterial, cabin);
        rightRoof.transform.rotation = Quaternion.Euler(0, 0, 31f);
        CreatePrimitive(PrimitiveType.Cylinder, "Chimney", new Vector3(1.8f, 5.1f, 0.7f), new Vector3(0.5f, 1.4f, 0.5f), stoneMaterial, cabin);
    }

    private void CreateForest()
    {
        var forest = new GameObject("Forest").transform;
        forest.SetParent(environmentRoot);
        var positions = new[]
        {
            new Vector3(-9, 0, -4), new Vector3(-7, 0, 3), new Vector3(-5, 0, 8), new Vector3(7, 0, -4),
            new Vector3(9, 0, 3), new Vector3(6, 0, 8), new Vector3(-10, 0, 7), new Vector3(11, 0, 8)
        };
        for (var i = 0; i < positions.Length; i++)
            CreateTree(positions[i], 0.85f + (i % 3) * 0.18f, forest);
    }

    private void CreateTree(Vector3 position, float size, Transform parent)
    {
        var tree = new GameObject("Tree").transform;
        tree.SetParent(parent);
        CreatePrimitive(PrimitiveType.Cylinder, "Trunk", position + Vector3.up * (1.7f * size), new Vector3(0.65f * size, 3.4f * size, 0.65f * size), barkMaterial, tree);
        CreatePrimitive(PrimitiveType.Sphere, "CanopyLow", position + Vector3.up * (3.7f * size), new Vector3(3.0f * size, 2.4f * size, 3.0f * size), leafMaterial, tree);
        CreatePrimitive(PrimitiveType.Sphere, "CanopyHigh", position + Vector3.up * (5.0f * size), new Vector3(2.15f * size, 1.8f * size, 2.15f * size), leafMaterial, tree);
    }

    private void CreateGrass()
    {
        var grass = new GameObject("Grass").transform;
        grass.SetParent(environmentRoot);
        for (var x = -11; x <= 11; x += 2)
        for (var z = -9; z <= 9; z += 2)
        {
            if (Mathf.Abs(x) < 4 && Mathf.Abs(z) < 4) continue;
            var clump = CreatePrimitive(PrimitiveType.Cube, "GrassClump", new Vector3(x + 0.25f * (z % 3), 0.26f, z), new Vector3(0.12f, 0.52f, 0.65f), grassMaterial, grass);
            clump.transform.rotation = Quaternion.Euler(0, (x * 27 + z * 13) % 90, 8f);
        }
    }

    private void CreateRocksAndPath()
    {
        var props = new GameObject("RocksAndPath").transform;
        props.SetParent(environmentRoot);
        for (var z = -10; z < -2; z += 2)
            CreatePrimitive(PrimitiveType.Cylinder, "PathStone", new Vector3(0, 0.05f, z), new Vector3(1.15f, 0.1f, 0.8f), stoneMaterial, props);
        foreach (var point in new[] { new Vector3(-4, 0.4f, -3), new Vector3(4, 0.35f, 3), new Vector3(-6, 0.45f, 5) })
            CreatePrimitive(PrimitiveType.Sphere, "Rock", point, new Vector3(1.2f, 0.7f, 0.9f), stoneMaterial, props);
    }

    private GameObject CreatePrimitive(PrimitiveType type, string name, Vector3 position, Vector3 scale, Material material, Transform parent)
    {
        var item = GameObject.CreatePrimitive(type);
        item.name = name;
        item.transform.SetParent(parent);
        item.transform.position = position;
        item.transform.localScale = scale;
        item.GetComponent<Renderer>().sharedMaterial = material;
        return item;
    }
}
