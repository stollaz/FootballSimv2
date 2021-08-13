# FootballSimv2
Terminal based Football Simulator

---

## Introduction
This basic Football Simulator aims to be a simple yet somewhat realistic simulator of association football matches. There aims to be two major aspects to the program:

1. Simulation of games involving randomly generated players, teams and stats
2. Simulation of games involving real life players, teams and stats

The idea started based on the idea of aspect 1. The early program would (and still can) generate a random team name using a location prefix 
(town name, selected from a file containing every town in the UK), and a standard football team name suffix (selected from a similar file 
of such suffixes, e.g. City, Town, F.C., etc.). 

There are two more aspects which each of the prior can be split into:

1. Simulation of a single game, stepping through the game in fixed-time intervals (currently and be default 3 minutes), wherein during each interval, a random attacking 
action is performed by a player of one team (A), and a player on the other team (B) attempt to defend against. These actions may be attempting a shot on goal, attempting 
to pass the ball, or attempting to dribble the ball. The player's generated stats are used in conjunction with random sampling on a sigmoid function, where identical statistical
values imply a 50% chance of either player winning the situation.
2. Simulation of an entire season, simulating quickly each game in the way outlined above, before displaying a final league table.

## Real Data
Data about real players is obtained by webscraping fbref.com, a football statistic site. These statistics are then interpreted into a number of attributes assigned to a player, 
and these attributes dictate how a player performs. [...]

---

## Current Features (v. b.2.0)
- Ability to generate two completely randomly generated teams
  - Within these teams, various statistical information about the players can be visualised
  - Teams can be saved to a file for use later
  - Games can be simulated between the two teams, either in step-by-step mode, or skip-to-result mode
    - After the game, user has the option to display goalscorers, display game stats (posession etc.), display player ratings, or simulate again
- Ability to load two saved teams for use as above
- Ability to simulate an entirely random season of a Premier League style football league
  - 20 randomly generated teams player each other twice each
  - Once all games are simulated, a league table is shown with all the usual league table statistics found on a basic football league
- Ability to simulate an entire season of the 2020/21 Premier League, using real teams, players, and player stats
  - As above, a league table is shown after simulation
  - Player data is obtained from all players of all Premier League teams from the 2020/21 season, provided the player played enough minutes to have details stats
  - Player data is converted to attributes the simulator can use via the use of arbitrary algorithms
  - A "Best XI" is selected for each team based on certain criteria to predict which players would be the best in the respective positions
  
  
## Intended Future Features
- General Simulation
  - More realistic use of attributes to perform actions, e.g. to reduce the number of goals scored overall
    - Note: The distribution of points and goals is much more realistic in a completely random season versus using real data
    - This suggests a problem with the using of the data / its conversion to stats, rather than a simulation problem, but both should be investigated
  - Customisable time interval in step-by-step match mode
  - Ability to use a loaded team versus a random team
  - Easier to view stats, and more of them
  - Use player positions more effectively and realistically
- Season Simulation
  - More detailed statistics, such as top scorers, most clean sheets, etc.
  - Detailed individual statistics for all players, e.g. goals scored, pass percentage, number of cards, etc.
- Use of real data
  - Better conversion from web scraped stats to in game attributes
  - Better calculation of "Best XI"
  - Rotatable team game-by-game, based on RNG
    - In future can be based on form, to figure out the XI most capable of winning games
  - Better positions
  - Implementation of a "Manager Mode": [*]
    - Manual customisation of the game XI for each fixture
    - Move step-by-step through each game in the season
    - Transfer of players between teams [**]
    - View more detailed statistics of players in the team
    
    
 [*] - Ambitious
 [**] - Very Ambitious
 
 ---
 
 
## Changelog v. b.2.2021.8.13.0
- Improved algorithm to generate a "Best XI"
  - Now only players in the correct positions will be selected for those positions
    - This means no more RBs playing at CB, etc.
  - In the case of a team having no players for a certain position, a more generic position will be looked for instead
    - e.g. If a team has no RBs, a player marked as DF (generic defender) may be selected instead
    - If no suitable replacement is found, a placeholder empty player is returned, however this never happens in testing
- Slightly altered some of the simulation algorithms to correctly use new stats
  
### Next Steps
  
  - Be less strict on formation - currently a formation of 4-3-3 with a midfield of CM-AM-CM is forced  
    - Allow the midfield 3 to have more configuration, such as DM-AM-DM, DM-CM-DM, etc.
      - This could be based on available players, or simply on which configuration has the highest rating
      - Alternatively instead of basing on preset configurations, for each position the game could check all possible midfield positions
        - The best 3 midfield players would be selected (excluding wingers, as they fit in as forwards)
    - As another step, allow alternate formations (e.g. 4-4-2, 4-2-3-1, 5-2-3, 3-4-3 etc.)
  - Further improve conversion from data to in game statistics, as well as simulation parameters
  - Allow viewing of lineups after season simulation
  - Implement advanced statistics for teams and players
