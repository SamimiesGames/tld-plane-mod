# PLANE MOD FOR THE LONG DARK
![banner didn't load properly](https://github.com/SamimiesGames/tld-plane-mod/blob/master/Assets/Plane_V1_IG.png?raw=true)
### Fly around great bear island and move bases fast 

## HOW THE MOD WORKS
When hit the edge of the map the aircraft and all 
its passengers will travel to the region that would be the way you travel.


#### WHERE TO FIND PLANES
- Planes can be found in a couple places on great bear.
    * Forsaken Airfield (WHEN NOT DOWNGRADED TO z_perilousconstraint)
    * Pleasant Valley

#### HOW TO FLY
- use `planemod_toggle_controller_facing` command to toggle the flight controller of the plane the player is looking at.
- W and S to control the throttle
- A and D to roll
- Mouse Y to control pitch
- Mouse X to control yaw

(Player movement is disabled during flight)

#### HOW THE PLANES FLY
- Engine Speed (RPM) controls the engine power of the aircraft
- The engine power is used to add speed to the aircraft
- If the climb angle is too high the plane stalls
- If the fuel level dips below 25% your engine starts to slowdown
- If the altitude is more than the max altitude of the plane your engine slows down


## ROADMAP

The roadmap is a collection of high priority project goals in a roughly chronological order. Please note that the roadmap may change as priorities shift.

- (FUNCTIONAL) Load&Save System 
  - Models loaded and automatically rigged across all applicable regions across multiple saves
- (CURRENT) Flight Controller System
  - Allows the user to control the aircraft
- Fuel System
  - Fuel Consumption
  - Fuel Refilling
- Cargo System
  - Allows the carrying of cargo
- Plane Crash System
  - Cargo Items Destroyed
  - Crash Afflictions
- Automatic Region Transitions
  - When flown to the edge of the map the player and passengers will be transitioned to the relevant region.
- Repair System
  - Your plane takes damage over time.
  - Battery changes
- Crash Site System
  - Multiple Models for completely destroyed planes
  - Crash Sites
- Scavenging System 
  - Allows the harvesting of crash sites.

#### Console Commands
`planemod_save` saves the data of all planes

`planemod_load` loads the data of all planes

`spawn_plane` spawns a new plane towards the player

`planemod_devmode` saves data to the main data file rather than a file dedicated to the current save

`planemod_force_update_model_streaming` forces model streaming to happen

`planemod_delete_all` deletes all planes

`planemod_delete_local` deletes all planes in the current region

`planemod_delete_recent` deletes the plane that was recently saved

`planemod_delete_facing` deletes the plane the player is facing

`planemod_toggle_controller` enables the plane controller of the plane that was recently saved

`planemod_toggle_controller_facing` enables the plane controller for the plane the player is facing