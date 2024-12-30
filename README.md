

# Steel Ascendant (2.5D Boss Fight)  
**Team PopoConda**  

| **Category**       | **Details**                        |
|---------------------|------------------------------------|
| **Team Members**    | - NahiyanSamit (Team Leader)       |
|                     | - itissaeed                       |
|                     | - Biswa76                         |
| **Mentor**          | - Warhammer4000                   |


## Game Introduction  

### Story  
In a dystopian near-future, the brilliant but deranged scientist Dr. Jarexal Umbravik sought ultimate power. His experiments on human enhancement led to the creation of mechanically modified super soldiers, engineered for brute strength, agility, and resilience. To achieve his ultimate goal of world domination, Dr. Umbravik used his body as an experiment, turning himself into an unstoppable Superhuman.  

You, a highly trained special operative equipped with advanced weaponry and a jetpack, are tasked with infiltrating his fortified laboratory, defeating his creations, and stopping him before it’s too late.  

---

## How to Play  
- Use a combination of firearms, equipment, and jetpack maneuvers to defeat enemies and bosses.  
- Learn boss patterns, exploit their weaknesses, and adapt your strategy to progress.  

---

## Artwork (Design)  

### Technical Requirements  
- **Resolution:** 1920x1080 (HD) minimum, scalable to higher resolutions.  
- **Art Style:** Cyberpunk-inspired, with a mix of 2D and 3D assets for a 2.5D effect.  

### Heads-Up Display (HUD)  
- Health bar and jetpack fuel indicator.  
- Ammo counter for the weapon reload.  
- Enemy health bar.  

---

## Characters  

### Character 1 (Protagonist)  
- **Description:** A skilled operative wearing a combat suit with a jetpack.  
- **Abilities:**  
  - Jetpack flight  
  - Advanced firearms proficiency  
  - Equipment like grenade types  

### Bosses  
1. **Boss 1:**  
   - **Abilities:** Heavy weaponry and laser-type ability.  
   - **Weakness:** Slow movement speed makes it vulnerable to hit-and-run tactics and aerial assaults using the jetpack.  
2. **Boss 2:**  
   - **Abilities:** Sharp shooting and the ability to slow the protagonist's movement for a limited time.  
   - **Weakness:** Can’t shoot continuously for a long period.  
3. **Boss 3:**  
   - **Abilities:** Fires powerful energy blasts and can perform quick dashes to avoid attacks.  
   - **Weakness:** Weak fusion core exposed during its energy recharge phase, making it vulnerable to attacks.  

---

## Level Design  
- **Big Arenas:** Vertical exploration using the jetpack.  
- **Environments:** Laboratories, industrial facilities, and urban ruins.  
- **Unique Settings:** Each boss encounter takes place in a distinct area.  

### Global Elements  
- Neon-lit environments with cyberpunk aesthetics.  
- Powerful bosses with mechanical features.  

---

## Player View  
- Side-scrolling 2.5D view with depth-enhancing 3D backgrounds.  

---

## Game Flow Chart  
- Menu → Boss Fight (Random).  

---

## Audio & Sound Effects  

### Player Elements  
- **Weapon Sounds:** Dynamic audio for firing and reloading.  
- **Jetpack Sounds:** Sound for jetpack when using it.  

### Global Elements  
- **Atmospheric Effects:** Laboratory alarms and background ambiance.  
- **Explosion Effects:** For dramatic encounters.  

---

## Splash Screens  
- Logo animations with ominous, futuristic audio cues.  

---

## Menu Effects  
- Sci-fi-inspired sound effects for navigation and button interactions.  

---

## Technical  

### System Requirements  
- *(Update Later)*  

### Game Architecture  
- Developed in Unity engine for Windows.  
- Modular level design for scalability and easy updates.  

### User Interface (Controls)  
- **Keyboard + Mouse**  
  - **Key Mapping:**  
    - **Movement:** W/A/S/D  
    - **Shoot:** Left mouse  

---

## Marketing  

### Key Features  
- Intense boss fights with unique mechanics and challenges.  
- Jetpack-fueled vertical combat and exploration.  
- Dynamic 2.5D visuals with cyberpunk aesthetics.  

---

## Development  

### Abstract Classes/Components  
- **CharacterController:** Handles movement, combat, and jetpack functionality.  
- **BossAI:** Governs unique attack patterns and behavior for each boss.  
- **EnvironmentManager:** Manages background elements and interactive props.  

### Derived Classes/Component Compositions  
- **ProtagonistController:** Inherits from `CharacterController`, with additional jetpack logic.  
- **Bosses:** Inherits from `BossAI`, with unique mutant-specific abilities.  
- **LabEnvironment:** Inherits from `EnvironmentManager`, with destructible elements.  



