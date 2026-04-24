# 🎰 Unity 2D Slot Game Assessment

🎮 **[Click Here to Play the Live WebGL Build!](https://[your-username].github.io/[your-repo-name]/)**

## 📄 Overview
This project is a fully functional 2D slot machine developed in Unity. It was designed from the ground up with a focus on clean software architecture, mathematical fairness, and satisfying game feel. Rather than relying on rigid visual checks, the game operates on a robust, event-driven backend utilizing Finite State Machines (FSM) and a strictly decoupled UI.

> **Note on Submission Structure:** The original assignment instructions requested the WebGL build be placed in a `/Builds/WebGL` folder. However, to provide a seamless, playable evaluation experience directly in the browser, the build has been routed to the `/docs` directory to comply with GitHub Pages hosting requirements.

---

## 🧠 Architectural Approach

### 1. Finite State Machine (FSM)
The core game loop (`Idle`, `Spinning`, `Evaluating`, `Payout`) is managed by a central `GameManager`. This ensures strict flow control, preventing edge-case bugs like double-betting, UI spam, or math desyncs during reel animations.

### 2. Event-Driven Decoupling
To maintain modularity, the core logic does not hold references to UI, Audio, or visual effects. Instead, it broadcasts C# `System.Action` events (e.g., `OnSpinStarted`, `OnWinProcessed`). Managers (like the `UIManager` and `AudioManager`) listen for these broadcasts and react accordingly. 

### 3. Data-Driven Design
Symbols and payouts are not hardcoded. They utilize Unity `ScriptableObjects` (`SymbolData`). This allows for frictionless scaling—adding a new symbol or adjusting a payout multiplier requires zero code changes.

---

## ✨ Features & Mechanics

### 🎲 Core Functionality
* **Weighted PRNG (Pseudorandom Number Generator):** The outcome of every spin is determined mathematically the moment the "Spin" button is clicked. It uses a weighted table, allowing precise control over symbol probability (e.g., Jackpots are mathematically rarer than low-tier wins).
* **Distance-Based Reel Snapping:** The visual reels do not rely on timers or physics colliders to evaluate a win. The spinning UI nodes wrap infinitely until they receive their pre-calculated target from the RNG engine, at which point they calculate the exact distance needed to snap perfectly onto the payline.
* **Dynamic Betting System:** Players can adjust their bet amounts securely. The system validates the bet against the active balance before initiating the state change.

### 🌟 Bonus Features & Polish ("Juice")
* **Session Telemetry (Stats Tracker):** A background `StatsManager` silently tracks Total Wagered, Total Won, and Net Profit across the active session, simulating live-ops data tracking.
* **Dynamic Audio System:** An event-listening `AudioManager` handles mechanical spinning loops, heavy "thuds" for individual reel stops, and dynamic UI feedback (including "Insufficient Funds" buzzers).
* **Rolling Number UI:** Balance deductions and payouts do not snap instantly. The UI mathematically lerps the numbers to give a satisfying "rolling odometer" effect.
* **Floating Combat-Style Text:** Contextual pop-ups (e.g., a green `+500` or red `-100`) instantiate over the UI to provide immediate visual feedback on transactions.
* **Automated Failsafes:** Features clean Pop-Up UI for "Insufficient Funds" and a hard-stop "Bankrupt / Game Over" state requiring a restart.

---

## 📁 Project Structure

The project is strictly organized to maintain readability and asset separation:

```text
/Assets
 ├── /Animations   # Lever pull animations and animation controllers
 ├── /Prefabs      # Reusable UI elements (SlotNodes, Floating Text)
 ├── /Scripts      # Core logic, segregated by domain (Managers, UI, Reels)
 ├── /Audio       # Mechanical SFX and UI audio clips
 ├── /UI/Sprites      # 2D source textures and sliced UI components
 └── /ScriptableObjects # Data containers for Symbol logic and weights
