namespace TLD_PlaneMod;

public class ShaderPostFixer
{
    public Shader shader;
    public GameObject target;
    
    public ShaderPostFixer(GameObject aGameObject, Shader aShader)
    {
        target = aGameObject;
        shader = aShader;
        
        Melon<PlaneMod>.Logger.Msg($"[ShaderPostFixer] gameObject={aGameObject.name}, shader={shader.name}");
        
        FixShader(target);
    }
    private void FixShader(GameObject gameObject)
    {
        if (!gameObject) return;
        int i = 0;

        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer)
        {
            foreach (var lMaterial in renderer.materials)
            {
                lMaterial.shader = shader;
            }
        }
            
        while ( i < gameObject.transform.childCount)
        {
            FixShader(gameObject.transform.GetChild(i).gameObject);
            i++;
        }
    }
}