using UnityEngine;

/// <summary>Creates a stylised forest cottage from Unity primitives.</summary>
public sealed class ForestSceneBuilder : MonoBehaviour
{
    private Transform root;
    private Material grass, bark, leaves, walls, roof, stone, darkWood, path, window, flower;

    private void Awake()
    {
        if (Application.isPlaying) BuildScene();
    }

    [ContextMenu("Build Forest Scene")]
    public void BuildScene()
    {
        if (GameObject.Find("EnvironmentRoot") != null) return;
        root = new GameObject("EnvironmentRoot").transform;
        MakeMaterials();
        MakeLightAndCamera();
        MakeGround();
        MakeCabin();
        MakeForest();
        MakeGarden();
        MakePath();
    }

    private void MakeMaterials()
    {
        grass = Material("Meadow", "245f1c", "438c2d");
        bark = Material("Bark", "321606", "64300d");
        darkWood = Material("Dark wood", "180805", "3a1708");
        leaves = Material("Leaves", "0b4512", "237c25");
        walls = Material("Timber walls", "88451e", "c87a39");
        roof = Material("Clay roof", "4b1007", "9a2c12");
        stone = Material("Field stone", "42433c", "74766c");
        path = Material("Path stone", "695128", "a17b3d");
        window = Material("Warm window", "d97a13", "ffe16c");
        flower = Material("Wildflowers", "d82c3d", "ffc440");
        window.EnableKeyword("_EMISSION");
        window.SetColor("_EmissionColor", new Color(0.7f, 0.18f, 0.01f));
    }

    private Material Material(string name, string hexA, string hexB)
    {
        ColorUtility.TryParseHtmlString("#" + hexA, out var a);
        ColorUtility.TryParseHtmlString("#" + hexB, out var b);
        var shader = Shader.Find("Standard") ?? Shader.Find("Diffuse");
        var result = new Material(shader) { name = name };
        var texture = new Texture2D(16, 16) { wrapMode = TextureWrapMode.Repeat, filterMode = FilterMode.Bilinear };
        for (var y = 0; y < 16; y++)
        for (var x = 0; x < 16; x++)
            texture.SetPixel(x, y, Color.Lerp(a, b, (Mathf.Sin(x * 1.7f + y * .8f) + 1f) * .5f));
        texture.Apply();
        result.mainTexture = texture;
        return result;
    }

