using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace TLD_PlaneMod
{
    public static class PlaneModSettings
    {
        public static string BASE_DATAPATH = "Mods\\planemod_data.json";
        public static string NEWSAVE_FORMATTABLE_DATAPATH = "Mods\\planemod_data{0}.json";
        
        public static string ASSETBUNDLE_PATH = "Mods\\assetsforplanemod_assets_all.bundle";
        public static string PLANEMOD_BUNDLENAME = "PLANEMOD";
        
        public static string ASSET_DEFINITION_PATH = "Mods\\planemod_assetdefinitions.json";
    }
    public class PlaneMod : MelonMod
    {
        public bool firstFrame;
        
        public override void OnInitializeMelon()
        {
            PlaneModLogger.Msg("Loading...");
            new UnityBundleManager();
            new PlaneModManager();

            uConsole.RegisterCommand("spawn_plane", new Action(SpawnPlane));
            
            uConsole.RegisterCommand("planemod_save", new Action(SavePlaneMod));
            uConsole.RegisterCommand("planemod_load", new Action(LoadPlaneMod));
            
            uConsole.RegisterCommand("planemod_devmode", new Action(DevMode));
            
            uConsole.RegisterCommand("planemod_force_update_model_streaming", new Action(ForceUpdateModelStreaming));
            
            uConsole.RegisterCommand("planemod_delete_all", new Action(DeleteAll));
            uConsole.RegisterCommand("planemod_delete_local", new Action(DeleteAllInRegion));
            uConsole.RegisterCommand("planemod_delete_recent", new Action(DeletePlane));
            uConsole.RegisterCommand("planemod_delete_facing", new Action(DeletePlaneFacing));
            
            uConsole.RegisterCommand("planemod_toggle_controller", new Action(ToggleAircraftController));
            uConsole.RegisterCommand("planemod_toggle_controller_facing", new Action(ToggleAircraftControllerFacing));
            
            PlaneModLogger.Msg("Loaded");
        }

        public RaycastHit RaycastFacing(float maxDistance)
        {
            RaycastHit hit;
            Physics.Raycast(
                GameManager.m_MainCamera.transform.position,
                GameManager.m_MainCamera.transform.forward,
                hitInfo: out hit,
                maxDistance,
                layerMask: Utils.m_WeaponProjectileCollisionLayerMask
            );

            return hit;
        }
 
        private void SpawnPlane()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(
                GameManager.m_MainCamera.transform.position,
                GameManager.m_MainCamera.transform.forward,
                hitInfo: out hit,
                1000,
                layerMask: Utils.m_WeaponProjectileCollisionLayerMask
            );
            Vector3 position = hasHit ? hit.point : GameManager.m_MainCamera.transform.position + GameManager.m_MainCamera.transform.forward * 1000;
            position -= GameManager.m_MainCamera.transform.forward;

            Quaternion rotation = Quaternion.Euler(0, GameManager.m_MainCamera.transform.eulerAngles.y, 0);
            
            GameObject plane = PlaneModAssetManager.Singleton.SpawnPlane("CropDuster01", position, rotation);
            plane.transform.Translate(0, 3.56f, 0);

            float distance = Mathf.RoundToInt(Vector3.Distance(position, GameManager.m_MainCamera.transform.position));
            
            PlaneModLogger.MsgHUD($"CropDuster01 spawned {distance}M away!");
        }

        private void DeleteAll()
        {
            int i = 0;
            PlaneModDataManager.Singleton.UpdateAircraftData();
            
            foreach (var data in PlaneModDataManager.Singleton.planeModData.aircraftData)
            {
                string guid = data.guid;
                
                Aircraft aircraft = AircraftManager.Singleton.GetAircraftByGUID(guid);
                if (aircraft != null)
                {
                    AircraftManager.Singleton.RemoveAircraft(aircraft);
                }
                
                PlaneModDataManager.Singleton.DeleteDataInstance(guid);
                i++;
            }

            if (i > 0)
            {
                PlaneModLogger.Msg($"[DeleteAll] Deleted {i} planes");
                ForceUpdateModelStreaming();
                PlaneModLogger.MsgHUD($"Deleted {i} planes!");
            }
        }
        
        private void DeleteAllInRegion()
        {
            int i = 0;
            PlaneModDataManager.Singleton.UpdateAircraftData();
            
            foreach (var aircraft in AircraftManager.Singleton.aircrafts)
            {
                AircraftManager.Singleton.RemoveAircraft(aircraft, true);
                i++;
            }

            if (i > 0)
            {
                PlaneModLogger.Msg($"[DeleteAllInRegion] Deleted {i} planes");
                ForceUpdateModelStreaming();
                PlaneModLogger.MsgHUD($"Deleted {i} planes!");
            }
        }

        private void DeletePlane()
        {
            string guid = PlaneModDataManager.Singleton.StackTopPlaneGUID;

            Aircraft aircraft = AircraftManager.Singleton.GetAircraftByGUID(guid);
            if (aircraft != null)
            {
                AircraftManager.Singleton.RemoveAircraft(aircraft);
            }
            
            PlaneModDataManager.Singleton.DeleteDataInstance(guid);
            ForceUpdateModelStreaming();
            
            PlaneModLogger.MsgHUD($"CropDuster01 {guid} deleted!");
        }

        public Aircraft GetPlaneFacing(float radius, float maxDistance)
        {
            RaycastHit hit = RaycastFacing(maxDistance);

            if (hit.collider == null)
            {
                return null;
            }

            Vector3 playerPosition = GameManager.m_MainCamera.transform.position;
            
            float closestDistance = Mathf.Infinity;
            Aircraft aircraft = null;
            
            foreach (var lAircraft in AircraftManager.Singleton.aircrafts)
            {
                if(lAircraft.planeGameObject == null) continue;

                float distance = Vector3.Distance(playerPosition, lAircraft.planeGameObject.transform.position);

                if (distance < closestDistance)
                {
                    aircraft = lAircraft;
                    closestDistance = distance;
                }
            }

            if (closestDistance > radius) return null;

            return aircraft;
        }
        private void DeletePlaneFacing()
        {
            Aircraft aircraft = GetPlaneFacing(100, 1000);

            if (aircraft == null)
            {
                PlaneModLogger.Msg("[DeletePlaneFacing] No plane nearby.");
                PlaneModLogger.MsgHUD($"No planes nearby.");
                return;
            }
            
            AircraftManager.Singleton.RemoveAircraft(aircraft, true);
            PlaneModLogger.MsgHUD($"{aircraft.guid} deleted");
            ForceUpdateModelStreaming();
        }

        private void ToggleAircraftControllerFacing()
        {
            Aircraft aircraft = GetPlaneFacing(100, 1000);

            if (aircraft == null)
            {
                PlaneModLogger.Msg("[ToggleAircraftControllerFacing] No plane nearby.");
                PlaneModLogger.MsgHUD($"No planes nearby.");
                return;
            }

            ToggleAircraftControllerComponent(aircraft);
        }

        private void ToggleAircraftController()
        {
            if (uConsole.GetNumParameters() != 1)
            {
                uConsole.Log("planemod_toggle_controller takes only one parameter");
                return;
            }
            
            string guid = uConsole.GetString();
            Aircraft aircraft = AircraftManager.Singleton.GetAircraftByGUID(guid);
            
            ToggleAircraftControllerComponent(aircraft);
        }

        private void ToggleAircraftControllerComponent(Aircraft aircraft)
        {
            AircraftController aircraftController = (AircraftController)aircraft.GetComponent("aircraftController");

            if (aircraftController == null)
            {
                PlaneModLogger.Msg($"{aircraft.guid} is not controllable!");
                PlaneModLogger.MsgHUD($"{aircraft.guid} is not controllable!");
                return;
            }
            
            bool active = !aircraftController.enabled;

            aircraft.SetComponentActive("aircraftController", active);
            string activeMessage = active ? "activated" : "disabled";
            
            PlaneModLogger.Msg($"[ToggleAircraftControllerComponent] {aircraft.guid} {activeMessage}");
            PlaneModLogger.MsgHUD($"{aircraft.guid} {activeMessage}!");
        }

        private void SavePlaneMod()
        {
            PlaneModDataManager.Singleton.SaveData();
        }

        private void LoadPlaneMod()
        {
            PlaneModDataManager.Singleton.LoadData();
        }
        
        private void ForceUpdateModelStreaming()
        {
            PlaneModDataManager.Singleton.UpdateModelStreaming(PlaneModDataManager.Singleton.lastSceneGUID, true);
        }
        
        private void DevMode()
        {
            PlaneModDataManager.Singleton.dataManagementMode = 
                PlaneModDataManager.Singleton.dataManagementMode == PlaneModDataManagementMode.Development ?
                    PlaneModDataManagementMode.Normal : 
                    PlaneModDataManagementMode.Development;
            
            PlaneModLogger.Msg($"[DevMode] Set dataManagementMode as {PlaneModDataManager.Singleton.dataManagementMode}");

            string hudMessage =
                PlaneModDataManager.Singleton.dataManagementMode == PlaneModDataManagementMode.Development
                    ? "Development Mode Activated!"
                    : "Development Mode Deactivated!";
             PlaneModLogger.MsgHUD(hudMessage);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!SceneManager.GetActiveScene().isLoaded || !GameManager.m_MainCamera) return;
            PlaneModManager.Singleton.Update();
        }
    }   
}
