using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace TLD_PlaneMod
{
    public static class PlaneModSettings
    {
        public static string BASE_DATAPATH = "Mods\\planemod_data.json";
        
        public static string ASSETBUNDLE_PATH = "Mods\\assetsforplanemod_assets_all.bundle";
        public static string PLANEMOD_BUNDLENAME = "PLANEMOD";
    }
    public class PlaneMod : MelonMod
    {
        public bool firstFrame;
        public GameObject plane;
        
        public override void OnInitializeMelon()
        {
            Melon<PlaneMod>.Logger.Msg($"OnInitializeMelon");
            new UnityBundleManager();
            new PlaneModManager();
            
            UnityBundleManager.Singleton.LoadAssetBundle(
                PlaneModSettings.PLANEMOD_BUNDLENAME, 
                PlaneModSettings.ASSETBUNDLE_PATH
            );
            
            //Resources.LoadAll("Mods\\AssetsForPlaneMod_unitybuiltinshaders_cd591411e19036d15ea86a2cbc358858.bundle");
        }

        public override void OnUpdate()
        {
            
            base.OnUpdate();
            
            
            if (!SceneManager.GetActiveScene().isLoaded || !GameManager.m_MainCamera) return;

            if (!firstFrame)
            {
                firstFrame = true;
                plane = TryCreateFromBundle("Assets/Prefabs/PLANEMOD_CropDuster01.prefab");
            }

            if (plane)
            {
                plane.transform.Rotate(0, 25 * Time.deltaTime, 0);
            }
            
        }

        public GameObject TryCreateFromBundle(string name)
        {
            GameObject lPrefab = UnityBundleManager
                .Singleton
                .GetPrefabFromAssetBundle(
                    PlaneModSettings.PLANEMOD_BUNDLENAME, name
                );
            
            if (lPrefab == null)
            {
                Melon<PlaneMod>.Logger.Msg($"TryCreateFromBundle couldn't find '{name}'");
                return null;
            }

            Vector3 position = GameManager.m_MainCamera.transform.position +
                               GameManager.m_MainCamera.transform.forward * 25;

            position.y += 6.75f;

            GameObject lGameObject = GameObject.Instantiate(lPrefab, position, Quaternion.identity);
            
            Shader standardShader = Shader.Find("Standard");

            new ShaderPostFixer(lGameObject, standardShader);
            
            Melon<PlaneMod>.Logger.Msg($"TryCreateFromBundle Instantiate 1 times '{lGameObject.name}'");
            
            return lGameObject;
        }
    }   
}
