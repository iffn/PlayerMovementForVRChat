# Player movement for VRChat
Combine various forms of movement in VRChat  
Video: https://www.youtube.com/watch?v=3kpFXVHB1sI  
Test it here: https://vrchat.com/home/world/wrld_b9ab04fd-f8b2-45c5-847f-2a7acfca3a1f  
Warning: VRChat has limited ways of implementing things that often work in obscure ways. Therefore, test things first and be prepared to potentially spend months working on weird workarounds that might work until the next update.

## Requirements
- NUMovementPlatformSyncModForVRChat  
https://github.com/iffn/NUMovementPlatformSyncModForVRChat
- NUMovement by Nestorboy  
https://github.com/Nestorboy/NUMovement
- CyanPlayerObjectPool by CyanLaser  
https://github.com/CyanLaser/CyanPlayerObjectPool
- SwimSystem 2.0 by Hirabiki  
https://booth.pm/ja/items/2127684

## Currently implemented
- Walking, running, turning, jumping: NUMovmenet
- Sliding down steep slopes: NUMovement
- Teleport: NUMovement
- Launch pad: NUMovement
- Teleport throwing: Custom script based on NUMovement
- Trampoline: Custom script based on NUMovement
- Wall and multi jumping: Custom script based on NUMovement
- Climbing: Custom script, linked to NUMovement
- Walking up steep slopes when correct material assigned: NUMovementPlatformSyncModForVRChat
- Platform with sync: NUMovementPlatformSyncModForVRChat
- Swimming, diving: Modified SwimSystem by Hirabiki

## Additional notes:
- Blender files saved with the latest non-beta version on Steam. Linking Unity to the newest Blender version should load models correctly.
