
------------------------------------------------------------------
# Bloodrush Devlog
------------------------------------------------------------------

------------------------------------------------------------------
## Guide
------------------------------------------------------------------
- Record and remove things in "Needed Additions (unordered)" and "Notable Bugs" as needed.
- "Daily Logging" is more of a diary structure where you just add and dont subtract from it.
    - If you have an ID3A, have the log entry be **ID3A** (but E for 3 (this is to not get confused while Ctr+F)) and the idea "name" be *like this*.
    - Once an idea has been implemented change **ID3A** to **DON3**. If we decided to pass on it can say **NOP3**.
- Try to be organized, but this isn't the largest scale project so it doesnt matter THAT much.
- Give vars and functions good names.

------------------------------------------------------------------
## Needed Additions (unordered)
------------------------------------------------------------------
### Enemies
- EnemyData (very similar mechanically to the TowerData)
- EnemyAnimator
- Enemy classes and behaviors (unsure on which pattern to implement)
- Enemy art. Cooper import it pls.
- Healthbars
- EnemyFactory
### Towers
- Tower Upgrade Paths
- Tower range display
- More tower types
- Have tower/projectile art be stored in TowerData
- Use template pattern for aiming strategies (all except spin strategy)
#### Projectiles
- ProjectileData
- Have different projectiles for each tower (and maybe tier (easy))
- ProjectileFactory
### UI
- Score, TowerPurchase, Pause/Play/SendWave, Currency indicators and buttons.
- Tower info and upgrade options on clicking on it.
- More obvious tower placement system.
### Map(s)
- More maps (different exits and entrances and multiple paths? (would be hella easy))
- Obstacles and backgrounds for the maps. (uneeded but wanted)
- Better tile art. Since this isnt procedurally generated, we can just make map art not tiled. Cooper go get the border art from ElevatOR Asphyxiator.
### Design
- Wave
- Map
- Enemy
- Tower
- Art
- Game
    - Difficulty levels
    - Balancing tower and upgrade costs (to some degree)
### Roadmap (Low Priority)
- Title screen and menu (itd look cool af)
### School Requirements
- At least 5 patterns
    - USING Strategy
    - USING Singleton
    - USING Observer
    - WILL USE Composite (Tower, TowerUpgrade)
    - WILL USE Facade (GameManager)
    - WILL USE Template (TowerAimingStrategy)
- Do we need tests? Coverage? Theres no way right? As many tests involve passing active Unity GameObjects through them and modifying them at runtime which wouldn't really work in the Scene view or Game view. hmm.

------------------------------------------------------------------
## Notable Bugs
------------------------------------------------------------------
### Enemies
- Might be slight issue with A* where I make enemies skip the first node to avoid irregular movements on tower placement, but this might cause enemies to move at angles opposed to cardinal directions. I think it's fine but will need to playtest to confirm.
    - Might want to have them keep the first node to fix this. But only the first time RegeneratePath runs.

------------------------------------------------------------------
## Daily Logging
------------------------------------------------------------------

### 4-7-26 TUE
- **IDEA** Need to add *bullet spread*, that is such an interesting mechanic and will help with making the towers feel the right amount of wonkyness.
- Def made this devlog too late, Bloodrush is still in its infancy but should've better documented what has been done.
    - Only implemented patterns so far: strategy (a lot), singleton, and whatever ObjectPooler falls under. 
    - **IDEA**  Thinking we should *skip decorator* as its just kind of an ugly strategy IMHO. We should sub it for the composite strategy as far as tower upgrades go, where each tower (even base) is an upgrade tier which has additive statistics and mechanics it builds from the root with. And each has multiple leaf/branches within it (the upgrade paths for towers).
- What do you think about the internal class within Tower? I like to organize things that way due to cleaner hierarchy but it can be weird FS.
