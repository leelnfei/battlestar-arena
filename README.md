# Welcome to Battlestar Arena!

A four person 2D arena shooter. Last players standing wins!

This project is licensed under the [GNU General Public License](https://github.com/kikiriki-studios-canada/battlestar-arena/blob/master/LICENSE).

Before contributing to this project, please review the [Code of Conduct](https://github.com/kikiriki-studios-canada/battlestar-arena/blob/master/CODE_OF_CONDUCT.md), and [Contributing Guidelines](https://github.com/kikiriki-studios-canada/battlestar-arena/blob/master/CONTRIBUTING.md).

Development of this project is maintained on a Trello board you can find [here](https://trello.com/b/ut7AXLvF).

**This project is developed using Unity version 2018.2.18f1**, *please* use the same version.

# Battlestar Arena Game Outline.

An outline of what is to be expected from the final result of the game.

## Mechanics.

### Player Movement.
Each player should be able to move in all directions on a 2D plane, with the option to use an additional movement booster once per a given amount of seconds, which will shoot the player slightly further in the direction they are already moving.

### Projectile Shooting.
Each player should be able to shoot one projectile at a time. The projectile automatically moves itself forward but its angle can be controlled. Once the fired projectile is destroyed or hits a target, the player must wait a given amount of seconds before being allowed to fire again. Projectiles can explode on contact with another player, the player who shot the projectile, as well as another projectile. Projectiles also naturally explode after a given amount of time.

### Map.
The map features four barriers enclosing it into a rectangular (or square) shape. The player is unable to cross these barriers (but is not damaged by it). Projectiles, however, are able to cross these barriers.

### Game Rules.
*Classic mode:* Four players spawn in four corners of the map. Once a projectile makes contact with a player, that player is eliminated from the game and left to spectate the remaining portion of the match (unless they choose to leave). The last player standing wins the match.

*Competitive mode:* Four players spawn in four corners of the map. Each player is awarded five life points. Once a projectile makes contact with a player, that player is deducted one life point. If a player loses all their life points, they are eliminated from the game and are left to spectate the remaining portion of the match (unless they choose to leave). The last player standing wins. After a given amount of time, the match enters sudden death mode, in which each player (regardless of their previous life points), is left with one life point. Movement speeds are reduced and projectile speeds are increased.

*Battle Royale mode:* Two to ten players are spawned across the map. Once a projectile makes contact with a player, that player is eliminated from the game and left to spectate the remaining portion of the match (unless they choose to leave). The last player standing wins the match. As the duration of the match goes on, the boundaries of the map shrink. If a player is near the boundary as it shrinks, they will be pushed by it.

*Other possible game modes:* Teams, in which two teams of two, three, or four players face off against each other. Rules mimic Classic mode however “friendly fire” is disabled.

## Game Art.

### Players.
Four different player designs, each of a different colour. If the teams mode were to be implemented, several player models of two opposite colours would need to be designed as well. Each player model needs to be animated to move in all directions. Each player also featured a jet pack on their back that can be used as a movement ability. In this case, a particle effect of flames must be animated from the back of the jetpack. Each player model features a missile launcher that they use to fire projectiles. When firing a projectile, the rocket launcher must feature a particle effect  animated to expel flames and smoke from the barrel of the launcher.

### Projectiles.
A moderately large rocket, coloured accordingly to the player model it was shot from. As each projectile moves through space, it leaves behind a particle effect trail animated as flames and smoke. Once a projectile makes contact with a player, or another projectile, it will “explode” into a particle effect animation of flames, and of course, smoke. The player is then covered by the effect and removed from play.

### Map.
The map background features a very dark and gloomy purple colour, with parallax stars layers above it, as well as the occasional animated shooting star. The barriers of the map feature four satellites in each corner of the map emitting animated lasers to each other, enclosing the map in a rectangular (or square) shape.

# Disclaimer

Outline is subject to be changed or updated. Most recent change or update was on 05/03/19.