    private void MakeLightAndCamera()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(.54f, .7f, .68f);
        RenderSettings.fogDensity = .009f;
        RenderSettings.ambientLight = new Color(.45f, .54f, .48f);
        var sun = new GameObject("Golden hour sun");
        sun.transform.SetParent(root);
        sun.transform.rotation = Quaternion.Euler(42, -28, 0);
        var light = sun.AddComponent<Light>();
        light.type = LightType.Directional; light.intensity = 1.2f; light.color = new Color(1f, .82f, .58f); light.shadows = LightShadows.Soft;
        var camera = new GameObject("Main Camera");
        camera.tag = "MainCamera"; camera.transform.SetParent(root);
        camera.transform.position = new Vector3(13.5f, 8.5f, -17.5f); camera.transform.LookAt(new Vector3(0, 2.2f, 0));
        var component = camera.AddComponent<Camera>();
        component.clearFlags = CameraClearFlags.SolidColor; component.backgroundColor = new Color(.39f, .62f, .76f); component.fieldOfView = 54;
    }

    private void MakeGround()
    {
        var meadow = P(PrimitiveType.Plane, "Meadow", Vector3.zero, new Vector3(4.8f, 1, 4.8f), grass, root);
        meadow.GetComponent<Renderer>().material.mainTextureScale = new Vector2(8, 8);
        P(PrimitiveType.Cylinder, "Cottage clearing", new Vector3(0, -.05f, .5f), new Vector3(6.8f, .09f, 6.8f), grass, root);
    }

    private void MakeCabin()
    {
        var cabin = Group("Cabin");
        P(PrimitiveType.Cube, "Stone foundation", new Vector3(0, .35f, .35f), new Vector3(6.4f, .7f, 5f), stone, cabin);
        P(PrimitiveType.Cube, "Timber walls", new Vector3(0, 2.05f, .35f), new Vector3(6f, 2.9f, 4.6f), walls, cabin);
        for (var x = -2.7f; x <= 2.7f; x += 1.35f) P(PrimitiveType.Cube, "Wall beam", new Vector3(x, 2.05f, -1.98f), new Vector3(.16f, 2.9f, .13f), darkWood, cabin);
        P(PrimitiveType.Cube, "Front beam", new Vector3(0, 3.15f, -1.98f), new Vector3(6.1f, .16f, .13f), darkWood, cabin);
        P(PrimitiveType.Cube, "Door", new Vector3(0, 1.55f, -2.03f), new Vector3(1.12f, 2.25f, .14f), darkWood, cabin);
        P(PrimitiveType.Sphere, "Door handle", new Vector3(.38f, 1.55f, -2.14f), new Vector3(.13f, .13f, .08f), window, cabin).GetComponent<Collider>().enabled = false;
        Window(new Vector3(-1.9f, 2.15f, -2.03f), cabin); Window(new Vector3(1.9f, 2.15f, -2.03f), cabin);
        Window(new Vector3(-3.03f, 2.15f, .25f), cabin, true); Window(new Vector3(3.03f, 2.15f, .25f), cabin, true);
        var left = P(PrimitiveType.Cube, "Roof left", new Vector3(-1.57f, 4.32f, .35f), new Vector3(4.05f, .34f, 5.45f), roof, cabin); left.transform.rotation = Quaternion.Euler(0, 0, -32);
        var right = P(PrimitiveType.Cube, "Roof right", new Vector3(1.57f, 4.32f, .35f), new Vector3(4.05f, .34f, 5.45f), roof, cabin); right.transform.rotation = Quaternion.Euler(0, 0, 32);
        P(PrimitiveType.Cube, "Roof ridge", new Vector3(0, 5.14f, .35f), new Vector3(.28f, .22f, 5.35f), darkWood, cabin);
        P(PrimitiveType.Cylinder, "Chimney", new Vector3(2.05f, 5.25f, .95f), new Vector3(.48f, 1.55f, .48f), stone, cabin);
        var porch = Group("Porch", cabin);
        P(PrimitiveType.Cube, "Porch floor", new Vector3(0, .66f, -2.85f), new Vector3(3.2f, .23f, 1.5f), bark, porch);
        P(PrimitiveType.Cube, "Front step", new Vector3(0, .3f, -3.55f), new Vector3(2.35f, .22f, .6f), stone, porch);
        foreach (var x in new[] { -1.28f, 1.28f }) { P(PrimitiveType.Cylinder, "Porch post", new Vector3(x, 1.75f, -3.3f), new Vector3(.16f, 2.2f, .16f), darkWood, porch); Lantern(new Vector3(x, 2.55f, -3.3f), porch); }
    }

    private void Window(Vector3 position, Transform parent, bool side = false)
    {
        P(PrimitiveType.Cube, "Warm window", position, side ? new Vector3(.12f, 1.12f, 1.15f) : new Vector3(1.25f, 1.12f, .12f), window, parent);
        P(PrimitiveType.Cube, "Window mullion", position, side ? new Vector3(.16f, 1.25f, .12f) : new Vector3(.12f, 1.25f, .16f), darkWood, parent);
        P(PrimitiveType.Cube, "Window crossbar", position, side ? new Vector3(.16f, .12f, 1.3f) : new Vector3(1.4f, .12f, .16f), darkWood, parent);
    }

    private void MakeForest()
    {
        var forest = Group("Forest");
        var positions = new[] { new Vector3(-10,0,-4),new Vector3(-8,0,3),new Vector3(-6,0,8),new Vector3(8,0,-4),new Vector3(10,0,3),new Vector3(6.5f,0,8),new Vector3(-11,0,7),new Vector3(12,0,8),new Vector3(-8,0,-9),new Vector3(9,0,-9) };
        for (var i = 0; i < positions.Length; i++) Tree(positions[i], .8f + (i % 4) * .15f, forest);
    }

    private void Tree(Vector3 position, float size, Transform parent)
    {
        var tree = Group("Pine tree", parent);
        P(PrimitiveType.Cylinder, "Trunk", position + Vector3.up * (1.8f * size), new Vector3(.52f * size, 3.6f * size, .52f * size), bark, tree);
        P(PrimitiveType.Sphere, "Lower canopy", position + Vector3.up * (3.4f * size), new Vector3(3.2f * size, 2f * size, 3.2f * size), leaves, tree);
        P(PrimitiveType.Sphere, "Middle canopy", position + Vector3.up * (4.6f * size), new Vector3(2.6f * size, 1.8f * size, 2.6f * size), leaves, tree);
        P(PrimitiveType.Sphere, "Top canopy", position + Vector3.up * (5.7f * size), new Vector3(1.8f * size, 1.5f * size, 1.8f * size), leaves, tree);
    }

    private void MakeGarden()
    {
        var garden = Group("Garden and meadow details");
        for (var x = -12; x <= 12; x += 2) for (var z = -10; z <= 10; z += 2) if (Mathf.Abs(x) >= 5 || Mathf.Abs(z) >= 5) Grass(new Vector3(x + (z % 3) * .22f, 0, z), garden);
        for (var i = 0; i < 24; i++) { var angle = i * 2.4f; var radius = 4.5f + (i % 4) * .65f; Flower(new Vector3(Mathf.Sin(angle) * radius, 0, Mathf.Cos(angle) * radius + .8f), garden); }
        foreach (var p in new[] { new Vector3(-4.4f,0,-1.2f),new Vector3(4.2f,0,1.8f),new Vector3(-3.9f,0,3.3f) }) Bush(p, garden);
        for (var x = -5.5f; x <= -2.5f; x += 1.5f) Fence(new Vector3(x,0,4.3f), garden, false);
        for (var z = 1.3f; z <= 4.3f; z += 1.5f) Fence(new Vector3(-5.5f,0,z), garden, true);
    }

    private void Grass(Vector3 p, Transform parent) { for (var i = 0; i < 3; i++) { var item = P(PrimitiveType.Cube, "Tall grass", p + new Vector3((i - 1) * .12f, .28f, (i % 2) * .1f), new Vector3(.06f, .56f, .34f), grass, parent); item.transform.rotation = Quaternion.Euler(0, i * 55, i == 1 ? 10 : -8); } }
    private void Flower(Vector3 p, Transform parent) { P(PrimitiveType.Cylinder, "Flower stem", p + Vector3.up * .24f, new Vector3(.035f,.48f,.035f), leaves, parent); var b = P(PrimitiveType.Sphere, "Flower bloom", p + Vector3.up * .55f, new Vector3(.24f,.16f,.24f), flower, parent); b.GetComponent<Collider>().enabled = false; }
    private void Bush(Vector3 p, Transform parent) { P(PrimitiveType.Sphere, "Bush", p + Vector3.up * .72f, new Vector3(1.5f,1.35f,1.3f), leaves, parent); P(PrimitiveType.Sphere, "Bush", p + new Vector3(.58f,.63f,.18f), new Vector3(1f,1.1f,1f), leaves, parent); }
    private void Fence(Vector3 p, Transform parent, bool rotate) { var direction = rotate ? Vector3.forward : Vector3.right; P(PrimitiveType.Cylinder,"Fence post",p,new Vector3(.11f,1.1f,.11f),darkWood,parent); P(PrimitiveType.Cylinder,"Fence post",p+direction*1.45f,new Vector3(.11f,1.1f,.11f),darkWood,parent); P(PrimitiveType.Cube,"Fence rail",p+direction*.72f+Vector3.up*.58f,rotate?new Vector3(.12f,.11f,1.55f):new Vector3(1.55f,.11f,.12f),darkWood,parent); }

    private void MakePath()
    {
        var props = Group("Path and props");
        foreach (var p in new[] {new Vector3(0,.06f,-10),new Vector3(-.25f,.06f,-8),new Vector3(.4f,.06f,-6.1f),new Vector3(.1f,.06f,-4.35f),new Vector3(0,.06f,-3.55f)}) P(PrimitiveType.Cylinder,"Stepping stone",p,new Vector3(1.15f,.1f,.78f),path,props);
        foreach (var p in new[] {new Vector3(-4.7f,.35f,-3.2f),new Vector3(4.4f,.28f,3.8f),new Vector3(-6f,.35f,5.5f),new Vector3(5.7f,.23f,-2.8f)}) P(PrimitiveType.Sphere,"Mossy rock",p,new Vector3(1.25f,.65f,.95f),stone,props);
        P(PrimitiveType.Cylinder,"Tree stump",new Vector3(4.3f,.42f,-2.7f),new Vector3(.65f,.84f,.65f),bark,props);
    }

    private void Lantern(Vector3 p, Transform parent) { var housing=P(PrimitiveType.Cube,"Porch lantern",p,new Vector3(.3f,.38f,.3f),darkWood,parent); var light=housing.AddComponent<Light>(); light.type=LightType.Point; light.range=5; light.intensity=1.2f; light.color=new Color(1f,.52f,.16f); }
    private Transform Group(string name, Transform parent = null) { var group=new GameObject(name).transform; group.SetParent(parent ?? root); return group; }
    private GameObject P(PrimitiveType type, string name, Vector3 position, Vector3 scale, Material material, Transform parent) { var item=GameObject.CreatePrimitive(type); item.name=name; item.transform.SetParent(parent); item.transform.position=position; item.transform.localScale=scale; item.GetComponent<Renderer>().sharedMaterial=material; return item; }
}
