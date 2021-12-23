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

## Initial Features (v. a.2.0)
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
  - Simulate games based on passages of play rather than "an action every 3 minutes"
    - Use some algorithm based on team stats etc to determine the likelihood of a team attempting an action (slow build up, counter attack, set peice, etc.) and how long to wait
      - Could use these team stats to assign values to teams e.g. aggression etc. 
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
- Conversion of the game to use a UI, or to integrate with Unity to create a better interface [*]
    
 [*] - Ambitious
 [**] - Very Ambitious
 
 ---
 
 
## Changelog v. a.2.2021.8.13.0
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

---

## Changelog v. a.2.2021.8.17.0
- Began to implement a testing function `TestCombinations()` to generate a more random match schedule
  - Previously in season simulation, each match would be simulated in order (e.g. Team0 vs Team1, Team0 vs Team2,... etc.)
  - This has no effect on the outcome of the season, however in the future I plan to enable the ability to slowly step through the season, seeing each game week one by one, and delving into each game if desired
  - Currently, the algorithm theoretically works, however the way I store the matchups needs to be changed - perhaps just store a tuple of teams in each game week?

### Next Steps (in addition to previous updates):
- Integrate the above into the existing `SimulateRealSeason()` function to verify there is no difference to the results
  - Use the `TestCombinations()` function to return a `List<GameWeek>` - so a list of all 38 game weeks, each containing the matchups for that week
  - Within the `GameWeek` class, change the way games are stored to be a tuple / array of either `RotatableTeam` / `Team`, or simply team ID numbers (0 through 19) which can be used to generalise 
  - Within each game week there also may need to be a list of `TeamGameStats` tuples for each game, so the outcome of each game can be investigated and the stats can be seen
  - Prior to simulating a game week, the player will need to select any fixtures they wish to step through step by step
    - Otherwise, all games will be simulated to the end, and the user will not be able to do this after seeing the result, as the game must be simulated again and may be different 
- Perhaps try and scrape data from other tiers of English football and simulate seasons with relegation and promotion? Just for fun

---

## Changelog v. a.2.2021.8.24.0
- Added extra cases in `GenerateBestXI()` to reduce the likeihood that teams can generate with missing players
  - Added a case for player numbers 6 and 8 (Central Midfielders) so that in the case that no players with the position 'CM' are found, the program looks for AM or DM players instead
  - As of now, all teams have 11 players, and this change brought West Ham from relegation candidates to mid table finishers again
    - This was because West Ham in particular had no 'CM' players, so was playing every game with 9 men
- Added a large `// TODO` block in `GenerateRealPlayer()` to outline next steps in reducing the number of goals scored

### Further Next Steps
- Add a bias to stat generation using data, to ensure that players stats make sense
  - Currently, precentiles are used to calculate stats with no regard to their context - FBREF calculates percentiles against players in the same position, rather than against all players
  - As a result, an attacking in the 20th percentile of goals scored actually likely is better at finishing than a defnsive player with the same percentile in the stat, as they are being compared to different (and in the attacker's case, better) player groups
  - One solution to this is to either add a bias to improve stats related to the players position, or to try and use a function to constrain stats within bounds (e.g. ensure goalkeepers always have a relatively high Goal Prevention stat)
    - As noted in the `// TODO` block, an example of this is Aaron Ramsdale (GK, Sheffield United) having a 38 Goal Prevention stat, leading to a player with a Finishing stat from almost always scoring against them with a shot
    - This is unrealistic, as even subpar keepers will not concede every shot to decent players, and arises from the fact that even though Aaron Ramsdale is a relatively poor goalkeeper, his Goal Prevention stat of 38 is not representative as all goalkeepers should by nature be better goal preventers than most other players
  - The other solution is to alter the goal chance algorithm to reduce dependence on the similarity of values, however as noted before, seasons simulated with completely random values tend to have quite a realistic number of goals scored, leading to the idea that the problem is likely in the stats rather than the algorithms

---

## Changelog v. a.2.2021.10.26.0
- Added a new mode, where a single game can be played using existing Premier League teams
  - User selects both teams to play, then is taken to the same menu used for simulating random games
- Fixed a bug where teams (e.g. Aston Villa) would still not populate all players slots
  - This is done by, if a player slot cannot be filled, simply picking the best player in that position class that hasn't already been picked
  - I thought this bug was fixed, and this solution is more of a fallback than a real fix, so should be re-evaluated

---

## Changelog v. a.2.2021.12.18.2
- Updated CSV database for 2021/22 rosters
  - All top 5 leagues are now included (Premier League, La Liga, Bundesliga, Serie A, Ligue 1)
    - Where possible, teams down to League 2 in England are populated, but in almost all cases there is not enough data to generate a CSV
  - When simulating an entire season or a game from a season, the player can pick which league to use
- Fixed a bug where players would not be put in the right position due to their position not being read in correctly
- Fixed a bug where the program would crash if there were not enough of a certain position in the team
  - This is solved by actually utilising the "position2" attribute of players, and using it if the list of a position is too small

### Further Next Steps
- Champions League simulation, using best teams of the top 5 leagues and simulating the tournament with correct rules
- Actually implement squad rotation
- Utilise a "position3"
- Improve positioning of players (e.g. for Man Utd, a CM is in the AM spot and an AM in the CM spot)
- Prioritise certain positions first
  - E.g. for PSG, Mbappe is picked as a winger since it is chosen first, leaving striker to be filled by a youth member
  - To do this, perhaps instead of filling by position, find the best players and put them in their best positions first

---

## Changelog v. a.2.2021.12.21.0
- Added mode to step through season week by week to see results
- Updated algorithm for generating fixtures, so that they appear random 

### Further Next Steps
- Allow ability to see game stats from any game during a week
- Allow ability to print current league table at any stage
- Allow ability to skip to the end / a specific week at any stage
- **[Hard]** Allow ability to replay any game stage by stage and see actions
  - This would require storing a log of every event printed to the console during a game
  - Would also require storing when the user needs to press enter to continue the game

---

# Alpha-2

## Changelog v. a.2.2021.12.22.0
- Added ability to replay matches from seasons action-by-action, and view game stats from those games
  - New game object is used for these games, which contains an event log of everything printed to the console
  - It also contains stats for each of the teams to be shown
  - These match objects can then be re-shown if the user wishes, allowing them to replay games

### Further Steps
- Store goalscorers, ratings and other match information in this game object, which should also be displayable
- Allow user to replay multiple games from a gameweek, or even games from earlier weeks eventually
  - To do multiple in a game week, loop asking until the user does not enter a match number
  - This will require re-printing the match list after recreating a game, so need to make sure this is done separately from the totalling of wins etc.
- More use of clearing the console when necessary with `Console.Clear()`
- Use player ratings to show team of the season / best rated player, which should be relatively easy once the storing of such ratings is done

## Changelog v. a.2.2021.12.23.0
- Added extra stats to view once a replayed game in a season is viewed
  - These include the options available when simulating a single game, such as viewing ratings etc.
- Added display of Team of the Season once a season is complete
  - The top rated players in a 4-3-3 formation are printed, as well as the overall highest rated player
  - This could be improved by fiddling with the position requirements, as sometimes a player like Salah will end up shown as a midfielder 
