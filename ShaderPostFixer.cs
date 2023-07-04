namespace TLD_PlaneMod;

public class ShaderPostFixer
{
    public Shader shader;
    public GameObject gameObject;
    
    public ShaderPostFixer(GameObject aGameObject, Shader aShader)
    {
        gameObject = aGameObject;
        shader = aShader;
        
        PlaneModLogger.MsgVerbose($"[ShaderPostFixer] gameObject={gameObject.name}, shader={shader.name}");
        
        FixShader(gameObject);
    }
    private void FixShader(GameObject subGameObject)
    {
        if (!subGameObject) return;
        int i = 0;

        Renderer renderer = subGameObject.GetComponent<Renderer>();
        if (renderer)
        {
            foreach (var lMaterial in renderer.materials)
            {
                lMaterial.shader = shader;
            }
        }
            
        while ( i < subGameObject.transform.childCount)
        {
            FixShader(subGameObject.transform.GetChild(i).gameObject);
            i++;
        }
    }
}