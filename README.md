# Mine Awareness & Demining Simulation

**Virtual Reality Simulation for Mine Awareness and the Demining Process.**

<img width="499" height="236" alt="DeminingAcademyLogo" src="https://github.com/user-attachments/assets/1510da40-e670-4ab5-8633-37087b331b23" />

Demining Academy VR aims to bridge the gap between theoretical knowledge and reality, providing a high-stakes environment with zero physical risk. This project was created to transition users from passive observers to active participants, helping them master 1:1 scale ordnance identification and real-world protocols.

---

## 🌍 Context & The Problem
**The Reality:** Ukraine is currently one of the most mine-contaminated countries on Earth, with millions of hectares designated as "red zones" littered with unexploded ordnance (UXO).

**The Challenge:** Traditional training methods (lectures, brochures) lack the physical tension and spatial awareness required to survive in a real minefield.

**The Solution:** SAFESTEP VR provides a safe sandbox to move users from just "knowing" to truly "feeling" the situation. We aim to help form habits where a mistake in the digital world creates a life-saving instinct in the real one.

---

## 🎮 Gameplay & Levels
The simulation features 3 gamified levels designed to progressively train the user:

### Level 0: Tutorial (The Virtual Museum)
A safe environment to inspect **1:1 models of mines** and master the tool belt controls without the risk of detonation.

### Level 1: Surface Threats
Focuses on identifying visible mines. Users learn to:
*   Spot surface-level threats.
*   Perform remote destruction protocols from behind cover.

### Level 2: Sub-Surface Search
Advanced training for hidden threats. Users practice:
*   Finding mines hidden under the soil.
*   Disarming mechanics using the digging tool.

---

## 🛠️ Key Features
*   **Full-Body Character:** Immersive first-person VR presence.
*   **Interactive Tool Belt:** Functional demining tools.
*   **Demining Mechanics:** Realistic digging and disarming interactions.
*   **Spatial Audio:** Background sounds and special effects to heighten tension.
*   **Interactive UI:** Wrist-based or spatial user menus for seamless navigation.

---

## ⚙️ Technical Development
This project was built using **Unity** with the **Universal Render Pipeline (URP)** to ensure performance optimization for VR.

### Key Technical Challenges Solved:
1.  **VR Optimization:**
    *   Implemented specialized Render Pipelines to handle the double-rendering requirement of VR.
    *   Used alternative environment building and procedural grass generation to maintain dense vegetation without heavy performance costs.
2.  **Visual Stability:**
    *   Converted assets to URP to fix "pink shader" glitches.
    *   Manually recalibrated textures for VR compatibility.
3.  **The "Digging" Mechanic:**
    *   Engineered a solution to prevent the shovel from clipping through the terrain.
    *   Balanced movement tracking to ensure the shovel interacts realistically with soil and landmines without poor physics glitches.

---

## 📚 Resources & Credits
**Project Management:** Managed using **ClickUp** (Kanban Board & AI Assistant).

**Assets & Tutorials:**
*   Unity Asset Store
*   Sketchfab
*   Valem Tutorials
*   GameDev Blueprint

**Audio:**
*   Freesound (Background sounds & SFX)

---

## 🚀 Future Roadmap
*   [ ] Add more gamified levels.
*   [ ] Create a full narrative storyline throughout the game.
*   [ ] Implement realistic defusing logic/mini-games.
*   [ ] Add a complete suite of fully interactive professional demining tools.

---

## 📥 Installation & Usage
1.  Clone the repository:
    ```bash
    git clone https://github.com/TheDeiw/Demining-Academy.git
    ```
2.  Open **Unity Hub**.
3.  Add the project folder.
4.  Open the project (Ensure you have the correct Unity VR modules installed).
5.  Connect your VR headset and hit **Play**.
