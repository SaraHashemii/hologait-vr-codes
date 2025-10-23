# Documentation 


 ## Contents
 
  [FPSCounterUI Documentation](#fpscounterui-documentation)  
  [ObstacleChallenge Documentation](#obstaclechallenge-documentation)  
  [BoundaryClamp Documentation](#boundaryclamp-documentation)  
  [ObstacleSpawner Documentation](#obstaclespawner-documentation)  
  [AudioManager Documentation](#audiomanager-documentation)  
  [UIManager Documentation](#uimanager-documentation)  
  [GameSettings Documentation](#gamesettings-documentation)  
  [FreezeStateTrigger Documentation](#freezestatetrigger-documentation)  
  [TheObstacle Documentation](#theobstacle-documentation)

---

### FPSCounterUI Documentation

**Purpose**

The FPSCounterUI script displays the frames per second (FPS) in a Unity VR game, providing real-time performance feedback, which is critical for ensuring a smooth VR experience for Parkinson's patients.

**Functionality**

- **Start():** Initializes timeLeft to updateInterval and logs a warning if fpsTextUI is not assigned.

- **Update():**
  
   - Tracks elapsed time (Time.unscaledDeltaTime) and frame count.
 
   - Every updateInterval seconds, calculates FPS as frames / accumulatedTime.
 
   - Updates fpsTextUI with the FPS value (formatted to one decimal place).
 
   - Resets counters (timeLeft, accumulatedTime, frames) for the next interval.
 
**Role in Game**

Ensures the game runs smoothly by displaying FPS, helping developers and players monitor performance, which is crucial for VR to avoid discomfort for Parkinson’s patients.

---

### ObstacleChallenge Documentation

**Purpose**

The ObstacleChallenge script, attached to obstacle GameObjects (pillars or cubes), handles player interactions in the VR game, detecting collisions and triggers to manage obstacle completion for Parkinson's patients navigating the corridor.

**Key Components**

- **Methods:**
  
   - OnCollisionEnter(Collision c): Detects collisions with the player (tagged "Player") and logs a hit message.
   
   - OnTriggerEnter(Collider other): Detects when the player enters the obstacle’s trigger zone, signaling completion to the ObstacleSpawner.
 
**Functionality**

- **Collision Detection:** Logs a debug message when the player physically hits the obstacle, indicating a failure to avoid it.
  
- **Trigger Detection:** When the player enters the trigger zone, it calls spawner.OnPillarComplete(true), notifying the spawner to replace the obstacle with a dummy and spawn the next one.

**Role in Game**

Manages player progression by detecting successful navigation (trigger entry) or failure (collision), enabling the game to advance to the next obstacle, critical for encouraging movement in Parkinson’s patients.

---

### BoundaryClamp Documentation

**Purpose**

The BoundaryClamp script prevents the player from passing through walls in the VR game, ensuring safe navigation for Parkinson’s patients by constraining their movement within the corridor.

**Key Components**
- **Methods:**
  
  -  LateUpdate(): Checks for overlaps and adjusts the player’s position.
 
 **Functionality**
- **LateUpdate():**
  - Uses Physics.OverlapSphere to detect colliders within radius of the player’s head position on wallLayers.
    
  - For each overlapping collider, finds the closest point to the head.
    
  - Calculates penetration depth (radius - distance to closest point).
    
  - If penetration > 0, moves the entire rig (transform.position) outward by the penetration distance along the normalized direction from the closest point to the head.
 
**Role in Game**

Ensures players remain within the corridor boundaries, preventing them from clipping through walls, which maintains a safe and immersive experience for Parkinson’s patients.

---

### ObstacleSpawner Documentation

**Purpose**

The ObstacleSpawner script manages the spawning and relocation of obstacles (pillars or cubes) in the VR game.

**Key Components**
- **Methods:**
  - Awake(): Instantiates pillar and cube prefabs, positioning them off-screen.
   
  - Start(): Initializes obstacles, sets scales from GameSettings, and starts with a random obstacle.
   
  - OnPillarComplete(bool): Handles obstacle completion, spawns a dummy, and relocates the next obstacle.
   
  - RelocateObstacle(): Positions the next obstacle ahead of the player, checks endpoint proximity, and updates scales.
 
**Functionality**
- **Initialization:** Creates pillar and cube instances, ensures no parent transforms, and links to ObstacleChallenge.
  
- **Obstacle Management:** Spawns obstacles at PlayerHead.position + spawnForward, scales them based on GameSettings, and deactivates previous obstacles, replacing them with dummies.
  
- **Endpoint Check:** Stops spawning if the obstacle is within 5 units of EndPointZ, logging "ReachedTheEnd".
  
- **Randomization:** Randomly selects between pillar and cube for each new obstacle.


**Role in Game**

Drives the core gameplay loop by spawning obstacles for the player to navigate, ensuring continuous challenges until the endpoint is reached, tailored to encourage movement for Parkinson’s patients.

---

### AudioManager Documentation

**Purpose**

The AudioManager script manages audio in the VR game, providing music, sound effects (SFX), and spatial audio cues to guide the users.

**Key Components**
- **Methods:**
  - Awake(): Initializes singleton, audio sources, and harmonic melodies.
    
  - PlayMusic(AudioClip), PlaySFX(AudioClip), PlaySpatial(AudioClip, Vector3): Play music, non-spatial, and spatial SFX.
    
  - PlayHarmonicMelodies(), PlaySpatialHarmonicMelodies(Vector3): Play sequential notes from harmonic sequences.
    
  - StopAllSounds(), StopAllSFX(), StopAllSpatial(): Stop various audio types.
    
  - UpdateVolumes(): Adjusts volume across all sources.
 
**Functionality**
- **Initialization:** Sets up a singleton, creates audio source pools (10 non-spatial, 5 spatial), and defines three harmonic chord sequences (C major, F major, etc.).
  
- **Audio Playback:** Plays music, SFX, or spatial audio with volume control, reusing or creating AudioSources as needed.
  
- **Harmonic Melodies:** Cycles through note sequences for pathway cues, supporting both non-spatial and spatial playback at specific positions.
  
- **Settings:** Allows muting and volume adjustment via PlayerPrefs, applied to all sources.

**Role in Game**

Provides immersive audio feedback, particularly spatial harmonic melodies for pathway objects, guiding Parkinson’s patients during freezing episodes with directional audio cues.

---

### UIManager Documentation

**Purpose**

The UIManager script controls the user interface in the VR game, managing menu navigation and settings adjustments for obstacle distances and audio, tailored for Parkinson’s patients.

**Key Components**
- **Methods:**
  - Awake(): Initializes UI by showing only optionsPanel.
   
  - OnStartButtonClicked(): Loads the CorridorScene to start the game.
  
  - OnOptionClicked(), ShowPillarSettings(), ShowObstacleSettings(), ShowSoundSettings(): Navigate between UI panels.
   
  - OnPillarDistanceChanged(Slider), OnPillarDiameterChanged(Slider), OnPathwayDistanceChanged(Slider): Update GameSettings with slider values.
   
  - ToggleMute(bool), GetSliderValueNormalized(Slider): Control audio settings via AudioManager.
 
**Functionality**
- **Menu Navigation:** Switches between the main menu and options, with subpanels for pillar, obstacle, and sound settings.

- **Settings Adjustment:** Updates GameSettings (PillarDistance, PillarDiameter, PathwayDistance) and AudioManager (volume, mute) based on slider inputs, displaying values in real-time.

- **Panel Management:** Ensures only one panel is active at a time using ShowOnly().

**Role in Game**

Provides an intuitive interface for players to start the game and customize obstacle spacing and audio, enhancing accessibility for Parkinson’s patients.

---

### GameSettings Documentation

**Purpose**

The GameSettings script stores global game settings for obstacle parameters, accessible across scenes, supporting customization for the users.

**Key Components**
- **Public Variables:**
  - PillarDistance: Distance between obstacles (default 3f).
   
  - PathwayDistance: Distance between pathway objects in centimeters (default 65f).
   
  - PillarDiameter: Obstacle diameter (default 1f).
   
- **Static Property:**
  
  - Instance: Singleton instance for global access.
 
**Functionality**
- **Awake():** Implements singleton pattern, ensuring one instance persists across scenes using DontDestroyOnLoad. Destroys duplicates.
  
- **Properties:** Provide getter/setter access to PillarDistance, PathwayDistance, and PillarDiameter, used by ObstacleSpawner and UIManager.

**Role in Game**

Centralizes configuration for obstacle placement and size, allowing UIManager to adjust settings and ObstacleSpawner to apply them, tailoring the game to patient needs.

---

### FreezeStateTrigger Documentation

**Purpose**

The FreezeStateTrigger script detects input from Parkinson’s patients to trigger pathway generation during freezing episodes, aiding navigation in the VR game.

**Key Components**
- **Methods:**
  - OnEnable(), OnDisable(): Enable/disable the input action.
   
  - Start(): Initializes ThePillarRef and logs warnings if references are missing.
   
  - ThePillarRef is a reference to the currently active obstacle in the game.
  
  - Update(): Checks for input (VR button or editor key P) to trigger pathway creation.
 
**Functionality**
- **Input Detection:** Listens for VR input or editor key press (P) to detect when the player requests a pathway.
  
- **Pathway Trigger:** CallsTheObstacle.CurrentActiveInstance.CreatePathWay(HeadOfPlayerTransform) to generate a guiding pathway from the player’s head position.

**Role in Game**

Enables players to request navigational aid during freezing episodes, triggering TheObstacle to create a pathway, critical for supporting Parkinson’s patients.

---

### TheObstacle Documentation

**Purpose**

The TheObstacle script, attached to obstacle GameObjects, generates visual and audio-guided pathways to help Parkinson’s patients navigate around obstacles during freezing episodes in the VR game. The objects have a pool of pathways in themselves and reuse the same objects each time. 

**Key Components**
- **Methods:**
  - Awake(): Initializes audio source and persists the object.
    
  - Start(): Creates a pathway object pool and initializes distances from GameSettings.
    
  - CreatePathWay(Transform): Generates a pathway from the player’s head to the obstacle.
    
  - GenerateStraightPathway(), GenerateCurvedPathway(): Create straight and curved pathway segments.
    
  - StartActivatingPathways(), ActivatePathwaysCoroutine(): Sequentially activate pathways with audio.
    
  - HideAllPathways(): Deactivates all pathway objects.
    
  - CalculateQuadraticBezierPoint(): Computes Bezier curve points for curved pathways.
 
**Note:** It's not exactly a Bezier curve, as the distance between the pathways remains the same on the Z axis. Thus, consistency remains the same across all paths, both in the curve and in the straight line.

**Functionality**
- **Pathway Generation:** Casts rays to detect walls (BoundryLeft, BoundryRight), determines the farther wall, and generates a pathway:
  
  - If the player is ≥3.5 units from the obstacle, creates a straight segment followed by a curved segment.
   
  - Otherwise, generates only a curved segment using a quadratic Bezier curve.
   
  - The curve is from the side of the farthest wall. [| ^ Path | ]

- **Pathway Activation:** Activates pathway objects sequentially with spatial audio cues (AudioManager.PlaySpatialHarmonicMelodies) at 550ms intervals.
  
- **Grounding:** Uses GetGroundPosition to ensure pathway objects are placed on the ground via raycasting.
  
- **Scaling:** Converts GameSettings.PathwayDistance from centimeters to Unity units.

**Role in Game**

Creates navigational pathways with visual and audio cues to guide Parkinson’s patients around obstacles, activated during freezing episodes, enhancing mobility and safety.
