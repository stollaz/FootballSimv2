using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using RoundRobin;

// https://github.com/stollaz/FootballSimv2/blob/master/README.md

namespace Footballv2
{

    // GK, LB, CB, RB, DM, CM, AM, LM, RM, RW, LW, ST, DF, MF, FW
    enum Position
    {
        GK,
        LB,
        CB,
        RB,
        DM,
        CM,
        AM,
        LM,
        RM,
        RW,
        LW,
        ST,
        DF,
        MF,
        FW

}

    enum ItemType{
        Text,
        Colour,
        Read,
        TextLine
    }

    class Player{
        private string name;
        public string Name{
            get { return name; }
            set { name = value; }
        }

        private int number;
        public int Number{
            get { return number; }
            set { number = Constrain(value); }
        }

        private Position position;
        public Position Position{
            get { return position; }
            set { position = value; }
        }

        private Position position2; // Secondary position for use with new version
        public Position Position2{
            get { return position2; }
            set { position2 = value; }
        }

        private int dribbling; // How likely you are to complete a dribble
        public int Dribbling{
            get { return dribbling; }
            set { dribbling = Constrain(value); }
        }

        private int finishing; // How likely you are to score from a shot
        public int Finishing{
            get { return finishing; }
            set { finishing = Constrain(value); }
        }

        private int tackling; // How likely you are to dispossess an opponent
        public int Tackling{
            get { return tackling; }
            set { tackling = Constrain(value); }
        }

        private int goalPrevention; // How likely you are to prevent a goal
        public int GoalPrevention{
            get { return goalPrevention; }
            set { goalPrevention = Constrain(value); }
        }

        private int passing; // How likely you are to complete a pass
        public int Passing{
            get { return passing; }
            set { passing = Constrain(value); }
        }

        private int assisting; // How likely you are to set up a shot / goal from a pass
        public int Assisting{
            get { return assisting; }
            set { assisting = Constrain(value); }
        }

        private int mentality; // How likely you are to make the correct decision, or to avoid a foul (UNUSED)
        public int Mentality{
            get { return mentality; }
            set { mentality = Constrain(value); }
        }

        private int overall;
        public int Overall{
            get { return overall; }
            set { ; }
        }

        private int positionalOverall;
        public int PositionalOverall{
            get { return positionalOverall; }
            set { ; }
        }

        private PlayerInGame inGameStats;
        public PlayerInGame InGameStats{
            get { return inGameStats; }
            set { inGameStats = value; }
        }

        private int CalculateOverall(){
            return (dribbling + finishing + tackling + passing + goalPrevention + assisting)/6;
        }

        public Position PositionType(Position p){
            switch (p){
                case Position.GK: return Position.GK;
                case Position.DF: return Position.DF;
                case Position.LB: return Position.DF;
                case Position.RB: return Position.DF;
                case Position.CB: return Position.DF;
                case Position.MF: return Position.MF;
                case Position.CM: return Position.MF;
                case Position.LM: return Position.MF;
                case Position.RM: return Position.MF;
                case Position.AM: return Position.MF;
                case Position.DM: return Position.MF;
                case Position.FW: return Position.FW;
                case Position.ST: return Position.FW;
                case Position.LW: return Position.FW;
                case Position.RW: return Position.FW;
                default: return p;
            }
        }

        // These are very crude positional overalls but they should still do a job
        private int CalculatePositionalOverall(){
            if (PositionType(position) == Position.GK){
                return Convert.ToInt32(0.3*goalPrevention + 0.3*tackling + 0.2*passing+0.1*assisting+0.05*(dribbling+finishing));
            }
            else if (PositionType(position) == Position.DF){
                return Convert.ToInt32(0.25*goalPrevention+0.25*tackling+0.2*passing+0.12*(assisting+dribbling)+0.06*finishing);
            }
            else if (PositionType(position) == Position.MF){
                return Convert.ToInt32(0.25*passing+0.25*assisting+0.2*dribbling+0.1*(finishing+goalPrevention+tackling));
            }
            else if (PositionType(position) == Position.FW){
                return Convert.ToInt32(0.5*finishing+0.2*dribbling+0.1*(passing+assisting)+0.05*(goalPrevention+tackling));
            }
            else return Convert.ToInt32((dribbling + finishing + tackling + passing + goalPrevention + assisting)/6);
        }

        public override string ToString(){
            return "[" + number + "][" + position + "] " + name; 
        }

        private ConsoleColor GetColour(int x){
            if (x > 90) return ConsoleColor.DarkGreen;
            else if (x > 75) return ConsoleColor.Green;
            else if (x > 50) return ConsoleColor.Yellow;
            else if (x > 30) return ConsoleColor.Red;
            else return ConsoleColor.DarkRed;
        }

        private void PrintWithColour(int x){
            ConsoleColor c = GetColour(x);
            Console.ForegroundColor = c;
            Console.WriteLine(x);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /*public void DisplayStats(){
            Console.WriteLine("[" + number + "] " + name + ", " + PrintPosition(position));
            Console.WriteLine("Pace: " + pace);
            Console.WriteLine("Finishing: " + finishing);
            Console.WriteLine("Defence: " + defence);
            Console.WriteLine("Control: " + control);
            Console.WriteLine("Mentality: " + mentality);
            Console.WriteLine("OVERALL: " + overall);
            Console.WriteLine("---");
        }*/

        public void DisplayStats(){
            Console.WriteLine("[" + number + "] " + name + ", " + PrintPosition(position));
            Console.Write("DRB: ");
            PrintWithColour(dribbling);
            Console.Write("FIN: ");
            PrintWithColour(finishing);
            Console.Write("TCK: ");
            PrintWithColour(tackling);
            Console.Write("PAS: ");
            PrintWithColour(passing);
            Console.Write("AST: ");
            PrintWithColour(assisting);
            Console.Write("GPV: ");
            PrintWithColour(goalPrevention);
            Console.Write("OVR: ");
            PrintWithColour(overall);
            Console.WriteLine("---");
        }

        public string CompactRepresentation(){
            return name + "," + number + "," + position + "," + dribbling + "," + finishing + "," + tackling + "," + passing + ","  + assisting + "," + goalPrevention;
        }

        public string PrintingRepresentation(){
            return name + "," + number + "," + (int)position + "," + dribbling + "," + finishing + "," + tackling + "," + passing + ","  + assisting + "," + goalPrevention;
        }

        public string PrintPosition(Position p){
            switch (p){
                case Position.GK:
                    return "Goalkeeper";
                case Position.LB:
                    return "Left Back";
                case Position.CB:
                    return "Centre Back";
                case Position.RB:
                    return "Right Back";
                case Position.DM:
                    return "Defensive Midfielder";
                case Position.CM:
                    return "Central Midfielder";
                case Position.AM:
                    return "Attacking Midfielder";
                case Position.LM:
                    return "Left Midfielder";
                case Position.RM:
                    return "Right Midfielder";
                case Position.RW:
                    return "Right Winger";
                case Position.LW:
                    return "Left Winger";
                case Position.ST:
                    return "Striker";
                default:
                    return "None";
            }
        }

        public Player(string _name, int _number, Position _position, int _dribbling, int _finishing, int _tackling, int _passing, int _assisting, int _goalPrevention){
            name = _name;
            number = _number;
            position = _position;
            dribbling = Constrain(_dribbling);
            finishing = Constrain(_finishing);
            tackling = Constrain(_tackling);
            passing = Constrain(_passing);
            assisting = Constrain(_assisting);
            goalPrevention = Constrain(_goalPrevention);
            mentality = 100-tackling;
            overall = CalculateOverall();
            positionalOverall = CalculatePositionalOverall();
        }


        public Player(string _name, int _number, Position _position, Position _position2, int _dribbling, int _finishing, int _tackling, int _passing, int _assisting, int _goalPrevention){
            name = _name;
            number = _number;
            position = _position;
            position2 = _position2;
            dribbling = Constrain(_dribbling);
            finishing = Constrain(_finishing);
            tackling = Constrain(_tackling);
            passing = Constrain(_passing);
            assisting = Constrain(_assisting);
            goalPrevention = Constrain(_goalPrevention);
            mentality = 100-tackling;
            overall = CalculateOverall();
            positionalOverall = CalculatePositionalOverall();
        }
        private int Constrain(int x){
            if (x > 99) return 99;
            else if (x < 0) return 0;
            else return x;
        }
    }

    class PlayerInGame{
        private Player player;
        public Player Player{
            get { return player; }
            set { player = value; }
        }

        private int cardValue;
        public int CardValue{
            get { return cardValue; }
            set { cardValue = value; }
        }

        private bool sentOff;
        public bool SentOff{
            get { return sentOff; }
            set { sentOff = value; }
        }

        private double rating;
        public double Rating{
            get { return rating; }
            set {
                if (value > 10) rating = 10;
                else if (value < 0) rating = 0;
                else rating = value;
            }
        }

        public void GiveCard(bool isYellow = true){
            Console.Write("> ");
            if (isYellow){
                if (cardValue == 1){
                    cardValue++;
                    sentOff = true;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("SECOND YELLOW! ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(player.Name + " has been sent off for a 2nd bookable offense.\n");
                    this.sentOff = true;
                }
                else{
                    cardValue++;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("YELLOW CARD! ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(player.Name + " has been booked.\n");
                }
            }
            else{
                sentOff = true;
                cardValue = 2;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("RED CARD! ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(player.Name + " has been sent off.\n");
                this.sentOff = true;
            }
        }

        public void GiveCard(ref Game game, bool isYellow = true){
            //Console.Write("> ");
            game.AddToLog(new LogItem(ItemType.Text, "> "));
            if (isYellow){
                if (cardValue == 1){
                    cardValue++;
                    sentOff = true;
                    // Console.ForegroundColor = ConsoleColor.Yellow;
                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Red"));
                    // Console.Write("SECOND YELLOW! ");
                    game.AddToLog(new LogItem(ItemType.Text, ("SECOND YELLOW! ")));
                    // Console.ForegroundColor = ConsoleColor.White;
                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                    // Console.Write(player.Name + " has been sent off for a 2nd bookable offense.\n");
                    game.AddToLog(new LogItem(ItemType.Text, (player.Name + " has been sent off for a 2nd bookable offense.\n")));
                    this.sentOff = true;
                }
                else{
                    cardValue++;
                    // Console.ForegroundColor = ConsoleColor.Yellow;
                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Yellow"));
                    // Console.Write("YELLOW CARD! ");
                    game.AddToLog(new LogItem(ItemType.Text, ("YELLOW CARD! ")));
                    // Console.ForegroundColor = ConsoleColor.White;
                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                    // Console.Write(player.Name + " has been booked.\n");
                    game.AddToLog(new LogItem(ItemType.Text, (player.Name + " has been booked.\n")));
                }
            }
            else{
                sentOff = true;
                cardValue = 2;
                // Console.ForegroundColor = ConsoleColor.Red;
                game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Red"));
                // Console.Write("RED CARD! ");
                game.AddToLog(new LogItem(ItemType.Text, ("RED CARD! ")));
                // Console.ForegroundColor = ConsoleColor.White;
                game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                // Console.Write(player.Name + " has been sent off.\n");
                game.AddToLog(new LogItem(ItemType.Text, (player.Name + " has been sent off.\n")));
                this.sentOff = true;
            }
        }

        public PlayerInGame(Player _player){
            player = _player;
            cardValue = 0;
            sentOff = false;
            rating = 6.0;
        }
    }

    class Team{
        private string name;
        public string Name{
            get { return name; }
            set { name = value; }
        }

        private Player[] players = new Player[11];
        public Player[] Players{
            get { return players; }
            set { players = value; }
        }

        private int rating;
        public int Rating{
            get { return rating; }
            set { rating = value; }
        }

        private Player null_player = new Player("null",0, Position.GK, 0,0,0,0,0,0);

        public void AddPlayer(Player p){
            int index = p.Number-1;
            if (players[index] != null_player) Console.WriteLine("Player with this number already exists. Try again.");
            else players[index] = p;
        }

        public void InitialiseTeam(){
            for (int i = 0; i < 11; i++) players[i] = null_player;
        }

        public Team(string _name){
            name = _name;
            InitialiseTeam();
        }

        public void CalculateRating(){
            int ovr = 0;
            foreach (var p in players) ovr += p.Overall;
            ovr /= 11;
            rating = ovr;
        }

        public void SaveTeam(){
            int count = 1;
            string fullPath = "assets/teams/" + name + ".txt";

            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while(File.Exists(newFullPath)) 
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }

            using (StreamWriter writer = new StreamWriter(newFullPath)) 
            {   
                writer.WriteLine(name); 
                foreach (Player p in players){
                    writer.WriteLine(p.PrintingRepresentation());
                }
            }
        }
    }

    // Variation on the Team class to allow player rotation instead of a static XI (TODO, WIP)
    class RotatableTeam{
        private string name;
        public string Name{
            get { return name; }
            set { name = value; }
        }

        // Use a list of players rather than a staticly sized array
        private List<Player> players = new List<Player>();
        public List<Player> Players{
            get { return players; }
            set { players = value; }
        }

        private Player[] bestXI = new Player[11];
        public Player[] BestXI{
            get { return bestXI; }
            set { bestXI = value; }
        }

        private int rating;
        public int Rating{
            get { return rating; }
            set { rating = value; }
        }

        private Player null_player = new Player("null",0, Position.GK, 0,0,0,0,0,0);

        public void AddPlayer(Player p){
            //int index = p.Number-1;
            //if (players[index] != null_player) Console.WriteLine("Player with this number already exists. Try again."); // This line may need to be altered
            //else players[index] = p;
            players.Add(p);
        }

        // This procedure may not be necessary since the players list no longer needs to have a certain number of members in it,
        // and filling it up with lots of null players may break later things - look into this
        public void InitialiseTeam(){
            for (int i = 0; i < 11; i++) players[i] = null_player;
        }

        public RotatableTeam(string _name){
            name = _name;
            //InitialiseTeam();
        }

        public void CalculateRating(){
            int ovr = 0;
            foreach (var p in players) ovr += p.Overall;
            ovr /= 11;
            rating = ovr;
        }

        private Player _GetBestPlayer(Position Pos, List<Player> OrderedList){
            Player ret = new Player("",0,0,0,0,0,0,0,0);
            bool pFound = false;
            int i = 0;
            switch (Pos){
                case Position.LB:
                    while (!pFound && i < OrderedList.Count){
                        Player p2 = OrderedList[i];
                        if (p2.Position == Position.LB || p2.Position2 == Position.LB){
                            pFound = true;
                            ret = OrderedList[i];
                            //ret.Number = 2;
                            //OrderedList[i].Number = 2;   // Left Back
                            //bestXI[1] = OrderedList[i];
                        }
                        else{
                            i++;
                        }
                    }
                    if (!pFound){
                        i = 0;
                        while (!pFound && i < OrderedList.Count){
                            Player p2 = OrderedList[i];
                            if (p2.Position == Position.DF || p2.Position2 == Position.DF){
                                pFound = true;
                                ret = OrderedList[i];
                            }
                            else{
                                i++;
                            }
                        }
                    }
                    break;
                case Position.RB:
                    while (!pFound && i < OrderedList.Count){
                        Player p2 = OrderedList[i];
                        if (p2.Position == Position.RB || p2.Position2 == Position.RB){
                            pFound = true;
                            ret = OrderedList[i];
                        }
                        else{
                            i++;
                        }
                    }
                    if (!pFound){
                        i = 0;
                        while (!pFound && i < OrderedList.Count){
                            Player p2 = OrderedList[i];
                            if (p2.Position == Position.DF || p2.Position2 == Position.DF){
                                pFound = true;
                                ret = OrderedList[i];
                            }
                            else{
                                i++;
                            }
                        }
                    }
                    break;
                case Position.CB:
                    while (!pFound && i < OrderedList.Count){
                        Player p2 = OrderedList[i];
                        if (p2.Position == Position.CB || p2.Position2 == Position.CB){
                            pFound = true;
                            ret = OrderedList[i];
                        }
                        else{
                            i++;
                        }
                    }
                    if (!pFound){
                        i = 0;
                        while (!pFound && i < OrderedList.Count){
                            Player p2 = OrderedList[i];
                            if (p2.Position == Position.DF || p2.Position2 == Position.DF){
                                pFound = true;
                                ret = OrderedList[i];
                            }
                            else{
                                i++;
                            }
                        }
                    }
                    break;
            }

            return ret;
        }

        private Player GetBestPlayer(Position Pos, List<Player> OrderedList){
            Player ret = new Player("",0,0,0,0,0,0,0,0); // Create temporary empty player
            bool pFound = false;
            int i = 0;
            
            while (!pFound && i < OrderedList.Count){   // Search the list for the "best" player that plays the desired position
                Player p = OrderedList[i];  // Assign current player to temporary player obj
                if (p.Position == Pos || p.Position2 == Pos){   // Check if either their primary or secondary position matches
                    pFound = true;  // If so, mark found
                    ret = p; // Store found player in object to be returned
                }
                else{ i++; } // Otherwise check the next player
            }
            if (!pFound){ // If the desired position does not exist in the list, try again but with a general check for DF / MF / FW
                i = 0;
                while (!pFound && i < OrderedList.Count){
                    Player p = OrderedList[i];
                    if (Pos == Position.LB || Pos == Position.RB || Pos == Position.CB){ // Generic check for defenders
                        if (p.Position == Position.DF || p.Position2 == Position.DF){
                        pFound = true;
                        ret = p;
                        }
                        else{ i++; }
                    }
                    else if (Pos == Position.LM || Pos == Position.RM || Pos == Position.CM || Pos == Position.AM || Pos == Position.DM){ // Generic check for midfielders
                        if (p.Position == Position.MF || p.Position2 == Position.MF){
                        pFound = true;
                        ret = p;
                        }
                        else{ i++; }
                    }
                    else if (Pos == Position.ST || Pos == Position.LW || Pos == Position.RW){ // Generic check for forwards
                        if (p.Position == Position.FW || p.Position2 == Position.FW){
                        pFound = true;
                        ret = p;
                        }
                        else{ i++; }
                    }
                }
            }
            if (!pFound){ // If there is still no player found, pick the best player in the list regardless of exact position
                Player p = OrderedList[0];
                pFound = true;
                ret = p;
            }

            return ret;
        }

        // This is a very crude way of generating a "Best XI" and will almost certainly not feature the best possible XI for each team, however it's a start and it can easily be worked on
        // TODO: Base each position on its own set of criteria, e.g.:
        //          - Wingers are the players with the best dribbling and assisting
        //          - Full backs are the players with the best defensive stats + dribbling
        //          - Attacking midfielders are the midfielders with the best finishing and assisting
        //          - Defensive midfielders are the midfielders with the best defensive stats
        public void GenerateBestXI(){
            List<Player> _GKS = new List<Player>();
            List<Player> _DFS = new List<Player>();
            List<Player> _MFS = new List<Player>();
            List<Player> _FWS = new List<Player>();
            foreach (var p in players){
                if (p.PositionType(p.Position) == Position.GK) _GKS.Add(p);
                else if (p.PositionType(p.Position) == Position.DF) _DFS.Add(p);
                else if (p.PositionType(p.Position) == Position.MF) _MFS.Add(p);
                else if (p.PositionType(p.Position) == Position.FW) _FWS.Add(p);
            }
            if (_MFS.Count < 3){
                Console.WriteLine("---");
                foreach (var p in players){
                    //Console.WriteLine("{0}: {1},{2}",p.Name,p.PrintPosition(p.Position), p.PrintPosition(p.Position2));
                    if (p.PositionType(p.Position2) == Position.MF) _MFS.Add(p);
                }
            }
            //Console.WriteLine("{0},{1},{2},{3}", _GKS.Count, _DFS.Count, _MFS.Count, _FWS.Count);

            //List<TeamSeasonStats> SortedList = season.OrderByDescending(s=>s.Points).ThenByDescending(s=>s.GoalDiff).ThenByDescending(s=>s.GoalsFor).ToList();
            List<Player> OrderedGKS = _GKS.OrderByDescending(p=>p.PositionalOverall).ThenByDescending(p=>p.GoalPrevention).ToList();
            List<Player> OrderedDFS = _DFS.OrderByDescending(p=>p.PositionalOverall).ThenByDescending(p=>p.GoalPrevention).ToList();
            List<Player> OrderedMFS = _MFS.OrderByDescending(p=>p.PositionalOverall).ThenByDescending(p=>p.Assisting).ToList();
            List<Player> OrderedFWS = _FWS.OrderByDescending(p=>p.PositionalOverall).ThenByDescending(p=>p.Finishing).ToList();

            OrderedGKS[0].Number = 1;   // Goalkeeper
            bestXI[0] = OrderedGKS[0];

            /*bool pFound = false;
            int i = 0;
            while (pFound || i <= OrderedDFS.Count){
                Player p2 = OrderedDFS[i];
                if (p2.Position == Position.LB || p2.Position2 == Position.LB){
                    pFound = true;
                    OrderedDFS[i].Number = 2;   // Left Back
                    bestXI[1] = OrderedDFS[i];
                }
                else{
                    i++;
                }
            }

            pFound = false;
            i = 0;
            while (pFound || i <= OrderedDFS.Count){
                Player p2 = OrderedDFS[i];
                if (p2.Position == Position.RB || p2.Position2 == Position.RB){
                    pFound = true;
                    OrderedDFS[i].Number = 3;   // Right Back
                    bestXI[2] = OrderedDFS[i];
                }
                else{
                    i++;
                }
            }*/
            
            /*OrderedDFS[2].Number = 4;   // Centre Back
            bestXI[3] = OrderedDFS[2];
            OrderedDFS[3].Number = 5;   // Centre Back
            bestXI[4] = OrderedDFS[3];

            OrderedMFS[0].Number = 6;   // Centre Mid / Defensive Mid
            bestXI[5] = OrderedMFS[0];
            OrderedMFS[1].Number = 8;   // Centre Mid / Defensive Mid
            bestXI[7] = OrderedMFS[1];
            OrderedMFS[2].Number = 10;  // Attacking Mid / Defensive Mid
            bestXI[9] = OrderedMFS[2];

            OrderedFWS[0].Number = 9;   // Striker
            bestXI[8] = OrderedFWS[0];

            if (OrderedFWS.Count < 2){
                OrderedMFS[3].Number = 7;   // Left Winger
                BestXI[6] = OrderedMFS[3];
                OrderedMFS[4].Number = 11;  // Right Winger
                BestXI[10] = OrderedMFS[4];
            }
            else{
                OrderedFWS[1].Number = 7;   // Left Winger
                bestXI[6] = OrderedFWS[1];
                OrderedFWS[2].Number = 11;  // Right Winger
                bestXI[10] = OrderedFWS[2];
            }*/

            // TODO: Maybe remove the found player from the list to avoid duplicates

            bestXI[1] = GetBestPlayer(Position.LB, OrderedDFS); // LB
            bestXI[1].Number = 2;
            OrderedDFS.Remove(bestXI[1]);

            bestXI[2] = GetBestPlayer(Position.RB, OrderedDFS); // RB
            bestXI[2].Number = 3;
            OrderedDFS.Remove(bestXI[2]);

            bestXI[3] = GetBestPlayer(Position.CB, OrderedDFS); // CB
            bestXI[3].Number = 4;
            OrderedDFS.Remove(bestXI[3]);

            bestXI[4] = GetBestPlayer(Position.CB, OrderedDFS); // CB
            bestXI[4].Number = 5;
            OrderedDFS.Remove(bestXI[4]);

            //--

            Player p6 = GetBestPlayer(Position.CM, OrderedMFS); // CM
            if (p6.Name == "") p6 = GetBestPlayer(Position.DM, OrderedMFS); // DM if no CMs
            if (p6.Name == "") p6 = GetBestPlayer(Position.AM, OrderedMFS); // AM if no CMs
            bestXI[5] = p6;
            bestXI[5].Number = 6;
            OrderedMFS.Remove(bestXI[5]);

            //bestXI[7] = GetBestPlayer(Position.CM, OrderedMFS); // CM
            Player p8 = GetBestPlayer(Position.CM, OrderedMFS); // CM
            if (p8.Name == "") p8 = GetBestPlayer(Position.DM, OrderedMFS); // DM if no CMs
            if (p8.Name == "") p8 = GetBestPlayer(Position.AM, OrderedMFS); // AM if no CMs
            bestXI[7] = p8;
            bestXI[7].Number = 8;
            OrderedMFS.Remove(bestXI[7]);

            bestXI[9] = GetBestPlayer(Position.AM, OrderedMFS); // AM
            bestXI[9].Number = 10;
            OrderedMFS.Remove(bestXI[10]);

            bestXI[8] = GetBestPlayer(Position.ST, OrderedFWS); // ST
            bestXI[8].Number = 9;
            OrderedFWS.Remove(bestXI[8]);

            if (OrderedFWS.Count < 2){  // If not enough forwards
                BestXI[6] = GetBestPlayer(Position.LM, OrderedMFS);
                BestXI[6].Number = 7;   // LW
                OrderedMFS.Remove(bestXI[6]);

                BestXI[10] = GetBestPlayer(Position.RM, OrderedMFS);
                BestXI[10].Number = 11;  // RW
                OrderedMFS.Remove(bestXI[10]);
            }
            else{
                BestXI[6] = GetBestPlayer(Position.LW, OrderedFWS);
                BestXI[6].Number = 7;   // LW
                OrderedFWS.Remove(bestXI[6]);

                BestXI[10] = GetBestPlayer(Position.RW, OrderedFWS);
                BestXI[10].Number = 11;  // RW
                OrderedFWS.Remove(bestXI[10]);
            }
        }

        public void SaveTeam(){
            int count = 1;
            string fullPath = "assets/teams/" + name + ".txt";

            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while(File.Exists(newFullPath)) 
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }

            using (StreamWriter writer = new StreamWriter(newFullPath)) 
            {   
                writer.WriteLine(name); 
                foreach (Player p in players){
                    writer.WriteLine(p.PrintingRepresentation());
                }
            }
        }
    }

    class Goal{
        private string name;

        private string team;
        
        private int minute;

        public Goal(string _name, string _team, int _minute){
            name = _name;
            team = _team;
            minute = _minute;
        }

        public void GoalInfo(){
            Console.WriteLine(minute + "' - " + name + " [" + team + "]");
        }
    }

    class TeamSeasonStats{
        private Team team;
        public Team Team{
            get { return team; }
            set { team = value; }
        }
        private int wins;
        public int Wins{
            get { return wins; }
            set { wins = value; }
        }
        private int draws;
        public int Draws{
            get { return draws; }
            set { draws = value; }
        }
        private int losses;
        public int Losses{
            get { return losses; }
            set { losses = value; }
        }

        private int goalsFor;
        public int GoalsFor{
            get { return goalsFor; }
            set { goalsFor = value; }
        }
        private int goalsAgainst;
        public int GoalsAgainst{
            get { return goalsAgainst; }
            set { goalsAgainst = value; }
        }

        private int points;
        public int Points{
            get { return points; }
            set { points = value; }
        }
        private int goalDiff;
        public int GoalDiff{
            get { return goalDiff; }
            set { goalDiff = value; }
        }

        public void calculatePoints(){
            points = wins*3 + draws;
        }

        public void calculateGoalDiff(){
            goalDiff = goalsFor - goalsAgainst;
        }

        public TeamSeasonStats(Team _team){
            team = _team;
        }
    }

    class TeamGameStats{
        private Team team;
        public Team Team{
            get { return team; }
            set { team = value; }
        }

        private int shots;
        public int Shots{
            get { return shots; }
            set { shots = value; }
        }

        private int shotsOnTarget;
        public int ShotsOnTarget{
            get { return shotsOnTarget; }
            set { shotsOnTarget = value; }
        }

        private int goals;
        public int Goals{
            get { return goals; }
            set { goals = value; }
        }

        private int saves;
        public int Saves{
            get { return saves; }
            set { saves = value; }
        }

        private int posession;
        public int Posession{
            get { return posession; }
            set { 
                if (value > 100) posession = 100;
                else if (value < 0) posession = 0;
                else posession = value; 
            }
        }

        private int fouls;
        public int Fouls{
            get { return fouls; }
            set { fouls = value; }
        }

        private int yellowCards;
        public int YellowCards{
            get { return yellowCards; }
            set { yellowCards = value; }
        }

        private int redCards;
        public int RedCards{
            get { return redCards; }
            set { redCards = value; }
        }

        private int passesAttempted;
        public int PassesAttempted{
            get { return passesAttempted; }
            set { passesAttempted = value; }
        }

        private int passes;
        public int Passes{
            get { return passes; }
            set { passes = value; }
        }

        private int interceptions;
        public int Interceptions{
            get { return interceptions; }
            set { interceptions = value; }
        }

        private int tackles;
        public int Tackles{
            get { return tackles; }
            set { tackles = value; }
        }

        private int shotAccuracy;
        public int ShotAccuracy{
            get { return shotAccuracy; }
            private set { shotAccuracy = value; }
        }

        private int passAccuracy;
        public int PassAccuracy{
            get { return passAccuracy; }
            private set { passAccuracy = value; }
        }

        public void calculateShotAccuracy(){
            if (shots == 0) shotAccuracy = 0;
            else shotAccuracy = (shotsOnTarget*100) / shots;
        }

        //public void calculatePassAccuracy(TeamGameStats otherTeam){
        //    passAccuracy = otherTeam.interceptions / passes;
        //}

        public void calculatePassAccuracy(){
            if (passesAttempted == 0) passAccuracy = 0;
            else passAccuracy = (passes*100) / passesAttempted;
        }

        public TeamGameStats(Team _team){
            team = _team;
            shots = 0;
            shotsOnTarget = 0;
            goals = 0;
            saves = 0;
            posession = 50;
            fouls = 0;
            yellowCards = 0;
            redCards = 0;
            passesAttempted = 0;
            passes = 0;
            interceptions = 0;
            tackles = 0;
            shotAccuracy = 0;
            passAccuracy = 0;
        }
    }

    class PlayerSeasonStats{    // Class for storing stats about a player over the course of a season
        private Player player;
        public Player Player{
            get { return player; }
            set { player = value; }
        }

        private RotatableTeam team;
        public RotatableTeam Team{
            get { return team; }
            set { team = value; }
        }

        private int gamesPlayed;
        public int GamesPlayed{
            get { return gamesPlayed; }
            set { gamesPlayed = value; }
        }

        private List<float> ratings; // Stores every rating from every game they have played

        private float avgRating;
        public float AvgRating{
            get { return avgRating; }
            set { avgRating = value; }
        }

        public void AddRating(float r){
            ratings.Add(r);
        }

        public void CalculateAvgRating(){
            avgRating = ratings.Sum()/gamesPlayed;
        }

        private int goalsScored;
        public int GoalsScored{
            get { return goalsScored; }
            set { goalsScored = value; }
        }

        private int assists;
        public int Assists{
            get { return assists; }
            set { assists = value; }
        }

        private int yellowCards;
        public int YellowCards{
            get { return yellowCards; }
            set { yellowCards = value; }
        }

        private int redCards;
        public int RedCards{
            get { return redCards; }
            set { redCards = value; }
        }

        public PlayerSeasonStats(Player p, RotatableTeam t){
            player = p;
            team = t;
            gamesPlayed = 0;
            ratings = new List<float>();
            avgRating = 0;
            goalsScored = 0;
            assists = 0;
            yellowCards = 0;
            redCards = 0;
        }

        public PlayerSeasonStats(Player p, Team t){
            player = p;

            RotatableTeam tx = new RotatableTeam(t.Name);
            tx.BestXI = t.Players;
            tx.Players = t.Players.ToList();
            tx.Rating = t.Rating;
            team = tx;

            gamesPlayed = 0;
            ratings = new List<float>();
            avgRating = 0;
            goalsScored = 0;
            assists = 0;
            yellowCards = 0;
            redCards = 0;
        }
    }

    class SeasonStats{

    }

    class LogItem{
        private ItemType itemType;
        public ItemType ItemType{
            get { return itemType; }
            set { itemType = value; }
        }

        private string itemValue;
        public string ItemValue{
            get { return itemValue; }
            set { itemValue = value; }
        }

        public LogItem(ItemType type, string value){
            itemType = type;
            itemValue = value;
        }

        public LogItem(string type, string value){
            switch (type.ToLower()){
                case "text":
                    itemType = ItemType.Text;
                    break;
                case "string":
                    itemType = ItemType.Text;
                    break;
                case "textline":
                    itemType = ItemType.TextLine;
                    break;
                case "stringline":
                    itemType = ItemType.TextLine;
                    break;
                case "colour":
                    itemType = ItemType.Colour;
                    break;
                case "color":
                    itemType = ItemType.Colour;
                    break;
                case "read":
                    itemType = ItemType.Read;
                    break;
                case "input":
                    itemType = ItemType.Read;
                    break;
                case "enter":
                    itemType = ItemType.Read;
                    break;
                default:
                    itemType = ItemType.Text;
                    break;
            }
            itemValue = value;
        }
    }

    class Game{

        private string id;
        public string Id{
            get { return id; }
            set { id = value; }
        }

        public Team team1;
        private Team Team1{
            get { return team1; }
            set { team1 = value; }
        }

        public Team team2;
        private Team Team2{
            get { return team2; }
            set { team2 = value; }
        }

        // Store stats for team 1
        private TeamGameStats team1Stats;
        public TeamGameStats Team1Stats{
            get { return team1Stats; }
            set { team1Stats = value; }
        }

        // Store stats for team 2
        private TeamGameStats team2Stats;
        public TeamGameStats Team2Stats{
            get { return team2Stats; }
            set { team2Stats = value; }
        }

        public List<PlayerInGame> team1Players;
        private List<PlayerInGame> Team1Players{
            get { return team1Players; }
            set { team1Players = value; }
        }

        public List<PlayerInGame> team2Players;
        private List<PlayerInGame> Team2Players{
            get { return team2Players; }
            set { team2Players = value; }
        }

        private List<int> score;
        public List<int> Score{
            get { return score; }
            set { score = value; }
        }

        // Store event log for match playback
        private List<LogItem> eventLog;
        public List<LogItem> EventLog{
            get { return eventLog; }
            set { eventLog = value; }
        }

        private List<Goal> goals;
        public List<Goal> Goals{
            get { return goals; }
            set { goals = value; }
        }

        // Add string to event log
        public void AddToLog(LogItem s){
            eventLog.Add(s);
        }

        // Add string to event log and print it out
        public void AddAndPrint(LogItem s){
            AddToLog(s);
            if (s.ItemType==ItemType.Text) Console.WriteLine(s.ItemValue);
            //if (s.ItemType==ItemType.Colour) Console.ForegroundColor = s.ItemValue; // TODO
        }

        public Game(string _id){ 
            id = _id; 
            eventLog = new List<LogItem>();
            goals = new List<Goal>();
            team1Players = new List<PlayerInGame>();
            team2Players = new List<PlayerInGame>();
        }
    }

    class GameWeek{
        private List<string> _games; // Store list of games, crudely as a string for now, but should change this eventually, e.g. to a Tuple of RotatableTeam?
        public List<string> _Games{
            get { return _games; }
            set { _games = value; }
        }

        private List<(RotatableTeam, RotatableTeam)> games; // Store list of games, crudely as a string for now, but should change this eventually, e.g. to a Tuple of RotatableTeam?
        public List<(RotatableTeam, RotatableTeam)> Games{
            get { return games; }
            set { games = value; }
        }

        private int weekNum; // Store weeknum as an int, for flavour
        public int WeekNum{
            get { return weekNum; }
            set { weekNum = value; }
        }

        public void AddGame(string s){
            _games.Add(s);
        }

        public void AddGame((RotatableTeam, RotatableTeam) s){
            games.Add(s);
        }

        public void Shuffle()   // Quick solution to shuffle a list to apply an extra layer of visual randomness
        {  
            games = games.OrderBy(x => Guid.NewGuid()).ToList();
        }

        public GameWeek(int n){
            weekNum = n;
            _games = new List<string>();
            games = new List<(RotatableTeam,RotatableTeam)>();
        }
    }

    class Program{

        public static string VERSION = "a.2.2021.12.22.0";    // Format: {alpha}.{alpha-number}.{year}.{month}.{day}.{instance}

        public static List<string> DATA = new List<string> {
            "NON_PEN_GOALS",
            "NON_PEN_XG",
            "TOTAL_SHOTS",
            "ASSISTS",
            "XA",
            "NON_PEN_XG+XA",
            "SHOT_CREATIONS",
            "PASSES_ATTEMPTED",
            "PASS_COMPLETION_PERCENTAGE",
            "PROGRESSIVE_PASSES",
            "PROGRESSIVE CARRIES",
            "DRIBBLES_COMPLETED",
            "TOUCHES_IN_AREA",
            "PROGRESSIVE_PASSES_RECIEVED",
            "PRESSURES",
            "TACKLES",
            "INTERCEPTIONS",
            "BLOCKS",
            "CLEARANCES",
            "AERIAL_DUELS_WON",
            "POST_SHOT_XG-CONCEDED",
            "GOALS_CONCEDED",
            "SAVE_PERCENTAGE",
            "POST_SHOT_XG_PER_SHOT_ON_TARGET",
            "PENALTY_SAVE_PERCENTAGE",
            "CLEAN_SHEET_PERCENTAGE",
            "TOUCHES",
            "LAUNCH_PERCENTAGE",
            "GOAL_KICKS",
            "AVG_GOAL_KICK_DISTANCE",
            "CROSSES_STOPPED_PERCENTAGE",
            "DEFENSIVE_ACTIONS_OUTSIDE_AREA",
            "AVG_DISTANCE_OF_DEFENSIVE_ACTIONS"
        };
        public static List<string> FW_DATA = new List<string> {
            "NON_PEN_GOALS",
            "NON_PEN_XG",
            "TOTAL_SHOTS",
            "ASSISTS",
            "XA",
            "NON_PEN_XG+XA",
            "SHOT_CREATIONS"
        };
        public static List<string> MF_DATA = new List<string> {
            "PASSES_ATTEMPTED",
            "PASS_COMPLETION_PERCENTAGE",
            "PROGRESSIVE_PASSES",
            "PROGRESSIVE CARRIES",
            "DRIBBLES_COMPLETED",
            "TOUCHES_IN_AREA",
            "PROGRESSIVE_PASSES_RECIEVED"
        };
        public static List<string> DF_DATA = new List<string> {
            "PRESSURES",
            "TACKLES",
            "INTERCEPTIONS",
            "BLOCKS",
            "CLEARANCES",
            "AERIAL_DUELS_WON"
        };
        public static List<string> GK_DATA = new List<string> {
            "POST_SHOT_XG-CONCEDED",
            "GOALS_CONCEDED",
            "SAVE_PERCENTAGE",
            "POST_SHOT_XG_PER_SHOT_ON_TARGET",
            "PENALTY_SAVE_PERCENTAGE",
            "CLEAN_SHEET_PERCENTAGE",
            "TOUCHES",
            "LAUNCH_PERCENTAGE",
            "GOAL_KICKS",
            "AVG_GOAL_KICK_DISTANCE",
            "CROSSES_STOPPED_PERCENTAGE",
            "DEFENSIVE_ACTIONS_OUTSIDE_AREA",
            "AVG_DISTANCE_OF_DEFENSIVE_ACTIONS"
        };

        static void Main(string[] args)
        {            
            Console.Clear();
            while (true){
                int option = StartMenu();
                switch(option){
                    case 0:
                        SimulateSeason();
                        break;
                    case 1:
                        SetupTeams();
                        break;
                    case 2:
                        LoadTeams();
                        break;
                    case 3:
                        csv_to_stats();
                        break;
                    case 4:
                        csv_to_stats(true);
                        break;
                    case 5:
                        SimulateRealGame();
                        break;
                    case 6:
                        TestCombinations();
                        break;
                    case 9:
                        System.Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Not coded yet");
                        break;
                }
                Console.WriteLine("\nPRESS ENTER TO CONTINUE");
                Console.ReadLine();
            }
            

            //Console.WriteLine(GenerateNormal(50,25)); // roughly a number centred around 50 that is likely to be between 0 and 100
        }

        static void PrintArray(bool[,] games){
            for (int a = 0; a < 20; a++){
                for (int b = 0; b < 20; b++){
                    if (games[a,b]) {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("T ");
                    }
                    else {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("F ");
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.WriteLine();
            }
        }

        static List<GameWeek> ListMatches(List<string> ListTeam)
        {
            if (ListTeam.Count % 2 != 0)
            {
                ListTeam.Add("Bye");
            }

            int numDays = (20 - 1);
            int halfSize = 20 / 2;

            List<string> teams = new List<string>();

            teams.AddRange(ListTeam.Skip(halfSize).Take(halfSize));
            teams.AddRange(ListTeam.Skip(1).Take(halfSize -1).ToArray().Reverse());

            int teamsSize = teams.Count;

            List<GameWeek> Fixtures = new List<GameWeek>();

            for (int day = 0; day < numDays; day++)
            {
                GameWeek gw = new GameWeek(day+1);

                Console.WriteLine("Day {0}", (day + 1));

                int teamIdx = day % teamsSize;

                Console.WriteLine("{0} vs {1}", teams[teamIdx], ListTeam[0]);
                gw.AddGame(String.Format("{0} vs {1}", teams[teamIdx], ListTeam[0]));

                for (int idx = 1; idx < halfSize; idx++)
                {               
                    int firstTeam = (day + idx) % teamsSize;
                    int secondTeam = (day  + teamsSize - idx) % teamsSize;
                    Console.WriteLine("{0} vs {1}", teams[firstTeam], teams[secondTeam]);
                    gw.AddGame(String.Format("{0} vs {1}", teams[firstTeam], teams[secondTeam]));
                }
                Fixtures.Add(gw);
            }

            return Fixtures;
        }

        static List<GameWeek> ListMatches(List<RotatableTeam> ListTeam)
        {
            int numDays = (20 - 1);
            int halfSize = 20 / 2;

            List<RotatableTeam> teams = new List<RotatableTeam>();

            teams.AddRange(ListTeam.Skip(halfSize).Take(halfSize));
            teams.AddRange(ListTeam.Skip(1).Take(halfSize -1).ToArray().Reverse());

            int teamsSize = teams.Count;

            List<GameWeek> Fixtures = new List<GameWeek>();

            for (int day = 0; day < numDays; day++)
            {
                GameWeek gw = new GameWeek(day+1);

                //Console.WriteLine("Day {0}", (day + 1));

                int teamIdx = day % teamsSize;

                gw.AddGame((teams[teamIdx], ListTeam[0]));

                for (int idx = 1; idx < halfSize; idx++)
                {               
                    int firstTeam = (day + idx) % teamsSize;
                    int secondTeam = (day  + teamsSize - idx) % teamsSize;
                    gw.AddGame((teams[firstTeam], teams[secondTeam]));
                }
                Fixtures.Add(gw);
            }

            return Fixtures;
        }

        static void TestCombinations(){
            List<string> teams = new List<string>() {"0","1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19"}; // List of teams
            List<GameWeek> Fixtures = ListMatches(teams);
            
            foreach (var f in Fixtures){
                Console.WriteLine("Game Week {0}",f.WeekNum);
                foreach (var g in f.Games) Console.WriteLine(g);
            }
        }

        static void SimulateSeason(){
            List<TeamSeasonStats> season = new List<TeamSeasonStats>();
            List<Team> teams = new List<Team>();

            for (int i = 0; i < 20; i++){
                List<Player> players = new List<Player>();

                players.Add(GeneratePlayer(Position.GK,1));
                players.Add(GeneratePlayer(Position.LB,2));
                players.Add(GeneratePlayer(Position.RB,3));
                players.Add(GeneratePlayer(Position.CB,4));
                players.Add(GeneratePlayer(Position.CB,5));
                players.Add(GeneratePlayer(Position.DM,6));
                players.Add(GeneratePlayer(Position.LM,7));
                players.Add(GeneratePlayer(Position.DM,8));
                players.Add(GeneratePlayer(Position.ST,9));
                players.Add(GeneratePlayer(Position.AM,10));
                players.Add(GeneratePlayer(Position.RM,11));

                Team team = new Team(GenerateTeamName());
                foreach (Player p in players) team.AddPlayer(p);
                team.CalculateRating();

                TeamSeasonStats t = new TeamSeasonStats(team);
                teams.Add(team);
                season.Add(t);
            }

            List<PlayerSeasonStats> playerStats = new List<PlayerSeasonStats>();

            foreach (var t in teams){
                Team t_ = new Team(t.Name);
                t_.Players = t.Players;
                t_.CalculateRating();

                TeamSeasonStats t0 = new TeamSeasonStats(t_);
                season.Add(t0);

                foreach (Player p in t_.Players) playerStats.Add(new PlayerSeasonStats(p, t));
            }

            for (int i = 0; i < 20; i++){
                for (int j = 0; j < 20; j++){
                    if (i != j){
                        Game g = SimulateGameInSeason(season[i].Team, season[j].Team, playerStats);
                        List<int> goals = g.Score;
                        if (goals[0] > goals[1]) {season[i].Wins++; season[j].Losses++;}
                        else if (goals[0] < goals[1]) {season[j].Wins++; season[i].Losses++;}
                        else {season[i].Draws++; season[j].Draws++;}

                        foreach (Player p in season[i].Team.Players) playerStats.Find(pl => pl.Player == p).GamesPlayed++;
                        foreach (Player p in season[j].Team.Players) playerStats.Find(pl => pl.Player == p).GamesPlayed++;

                        season[i].GoalsFor += goals[0];
                        season[j].GoalsFor += goals[1];

                        season[i].GoalsAgainst += goals[1];
                        season[j].GoalsAgainst += goals[0];
                    }
                }
            }

            for (int k = 0; k < 20; k++){
                season[k].calculateGoalDiff();
                season[k].calculatePoints();
            }

            ShowTable(season, teams, playerStats);

            /*List<TeamSeasonStats> SortedList = season.OrderByDescending(s=>s.Points).ThenByDescending(s=>s.GoalDiff).ThenByDescending(s=>s.GoalsFor).ToList();

            Console.WriteLine("SEASON TABLE");
            Console.WriteLine("POS TEAM                                  PTS  W   D   L   GF   GA   GD   ");
            for(int l = 0; l < 20; l++){
                TeamSeasonStats t = SortedList[l];
                if (l+1 == 1) Console.ForegroundColor = ConsoleColor.Yellow;
                else if (l+1 == 2 || l+1 == 3 || l+1 == 4) Console.ForegroundColor = ConsoleColor.Green;
                else if (l+1 == 5) Console.ForegroundColor = ConsoleColor.Magenta;
                else if (l+1 == 18 || l+1 == 19 || l+1 == 20) Console.ForegroundColor = ConsoleColor.Red;
                Console.Write((l+1).ToString().PadRight(4,' ')); // Position
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(t.Team.Name.PadRight(38,' ')); // Name
                Console.Write(t.Points.ToString().PadRight(5,' ')); // Points
                Console.Write(t.Wins.ToString().PadRight(4,' ')); // Wins
                Console.Write(t.Draws.ToString().PadRight(4,' ')); // Draws
                Console.Write(t.Losses.ToString().PadRight(4,' ')); // Losses
                Console.Write(t.GoalsFor.ToString().PadRight(5,' ')); // Goals For
                Console.Write(t.GoalsAgainst.ToString().PadRight(5,' ')); // Goals Against
                Console.Write(t.GoalDiff.ToString().PadRight(5,' ')); // Goal Difference
                //t.Team.CalculateRating();
                //Console.Write(t.Team.Rating.ToString().PadRight(5,' ')); // Rating
                Console.Write("\n");
            }*/
        }

        static void csv_to_stats(bool step_by_step = false){
            //PrintOne();
            //PrintAllInLeague();
            List<RotatableTeam> Teams = GenerateLeague();
            SimulateRealSeason(Teams, step_by_step);
        }

        public static List<RotatableTeam> GenerateLeague() {
            string[] regions = {"ENG","ESP","FRA","ITA","GER"};
            Console.Write("Enter region code for league (ENG, ESP, FRA, ITA, GER) > ");
            string r = Console.ReadLine().ToUpper();
            if (!regions.Contains(r)){
                Console.WriteLine("Invalid input. Defaulting to ENG.");
                r = "ENG";
            }

            string root = "csvs/"+r+"1";
            string[] teams = Directory.GetDirectories(root);
            //var directory_in_str = "csvs/Manchester Utd/";
            List<RotatableTeam> TEAMS = new List<RotatableTeam>();
            foreach (string team in teams){
                //Console.WriteLine(team);
                RotatableTeam t = new RotatableTeam(Path.GetFileName(team));
                string[] players = Directory.GetFiles(team);
                foreach (var p in players){
                    Player a = GenerateRealPlayer(p);
                    //Console.WriteLine("----{0}: {1},{2}",a.Name, a.Position, a.Position2);
                    t.AddPlayer(a);
                }
                TEAMS.Add(t);
            }

            foreach (var t in TEAMS){
                /*Console.WriteLine(t.Name);
                foreach (var p in t.Players){
                    Console.WriteLine(p.Name + ": " + p.Overall);
                }*/
                t.GenerateBestXI();
            }
            return TEAMS;
        }

        public static void PrintAllInLeague() {
            string root = "csvs/ENG1";
            string[] teams = Directory.GetDirectories(root);
            //var directory_in_str = "csvs/Manchester Utd/";
            foreach (string team in teams){
                string[] players = Directory.GetFiles(team);
                foreach (var p in players){
                    PrintStats(p);
                }
            }
        }

        static void SimulateRealGame(){
            Console.WriteLine();
            List<RotatableTeam> teams = GenerateLeague();

            RotatableTeam Rteam1;
            RotatableTeam Rteam2;
            int i = 1;
            foreach (var a in teams){
                Console.WriteLine("{0} {1}",i,a.Name); i++;
            }
            Console.Write("\nEnter Team 1: > ");
            bool valid = false;
            int t1=-1;
            while (!valid){
                try{t1 = int.Parse(Console.ReadLine());}
                catch{}
                if (t1 >=1 && t1 <= 20) valid = true;
                else{Console.Write("Invalid Entry. Enter Team 1: > ");}
            }
            
            Rteam1 = teams[t1-1];
            Console.WriteLine("{0} selected.", Rteam1.Name);

            Console.WriteLine();
            i = 1;
            foreach (var a in teams){
                Console.WriteLine("{0} {1}",i,a.Name); i++;
            }
            Console.Write("\nEnter Team 2: > ");
            valid = false;
            int t2=-1;
            while (!valid){
                try{t2 = int.Parse(Console.ReadLine());}
                catch{}
                if (t2 >=1 && t2 <= 20) valid = true;
                else{Console.Write("Invalid Entry. Enter Team 2: > ");}
            }
            Rteam2 = teams[t2-1];
            Console.WriteLine("{0} selected.", Rteam2.Name);

            Team team_1 = new Team(Rteam1.Name);
            team_1.Players = Rteam1.BestXI;
            team_1.CalculateRating();
            Team team_2 = new Team(Rteam2.Name);
            team_2.Players = Rteam2.BestXI;
            team_2.CalculateRating();

            List<Player> team1 = team_1.Players.ToList();
            List<Player> team2 = team_2.Players.ToList();
            
            Console.WriteLine("Team 1: " + team_1.Name);
            Console.Write("TEAM OVERALL: ");
            PrintWithColour(team_1.Rating);
            
            Console.WriteLine("\nTeam 2: " + team_2.Name);
            Console.Write("TEAM OVERALL: ");
            PrintWithColour(team_2.Rating);

            Console.WriteLine("\n");

            int option = Menu();

            while (option != 9){
                if (option == 8){
                    //SaveTeam(team1, "team1");
                    //SaveTeam(team2, "team2");
                    team_1.SaveTeam();
                    team_2.SaveTeam();
                }
                if (option == 9) break;
                else if (option == 10) SimulateGame(team_1, team_2, true);
                else if (option == 11) SimulateGame(team_1, team_2, false);
                else if (option == 12){
                    Console.WriteLine("Rerolling teams is not available in this mode.");
                    Console.WriteLine("To select different teams, go BACK (9).");
                }
                else{
                    Console.WriteLine("\nTeam 1: " + team_1.Name + "\n");
                    PrintTeam(team1, option);
                    Console.Write("\nTEAM OVERALL: ");
                    PrintWithColour(CalculateTeamOverall(team1));

                    Console.WriteLine("\nTeam 2: " + team_2.Name + "\n");
                    PrintTeam(team2, option);
                    Console.Write("\nTEAM OVERALL: ");
                    PrintWithColour(CalculateTeamOverall(team2));
                }
                if (option != 9) {
                    Console.WriteLine("\nPRESS ENTER TO CONTINUE");
                    Console.ReadLine();
                    option = Menu();
                }
            }

            //SimulateGame(T1, T2, true);
        }

        static void RecreateGameFromLog(Game game){
            Console.Clear();
            Console.WriteLine("Options: ");

            int option = 0;

            bool simulated = false;

            while (option != 9 && option != 3){
                option = EndGameMenuInSeason(true);

                switch(option){
                    case 0:
                        Console.Clear();
                        Console.WriteLine("\nGoalscorers:");
                        foreach (var g in game.Goals) g.GoalInfo();
                        Console.WriteLine();
                        Console.WriteLine("==========");
                        Console.WriteLine();
                        break;
                    case 1:
                        Console.Clear();
                        Console.WriteLine();
                        PrintStats(game.Team1Stats, game.Team2Stats);
                        Console.WriteLine();
                        Console.WriteLine("==========");
                        Console.WriteLine();
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine();
                        PrintRatings(game.team1Players, game.team2Players, game.team1, game.team2);
                        Console.WriteLine();
                        Console.WriteLine("==========");
                        Console.WriteLine();
                        break;
                    case 3:
                        simulated = true;
                        Console.Clear();
                        foreach (var item in game.EventLog){
                            switch(item.ItemType){
                                case ItemType.Read:
                                    Console.Read();
                                    break;
                                case ItemType.Colour:
                                    string c = item.ItemValue.Split('.')[1];
                                    Console.ForegroundColor = (ConsoleColor) Enum.Parse(typeof(ConsoleColor), c);
                                    break;
                                case ItemType.Text:
                                    Console.Write(item.ItemValue);
                                    break;
                                case ItemType.TextLine:
                                    Console.WriteLine(item.ItemValue);
                                    break;
                                default:
                                    throw new ArgumentException("Invalid item type.");
                            }
                        }
                        break;
                    case 9:
                        break;
                    default:
                        break;
                }
            }
            Console.Clear();

            Console.WriteLine("\nWould you like to see stats again? ");

            option = 0;

            if (simulated){
                while (option != 9 && option != 3){
                    option = EndGameMenuInSeason(false);

                    switch(option){
                        case 0:
                            Console.WriteLine("\nGoalscorers:");
                            foreach (var g in game.Goals) g.GoalInfo();
                            Console.WriteLine();
                            Console.WriteLine("==========");
                            Console.WriteLine();
                            break;
                        case 1:
                            Console.WriteLine();
                            PrintStats(game.Team1Stats, game.Team2Stats);
                            Console.WriteLine();
                            Console.WriteLine("==========");
                            Console.WriteLine();
                            break;
                        case 2:
                            Console.WriteLine();
                            PrintRatings(game.team1Players, game.team2Players, game.team1, game.team2);
                            Console.WriteLine();
                            Console.WriteLine("==========");
                            Console.WriteLine();
                            break;
                        case 9:
                            break;
                        default:
                            break;
                    }
                }
            }
            Console.Clear();
        }

        static void SimulateRealSeason(List<RotatableTeam> teams, bool step_by_step = false){
            List<TeamSeasonStats> season = new List<TeamSeasonStats>();

            List<PlayerSeasonStats> playerStats = new List<PlayerSeasonStats>();

            foreach (RotatableTeam t in teams){
                Team t_ = new Team(t.Name);
                t_.Players = t.BestXI;
                t_.CalculateRating();

                TeamSeasonStats t0 = new TeamSeasonStats(t_);
                season.Add(t0);

                foreach (Player p in t_.Players) playerStats.Add(new PlayerSeasonStats(p, t));
            }

            List<GameWeek> first19 = ListMatches(teams); // Get list of first 19 game weeks
            first19 = first19.OrderBy(x => Guid.NewGuid()).ToList(); // Shuffle list of weeks
            //foreach (var gameweek in gameweeks) gameweek.Shuffle(); // Shuffle them
            int weeknum = 20; // Initialise week number to continue from here

            // var a = gameweeks.Concat(gameweeks);
            // List<GameWeek> b = a.ToList();

            List<GameWeek> next19 = new List<GameWeek>();
            foreach (var week in first19){ // Generate next 19 game weeks by reversing each fixture
                GameWeek gw = new GameWeek(weeknum); // Create new game week
                foreach (var game in week.Games) gw.AddGame((game.Item2, game.Item1)); // Add reverse of corresponding fixture to list
                weeknum++; // Increment week number
                next19.Add(gw); // Add to list
            }
            next19 = next19.OrderBy(x => Guid.NewGuid()).ToList(); // Shuffle list

            List<GameWeek> gameweeks = first19.Concat(next19).ToList(); // Append next 19 games onto first 19 games

            weeknum = 1;
            foreach (var gw in gameweeks) { // Go through each gameweek...
                gw.Shuffle(); // Shuffle the fixture order
                gw.WeekNum = weeknum; // Update the week number to be correct
                weeknum++;
            }

            List<List<Game>> allGameWeeks = new List<List<Game>>();

            foreach (var week in gameweeks){
                Console.Clear();
                //if (step_by_step) Console.WriteLine("Game Week {0}:",week.WeekNum);
                int i = 0;
                List<Game> gamesInWeek = new List<Game>();
                foreach (var game in week.Games){
                    TeamSeasonStats t1 = season.Single(p => p.Team.Name == game.Item1.Name);
                    TeamSeasonStats t2 = season.Single(p => p.Team.Name == game.Item2.Name);
                    Game g = SimulateGameInSeason(t1.Team, t2.Team, playerStats);
                    gamesInWeek.Add(g);

                    List<int> goals = g.Score;
                    foreach (Player p in t1.Team.Players) playerStats.Find(pl => pl.Player == p).GamesPlayed++;
                    foreach (Player p in t2.Team.Players) playerStats.Find(pl => pl.Player == p).GamesPlayed++;

                    if (goals[0] > goals[1]) {t1.Wins++; t2.Losses++;}
                    else if (goals[0] < goals[1]) {t2.Wins++; t1.Losses++;}
                    else {t1.Draws++; t2.Draws++;}

                    t1.GoalsFor += goals[0];
                    t2.GoalsFor += goals[1];

                    t1.GoalsAgainst += goals[1];
                    t2.GoalsAgainst += goals[0];

                    i++;
                }
                allGameWeeks.Add(gamesInWeek);

                if (step_by_step){
                    int num = -1;
                    while (num != -2){
                        i = 0;
                        Console.WriteLine("Game Week {0}:",week.WeekNum);
                        foreach (Game g in gamesInWeek){
                            List<int> goals = g.Score;
                            ConsoleColor currentForeground = Console.ForegroundColor;
                            Console.Write("Game {0}:        ",i);
                            if (goals[0] > goals[1]) Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(g.team1.Name.PadRight(17,' ')); // Name
                            Console.ForegroundColor = currentForeground;
                            Console.Write(goals[0].ToString().PadRight(2,' '));
                            Console.Write(" - ");
                            Console.Write(goals[1].ToString().PadLeft(2,' '));
                            Console.Write("  ");
                            if (goals[0] < goals[1]) Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(g.team2.Name.PadRight(17,' ')); // Name
                            Console.ForegroundColor = currentForeground;
                            Console.Write("\n");

                            i++;
                        }
                        Console.WriteLine("\nEnter a game number to get stats on that game. Enter '10' to see the current league table.");
                        Console.WriteLine("Otherwise, press Enter to continue...");
                        Console.Write("> ");
                        try { num = int.Parse(Console.ReadLine()); } catch {num = -2;}
                        if (num > -1 && num <= 9){
                            Console.Clear();
                            RecreateGameFromLog(gamesInWeek.ElementAt(num));
                        }
                        else if (num == 10) {
                            ShowTable(season, teams, playerStats);
                            Console.WriteLine("\nPress Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                    }
                }
            }

            ShowTable(season, teams, playerStats);
        }

        static void ShowTable(List<TeamSeasonStats> season, List<RotatableTeam> teams, List<PlayerSeasonStats> playerStats){
            
            for (int k = 0; k < teams.Count; k++){
                season[k].calculateGoalDiff();
                season[k].calculatePoints();
            }

            List<TeamSeasonStats> SortedList = season.OrderByDescending(s=>s.Points).ThenByDescending(s=>s.GoalDiff).ThenByDescending(s=>s.GoalsFor).ToList();

            Console.WriteLine("SEASON TABLE");
            Console.WriteLine("POS TEAM                                  PTS  W   D   L   GF   GA   GD   ");
            for(int l = 0; l < teams.Count; l++){
                TeamSeasonStats t = SortedList[l];
                if (l+1 == 1) Console.ForegroundColor = ConsoleColor.Yellow;
                else if (l+1 == 2 || l+1 == 3 || l+1 == 4) Console.ForegroundColor = ConsoleColor.Green;
                else if (l+1 == 5) Console.ForegroundColor = ConsoleColor.Magenta;
                else if (l+1 > (teams.Count-3)) Console.ForegroundColor = ConsoleColor.Red;
                Console.Write((l+1).ToString().PadRight(4,' ')); // Position
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(t.Team.Name.PadRight(38,' ')); // Name
                Console.Write(t.Points.ToString().PadRight(5,' ')); // Points
                Console.Write(t.Wins.ToString().PadRight(4,' ')); // Wins
                Console.Write(t.Draws.ToString().PadRight(4,' ')); // Draws
                Console.Write(t.Losses.ToString().PadRight(4,' ')); // Losses
                Console.Write(t.GoalsFor.ToString().PadRight(5,' ')); // Goals For
                Console.Write(t.GoalsAgainst.ToString().PadRight(5,' ')); // Goals Against
                Console.Write(t.GoalDiff.ToString().PadRight(5,' ')); // Goal Difference
                //t.Team.CalculateRating();
                //Console.Write(t.Team.Rating.ToString().PadRight(5,' ')); // Rating
                Console.Write("\n");
            }

            Console.WriteLine("PRESS ENTER TO VIEW INDIVIDUAL AWARDS...");
            Console.ReadLine();

            List<PlayerSeasonStats> orderedStatsByGoals = playerStats.OrderByDescending(p=>p.GoalsScored).ToList();
            PlayerSeasonStats topScorer = orderedStatsByGoals[0];
            List<PlayerSeasonStats> orderedStatsByAssists = playerStats.OrderByDescending(p=>p.Assists).ToList(); // This seems redundant since assists aren't done properly enough to really mean anything (some seasons, the top assister has 3)
            PlayerSeasonStats topAssister = orderedStatsByAssists[0];
            Console.WriteLine("\nTOP SCORER: {0} ({1}): {2} goals",topScorer.Player.Name, topScorer.Team.Name, topScorer.GoalsScored);
            Console.WriteLine("TOP ASSISTER: {0} ({1}): {2} assists",topAssister.Player.Name, topAssister.Team.Name, topAssister.Assists);

            foreach (var p in playerStats) p.CalculateAvgRating();
            List<PlayerSeasonStats> orderedStatsByRating = playerStats.OrderByDescending(p=>p.AvgRating).ToList();
            PlayerSeasonStats topRating = orderedStatsByRating[0];
            Console.Write("\nTOP RATED: {0} ({1}): ",topRating.Player.Name, topRating.Team.Name); PrintRatingWithColour(Math.Round(topRating.AvgRating,2));

            int PADDING = 55;

            Console.WriteLine("TEAM OF THE SEASON");
            List<PlayerSeasonStats> TOTS = new List<PlayerSeasonStats>();
            Console.WriteLine("[7]----[9]----[11]\n------------------\n------------------\n--[6]--[10]--[8]--\n------------------\n------------------\n[2]--[4]--[5]--[3]\n-------[1]-------");
            PlayerSeasonStats bestGK = orderedStatsByRating.Find(p => p.Player.Position == Position.GK);
            TOTS.Add(bestGK);
            Console.Write(String.Format("\n[1] [GK] : {0} ({1}): ",bestGK.Player.Name, bestGK.Team.Name).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestGK.AvgRating,2), false);

            PlayerSeasonStats bestLB = orderedStatsByRating.Find(p => (p.Player.Position == Position.LB || p.Player.Position2 == Position.LB) && !TOTS.Contains(p));
            TOTS.Add(bestLB);
            Console.Write(String.Format("\n[2] [{2}] : {0} ({1}): ",bestLB.Player.Name, bestLB.Team.Name, bestLB.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestLB.AvgRating,2), false);

            PlayerSeasonStats bestRB = orderedStatsByRating.Find(p => (p.Player.Position == Position.RB || p.Player.Position2 == Position.RB) && !TOTS.Contains(p));
            TOTS.Add(bestRB);
            Console.Write(String.Format("\n[3] [{2}] : {0} ({1}): ",bestRB.Player.Name, bestRB.Team.Name, bestRB.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestRB.AvgRating,2), false);

            PlayerSeasonStats bestCB1 = orderedStatsByRating.Find(p => (p.Player.Position == Position.CB || p.Player.Position2 == Position.CB || p.Player.Position == Position.DF || p.Player.Position2 == Position.CB) && !TOTS.Contains(p));
            TOTS.Add(bestCB1);
            Console.Write(String.Format("\n[4] [{2}] : {0} ({1}): ",bestCB1.Player.Name, bestCB1.Team.Name, bestCB1.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestCB1.AvgRating,2), false);

            PlayerSeasonStats bestCB2 = orderedStatsByRating.Find(p => p != bestCB1 && (p.Player.Position == Position.CB || p.Player.Position2 == Position.CB || p.Player.Position == Position.DF || p.Player.Position2 == Position.CB) && !TOTS.Contains(p));
            TOTS.Add(bestCB2);
            Console.Write(String.Format("\n[5] [{2}] : {0} ({1}): ",bestCB2.Player.Name, bestCB2.Team.Name, bestCB2.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestCB2.AvgRating,2), false);

            PlayerSeasonStats bestMF1 = orderedStatsByRating.Find(p => (p.Player.Position == Position.MF || p.Player.Position2 == Position.MF || p.Player.Position == Position.DM || p.Player.Position2 == Position.DM
                || p.Player.Position == Position.AM || p.Player.Position2 == Position.AM
                || p.Player.Position == Position.CM || p.Player.Position2 == Position.CM) && !TOTS.Contains(p));
            TOTS.Add(bestMF1);
            Console.Write(String.Format("\n[6] [{2}] : {0} ({1}): ",bestMF1.Player.Name, bestMF1.Team.Name, bestMF1.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestMF1.AvgRating,2), false);

            PlayerSeasonStats bestMF2 = orderedStatsByRating.Find(p => (p.Player.Position == Position.MF || p.Player.Position2 == Position.MF || p.Player.Position == Position.DM || p.Player.Position2 == Position.DM
                || p.Player.Position == Position.AM || p.Player.Position2 == Position.AM
                || p.Player.Position == Position.CM || p.Player.Position2 == Position.CM) && !TOTS.Contains(p));
            TOTS.Add(bestMF2);
            Console.Write(String.Format("\n[10][{2}] : {0} ({1}): ",bestMF2.Player.Name, bestMF2.Team.Name, bestMF2.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestMF2.AvgRating,2), false);

            PlayerSeasonStats bestMF3 = orderedStatsByRating.Find(p => (p.Player.Position == Position.MF || p.Player.Position2 == Position.MF || p.Player.Position == Position.DM || p.Player.Position2 == Position.DM
                || p.Player.Position == Position.AM || p.Player.Position2 == Position.AM
                || p.Player.Position == Position.CM || p.Player.Position2 == Position.CM) && !TOTS.Contains(p));
            TOTS.Add(bestMF3);
            Console.Write(String.Format("\n[8] [{2}] : {0} ({1}): ",bestMF3.Player.Name, bestMF3.Team.Name, bestMF3.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestMF3.AvgRating,2), false);

            PlayerSeasonStats bestST = orderedStatsByRating.Find(p => (p.Player.Position == Position.ST || p.Player.Position2 == Position.ST || p.Player.Position == Position.FW || p.Player.Position2 == Position.FW) && !TOTS.Contains(p));
            TOTS.Add(bestST);

            PlayerSeasonStats bestLW = orderedStatsByRating.Find(p => (p.Player.Position == Position.LW || p.Player.Position2 == Position.LW || p.Player.Position == Position.LM || p.Player.Position2 == Position.LM
            || p.Player.Position == Position.FW || p.Player.Position2 == Position.FW) && !TOTS.Contains(p));
            TOTS.Add(bestLW);
            Console.Write(String.Format("\n[7] [{2}] : {0} ({1}): ",bestLW.Player.Name, bestLW.Team.Name, bestLW.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestLW.AvgRating,2), false);

            Console.Write(String.Format("\n[9] [{2}] : {0} ({1}): ",bestST.Player.Name, bestST.Team.Name, bestST.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestST.AvgRating,2), false);

            PlayerSeasonStats bestRW = orderedStatsByRating.Find(p => (p.Player.Position == Position.RW || p.Player.Position2 == Position.RW || p.Player.Position == Position.RM || p.Player.Position2 == Position.RM
            || p.Player.Position == Position.FW || p.Player.Position2 == Position.FW) && !TOTS.Contains(p));
            TOTS.Add(bestRW);
            Console.Write(String.Format("\n[11][{2}] : {0} ({1}): ",bestRW.Player.Name, bestRW.Team.Name, bestRW.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestRW.AvgRating,2), false);
        }

        static void ShowTable(List<TeamSeasonStats> season, List<Team> teams, List<PlayerSeasonStats> playerStats){
            
            for (int k = 0; k < teams.Count; k++){
                season[k].calculateGoalDiff();
                season[k].calculatePoints();
            }

            List<TeamSeasonStats> SortedList = season.OrderByDescending(s=>s.Points).ThenByDescending(s=>s.GoalDiff).ThenByDescending(s=>s.GoalsFor).ToList();

            Console.WriteLine("SEASON TABLE");
            Console.WriteLine("POS TEAM                                  PTS  W   D   L   GF   GA   GD   ");
            for(int l = 0; l < teams.Count; l++){
                TeamSeasonStats t = SortedList[l];
                if (l+1 == 1) Console.ForegroundColor = ConsoleColor.Yellow;
                else if (l+1 == 2 || l+1 == 3 || l+1 == 4) Console.ForegroundColor = ConsoleColor.Green;
                else if (l+1 == 5) Console.ForegroundColor = ConsoleColor.Magenta;
                else if (l+1 > (teams.Count-3)) Console.ForegroundColor = ConsoleColor.Red;
                Console.Write((l+1).ToString().PadRight(4,' ')); // Position
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(t.Team.Name.PadRight(38,' ')); // Name
                Console.Write(t.Points.ToString().PadRight(5,' ')); // Points
                Console.Write(t.Wins.ToString().PadRight(4,' ')); // Wins
                Console.Write(t.Draws.ToString().PadRight(4,' ')); // Draws
                Console.Write(t.Losses.ToString().PadRight(4,' ')); // Losses
                Console.Write(t.GoalsFor.ToString().PadRight(5,' ')); // Goals For
                Console.Write(t.GoalsAgainst.ToString().PadRight(5,' ')); // Goals Against
                Console.Write(t.GoalDiff.ToString().PadRight(5,' ')); // Goal Difference
                //t.Team.CalculateRating();
                //Console.Write(t.Team.Rating.ToString().PadRight(5,' ')); // Rating
                Console.Write("\n");
            }

            Console.WriteLine("PRESS ENTER TO VIEW INDIVIDUAL AWARDS...");
            Console.ReadLine();

            List<PlayerSeasonStats> orderedStatsByGoals = playerStats.OrderByDescending(p=>p.GoalsScored).ToList();
            PlayerSeasonStats topScorer = orderedStatsByGoals[0];
            List<PlayerSeasonStats> orderedStatsByAssists = playerStats.OrderByDescending(p=>p.Assists).ToList(); // This seems redundant since assists aren't done properly enough to really mean anything (some seasons, the top assister has 3)
            PlayerSeasonStats topAssister = orderedStatsByAssists[0];
            Console.WriteLine("\nTOP SCORER: {0} ({1}): {2} goals",topScorer.Player.Name, topScorer.Team.Name, topScorer.GoalsScored);
            Console.WriteLine("TOP ASSISTER: {0} ({1}): {2} assists",topAssister.Player.Name, topAssister.Team.Name, topAssister.Assists);

            foreach (var p in playerStats) p.CalculateAvgRating();
            List<PlayerSeasonStats> orderedStatsByRating = playerStats.OrderByDescending(p=>p.AvgRating).ToList();
            PlayerSeasonStats topRating = orderedStatsByRating[0];
            Console.Write("\nTOP RATED: {0} ({1}): ",topRating.Player.Name, topRating.Team.Name); PrintRatingWithColour(Math.Round(topRating.AvgRating,2));

            int PADDING = 55;

            Console.WriteLine("TEAM OF THE SEASON");
            List<PlayerSeasonStats> TOTS = new List<PlayerSeasonStats>();
            Console.WriteLine("[7]----[9]----[11]\n------------------\n------------------\n--[6]--[10]--[8]--\n------------------\n------------------\n[2]--[4]--[5]--[3]\n-------[1]-------");
            PlayerSeasonStats bestGK = orderedStatsByRating.Find(p => p.Player.Position == Position.GK);
            TOTS.Add(bestGK);
            Console.Write(String.Format("\n[1] [GK] : {0} ({1}): ",bestGK.Player.Name, bestGK.Team.Name).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestGK.AvgRating,2), false);

            PlayerSeasonStats bestLB = orderedStatsByRating.Find(p => (p.Player.Position == Position.LB || p.Player.Position2 == Position.LB) && !TOTS.Contains(p));
            TOTS.Add(bestLB);
            Console.Write(String.Format("\n[2] [{2}] : {0} ({1}): ",bestLB.Player.Name, bestLB.Team.Name, bestLB.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestLB.AvgRating,2), false);

            PlayerSeasonStats bestRB = orderedStatsByRating.Find(p => (p.Player.Position == Position.RB || p.Player.Position2 == Position.RB) && !TOTS.Contains(p));
            TOTS.Add(bestRB);
            Console.Write(String.Format("\n[3] [{2}] : {0} ({1}): ",bestRB.Player.Name, bestRB.Team.Name, bestRB.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestRB.AvgRating,2), false);

            PlayerSeasonStats bestCB1 = orderedStatsByRating.Find(p => (p.Player.Position == Position.CB || p.Player.Position2 == Position.CB || p.Player.Position == Position.DF || p.Player.Position2 == Position.CB) && !TOTS.Contains(p));
            TOTS.Add(bestCB1);
            Console.Write(String.Format("\n[4] [{2}] : {0} ({1}): ",bestCB1.Player.Name, bestCB1.Team.Name, bestCB1.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestCB1.AvgRating,2), false);

            PlayerSeasonStats bestCB2 = orderedStatsByRating.Find(p => p != bestCB1 && (p.Player.Position == Position.CB || p.Player.Position2 == Position.CB || p.Player.Position == Position.DF || p.Player.Position2 == Position.CB) && !TOTS.Contains(p));
            TOTS.Add(bestCB2);
            Console.Write(String.Format("\n[5] [{2}] : {0} ({1}): ",bestCB2.Player.Name, bestCB2.Team.Name, bestCB2.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestCB2.AvgRating,2), false);

            PlayerSeasonStats bestMF1 = orderedStatsByRating.Find(p => (p.Player.Position == Position.MF || p.Player.Position2 == Position.MF || p.Player.Position == Position.DM || p.Player.Position2 == Position.DM
                || p.Player.Position == Position.AM || p.Player.Position2 == Position.AM
                || p.Player.Position == Position.CM || p.Player.Position2 == Position.CM) && !TOTS.Contains(p));
            TOTS.Add(bestMF1);
            Console.Write(String.Format("\n[6] [{2}] : {0} ({1}): ",bestMF1.Player.Name, bestMF1.Team.Name, bestMF1.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestMF1.AvgRating,2), false);

            PlayerSeasonStats bestMF2 = orderedStatsByRating.Find(p => (p.Player.Position == Position.MF || p.Player.Position2 == Position.MF || p.Player.Position == Position.DM || p.Player.Position2 == Position.DM
                || p.Player.Position == Position.AM || p.Player.Position2 == Position.AM
                || p.Player.Position == Position.CM || p.Player.Position2 == Position.CM) && !TOTS.Contains(p));
            TOTS.Add(bestMF2);
            Console.Write(String.Format("\n[10][{2}] : {0} ({1}): ",bestMF2.Player.Name, bestMF2.Team.Name, bestMF2.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestMF2.AvgRating,2), false);

            PlayerSeasonStats bestMF3 = orderedStatsByRating.Find(p => (p.Player.Position == Position.MF || p.Player.Position2 == Position.MF || p.Player.Position == Position.DM || p.Player.Position2 == Position.DM
                || p.Player.Position == Position.AM || p.Player.Position2 == Position.AM
                || p.Player.Position == Position.CM || p.Player.Position2 == Position.CM) && !TOTS.Contains(p));
            TOTS.Add(bestMF3);
            Console.Write(String.Format("\n[8] [{2}] : {0} ({1}): ",bestMF3.Player.Name, bestMF3.Team.Name, bestMF3.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestMF3.AvgRating,2), false);

            PlayerSeasonStats bestST = orderedStatsByRating.Find(p => (p.Player.Position == Position.ST || p.Player.Position2 == Position.ST || p.Player.Position == Position.FW || p.Player.Position2 == Position.FW) && !TOTS.Contains(p));
            TOTS.Add(bestST);

            PlayerSeasonStats bestLW = orderedStatsByRating.Find(p => (p.Player.Position == Position.LW || p.Player.Position2 == Position.LW || p.Player.Position == Position.LM || p.Player.Position2 == Position.LM
            || p.Player.Position == Position.FW || p.Player.Position2 == Position.FW) && !TOTS.Contains(p));
            TOTS.Add(bestLW);
            Console.Write(String.Format("\n[7] [{2}] : {0} ({1}): ",bestLW.Player.Name, bestLW.Team.Name, bestLW.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestLW.AvgRating,2), false);

            Console.Write(String.Format("\n[9] [{2}] : {0} ({1}): ",bestST.Player.Name, bestST.Team.Name, bestST.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestST.AvgRating,2), false);

            PlayerSeasonStats bestRW = orderedStatsByRating.Find(p => (p.Player.Position == Position.RW || p.Player.Position2 == Position.RW || p.Player.Position == Position.RM || p.Player.Position2 == Position.RM
            || p.Player.Position == Position.FW || p.Player.Position2 == Position.FW) && !TOTS.Contains(p));
            TOTS.Add(bestRW);
            Console.Write(String.Format("\n[11][{2}] : {0} ({1}): ",bestRW.Player.Name, bestRW.Team.Name, bestRW.Player.Position).PadRight(PADDING,' ')); PrintRatingWithColour(Math.Round(bestRW.AvgRating,2), false);
        }

        // TODO: Function to avoid repetition of the above
        static PlayerSeasonStats getTOTS(Position p){
           return null;
        }

        static int StartMenu(){
            Console.Clear();
            int option = 1;

            Console.WriteLine("\nWelcome to Bad Football Simulator v. " + VERSION + "!\nOptions: ");
            Console.WriteLine("0. Simulate Random Season"); // SimulateSeason()
            Console.WriteLine("1. Generate Random Teams"); // SetupTeams()
            Console.WriteLine("2. Load Teams"); // LoadTeam()
            Console.WriteLine("3. Simulate League Season (Skip to End)"); // SimulateRealSeason()
            Console.WriteLine("4. Simulate League Season (Step by Step)"); // SimulateRealSeason(true)
            Console.WriteLine("5. Simulate Single League Game");
            Console.WriteLine("6. Debug (Testing Combinations)");
            //Console.WriteLine("4. Create a Team"); // ???
            //Console.WriteLine("5. Create a Player"); // ???
            //Console.WriteLine("6. ");
            //Console.WriteLine("7. ");
            //Console.WriteLine("8. ");
            Console.WriteLine("9. Quit");
            Console.Write("---\n> ");

            option = int.Parse(Console.ReadLine());

            return option;
        }
        
        static int Menu(){
            int option = 1;

            Console.WriteLine("Menu: ");
            Console.WriteLine("0. Display Basic Player Info");
            Console.WriteLine("1. Display Overalls");
            Console.WriteLine("2. Display Shirt Numbers");
            Console.WriteLine("3. Display Pace");
            Console.WriteLine("4. Display Finishing");
            Console.WriteLine("5. Display Defence");
            Console.WriteLine("6. Display Control");
            Console.WriteLine("7. Display Mentality");
            Console.WriteLine("8. Save Teams");
            Console.WriteLine("9. Back");
            Console.WriteLine("10. Simulate Game (Step by Step)");
            Console.WriteLine("11. Simulate Game (Skip to End)");
            Console.WriteLine("12. Reroll Teams");
            Console.Write("---\n> ");

            try{option = int.Parse(Console.ReadLine());}catch{option=-1;}

            return option;
        }

        static int EndGameMenuInSeason(bool before){
            int option;

            Console.WriteLine("0. Display Goalscorers");
            Console.WriteLine("1. Display Game Stats");
            Console.WriteLine("2. Display Player Ratings");
            if (before) Console.WriteLine("3. Step Through Game Action by Action");
            Console.WriteLine("9. Exit Game (Back to Games)");
            Console.Write("---\n> ");

            try{option = int.Parse(Console.ReadLine());}catch{option=-1;}

            return option;
        }

        static int EndGameMenu(){
            int option;

            Console.WriteLine("0. Display Goalscorers");
            Console.WriteLine("1. Display Game Stats");
            Console.WriteLine("2. Display Player Ratings");
            Console.WriteLine("3. Simulate Game Again (Step by Step)");
            Console.WriteLine("4. Simulate Game Again (Skip to Result)");
            Console.WriteLine("9. Exit Game (Back to Teams)");
            Console.Write("---\n> ");

            try{option = int.Parse(Console.ReadLine());}catch{option=-1;}

            return option;
        }

        static void ShowTeam1Scored(Team team1, Team team2, int team1_score, int team2_score){
            Console.Write("> " + team1.Name + " [");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(team1_score);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] - " + team2_score + " " + team2.Name + "\n"); // Print current score
        }

        static void ShowTeam1Scored(ref Game game, Team team1, Team team2, int team1_score, int team2_score){
            // Console.Write("> " + team1.Name + " [");
            game.AddToLog(new LogItem(ItemType.Text, ("> " + team1.Name + " [")));
            // Console.ForegroundColor = ConsoleColor.Green;
            game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
            // Console.Write(team1_score);
            game.AddToLog(new LogItem(ItemType.Text, team1_score.ToString()));
            // Console.ForegroundColor = ConsoleColor.White;
            game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
            // Console.Write("] - " + team2_score + " " + team2.Name + "\n"); // Print current score
            game.AddToLog(new LogItem(ItemType.Text, ("] - " + team2_score + " " + team2.Name + "\n")));
        }

        static void ShowTeam2Scored(Team team1, Team team2, int team1_score, int team2_score){
            Console.Write("> " + team1.Name + " " + team1_score + " - [");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(team2_score);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] " + team2.Name + "\n"); // Print current score
        }

        static void ShowTeam2Scored(ref Game game, Team team1, Team team2, int team1_score, int team2_score){
            // Console.Write("> " + team1.Name + " " + team1_score + " - [");
            game.AddToLog(new LogItem(ItemType.Text, ("> " + team1.Name + " " + team1_score + " - [")));
            // Console.ForegroundColor = ConsoleColor.Green;
            game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
            // Console.Write(team2_score);
            game.AddToLog(new LogItem(ItemType.Text, team2_score.ToString()));
            // Console.ForegroundColor = ConsoleColor.White;
            game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
            // Console.Write("] " + team2.Name + "\n"); // Print current score
            game.AddToLog(new LogItem(ItemType.Text, ("] " + team2.Name + "\n")));
        }

        static void PrintStats(TeamGameStats team1, TeamGameStats team2){
            team1.calculatePassAccuracy();
            team1.calculateShotAccuracy();
            team2.calculatePassAccuracy();
            team2.calculateShotAccuracy();
            Console.WriteLine("GAME STATS");

            int padValue = 5;
            if (team1.Team.Name.Length > padValue) padValue = team1.Team.Name.Length;
            if (team2.Team.Name.Length > padValue) padValue = team2.Team.Name.Length;

            //Console.WriteLine(team1.Team.Name + "                                                                    " + team2.Team.Name);
            //Console.WriteLine(team1.Team.Name.PadRight(38,' ') + " - " + team2.Team.Name.PadLeft(38,' '));
            Console.WriteLine("".PadRight(17,' ') + team1.Team.Name.PadLeft(padValue,' ') + " - " + team2.Team.Name.PadRight(padValue,' '));
            Console.WriteLine("Goals".PadRight(17,' ') + team1.Goals.ToString().PadLeft(padValue,' ') + " - " + team2.Goals.ToString().PadRight(padValue,' '));
            Console.WriteLine("Posession".PadRight(17,' ') + (team1.Posession.ToString() + "%").PadLeft(padValue,' ') + " - " + (team2.Posession.ToString() + "%").PadRight(padValue,' '));
            Console.WriteLine("Shots".PadRight(17,' ') + team1.Shots.ToString().PadLeft(padValue,' ') + " - " + team2.Shots.ToString().PadRight(padValue,' '));
            Console.WriteLine("Shots On Target".PadRight(17,' ') + team1.ShotsOnTarget.ToString().PadLeft(padValue,' ') + " - " + team2.ShotsOnTarget.ToString().PadRight(padValue,' '));
            Console.WriteLine("Passes Attempted".PadRight(17,' ') + team1.PassesAttempted.ToString().PadLeft(padValue,' ') + " - " + team2.PassesAttempted.ToString().PadRight(padValue,' '));
            Console.WriteLine("Passes Completed".PadRight(17,' ') + team1.Passes.ToString().PadLeft(padValue,' ') + " - " + team2.Passes.ToString().PadRight(padValue,' '));
            Console.WriteLine("Saves".PadRight(17,' ') + team1.Saves.ToString().PadLeft(padValue,' ') + " - " + team2.Saves.ToString().PadRight(padValue,' '));
            Console.WriteLine("Tackles".PadRight(17,' ') + team1.Tackles.ToString().PadLeft(padValue,' ') + " - " + team2.Tackles.ToString().PadRight(padValue,' '));
            Console.WriteLine("Interceptions".PadRight(17,' ') + team1.Interceptions.ToString().PadLeft(padValue,' ') + " - " + team2.Interceptions.ToString().PadRight(padValue,' '));
            Console.WriteLine("Fouls".PadRight(17,' ') + team1.Fouls.ToString().PadLeft(padValue,' ') + " - " + team2.Fouls.ToString().PadRight(padValue,' '));
            Console.WriteLine("Yellow Cards".PadRight(17,' ') + team1.YellowCards.ToString().PadLeft(padValue,' ') + " - " + team2.YellowCards.ToString().PadRight(padValue,' '));
            Console.WriteLine("Red Cards".PadRight(17,' ') + team1.RedCards.ToString().PadLeft(padValue,' ') + " - " + team2.RedCards.ToString().PadRight(padValue,' '));
            Console.WriteLine("Shot Accuracy".PadRight(17,' ') + (team1.ShotAccuracy.ToString() + "%").PadLeft(padValue,' ') + " - " + (team2.ShotAccuracy.ToString() + "%").PadRight(padValue,' '));
            Console.WriteLine("Pass Accuracy".PadRight(17,' ') + (team1.PassAccuracy.ToString() + "%").PadLeft(padValue,' ') + " - " + (team2.PassAccuracy.ToString() + "%").PadRight(padValue,' '));
        }

        static void SimulateGame(Team team1, Team team2, bool show = true){
            Console.WriteLine("Welcome to the game between " + team1.Name + " and " + team2.Name + "!\n");

            int team1_score = 0;
            int team2_score = 0;

            List<Goal> goals = new List<Goal>();

            Player gk1 = team1.Players[0]; // Team 1s goalkeeper
            Player gk2 = team2.Players[0]; // Team 2s goalkeeper

            TeamGameStats team1stats = new TeamGameStats(team1);
            TeamGameStats team2stats = new TeamGameStats(team2);

            List<PlayerInGame> playersInGames1 = new List<PlayerInGame>();
            foreach (Player p in team1.Players) { p.InGameStats = new PlayerInGame(p); playersInGames1.Add(p.InGameStats); }
            List<PlayerInGame> playersInGames2 = new List<PlayerInGame>();
            foreach (Player p in team2.Players) { p.InGameStats = new PlayerInGame(p); playersInGames2.Add(p.InGameStats); }

            Random r = new Random();

            bool isTeam1 = true;
            if (show){
                Console.WriteLine("Press ENTER to begin game, and to advance through the game");
                Console.ReadLine();
            }

            for (int i = 3; i <= 90; i+=3){ // Simulate in 5 minute increments
                Console.WriteLine("");
                if (i%10==1 && i!=11) Console.WriteLine(i + "st Minute");
                else if (i%10==2 && i!=12) Console.WriteLine(i + "nd Minute");
                else if (i%10==3 && i!=13) Console.WriteLine(i + "rd Minute");
                else Console.WriteLine(i + "th Minute");
                Console.WriteLine(team1.Name + " " + team1_score + " - " + team2_score + " " + team2.Name); // Print current score
                Console.WriteLine("===");

                // Decide whether a pass, dribble or shot is attempted

                int choice = r.Next(0,3); // 0 = pass, 1 = dribble, 2 = shot

                //Player p1,p2,q1,q2;

                Player p1 = team1.Players[r.Next(1,11)]; // Pick a random player from team 1
                while (p1.InGameStats.SentOff) p1 = team1.Players[r.Next(1,11)]; // Make sure player is not sent off
                Player p2 = team2.Players[r.Next(1,11)]; // Pick a random player from team 2
                while (p2.InGameStats.SentOff) p2 = team2.Players[r.Next(1,11)];
                Player q1 = team1.Players[r.Next(1,11)]; // Pick a random player from team 1
                while (q1.InGameStats.SentOff) q1 = team1.Players[r.Next(1,11)]; 
                Player q2 = team2.Players[r.Next(1,11)]; // Pick a random player from team 2
                while (q2.InGameStats.SentOff) q2 = team2.Players[r.Next(1,11)];

                PlayerInGame p1_ = playersInGames1.Find(p => p.Player.Equals(p1));
                PlayerInGame p2_ = playersInGames2.Find(p => p.Player.Equals(p2));
                PlayerInGame q1_ = playersInGames1.Find(p => p.Player.Equals(q1));
                PlayerInGame q2_ = playersInGames2.Find(p => p.Player.Equals(q2));

                if (isTeam1){
                    team1stats.Posession+=2;
                    team2stats.Posession-=2;
                    Position pos = p1.Position;
                    if (pos == Position.CB || pos == Position.LB || pos == Position.RB){
                        if (choice == 2) choice = r.Next(0,3);
                    }
                    else if (pos == Position.ST){
                        if (choice != 2) choice = r.Next(0,3);
                    }
                    
                    if (choice == 0){   // PASS
                    // pass then chance to shoot
                        Player p3 = team1.Players[r.Next(0,11)]; // Pick random player to pass to
                        while (p3 == p1 || p3.InGameStats.SentOff) p3 = team1.Players[r.Next(0,11)];
                        PlayerInGame p3_ = playersInGames1.Find(p => p.Player.Equals(p3));
                        Console.WriteLine("> [" + team1.Name + "] " + p1.Name + " attempts to pass to " + p3.Name + ".");

                        bool passed = AttemptPass(p1, p3, p2);
                        team1stats.PassesAttempted++; // Increment attempted passes

                        if (passed){ // If pass is successful
                            team1stats.Posession+=3; // Alter posession numbers accordingly
                            team2stats.Posession-=3; 
                            team1stats.Passes++; // Increment successful passes

                            //p1_.Rating+=0.5; // Alter player ratings accordingly
                            //p3_.Rating+=0.5;
                            p1.InGameStats.Rating+=0.5; // Alter player ratings accordingly
                            p3.InGameStats.Rating+=0.3;

                            // If new player (p3) has high finishing and high mentality, take a shot
                            // Otherwise attack fails
                            if (p3.Finishing >= 50){
                                int a = r.Next(0,100);
                                if (p3.Mentality > a){
                                    // shoot
                                    //team1stats.Shots++;
                                    bool scored = AttemptShot(team1stats, team2stats, p3, true);

                                    if (scored) {
                                        team1_score++;

                                        ShowTeam1Scored(team1, team2, team1_score, team2_score);

                                        goals.Add(new Goal(p3.Name, team1.Name, i));
                                    }
                                }
                            }
                        }
                        else{
                            team2stats.Posession+=3;
                            team1stats.Posession-=3;
                            team2stats.Interceptions++;

                            p1.InGameStats.Rating-=0.5;
                            p2.InGameStats.Rating+=0.5;
                            // If new player (p2) has high finishing and high mentality, take a shot
                            // Otherwise attack fails
                            if (p2.Finishing >= 50){
                                int a = r.Next(0,100);
                                if (p2.Mentality > a){
                                    // shoot
                                    //team2stats.Shots++;
                                    bool scored = AttemptShot(team1stats, team2stats, p2, false);
                                    if (scored) {
                                        team2_score++;

                                        ShowTeam2Scored(team1, team2, team1_score, team2_score);

                                        goals.Add(new Goal(p2.Name, team2.Name, i));
                                    }
                                }
                            }
                        }
                    }
                    else if (choice == 1){
                        // dribble then chance to shoot
                        Console.WriteLine("> [" + team1.Name + "] " + p1.Name + " attempts to dribble.");
                        bool dribbled = AttemptDribble(p1, p2);
                        if (dribbled){
                            p1.InGameStats.Rating+=0.5;
                            p2.InGameStats.Rating-=0.1;
                            team1stats.Posession+=3;
                            team2stats.Posession-=3;
                            // If player (p1) has high finishing and high mentality, take a shot
                            // Otherwise attack fails
                            if (p1.Finishing >= 50){
                                int a = r.Next(0,100);
                                if (p1.Mentality > a){
                                    // shoot
                                    //team1stats.Shots++;
                                    bool scored = AttemptShot(team1stats, team2stats, p1, true);
                                    if (scored) {
                                        team1_score++;
                                        
                                        ShowTeam1Scored(team1, team2, team1_score, team2_score);

                                        goals.Add(new Goal(p1.Name, team1.Name, i));
                                    }
                                }
                            }
                        }
                        else{
                            team2stats.Tackles++;
                            // If tackling player has low mentality have a chance to commit a foul
                            int b = r.Next(0,50);
                            if (p2.Mentality < b){
                                p2.InGameStats.Rating-=0.3;
                                team2stats.Fouls++;
                                /*if (show) */Console.WriteLine(p2.Name + " fouled " + p1.Name + ".");

                                // Decide if a card should be shown
                                // IDEA: Change goals thingy to events, store cards in it too?
                                int yellowNum = GenerateNormal(30,10);
                                double yellowThreshold = Sigmoid(yellowNum, p2.Mentality,-0.05,0);
                                int redNum = GenerateNormal(5,2); // https://www.desmos.com/calculator/2kmx0enkkz
                                double redThreshold = Sigmoid(redNum, p2.Mentality,-0.063,0.5);
                                while (redThreshold > yellowThreshold) {redNum = GenerateNormal(5,2); redThreshold = Sigmoid(redNum, p2.Mentality,-0.063,0.5);}
                                double randnum = r.NextDouble();
                                //Console.WriteLine("DEBUG: ");
                                //Console.WriteLine("yellowThreshold = " + yellowNum + ", redThreshold = " + redNum + ", mentality = " + p2.Mentality);
                                //Console.WriteLine("yellowChance = " + yellowThreshold + ", redChance = " + redThreshold + ", randnum = " + randnum);
                                if (randnum < redThreshold){
                                    //Console.WriteLine("should send off...");
                                    p2.InGameStats.GiveCard(false);
                                    p2.InGameStats.Rating-=2;
                                    team2stats.RedCards++;
                                }
                                else if (randnum < yellowThreshold){
                                    //Console.WriteLine("should book...");
                                    p2.InGameStats.GiveCard();
                                    p2.InGameStats.Rating-=0.5;
                                    team2stats.YellowCards++;
                                }
                                //else Console.WriteLine("should do nothing...");

                                bool scored;
                                if (p1.Position == Position.ST || p1.Position == Position.AM){
                                    scored = AttemptSetpiece(team1stats, team2stats, false, isTeam1);
                                }
                                else if (p1.Position == Position.LW || p1.Position == Position.RW || p1.Position == Position.LM || p1.Position == Position.RM || p1.Position == Position.CM){
                                    int c = r.Next(0,100);
                                    if (p1.Mentality > c) {
                                        scored = AttemptSetpiece(team1stats, team2stats, false, isTeam1);
                                    }
                                    else{
                                        scored = AttemptSetpiece(team1stats, team2stats, true, isTeam1);
                                    }
                                }
                                else{
                                    scored = AttemptSetpiece(team1stats, team2stats, true, isTeam1);
                                }
                                if (scored) {
                                    team1_score++;
                                    ShowTeam1Scored(team1, team2, team1_score, team2_score);
                                    goals.Add(new Goal (getSetPieceTaker(team1).Name, team1.Name,i));
                                }
                            }
                            else{
                                p2.InGameStats.Rating+=0.5;
                                team2stats.Posession+=3;
                                team1stats.Posession-=3;
                                /*if (show) */Console.WriteLine(p2.Name + " tackles " + p1.Name + ".");
                                // If new player (p2) has high finishing and high mentality, take a shot
                                // Otherwise attack fails
                                if (p2.Finishing >= 50){
                                    int a = r.Next(0,100);
                                    if (p2.Mentality > a){
                                        // shoot
                                        //team2stats.Shots++;
                                        bool scored = AttemptShot(team1stats, team2stats, p2, false);
                                        if (scored) {
                                            team2_score++;
                                            
                                            ShowTeam2Scored(team1, team2, team1_score, team2_score);

                                            goals.Add(new Goal(p2.Name, team2.Name, i));
                                        }
                                    }
                                }
                            }
                            // If new player (p2) has high finishing and high mentality, take a shot
                            // Otherwise attack fails
                        }
                    }
                    else{
                        team1stats.Posession+=2;
                        team2stats.Posession-=2;
                        // shoot
                        //team1stats.Shots++;
                        bool scored = AttemptShot(team1stats, team2stats, p1, true);
                        if (scored) {
                            team1_score++;
                            
                            ShowTeam1Scored(team1, team2, team1_score, team2_score);

                            goals.Add(new Goal(p1.Name, team1.Name, i));
                        }
                        /*Console.WriteLine("[" + team1.Name + "] " + p1.Name + " attempts a shot.");
                        if (p1.Finishing >= gk2.Defence){
                            Console.WriteLine("GOAL! " + p1.Name + " scores for " + team1.Name + "!");
                            team1_score++;
                        }
                        else{
                            Console.WriteLine("SAVE! " + gk1.Name + " saves the shot for " + team2.Name + "!");
                        }*/
                    }
                }
                else{
                    team2stats.Posession+=2;
                    team1stats.Posession-=2;
                    Position pos = q1.Position;
                    if (pos == Position.CB || pos == Position.LB || pos == Position.RB){
                        if (choice == 2) choice = r.Next(0,3);
                    }
                    else if (pos == Position.ST){
                        if (choice != 2) choice = r.Next(0,3);
                    }
                    if (choice == 0){
                        // pass then chance to shoot
                        Player q3 = team2.Players[r.Next(0,11)];
                        while (q3 == q1 || q3.InGameStats.SentOff) q3 = team2.Players[r.Next(0,11)];
                        PlayerInGame q3_ = playersInGames2.Find(p => p.Player.Equals(q3));
                        Console.WriteLine("> [" + team2.Name + "] " + q2.Name + " attempts to pass to " + q3.Name + ".");

                        bool passed = AttemptPass(q2, q3, q1);
                        team2stats.PassesAttempted++;

                        if (passed){
                            team2stats.Posession+=3;
                            team1stats.Posession-=3;
                            team2stats.Passes++;

                            q2.InGameStats.Rating+=0.5;
                            q3.InGameStats.Rating+=0.5;
                            // If new player (q3) has high finishing and high mentality, take a shot
                            // Otherwise attack fails
                            if (q3.Finishing >= 50){
                                int a = r.Next(0,100);
                                if (q3.Mentality > a){
                                    // shoot
                                    bool scored = AttemptShot(team1stats, team2stats, q3, false);
                                    //team2stats.Shots++;
                                    if (scored) {
                                        team2_score++;
                                        
                                        ShowTeam2Scored(team1, team2, team1_score, team2_score);

                                        goals.Add(new Goal(q3.Name, team2.Name, i));
                                    }
                                }
                            }
                        }
                        else{
                            team1stats.Posession+=3;
                            team2stats.Posession-=3;
                            team1stats.Interceptions++;

                            q2.InGameStats.Rating-=0.5;
                            q1.InGameStats.Rating+=0.5;
                            // If new player (q1) has high finishing and high mentality, take a shot
                            // Otherwise attack fails
                            if (q1.Finishing >= 50){
                                int a = r.Next(0,100);
                                if (q1.Mentality > a){
                                    // shoot
                                    bool scored = AttemptShot(team1stats, team2stats, q1, true);
                                    //team1stats.Shots++;
                                    if (scored) {
                                        team1_score++;
                                        
                                        ShowTeam1Scored(team1, team2, team1_score, team2_score);

                                        goals.Add(new Goal(q1.Name, team1.Name, i));
                                    }
                                }
                            }
                        }
                        
                    }
                    else if (choice == 1){
                        // dribble then chance to shoot
                        Console.WriteLine("> [" + team2.Name + "] " + q2.Name + " attempts to dribble.");
                        bool dribbled = AttemptDribble(q2, q1);
                        if (dribbled){
                            team2stats.Posession+=3;
                            team1stats.Posession-=3;

                            q2.InGameStats.Rating+=0.5;
                            q1.InGameStats.Rating-=0.1;
                            // If player (q2) has high finishing and high mentality, take a shot
                            // Otherwise attack fails
                            if (q2.Finishing >= 50){
                                int a = r.Next(0,100);
                                if (q2.Mentality > a){
                                    // shoot
                                    bool scored = AttemptShot(team1stats, team2stats, q2, false);
                                    //team2stats.Shots++;
                                    if (scored) {
                                        team2_score++;
                                        
                                        ShowTeam2Scored(team1, team2, team1_score, team2_score);

                                        goals.Add(new Goal(q2.Name, team2.Name, i));
                                    }
                                }
                            }
                        }
                        else{
                            team1stats.Tackles++;
                            int b = r.Next(0,50);
                            if (q1.Mentality < b){
                                q1.InGameStats.Rating-=0.3;
                                team1stats.Fouls++;
                                /*if (show) */Console.WriteLine(q1.Name + " fouled " + q2.Name + ".");

                                int yellowNum = GenerateNormal(30,10);
                                double yellowThreshold = Sigmoid(yellowNum, q1.Mentality,-0.05,0);
                                int redNum = GenerateNormal(5,2);
                                double redThreshold = Sigmoid(redNum, q1.Mentality,-0.063,0.5);
                                while (redThreshold > yellowThreshold) {redNum = GenerateNormal(5,2); redThreshold = Sigmoid(redNum, q1.Mentality,-0.063,0.5);}
                                double randnum = r.NextDouble();
                                //Console.WriteLine("DEBUG 2: ");
                                //Console.WriteLine("yellowThreshold = " + yellowNum + ", redThreshold = " + redNum + ", mentality = " + q1.Mentality);
                                //Console.WriteLine("yellowChance = " + yellowThreshold + ", redChance = " + redThreshold + ", randnum = " + randnum);
                                if (randnum < redThreshold){
                                    //Console.WriteLine("should send off...");
                                    q1.InGameStats.GiveCard(false);
                                    q1.InGameStats.Rating-=2;
                                    team1stats.RedCards++;
                                }
                                else if (randnum < yellowThreshold){
                                    //Console.WriteLine("should book...");
                                    q1.InGameStats.GiveCard();
                                    q1.InGameStats.Rating-=0.5;
                                    team1stats.YellowCards++;
                                }
                                //else Console.WriteLine("should do nothing...");

                                bool scored;
                                if (q2.Position == Position.ST || q2.Position == Position.AM){
                                    scored = AttemptSetpiece(team1stats, team2stats, false, isTeam1);
                                }
                                else if (q2.Position == Position.LW || q2.Position == Position.RW || q2.Position == Position.LM || q2.Position == Position.RM || q2.Position == Position.CM){
                                    int c = r.Next(0,100);
                                    if (q2.Mentality > c) {
                                        scored = AttemptSetpiece(team1stats, team2stats, false, isTeam1);
                                    }
                                    else{
                                        scored = AttemptSetpiece(team1stats, team2stats, true, isTeam1);
                                    }
                                }
                                else{
                                    scored = AttemptSetpiece(team1stats, team2stats, true, isTeam1);
                                }
                                if (scored) {
                                    team2_score++;
                                    ShowTeam2Scored(team1, team2, team1_score, team2_score);
                                    goals.Add(new Goal (getSetPieceTaker(team2).Name, team2.Name,i));
                                }
                            }
                            else{
                                team1stats.Posession+=3;
                                team2stats.Posession-=3;

                                q1.InGameStats.Rating+=0.5;
                                q2.InGameStats.Rating-=0.5;
                                /*if (show) */Console.WriteLine(q1.Name + " tackles " + q2.Name + ".");
                                // If new player (q1) has high finishing and high mentality, take a shot
                                // Otherwise attack fails
                                if (q1.Finishing >= 50){
                                    int a = r.Next(0,100);
                                    if (q1.Mentality > a){
                                        // shoot
                                        bool scored = AttemptShot(team1stats, team2stats, q1, true);
                                        if (scored) {
                                            team1_score++;
                                            
                                            ShowTeam1Scored(team1, team2, team1_score, team2_score);

                                            goals.Add(new Goal(q1.Name, team1.Name, i));
                                        }
                                    }
                                }
                            }
                            
                        }
                    }
                    else{
                        team2stats.Posession+=2;
                        team1stats.Posession-=2;
                        // shoot
                        bool scored = AttemptShot(team1stats, team2stats, q2, false);
                        if (scored) {
                            team2_score++;
                            
                            ShowTeam2Scored(team1, team2, team1_score, team2_score);

                            goals.Add(new Goal(q2.Name, team2.Name, i));
                        }
                        /*Console.WriteLine("[" + team1.Name + "] " + p1.Name + " attempts a shot.");
                        if (p1.Finishing >= gk2.Defence){
                            Console.WriteLine("GOAL! " + p1.Name + " scores for " + team1.Name + "!");
                            team1_score++;
                        }
                        else{
                            Console.WriteLine("SAVE! " + gk1.Name + " saves the shot for " + team2.Name + "!");
                        }*/
                    }
                }

                Console.WriteLine("");
                if (show) Console.ReadLine();
                isTeam1 = !isTeam1;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("FULL TIME!");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Final Score: " + team1.Name + " " + team1_score + " - " + team2_score + " " + team2.Name);

            Console.WriteLine("\nAdditional Options: ");

            int option = 0;

            while (option != 9 && option != 3){
                option = EndGameMenu();

                switch(option){
                    case 0:
                        Console.WriteLine("\nGoalscorers:");
                        foreach (var g in goals) g.GoalInfo();
                        Console.WriteLine();
                        Console.WriteLine("==========");
                        Console.WriteLine();
                        break;
                    case 1:
                        Console.WriteLine();
                        PrintStats(team1stats, team2stats);
                        Console.WriteLine();
                        Console.WriteLine("==========");
                        Console.WriteLine();
                        break;
                    case 2:
                        Console.WriteLine();
                        PrintRatings(playersInGames1, playersInGames2, team1, team2);
                        Console.WriteLine();
                        Console.WriteLine("==========");
                        Console.WriteLine();
                        break;
                    case 3:
                        SimulateGame(team1, team2, true);
                        option = 9;
                        break;
                    case 4:
                        SimulateGame(team1, team2, false);
                        option = 9;
                        break;
                    case 9:
                        break;
                    default:
                        break;
                }
            }
        }

        static Game SimulateGameInSeason(Team team1, Team team2, List<PlayerSeasonStats> playerStats = null){
            int team1_score = 0;
            int team2_score = 0;

            Game game = new Game(String.Format("{0} v {1}",team1.Name, team2.Name));

            game.AddToLog(new LogItem(ItemType.TextLine, String.Format("Welcome to the game between " + team1.Name + " and " + team2.Name + "!\n")));

            List<Goal> goals = new List<Goal>();

            Player gk1 = team1.Players[0]; // Team 1s goalkeeper
            Player gk2 = team2.Players[0]; // Team 2s goalkeeper

            TeamGameStats team1stats = new TeamGameStats(team1);
            TeamGameStats team2stats = new TeamGameStats(team2);

            List<PlayerInGame> playersInGames1 = new List<PlayerInGame>();
            foreach (Player p in team1.Players) { p.InGameStats = new PlayerInGame(p); playersInGames1.Add(p.InGameStats); }
            List<PlayerInGame> playersInGames2 = new List<PlayerInGame>();
            foreach (Player p in team2.Players) { p.InGameStats = new PlayerInGame(p); playersInGames2.Add(p.InGameStats); }

            Random r = new Random();

            bool isTeam1 = true;

            game.AddToLog(new LogItem(ItemType.TextLine, "Press ENTER to begin game, and to advance through the game"));
            game.AddToLog(new LogItem(ItemType.Read, ""));

            for (int i = 3; i <= 90; i+=3){ // Simulate in 5 minute increments
                game.AddToLog(new LogItem(ItemType.TextLine, ""));
                if (i%10==1 && i!=11) game.AddToLog(new LogItem(ItemType.TextLine, (i + "st Minute")));
                else if (i%10==2 && i!=12) game.AddToLog(new LogItem(ItemType.TextLine, (i + "nd Minute")));
                else if (i%10==3 && i!=13) game.AddToLog(new LogItem(ItemType.TextLine, (i + "rd Minute")));
                else game.AddToLog(new LogItem(ItemType.TextLine, (i + "th Minute")));
                game.AddToLog(new LogItem(ItemType.TextLine, (team1.Name + " " + team1_score + " - " + team2_score + " " + team2.Name))); // Print current score
                game.AddToLog(new LogItem(ItemType.TextLine, "==="));

                int choice = r.Next(0,3); // 0 = pass, 1 = dribble, 2 = shot

                Player p1 = team1.Players[r.Next(1,11)]; // Pick a random player from team 1
                while (p1.InGameStats.SentOff) p1 = team1.Players[r.Next(1,11)]; // Make sure player is not sent off
                Player p2 = team2.Players[r.Next(1,11)]; // Pick a random player from team 2
                while (p2.InGameStats.SentOff) p2 = team2.Players[r.Next(1,11)];
                Player q1 = team1.Players[r.Next(1,11)]; // Pick a random player from team 1
                while (q1.InGameStats.SentOff) q1 = team1.Players[r.Next(1,11)]; 
                Player q2 = team2.Players[r.Next(1,11)]; // Pick a random player from team 2
                while (q2.InGameStats.SentOff) q2 = team2.Players[r.Next(1,11)];

                PlayerInGame p1_ = playersInGames1.Find(p => p.Player.Equals(p1));
                PlayerInGame p2_ = playersInGames2.Find(p => p.Player.Equals(p2));
                PlayerInGame q1_ = playersInGames1.Find(p => p.Player.Equals(q1));
                PlayerInGame q2_ = playersInGames2.Find(p => p.Player.Equals(q2));

                if (isTeam1){
                    Position pos = p1.Position;
                    if (pos == Position.CB || pos == Position.LB || pos == Position.RB){
                        if (choice == 2) choice = r.Next(0,3);
                    }
                    else if (pos == Position.ST){
                        if (choice != 2) choice = r.Next(0,3);
                    }
                    
                    if (choice == 0){
                    // pass then chance to shoot
                        Player p3 = team1.Players[r.Next(0,11)];
                        while (p3 == p1) p3 = team1.Players[r.Next(0,11)];
                        PlayerInGame p3_ = playersInGames1.Find(p => p.Player.Equals(p3));
                        //Console.WriteLine("> [" + team1.Name + "] " + p1.Name + " attempts to pass to " + p3.Name + ".");
                        game.AddToLog(new LogItem(ItemType.TextLine, ("> [" + team1.Name + "] " + p1.Name + " attempts to pass to " + p3.Name + ".")));

                        bool passed = AttemptPass(ref game, p1, p3, p2, false);

                        if (passed){
                            team1stats.Posession+=3; // Alter posession numbers accordingly
                            team2stats.Posession-=3; 
                            team1stats.Passes++; // Increment successful passes

                            //p1_.Rating+=0.5; // Alter player ratings accordingly
                            //p3_.Rating+=0.5;
                            p1.InGameStats.Rating+=0.5; // Alter player ratings accordingly
                            p3.InGameStats.Rating+=0.3;
                            // If new player (p3) has high finishing and high mentality, take a shot
                            // Otherwise attack fails
                            if (p3.Finishing >= 50){
                                int a = r.Next(0,100);
                                if (p3.Mentality > a){
                                    // shoot
                                    bool scored = AttemptShot(ref game, team1stats, team2stats, p3, true, false);
                                    if (scored) {
                                        team1_score++;

                                        //ShowTeam1Scored(ref game, team1, team2, team1_score, team2_score);
                                        game.AddToLog(new LogItem(ItemType.Text, ("> " + team1.Name + " [")));
                                        game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
                                        game.AddToLog(new LogItem(ItemType.Text, team1_score.ToString()));
                                        game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                                        game.AddToLog(new LogItem(ItemType.Text, ("] - " + team2_score + " " + team2.Name + "\n")));

                                        goals.Add(new Goal(p3.Name, team1.Name, i));
                                        try{playerStats.Find(pl => pl.Player == p3).GoalsScored++;
                                        playerStats.Find(pl => pl.Player == p1).Assists++;}catch{}
                                    }
                                }
                            }
                        }
                        else{
                            team2stats.Posession+=3;
                            team1stats.Posession-=3;
                            team2stats.Interceptions++;

                            p1.InGameStats.Rating-=0.5;
                            p2.InGameStats.Rating+=0.5;
                            // If new player (p2) has high finishing and high mentality, take a shot
                            // Otherwise attack fails
                            if (p2.Finishing >= 50){
                                int a = r.Next(0,100);
                                if (p2.Mentality > a){
                                    // shoot
                                    bool scored = AttemptShot(ref game, team1stats, team2stats, p2, false, false);
                                    if (scored) {
                                        team2_score++;

                                        // ShowTeam2Scored(ref game, team1, team2, team1_score, team2_score);
                                        game.AddToLog(new LogItem(ItemType.Text, ("> " + team1.Name + " " + team1_score + " - [")));
                                        game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
                                        game.AddToLog(new LogItem(ItemType.Text, team2_score.ToString()));
                                        game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                                        game.AddToLog(new LogItem(ItemType.Text, ("] " + team2.Name + "\n")));

                                        goals.Add(new Goal(p2.Name, team2.Name, i));
                                        try{playerStats.Find(pl => pl.Player == p2).GoalsScored++;}catch{}
                                    }
                                }
                            }
                        }
                    }
                    else if (choice == 1){
                        // dribble then chance to shoot
                        //Console.WriteLine("> [" + team1.Name + "] " + p1.Name + " attempts to dribble.");
                        game.AddToLog(new LogItem(ItemType.TextLine, ("> [" + team1.Name + "] " + p1.Name + " attempts to dribble.")));
                        bool dribbled = AttemptDribble(ref game, p1, p2, false);
                        if (dribbled){
                            p1.InGameStats.Rating+=0.5;
                            p2.InGameStats.Rating-=0.1;
                            team1stats.Posession+=3;
                            team2stats.Posession-=3;
                            // If player (p1) has high finishing and high mentality, take a shot
                            // Otherwise attack fails
                            if (p1.Finishing >= 50){
                                int a = r.Next(0,100);
                                if (p1.Mentality > a){
                                    // shoot
                                    bool scored = AttemptShot(ref game, team1stats, team2stats, p1, true, false);
                                    if (scored) {
                                        team1_score++;
                                        
                                        // ShowTeam1Scored(ref game, team1, team2, team1_score, team2_score);
                                        game.AddToLog(new LogItem(ItemType.Text, ("> " + team1.Name + " [")));
                                        game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
                                        game.AddToLog(new LogItem(ItemType.Text, team1_score.ToString()));
                                        game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                                        game.AddToLog(new LogItem(ItemType.Text, ("] - " + team2_score + " " + team2.Name + "\n")));

                                        goals.Add(new Goal(p1.Name, team1.Name, i));
                                        try{playerStats.Find(pl => pl.Player == p1).GoalsScored++;}catch{}
                                    }
                                }
                            }
                        }
                        team2stats.Tackles++;
                            // If tackling player has low mentality have a chance to commit a foul
                            int b = r.Next(0,50);
                            if (p2.Mentality < b){
                                p2.InGameStats.Rating-=0.3;
                                team2stats.Fouls++;
                                ///*if (show) */Console.WriteLine(p2.Name + " fouled " + p1.Name + ".");
                                game.AddToLog(new LogItem(ItemType.TextLine, (p2.Name + " fouled " + p1.Name + ".")));

                                // Decide if a card should be shown
                                // IDEA: Change goals thingy to events, store cards in it too?
                                int yellowNum = GenerateNormal(30,10);
                                double yellowThreshold = Sigmoid(yellowNum, p2.Mentality,-0.05,0);
                                int redNum = GenerateNormal(5,2); // https://www.desmos.com/calculator/2kmx0enkkz
                                double redThreshold = Sigmoid(redNum, p2.Mentality,-0.063,0.5);
                                while (redThreshold > yellowThreshold) {redNum = GenerateNormal(5,2); redThreshold = Sigmoid(redNum, p2.Mentality,-0.063,0.5);}
                                double randnum = r.NextDouble();
                                //Console.WriteLine("DEBUG: ");
                                //Console.WriteLine("yellowThreshold = " + yellowNum + ", redThreshold = " + redNum + ", mentality = " + p2.Mentality);
                                //Console.WriteLine("yellowChance = " + yellowThreshold + ", redChance = " + redThreshold + ", randnum = " + randnum);
                                if (randnum < redThreshold){
                                    //Console.WriteLine("should send off...");
                                    p2.InGameStats.GiveCard(ref game, false);
                                    p2.InGameStats.Rating-=2;
                                    team2stats.RedCards++;
                                }
                                else if (randnum < yellowThreshold){
                                    //Console.WriteLine("should book...");
                                    p2.InGameStats.GiveCard(ref game);
                                    p2.InGameStats.Rating-=0.5;
                                    team2stats.YellowCards++;
                                }
                                //else Console.WriteLine("should do nothing...");

                                bool scored;
                                if (p1.Position == Position.ST || p1.Position == Position.AM){
                                    scored = AttemptSetpiece(ref game, team1stats, team2stats, false, isTeam1);
                                }
                                else if (p1.Position == Position.LW || p1.Position == Position.RW || p1.Position == Position.LM || p1.Position == Position.RM || p1.Position == Position.CM){
                                    int c = r.Next(0,100);
                                    if (p1.Mentality > c) {
                                        scored = AttemptSetpiece(ref game, team1stats, team2stats, false, isTeam1);
                                    }
                                    else{
                                        scored = AttemptSetpiece(ref game, team1stats, team2stats, true, isTeam1);
                                    }
                                }
                                else{
                                    scored = AttemptSetpiece(ref game, team1stats, team2stats, true, isTeam1);
                                }
                                if (scored) {
                                    team1_score++;
                                    // ShowTeam1Scored(ref game, team1, team2, team1_score, team2_score);
                                    game.AddToLog(new LogItem(ItemType.Text, ("> " + team1.Name + " [")));
                                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
                                    game.AddToLog(new LogItem(ItemType.Text, team1_score.ToString()));
                                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                                    game.AddToLog(new LogItem(ItemType.Text, ("] - " + team2_score + " " + team2.Name + "\n")));
                                    goals.Add(new Goal (getSetPieceTaker(team1).Name, team1.Name,i));
                                }
                            }
                            else{
                                p2.InGameStats.Rating+=0.5;
                                team2stats.Posession+=3;
                                team1stats.Posession-=3;
                                ///*if (show) */Console.WriteLine(p2.Name + " tackles " + p1.Name + ".");
                                game.AddToLog(new LogItem(ItemType.TextLine, (p2.Name + " tackles " + p1.Name + ".")));
                                // If new player (p2) has high finishing and high mentality, take a shot
                                // Otherwise attack fails
                                if (p2.Finishing >= 50){
                                    int a = r.Next(0,100);
                                    if (p2.Mentality > a){
                                        // shoot
                                        //team2stats.Shots++;
                                        bool scored = AttemptShot(ref game, team1stats, team2stats, p2, false);
                                        if (scored) {
                                            team2_score++;
                                            
                                            // ShowTeam2Scored(ref game, team1, team2, team1_score, team2_score);
                                            game.AddToLog(new LogItem(ItemType.Text, ("> " + team1.Name + " " + team1_score + " - [")));
                                            game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
                                            game.AddToLog(new LogItem(ItemType.Text, team2_score.ToString()));
                                            game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                                            game.AddToLog(new LogItem(ItemType.Text, ("] " + team2.Name + "\n")));

                                            goals.Add(new Goal(p2.Name, team2.Name, i));
                                        }
                                    }
                                }
                            }
                    }
                    else{
                        team1stats.Posession+=2;
                        team2stats.Posession-=2;
                        // shoot
                        bool scored = AttemptShot(ref game, team1stats, team2stats, p1, true, false);
                        if (scored) {
                            team1_score++;
                            
                            // ShowTeam1Scored(ref game, team1, team2, team1_score, team2_score);
                            game.AddToLog(new LogItem(ItemType.Text, ("> " + team1.Name + " [")));
                            game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
                            game.AddToLog(new LogItem(ItemType.Text, team1_score.ToString()));
                            game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                            game.AddToLog(new LogItem(ItemType.Text, ("] - " + team2_score + " " + team2.Name + "\n")));

                            goals.Add(new Goal(p1.Name, team1.Name, i));
                            try{playerStats.Find(pl => pl.Player == p1).GoalsScored++;}catch{}
                        }
                        /*Console.WriteLine("[" + team1.Name + "] " + p1.Name + " attempts a shot.");
                        if (p1.Finishing >= gk2.Defence){
                            Console.WriteLine("GOAL! " + p1.Name + " scores for " + team1.Name + "!");
                            team1_score++;
                        }
                        else{
                            Console.WriteLine("SAVE! " + gk1.Name + " saves the shot for " + team2.Name + "!");
                        }*/
                    }
                }
                else{
                    team2stats.Posession+=2;
                    team1stats.Posession-=2;
                    Position pos = q1.Position;
                    if (pos == Position.CB || pos == Position.LB || pos == Position.RB){
                        if (choice == 2) choice = r.Next(0,3);
                    }
                    else if (pos == Position.ST){
                        if (choice != 2) choice = r.Next(0,3);
                    }
                    if (choice == 0){
                        // pass then chance to shoot
                        Player q3 = team2.Players[r.Next(0,11)];
                        while (q3 == q1) q3 = team2.Players[r.Next(0,11)];
                        PlayerInGame q3_ = playersInGames2.Find(p => p.Player.Equals(q3));
                        //Console.WriteLine("> [" + team2.Name + "] " + q2.Name + " attempts to pass to " + q3.Name + ".");
                        game.AddToLog(new LogItem(ItemType.TextLine, ("> [" + team2.Name + "] " + q2.Name + " attempts to pass to " + q3.Name + ".")));

                        bool passed = AttemptPass(ref game, q2, q3, q1, false);
                        team2stats.PassesAttempted++;

                        if (passed){
                            team2stats.Posession+=3;
                            team1stats.Posession-=3;
                            team2stats.Passes++;

                            q2.InGameStats.Rating+=0.5;
                            q3.InGameStats.Rating+=0.5;
                            // If new player (q3) has high finishing and high mentality, take a shot
                            // Otherwise attack fails
                            if (q3.Finishing >= 50){
                                int a = r.Next(0,100);
                                if (q3.Mentality > a){
                                    // shoot
                                    bool scored = AttemptShot(ref game, team1stats, team2stats, q3, false, false);
                                    if (scored) {
                                        team2_score++;
                                        
                                        // ShowTeam2Scored(ref game, team1, team2, team1_score, team2_score);
                                        game.AddToLog(new LogItem(ItemType.Text, ("> " + team1.Name + " " + team1_score + " - [")));
                                        game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
                                        game.AddToLog(new LogItem(ItemType.Text, team2_score.ToString()));
                                        game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                                        game.AddToLog(new LogItem(ItemType.Text, ("] " + team2.Name + "\n")));

                                        goals.Add(new Goal(q3.Name, team2.Name, i));
                                        try{playerStats.Find(pl => pl.Player == q3).GoalsScored++;
                                        playerStats.Find(pl => pl.Player == q2).Assists++;}catch{}
                                    }
                                }
                            }
                        }
                        else{
                            team1stats.Posession+=3;
                            team2stats.Posession-=3;
                            team1stats.Interceptions++;

                            q2.InGameStats.Rating-=0.5;
                            q1.InGameStats.Rating+=0.5;
                            // If new player (q1) has high finishing and high mentality, take a shot
                            // Otherwise attack fails
                            if (q1.Finishing >= 50){
                                int a = r.Next(0,100);
                                if (q1.Mentality > a){
                                    // shoot
                                    bool scored = AttemptShot(ref game, team1stats, team2stats, q1, true, false);
                                    if (scored) {
                                        team1_score++;
                                        
                                        // ShowTeam1Scored(ref game, team1, team2, team1_score, team2_score);
                                        game.AddToLog(new LogItem(ItemType.Text, ("> " + team1.Name + " [")));
                                        game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
                                        game.AddToLog(new LogItem(ItemType.Text, team1_score.ToString()));
                                        game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                                        game.AddToLog(new LogItem(ItemType.Text, ("] - " + team2_score + " " + team2.Name + "\n")));

                                        goals.Add(new Goal(q1.Name, team1.Name, i));
                                        try{playerStats.Find(pl => pl.Player == q1).GoalsScored++;}catch{}
                                    }
                                }
                            }
                        }
                        
                    }
                    else if (choice == 1){
                        // dribble then chance to shoot
                        //Console.WriteLine("> [" + team2.Name + "] " + q2.Name + " attempts to dribble.");
                        bool dribbled = AttemptDribble(ref game, q2, q1, false);
                        if (dribbled){
                            team2stats.Posession+=3;
                            team1stats.Posession-=3;

                            q2.InGameStats.Rating+=0.5;
                            q1.InGameStats.Rating-=0.1;
                            // If player (q2) has high finishing and high mentality, take a shot
                            // Otherwise attack fails
                            if (p2.Finishing >= 50){
                                int a = r.Next(0,100);
                                if (p2.Mentality > a){
                                    // shoot
                                    bool scored = AttemptShot(ref game, team1stats, team2stats, q2, false, false);
                                    if (scored) {
                                        team2_score++;
                                        
                                        // ShowTeam2Scored(ref game, team1, team2, team1_score, team2_score);
                                        game.AddToLog(new LogItem(ItemType.Text, ("> " + team1.Name + " " + team1_score + " - [")));
                                        game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
                                        game.AddToLog(new LogItem(ItemType.Text, team2_score.ToString()));
                                        game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                                        game.AddToLog(new LogItem(ItemType.Text, ("] " + team2.Name + "\n")));

                                        goals.Add(new Goal(q2.Name, team2.Name, i));
                                        try{playerStats.Find(pl => pl.Player == q2).GoalsScored++;}catch{}
                                    }
                                }
                            }
                        }
                        else{
                            team1stats.Tackles++;
                            int b = r.Next(0,50);
                            if (q1.Mentality < b){
                                q1.InGameStats.Rating-=0.3;
                                team1stats.Fouls++;
                                ///*if (show) */Console.WriteLine(q1.Name + " fouled " + q2.Name + ".");
                                game.AddToLog(new LogItem(ItemType.TextLine,(q1.Name + " fouled " + q2.Name + ".")));

                                int yellowNum = GenerateNormal(30,10);
                                double yellowThreshold = Sigmoid(yellowNum, q1.Mentality,-0.05,0);
                                int redNum = GenerateNormal(5,2);
                                double redThreshold = Sigmoid(redNum, q1.Mentality,-0.063,0.5);
                                while (redThreshold > yellowThreshold) {redNum = GenerateNormal(5,2); redThreshold = Sigmoid(redNum, q1.Mentality,-0.063,0.5);}
                                double randnum = r.NextDouble();
                                //Console.WriteLine("DEBUG 2: ");
                                //Console.WriteLine("yellowThreshold = " + yellowNum + ", redThreshold = " + redNum + ", mentality = " + q1.Mentality);
                                //Console.WriteLine("yellowChance = " + yellowThreshold + ", redChance = " + redThreshold + ", randnum = " + randnum);
                                if (randnum < redThreshold){
                                    //Console.WriteLine("should send off...");
                                    q1.InGameStats.GiveCard(ref game, false);
                                    q1.InGameStats.Rating-=2;
                                    team1stats.RedCards++;
                                }
                                else if (randnum < yellowThreshold){
                                    //Console.WriteLine("should book...");
                                    q1.InGameStats.GiveCard(ref game);
                                    q1.InGameStats.Rating-=0.5;
                                    team1stats.YellowCards++;
                                }
                                //else Console.WriteLine("should do nothing...");

                                bool scored;
                                if (q2.Position == Position.ST || q2.Position == Position.AM){
                                    scored = AttemptSetpiece(ref game, team1stats, team2stats, false, isTeam1);
                                }
                                else if (q2.Position == Position.LW || q2.Position == Position.RW || q2.Position == Position.LM || q2.Position == Position.RM || q2.Position == Position.CM){
                                    int c = r.Next(0,100);
                                    if (q2.Mentality > c) {
                                        scored = AttemptSetpiece(ref game, team1stats, team2stats, false, isTeam1);
                                    }
                                    else{
                                        scored = AttemptSetpiece(ref game, team1stats, team2stats, true, isTeam1);
                                    }
                                }
                                else{
                                    scored = AttemptSetpiece(ref game, team1stats, team2stats, true, isTeam1);
                                }
                                if (scored) {
                                    team2_score++;
                                    // ShowTeam2Scored(ref game, team1, team2, team1_score, team2_score);
                                    game.AddToLog(new LogItem(ItemType.Text, ("> " + team1.Name + " " + team1_score + " - [")));
                                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
                                    game.AddToLog(new LogItem(ItemType.Text, team2_score.ToString()));
                                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                                    game.AddToLog(new LogItem(ItemType.Text, ("] " + team2.Name + "\n")));
                                    goals.Add(new Goal (getSetPieceTaker(team2).Name, team2.Name,i));
                                }
                            }
                            else{
                                team1stats.Posession+=3;
                                team2stats.Posession-=3;

                                q1.InGameStats.Rating+=0.5;
                                q2.InGameStats.Rating-=0.5;
                                ///*if (show) */Console.WriteLine(q1.Name + " tackles " + q2.Name + ".");
                                game.AddToLog(new LogItem(ItemType.TextLine, (q1.Name + " tackles " + q2.Name + ".")));
                                // If new player (q1) has high finishing and high mentality, take a shot
                                // Otherwise attack fails
                                if (q1.Finishing >= 50){
                                    int a = r.Next(0,100);
                                    if (q1.Mentality > a){
                                        // shoot
                                        bool scored = AttemptShot(ref game, team1stats, team2stats, q1, true);
                                        if (scored) {
                                            team1_score++;
                                            
                                            // ShowTeam1Scored(ref game, team1, team2, team1_score, team2_score);
                                            game.AddToLog(new LogItem(ItemType.Text, ("> " + team1.Name + " [")));
                                            game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
                                            game.AddToLog(new LogItem(ItemType.Text, team1_score.ToString()));
                                            game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                                            game.AddToLog(new LogItem(ItemType.Text, ("] - " + team2_score + " " + team2.Name + "\n")));

                                            goals.Add(new Goal(q1.Name, team1.Name, i));
                                        }
                                    }
                                }
                            }
                            
                        }
                    }
                    else{
                        team2stats.Posession+=2;
                        team1stats.Posession-=2;
                        // shoot
                        bool scored = AttemptShot(ref game, team1stats, team2stats, q2, false, false);
                        if (scored) {
                            team2_score++;
                            
                            // ShowTeam2Scored(ref game, team1, team2, team1_score, team2_score);
                            game.AddToLog(new LogItem(ItemType.Text, ("> " + team1.Name + " " + team1_score + " - [")));
                            game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
                            game.AddToLog(new LogItem(ItemType.Text, team2_score.ToString()));
                            game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                            game.AddToLog(new LogItem(ItemType.Text, ("] " + team2.Name + "\n")));

                            goals.Add(new Goal(q2.Name, team2.Name, i));
                            try{playerStats.Find(pl => pl.Player == q2).GoalsScored++;}catch{}
                        }
                    }
                }
                game.AddToLog(new LogItem(ItemType.TextLine, ""));
                game.AddToLog(new LogItem(ItemType.Read, ""));
                isTeam1 = !isTeam1;
            }
            //Console.ForegroundColor = ConsoleColor.Red;
            game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Red"));
            //Console.WriteLine("FULL TIME!");
            game.AddToLog(new LogItem(ItemType.TextLine, ("FULL TIME!")));
            //Console.ForegroundColor = ConsoleColor.White;
            game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
            //Console.WriteLine("Final Score: " + team1.Name + " " + team1_score + " - " + team2_score + " " + team2.Name);
            game.AddToLog(new LogItem(ItemType.TextLine, ("Final Score: " + team1.Name + " " + team1_score + " - " + team2_score + " " + team2.Name)));

            //try{playerStats.Find(pl => pl.Player == p3).GoalsScored++;
            foreach (Player p in team1.Players) playerStats.Find(pl => pl.Player == p).AddRating((float)p.InGameStats.Rating);
            foreach (Player p in team2.Players) playerStats.Find(pl => pl.Player == p).AddRating((float)p.InGameStats.Rating);
            
            game.Team1Stats = team1stats;
            game.Team2Stats = team2stats;
            game.Goals = goals;
            game.team1 = team1;
            game.team2 = team2;
            game.team1Players = playersInGames1;
            game.team2Players = playersInGames2;

            List<int> score = new List<int>();
            score.Add(team1_score);
            score.Add(team2_score);

            game.Score = score;

            return game;
        }

        static bool AttemptPass(Player p1, Player p2, Player p3, bool show = true){
            Random rand = new Random(0);

            double passchance = Sigmoid(Math.Max(p1.Passing, p2.Passing), Math.Max(p3.Tackling,p3.GoalPrevention), -0.03, -0.5); // https://www.desmos.com/calculator/kn9tpwdan5
            double randnum = rand.NextDouble();
            if (randnum < passchance){
                if (show) Console.WriteLine("> The pass was successful.");
                return true;
            }
            else{
                if (show) Console.WriteLine("> The pass is intercepted by " + p3.Name +".");
                return false;
            }
        }

        static bool AttemptPass(ref Game game, Player p1, Player p2, Player p3, bool show = true){
            Random rand = new Random(0);

            double passchance = Sigmoid(Math.Max(p1.Passing, p2.Passing), Math.Max(p3.Tackling,p3.GoalPrevention), -0.03, -0.5); // https://www.desmos.com/calculator/kn9tpwdan5
            double randnum = rand.NextDouble();
            if (randnum < passchance){
                //if (show) Console.WriteLine("> The pass was successful.");
                game.AddToLog(new LogItem(ItemType.TextLine, ("> The pass was successful.")));
                return true;
            }
            else{
                //if (show) Console.WriteLine("> The pass is intercepted by " + p3.Name +".");
                game.AddToLog(new LogItem(ItemType.TextLine, ("> The pass is intercepted by " + p3.Name +".")));
                return false;
            }
        }

        static Player getSetPieceTaker(Team team){
            Player bestPlayer = new Player("",0,Position.GK, 0,0,0,0,0,0);

            foreach (var p in team.Players) {
                if (p.Finishing > bestPlayer.Finishing) bestPlayer = p;
            }

            return bestPlayer;
        }

        static bool AttemptDribble(Player p1, Player p2, bool show = true){
            Random rand = new Random(0);

            double dribblechance = Sigmoid(p1.Dribbling, p2.Tackling, -0.03, -0.5); // https://www.desmos.com/calculator/kn9tpwdan5
            double randnum = rand.NextDouble();
            if (show) Console.Write("> ");
            if (randnum < dribblechance){
                if (show) Console.WriteLine(p1.Name + " dribbled past " + p2.Name + ".");
                return true;
            }
            else{
                //if (show) Console.WriteLine(p2.Name + " tackled " + p1.Name + ".");
                return false;
            }
        }

        static bool AttemptDribble(ref Game game, Player p1, Player p2, bool show = true){
            Random rand = new Random(0);

            double dribblechance = Sigmoid(p1.Dribbling, p2.Tackling, -0.03, -0.5); // https://www.desmos.com/calculator/kn9tpwdan5
            double randnum = rand.NextDouble();
            //if (show) Console.Write("> ");
            game.AddToLog(new LogItem(ItemType.Text, "> "));
            if (randnum < dribblechance){
                //if (show) Console.WriteLine(p1.Name + " dribbled past " + p2.Name + ".");
                game.AddToLog(new LogItem(ItemType.TextLine, (p1.Name + " dribbled past " + p2.Name + ".")));
                return true;
            }
            else{
                //if (show) Console.WriteLine(p2.Name + " tackled " + p1.Name + ".");
                return false;
            }
        }

        // Simulate a set peice (free kick or penalty)
        static bool AttemptSetpiece(TeamGameStats team1, TeamGameStats team2, bool isFreeKick, bool isTeam1, bool show = true){
            Team teamTaking;

            if (isTeam1) teamTaking = team1.Team;
            else teamTaking = team2.Team;

            Player bestPlayer = getSetPieceTaker(teamTaking);

            Console.Write("> ");
            if (isFreeKick){
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("FREE KICK! ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(teamTaking.Name + " are awarded a free kick.");
                Console.Write("\n> [" + teamTaking.Name + "] " + bestPlayer.Name + " steps up to take the free kick...\n");
                //bestPlayer.Finishing -= 5;
            }
            else{
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write("PENALTY! ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(teamTaking.Name + " are awarded a penalty.");
                Console.Write("\n> [" + teamTaking.Name + "] " + bestPlayer.Name + " steps up to take the penalty...\n");
                bestPlayer.Finishing += 10;               
            }

            bool goal = AttemptShot(team1, team2, bestPlayer, isTeam1, show);
            
            return goal;
        }

        static bool AttemptSetpiece(ref Game game, TeamGameStats team1, TeamGameStats team2, bool isFreeKick, bool isTeam1, bool show = true){
            Team teamTaking;

            if (isTeam1) teamTaking = team1.Team;
            else teamTaking = team2.Team;

            Player bestPlayer = getSetPieceTaker(teamTaking);

            // Console.Write("> ");
            game.AddToLog(new LogItem(ItemType.Text, "> "));
            if (isFreeKick){
                // Console.ForegroundColor = ConsoleColor.Magenta;
                game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Magenta"));
                // Console.Write("FREE KICK! ");
                game.AddToLog(new LogItem(ItemType.Text, "FREE KICK! "));
                // Console.ForegroundColor = ConsoleColor.White;
                game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                // Console.Write(teamTaking.Name + " are awarded a free kick.");
                game.AddToLog(new LogItem(ItemType.Text, (teamTaking.Name + " are awarded a free kick.")));
                // Console.Write("\n> [" + teamTaking.Name + "] " + bestPlayer.Name + " steps up to take the free kick...\n");
                game.AddToLog(new LogItem(ItemType.Text, ("\n> [" + teamTaking.Name + "] " + bestPlayer.Name + " steps up to take the free kick...\n")));
                //bestPlayer.Finishing -= 5;
            }
            else{
                // Console.ForegroundColor = ConsoleColor.DarkMagenta;
                game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.DarkMagenta"));
                // Console.Write("PENALTY! ");
                game.AddToLog(new LogItem(ItemType.Text, "PENALTY! "));
                // Console.ForegroundColor = ConsoleColor.White;
                game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                // Console.Write(teamTaking.Name + " are awarded a penalty.");
                game.AddToLog(new LogItem(ItemType.Text, (teamTaking.Name + " are awarded a penalty.")));
                // Console.Write("\n> [" + teamTaking.Name + "] " + bestPlayer.Name + " steps up to take the penalty...\n");
                game.AddToLog(new LogItem(ItemType.Text, ("\n> [" + teamTaking.Name + "] " + bestPlayer.Name + " steps up to take the penalty...\n")));
                // bestPlayer.Finishing += 10;               
            }

            bool goal = AttemptShot(ref game, team1, team2, bestPlayer, isTeam1, show);
            
            return goal;
        }

        static bool AttemptShot(TeamGameStats team1stats, TeamGameStats team2stats, Player p1, bool isTeam1, bool show = true){ // Simulate a shot
            Player gk1 = team1stats.Team.Players[0]; // Team 1s goalkeeper
            Player gk2 = team2stats.Team.Players[0]; // Team 2s goalkeeper

            //Player p1 = p1_.Player;

            if (isTeam1) team1stats.Shots++;
            else team2stats.Shots++;

            Random rand = new Random();
            if (show) {
                if (isTeam1) Console.WriteLine("> [" + team1stats.Team.Name + "] " + p1.Name + " attempts a shot.");
                else Console.WriteLine("> [" + team2stats.Team.Name + "] " + p1.Name + " attempts a shot.");
            }

            if (show) Console.Write("> ");

            double hitchance = GenerateNormal(40,25);
            if (p1.Finishing < hitchance){
                p1.InGameStats.Rating-=0.5; // Lower rating for missed shot
                if (show) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("MISS. ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(p1.Name + " missed the target.");
                }
                return false;
            }

            if (isTeam1){
                team1stats.ShotsOnTarget++;
                double shotchance = Sigmoid(p1.Finishing, gk2.Tackling);
                double randnum = rand.NextDouble();
                if (randnum < shotchance){
                    p1.InGameStats.Rating+=1; // Increase rating for goal
                    gk2.InGameStats.Rating-=0.3; // Decrease keeper rating for a failed save
                    team1stats.Goals++;
                    if (show){
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("GOAL! ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(p1.Name + " scores for " + team1stats.Team.Name + "!");
                    }
                    return true;
                }
                else{
                    p1.InGameStats.Rating+=0.3; // Slightly increase rating for on target shot
                    gk2.InGameStats.Rating+=0.5; // Increase keeper rating for a save
                    team2stats.Saves++;
                    if (show){
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("SAVE! ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(gk2.Name + " saves the shot for " + team2stats.Team.Name + "!");
                    }
                    return false;
                }  
            }
            else{
                team2stats.ShotsOnTarget++;
                double shotchance = Sigmoid(p1.Finishing, gk1.Tackling);
                double randnum = rand.NextDouble();
                if (randnum < shotchance){
                    p1.InGameStats.Rating+=1; // Increase rating for goal
                    gk1.InGameStats.Rating-=0.3; // Decrease keeper rating for a failed save
                    team2stats.Goals++;
                    if (show){
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("GOAL! ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(p1.Name + " scores for " + team2stats.Team.Name + "!");
                    }
                    return true;
                }
                else{
                    p1.InGameStats.Rating+=0.3; // Slightly increase rating for on target shot
                    gk1.InGameStats.Rating+=0.5; // Increase keeper rating for a save
                    team1stats.Saves++;
                    if (show){
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("SAVE! ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(gk1.Name + " saves the shot for " + team1stats.Team.Name + "!");
                    }
                    return false;
                }  
            }        
        }

        static bool AttemptShot(ref Game game, TeamGameStats team1stats, TeamGameStats team2stats, Player p1, bool isTeam1, bool show = true){ // Simulate a shot
            Player gk1 = team1stats.Team.Players[0]; // Team 1s goalkeeper
            Player gk2 = team2stats.Team.Players[0]; // Team 2s goalkeeper

            //Player p1 = p1_.Player;

            if (isTeam1) team1stats.Shots++;
            else team2stats.Shots++;

            Random rand = new Random();
            if (isTeam1) game.AddToLog(new LogItem(ItemType.TextLine, ("> [" + team1stats.Team.Name + "] " + p1.Name + " attempts a shot.")));
            else game.AddToLog(new LogItem(ItemType.TextLine, ("> [" + team2stats.Team.Name + "] " + p1.Name + " attempts a shot.")));

            game.AddToLog(new LogItem(ItemType.Text, "> "));

            double hitchance = GenerateNormal(40,25);
            if (p1.Finishing < hitchance){
                p1.InGameStats.Rating-=0.5; // Lower rating for missed shot
                //Console.ForegroundColor = ConsoleColor.Red;
                game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Red"));
                //Console.Write("MISS. ");
                game.AddToLog(new LogItem(ItemType.Text, "MISS. "));
                //Console.ForegroundColor = ConsoleColor.White;
                game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                //Console.WriteLine(p1.Name + " missed the target.");
                game.AddToLog(new LogItem(ItemType.TextLine, (p1.Name + " missed the target.")));
                return false;
            }

            if (isTeam1){
                team1stats.ShotsOnTarget++;
                double shotchance = Sigmoid(p1.Finishing, gk2.Tackling);
                double randnum = rand.NextDouble();
                if (randnum < shotchance){
                    p1.InGameStats.Rating+=1; // Increase rating for goal
                    gk2.InGameStats.Rating-=0.3; // Decrease keeper rating for a failed save
                    team1stats.Goals++;
                    // Console.ForegroundColor = ConsoleColor.Green;
                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
                    // Console.Write("GOAL! ");
                    game.AddToLog(new LogItem(ItemType.Text, "GOAL! "));
                    // Console.ForegroundColor = ConsoleColor.White;
                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                    // Console.WriteLine(p1.Name + " scores for " + team1stats.Team.Name + "!");
                    game.AddToLog(new LogItem(ItemType.TextLine, (p1.Name + " scores for " + team1stats.Team.Name + "!")));
                    return true;
                }
                else{
                    p1.InGameStats.Rating+=0.3; // Slightly increase rating for on target shot
                    gk2.InGameStats.Rating+=0.5; // Increase keeper rating for a save
                    team2stats.Saves++;
                    // Console.ForegroundColor = ConsoleColor.Yellow;
                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Yellow"));
                    // Console.Write("SAVE! ");
                    game.AddToLog(new LogItem(ItemType.Text, "SAVE! "));
                    // Console.ForegroundColor = ConsoleColor.White;
                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                    // Console.WriteLine(gk2.Name + " saves the shot for " + team2stats.Team.Name + "!");
                    game.AddToLog(new LogItem(ItemType.TextLine, (gk2.Name + " saves the shot for " + team2stats.Team.Name + "!")));
                    return false;
                }  
            }
            else{
                team2stats.ShotsOnTarget++;
                double shotchance = Sigmoid(p1.Finishing, gk1.Tackling);
                double randnum = rand.NextDouble();
                if (randnum < shotchance){
                    p1.InGameStats.Rating+=1; // Increase rating for goal
                    gk1.InGameStats.Rating-=0.3; // Decrease keeper rating for a failed save
                    team2stats.Goals++;
                    // Console.ForegroundColor = ConsoleColor.Green;
                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Green"));
                    // Console.Write("GOAL! ");
                    game.AddToLog(new LogItem(ItemType.Text, "GOAL! "));
                    // Console.ForegroundColor = ConsoleColor.White;
                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                    // Console.WriteLine(p1.Name + " scores for " + team2stats.Team.Name + "!");
                    game.AddToLog(new LogItem(ItemType.TextLine, (p1.Name + " scores for " + team2stats.Team.Name + "!")));
                    return true;
                }
                else{
                    p1.InGameStats.Rating+=0.3; // Slightly increase rating for on target shot
                    gk1.InGameStats.Rating+=0.5; // Increase keeper rating for a save
                    team1stats.Saves++;
                    // Console.ForegroundColor = ConsoleColor.Yellow;
                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.Yellow"));
                    // Console.Write("SAVE! ");
                    game.AddToLog(new LogItem(ItemType.Text, "SAVE! "));
                    // Console.ForegroundColor = ConsoleColor.White;
                    game.AddToLog(new LogItem(ItemType.Colour, "ConsoleColor.White"));
                    // Console.WriteLine(gk2.Name + " saves the shot for " + team2stats.Team.Name + "!");
                    game.AddToLog(new LogItem(ItemType.TextLine, (gk2.Name + " saves the shot for " + team1stats.Team.Name + "!")));
                    return false;
                }  
            }        
        }

        static double Sigmoid(int stat1, int stat2, double tightness = -0.05, double offset = 0.5){
            int x = stat1 - stat2;
            double b = tightness;
            return 1/(1+Math.Exp(offset+b*x)); // Sigmoid centred around 0 (0 -> 0.5)
            // If stats are the same, 50% chance. If stat1 (e.g. finishing) is much bigger than stat2 (e.g. defence), high probability, else low
        }

        static List<Team> GenerateTeams(){
            List<Team> teams = new List<Team>();

            List<Player> team1 = new List<Player>();

            team1.Add(GeneratePlayer(Position.GK,1));
            team1.Add(GeneratePlayer(Position.LB,2));
            team1.Add(GeneratePlayer(Position.RB,3));
            team1.Add(GeneratePlayer(Position.CB,4));
            team1.Add(GeneratePlayer(Position.CB,5));
            team1.Add(GeneratePlayer(Position.DM,6));
            team1.Add(GeneratePlayer(Position.LM,7));
            team1.Add(GeneratePlayer(Position.DM,8));
            team1.Add(GeneratePlayer(Position.ST,9));
            team1.Add(GeneratePlayer(Position.AM,10));
            team1.Add(GeneratePlayer(Position.RM,11));

            Team team_1 = new Team(GenerateTeamName());
            foreach (Player p in team1) team_1.AddPlayer(p);
            team_1.CalculateRating();

            List<Player> team2 = new List<Player>();

            team2.Add(GeneratePlayer(Position.GK,1));
            team2.Add(GeneratePlayer(Position.LB,2));
            team2.Add(GeneratePlayer(Position.RB,3));
            team2.Add(GeneratePlayer(Position.CB,4));
            team2.Add(GeneratePlayer(Position.CB,5));
            team2.Add(GeneratePlayer(Position.DM,6));
            team2.Add(GeneratePlayer(Position.LM,7));
            team2.Add(GeneratePlayer(Position.DM,8));
            team2.Add(GeneratePlayer(Position.ST,9));
            team2.Add(GeneratePlayer(Position.AM,10));
            team2.Add(GeneratePlayer(Position.RM,11));

            Team team_2 = new Team(GenerateTeamName());
            foreach (Player p in team2) team_2.AddPlayer(p);
            team_2.CalculateRating();

            teams.Add(team_1);
            teams.Add(team_2);

            return teams;
        }

        static void LoadTeams(){
            Console.WriteLine("Hello");

            Console.WriteLine("Enter team 1 file name: ");
            Console.Write("> ");

            //Team team1 = ReadTeam("Manchester United");
            Team team1 = ReadTeam(Console.ReadLine());

            Console.WriteLine("Loaded " + team1.Name);

            Team team2 = new Team("");
            bool v = false;
            //Team team1 = ReadTeam("Manchester United");
            while (!v){
                Console.WriteLine("Enter team 2 file name: ");
                Console.Write("> ");
                try{
                    team2 = ReadTeam(Console.ReadLine());
                }
                catch(Exception e){
                    Console.WriteLine("Invalid file, try again");
                    if (e.Equals(e)) {}
                }
                if (team2.Name != null && team2.Name != "") v = true;
            }
            
            Console.WriteLine("Loaded " + team2.Name);

            Console.WriteLine("Team 1: " + team1.Name);
            Console.Write("TEAM OVERALL: ");
            team1.CalculateRating();
            PrintWithColour(team1.Rating);
            
            Console.WriteLine("\nTeam 2: " + team2.Name);
            Console.Write("TEAM OVERALL: ");
            team2.CalculateRating();
            PrintWithColour(team2.Rating);

            Console.WriteLine("\n");

            int option = Menu();

            while (option != 9){
                if (option == 8){
                    //SaveTeam(team1, "team1");
                    //SaveTeam(team2, "team2");
                    team1.SaveTeam();
                    team2.SaveTeam();
                }
                else if (option == 9) break;
                else if (option == 10) SimulateGame(team1, team2, true);
                else if (option == 11) SimulateGame(team1, team2, false);
                else if (option == 12){
                    SetupTeams();
                }
                else{
                    Console.WriteLine("\nTeam 1: " + team1.Name + "\n");
                    PrintTeam(team1.Players.ToList(), option);
                    Console.Write("\nTEAM OVERALL: ");
                    PrintWithColour(CalculateTeamOverall(team1.Players.ToList()));

                    Console.WriteLine("\nTeam 2: " + team2.Name + "\n");
                    PrintTeam(team2.Players.ToList(), option);
                    Console.Write("\nTEAM OVERALL: ");
                    PrintWithColour(CalculateTeamOverall(team2.Players.ToList()));
                }
                if (option != 9) {
                    Console.WriteLine("\nPRESS ENTER TO CONTINUE");
                    Console.ReadLine();
                    option = Menu();
                }
            }
        }

        static void SetupTeams(){
            List<Team> teams = GenerateTeams();
            Team team_1 = teams[0];
            List<Player> team1 = team_1.Players.ToList();
            Team team_2 = teams[1];
            List<Player> team2 = team_2.Players.ToList();
            
            Console.WriteLine("Team 1: " + team_1.Name);
            Console.Write("TEAM OVERALL: ");
            PrintWithColour(team_1.Rating);
            
            Console.WriteLine("\nTeam 2: " + team_2.Name);
            Console.Write("TEAM OVERALL: ");
            PrintWithColour(team_2.Rating);

            Console.WriteLine("\n");

            int option = Menu();

            while (option != 9){
                if (option == 8){
                    //SaveTeam(team1, "team1");
                    //SaveTeam(team2, "team2");
                    team_1.SaveTeam();
                    team_2.SaveTeam();
                }
                if (option == 9) break;
                else if (option == 10) SimulateGame(team_1, team_2, true);
                else if (option == 11) SimulateGame(team_1, team_2, false);
                else if (option == 12){
                    teams = GenerateTeams();
                    team_1 = teams[0];
                    team1 = team_1.Players.ToList();
                    team_2 = teams[1];
                    team2 = team_2.Players.ToList();

                    Console.WriteLine("Team 1: " + team_1.Name);
                    Console.Write("TEAM OVERALL: ");
                    PrintWithColour(team_1.Rating);
                    
                    Console.WriteLine("\nTeam 2: " + team_2.Name);
                    Console.Write("TEAM OVERALL: ");
                    PrintWithColour(team_2.Rating);
                }
                else{
                    Console.WriteLine("\nTeam 1: " + team_1.Name + "\n");
                    PrintTeam(team1, option);
                    Console.Write("\nTEAM OVERALL: ");
                    PrintWithColour(CalculateTeamOverall(team1));

                    Console.WriteLine("\nTeam 2: " + team_2.Name + "\n");
                    PrintTeam(team2, option);
                    Console.Write("\nTEAM OVERALL: ");
                    PrintWithColour(CalculateTeamOverall(team2));
                }
                if (option != 9) {
                    Console.WriteLine("\nPRESS ENTER TO CONTINUE");
                    Console.ReadLine();
                    option = Menu();
                }
            }
        }

        static void PrintTeam(List<Player> t){
            /*
            -------[ST]-------
            ------------------
            [LM]---[AM]---[RM]
            ------------------
            ---[DM]----[DM]---
            ------------------
            [LB]-[CB][CB]-[RB]
            -------[GK]-------
            */

            Console.Write("-------");
            PrintPositionWithColour(t[9-1].Overall); // ST
            Console.Write("-------");
            Console.WriteLine("\n------------------");
            PrintPositionWithColour(t[7-1].Overall); // LM
            Console.Write("---");
            PrintPositionWithColour(t[10-1].Overall); // AM
            Console.Write("---");
            PrintPositionWithColour(t[11-1].Overall); // RM
            Console.WriteLine("\n------------------");
            Console.Write("---");
            PrintPositionWithColour(t[6-1].Overall); // DM
            Console.Write("----");
            PrintPositionWithColour(t[8-1].Overall); // DM
            Console.Write("---");
            Console.WriteLine("\n------------------");
            PrintPositionWithColour(t[2-1].Overall); // LB
            Console.Write("-");
            PrintPositionWithColour(t[4-1].Overall); // CB
            PrintPositionWithColour(t[5-1].Overall); // CB
            Console.Write("-");
            PrintPositionWithColour(t[3-1].Overall); // RB
            Console.Write("\n-------");
            PrintPositionWithColour(t[1-1].Overall); // GK
            Console.Write("-------\n");
        }

        static void PrintTeam(List<Player> t, int mode){
            /*
            -------[ST]-------
            ------------------
            [LM]---[AM]---[RM]
            ------------------
            ---[DM]----[DM]---
            ------------------
            [LB]-[CB][CB]-[RB]
            -------[GK]-------
            */

            switch (mode){
                case 0:
                    Console.WriteLine("--------[9]-------\n------------------\n[7]----[10]---[11]\n------------------\n----[6]-----[8]---\n------------------\n[2]--[4]--[5]--[3]\n--------[1]-------");
                    foreach (var p in t) {Console.Write(p.ToString() + ", "); PrintWithColour(p.Overall); }
                    break;
                case 1:
                    Console.Write("-------");
                    PrintPositionWithColour(t[9-1].Overall); // ST
                    Console.Write("-------");
                    Console.WriteLine("\n------------------");
                    PrintPositionWithColour(t[7-1].Overall); // LM
                    Console.Write("---");
                    PrintPositionWithColour(t[10-1].Overall); // AM
                    Console.Write("---");
                    PrintPositionWithColour(t[11-1].Overall); // RM
                    Console.WriteLine("\n------------------");
                    Console.Write("---");
                    PrintPositionWithColour(t[6-1].Overall); // DM
                    Console.Write("----");
                    PrintPositionWithColour(t[8-1].Overall); // DM
                    Console.Write("---");
                    Console.WriteLine("\n------------------");
                    PrintPositionWithColour(t[2-1].Overall); // LB
                    Console.Write("-");
                    PrintPositionWithColour(t[4-1].Overall); // CB
                    PrintPositionWithColour(t[5-1].Overall); // CB
                    Console.Write("-");
                    PrintPositionWithColour(t[3-1].Overall); // RB
                    Console.Write("\n-------");
                    PrintPositionWithColour(t[1-1].Overall); // GK
                    Console.Write("-------\n");
                    break;
                case 2:
                    Console.WriteLine("--------[9]-------\n------------------\n[7]----[10]---[11]\n------------------\n----[6]-----[8]---\n------------------\n[2]--[4]--[5]--[3]\n--------[1]-------");
                    break;
                case 3:
                    Console.Write("-------");
                    PrintPositionWithColour(t[9-1].Dribbling); // ST
                    Console.Write("-------");
                    Console.WriteLine("\n------------------");
                    PrintPositionWithColour(t[7-1].Dribbling); // LM
                    Console.Write("---");
                    PrintPositionWithColour(t[10-1].Dribbling); // AM
                    Console.Write("---");
                    PrintPositionWithColour(t[11-1].Dribbling); // RM
                    Console.WriteLine("\n------------------");
                    Console.Write("---");
                    PrintPositionWithColour(t[6-1].Dribbling); // DM
                    Console.Write("----");
                    PrintPositionWithColour(t[8-1].Dribbling); // DM
                    Console.Write("---");
                    Console.WriteLine("\n------------------");
                    PrintPositionWithColour(t[2-1].Dribbling); // LB
                    Console.Write("-");
                    PrintPositionWithColour(t[4-1].Dribbling); // CB
                    PrintPositionWithColour(t[5-1].Dribbling); // CB
                    Console.Write("-");
                    PrintPositionWithColour(t[3-1].Dribbling); // RB
                    Console.Write("\n-------");
                    PrintPositionWithColour(t[1-1].Dribbling); // GK
                    Console.Write("-------\n");
                    break;
                case 4:
                    Console.Write("-------");
                    PrintPositionWithColour(t[9-1].Finishing); // ST
                    Console.Write("-------");
                    Console.WriteLine("\n------------------");
                    PrintPositionWithColour(t[7-1].Finishing); // LM
                    Console.Write("---");
                    PrintPositionWithColour(t[10-1].Finishing); // AM
                    Console.Write("---");
                    PrintPositionWithColour(t[11-1].Finishing); // RM
                    Console.WriteLine("\n------------------");
                    Console.Write("---");
                    PrintPositionWithColour(t[6-1].Finishing); // DM
                    Console.Write("----");
                    PrintPositionWithColour(t[8-1].Finishing); // DM
                    Console.Write("---");
                    Console.WriteLine("\n------------------");
                    PrintPositionWithColour(t[2-1].Finishing); // LB
                    Console.Write("-");
                    PrintPositionWithColour(t[4-1].Finishing); // CB
                    PrintPositionWithColour(t[5-1].Finishing); // CB
                    Console.Write("-");
                    PrintPositionWithColour(t[3-1].Finishing); // RB
                    Console.Write("\n-------");
                    PrintPositionWithColour(t[1-1].Finishing); // GK
                    Console.Write("-------\n");
                    break;
                case 5:
                    Console.Write("-------");
                    PrintPositionWithColour(t[9-1].Tackling); // ST
                    Console.Write("-------");
                    Console.WriteLine("\n------------------");
                    PrintPositionWithColour(t[7-1].Tackling); // LM
                    Console.Write("---");
                    PrintPositionWithColour(t[10-1].Tackling); // AM
                    Console.Write("---");
                    PrintPositionWithColour(t[11-1].Tackling); // RM
                    Console.WriteLine("\n------------------");
                    Console.Write("---");
                    PrintPositionWithColour(t[6-1].Tackling); // DM
                    Console.Write("----");
                    PrintPositionWithColour(t[8-1].Tackling); // DM
                    Console.Write("---");
                    Console.WriteLine("\n------------------");
                    PrintPositionWithColour(t[2-1].Tackling); // LB
                    Console.Write("-");
                    PrintPositionWithColour(t[4-1].Tackling); // CB
                    PrintPositionWithColour(t[5-1].Tackling); // CB
                    Console.Write("-");
                    PrintPositionWithColour(t[3-1].Tackling); // RB
                    Console.Write("\n-------");
                    PrintPositionWithColour(t[1-1].Tackling); // GK
                    Console.Write("-------\n");
                    break;
                case 6:
                    Console.Write("-------");
                    PrintPositionWithColour(t[9-1].Passing); // ST
                    Console.Write("-------");
                    Console.WriteLine("\n------------------");
                    PrintPositionWithColour(t[7-1].Passing); // LM
                    Console.Write("---");
                    PrintPositionWithColour(t[10-1].Passing); // AM
                    Console.Write("---");
                    PrintPositionWithColour(t[11-1].Passing); // RM
                    Console.WriteLine("\n------------------");
                    Console.Write("---");
                    PrintPositionWithColour(t[6-1].Passing); // DM
                    Console.Write("----");
                    PrintPositionWithColour(t[8-1].Passing); // DM
                    Console.Write("---");
                    Console.WriteLine("\n------------------");
                    PrintPositionWithColour(t[2-1].Passing); // LB
                    Console.Write("-");
                    PrintPositionWithColour(t[4-1].Passing); // CB
                    PrintPositionWithColour(t[5-1].Passing); // CB
                    Console.Write("-");
                    PrintPositionWithColour(t[3-1].Passing); // RB
                    Console.Write("\n-------");
                    PrintPositionWithColour(t[1-1].Passing); // GK
                    Console.Write("-------\n");
                    break;
                case 7:
                    Console.Write("-------");
                    PrintPositionWithColour(t[9-1].Mentality); // ST
                    Console.Write("-------");
                    Console.WriteLine("\n------------------");
                    PrintPositionWithColour(t[7-1].Mentality); // LM
                    Console.Write("---");
                    PrintPositionWithColour(t[10-1].Mentality); // AM
                    Console.Write("---");
                    PrintPositionWithColour(t[11-1].Mentality); // RM
                    Console.WriteLine("\n------------------");
                    Console.Write("---");
                    PrintPositionWithColour(t[6-1].Mentality); // DM
                    Console.Write("----");
                    PrintPositionWithColour(t[8-1].Mentality); // DM
                    Console.Write("---");
                    Console.WriteLine("\n------------------");
                    PrintPositionWithColour(t[2-1].Mentality); // LB
                    Console.Write("-");
                    PrintPositionWithColour(t[4-1].Mentality); // CB
                    PrintPositionWithColour(t[5-1].Mentality); // CB
                    Console.Write("-");
                    PrintPositionWithColour(t[3-1].Mentality); // RB
                    Console.Write("\n-------");
                    PrintPositionWithColour(t[1-1].Mentality); // GK
                    Console.Write("-------\n");
                    break;
                case 8:
                    // TODO
                    break;
                default:
                    Console.Write("-------");
                    PrintPositionWithColour(t[9-1].Overall); // ST
                    Console.Write("-------");
                    Console.WriteLine("\n------------------");
                    PrintPositionWithColour(t[7-1].Overall); // LM
                    Console.Write("---");
                    PrintPositionWithColour(t[10-1].Overall); // AM
                    Console.Write("---");
                    PrintPositionWithColour(t[11-1].Overall); // RM
                    Console.WriteLine("\n------------------");
                    Console.Write("---");
                    PrintPositionWithColour(t[6-1].Overall); // DM
                    Console.Write("----");
                    PrintPositionWithColour(t[8-1].Overall); // DM
                    Console.Write("---");
                    Console.WriteLine("\n------------------");
                    PrintPositionWithColour(t[2-1].Overall); // LB
                    Console.Write("-");
                    PrintPositionWithColour(t[4-1].Overall); // CB
                    PrintPositionWithColour(t[5-1].Overall); // CB
                    Console.Write("-");
                    PrintPositionWithColour(t[3-1].Overall); // RB
                    Console.Write("\n-------");
                    PrintPositionWithColour(t[1-1].Overall); // GK
                    Console.Write("-------\n");
                    break;
            }
        }

        static void PrintRatingWithColour(double rating, bool newline = true){
            if (rating <= 5) Console.ForegroundColor = ConsoleColor.DarkRed;
            else if (rating < 6) Console.ForegroundColor = ConsoleColor.Red;
            else if (rating < 7) Console.ForegroundColor = ConsoleColor.Yellow;
            else if (rating < 8) Console.ForegroundColor = ConsoleColor.Green;
            else if (rating < 9) Console.ForegroundColor = ConsoleColor.DarkGreen;
            else Console.ForegroundColor = ConsoleColor.Cyan;

            Console.Write(rating.ToString("0.0"));
            if (newline) Console.Write("\n");

            Console.ForegroundColor = ConsoleColor.White;
        }

        static void PrintRatings(List<PlayerInGame> t1, List<PlayerInGame> t2, Team team1, Team team2){
            Console.WriteLine("PLAYER RATINGS: \n");
            Console.WriteLine(team1.Name + ":");
            
            Console.WriteLine("--------[9]-------\n------------------\n[7]----[10]---[11]\n------------------\n----[6]-----[8]---\n------------------\n[2]--[4]--[5]--[3]\n--------[1]-------\n");
            /*foreach (var p in t1){
                Console.Write(p.Player.ToString() + ", "); 
                PrintRatingWithColour(p.Rating);
            }*/

            foreach (var p in team1.Players){
                Console.Write(p.ToString() + ", "); 
                PrintRatingWithColour(p.InGameStats.Rating);
            }

            Console.WriteLine("\n"+team2.Name + ":");
            
            Console.WriteLine("\n--------[9]-------\n------------------\n[7]----[10]---[11]\n------------------\n----[6]-----[8]---\n------------------\n[2]--[4]--[5]--[3]\n--------[1]-------\n");
            /*foreach (var p in t2){
                Console.Write(p.Player.ToString() + ", "); 
                PrintRatingWithColour(p.Rating);
            }*/

            foreach (var p in team2.Players){
                Console.Write(p.ToString() + ", "); 
                PrintRatingWithColour(p.InGameStats.Rating);
            }
            //foreach (var p in t) {Console.Write(p.ToString() + ", "); PrintWithColour(p.Overall); }
        }

        static void SaveTeam(List<Player> t, string filename){
            int count = 1;
            string fullPath = "assets/teams/" + filename + ".txt";

            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while(File.Exists(newFullPath)) 
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }

            using (StreamWriter writer = new StreamWriter(newFullPath)) 
            {    
                foreach (Player p in t){
                    writer.WriteLine(p.PrintingRepresentation());
                }
            }
        }

        static Team ReadTeam(string filename){
            //List<Player> newteam = new List<Player>();

            var team_file = File.ReadAllLines("assets/teams/"+filename+".txt");

            string teamname = team_file[0];
            Team new_team = new Team(teamname);

            string[] separator = {","};

            for (int i = 1; i <= 11; i++) {
                string line = team_file[i];
                string[] stats = line.Split(separator, 8, StringSplitOptions.RemoveEmptyEntries);
                Player p = new Player(stats[0], int.Parse(stats[1]), (Position)int.Parse(stats[2]), int.Parse(stats[3]), int.Parse(stats[4]), int.Parse(stats[5]), int.Parse(stats[6]), int.Parse(stats[7]),0); // TODO CHANGE CHANGE
                new_team.AddPlayer(p);
            }

            // TODO
            // Split by line (player) and ',' (stats)
            // Create player object and add to list to return

            return new_team;
        }

        static int CalculateTeamOverall(List<Player> t){
            int ovr = 0;
            foreach (var p in t) ovr += p.Overall;
            ovr /= 11;
            return ovr;
        }

        static ConsoleColor GetColour(int x){
            if (x > 90) return ConsoleColor.DarkGreen;
            else if (x > 75) return ConsoleColor.Green;
            else if (x > 50) return ConsoleColor.Yellow;
            else if (x > 30) return ConsoleColor.Red;
            else return ConsoleColor.DarkRed;
        }

        static void PrintWithColour(int x){
            ConsoleColor c = GetColour(x);
            Console.ForegroundColor = c;
            Console.WriteLine(x);
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void PrintPositionWithColour(int x){
            ConsoleColor c = GetColour(x);
            Console.Write("[");
            Console.ForegroundColor = c;
            Console.Write(x);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("]");
        }

        static Player GeneratePlayer(){
            Player newPlayer = new Player(
                GeneratePlayerName(),
                9,
                Position.ST,
                GenerateDribbling(Position.ST),
                GenerateFinishing(Position.ST),
                GenerateDefence(Position.ST),
                GeneratePassing(Position.ST),
                GenerateAssisting(Position.ST),
                GenerateDefence(Position.ST) // This is goal prevention but using defense should be fine for now
            );

            return newPlayer;
        }

        static Player GeneratePlayer(Position p, int number){
            Player newPlayer = new Player(
                GeneratePlayerName(),
                number,
                p,
                GenerateDribbling(p),
                GenerateFinishing(p),
                GenerateDefence(p),
                GenerateDefence(p),
                GeneratePassing(p),
                GenerateAssisting(p)
            );

            return newPlayer;
        }

        // TODO: Change firstname list to only include male names
        static string GeneratePlayerName(){
            // TODO
            string firstname = "";
            string surname = "";

            //int fn_len = 18239;
            //int sn_len = 20000;

            var rand = new Random();

            var fn_file = File.ReadAllLines("assets/firstnames.txt");
            firstname = fn_file[rand.Next(0, fn_file.Length-1)];

            var sn_file = File.ReadAllLines("assets/surnames.txt");
            surname = sn_file[rand.Next(0, sn_file.Length-1)];

            return firstname + " " + surname;
        }

        static string GenerateTeamName(){
            // TODO
            string town = "";
            string extension = "";

            //int fn_len = 18239;
            //int sn_len = 20000;

            var rand = new Random();

            var town_file = File.ReadAllLines("assets/towns.txt");
            town = town_file[rand.Next(0, town_file.Length-1)];

            var ex_file = File.ReadAllLines("assets/team_extensions.txt");
            extension = ex_file[rand.Next(0, ex_file.Length-1)];

            return town + " " + extension;
        }

        static int GenerateNormal(int mean, float std){
            Random rand = new Random();

            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();

            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            //Console.WriteLine(randStdNormal);
            double randNormal = mean + std * randStdNormal; //random normal(mean,stdDev^2)
            //Console.WriteLine(randNormal);
            return (int)randNormal; //(int)Math.Round(randNormal * 100,0);
        }

        static int GenerateDribbling(Position p){
            var rand = new Random();

            switch (p){
                case Position.GK:
                    return GenerateNormal(25,12);
                case Position.LB:
                    return GenerateNormal(60, 10);
                case Position.RB:
                    return GenerateNormal(60, 10);
                case Position.CB:
                    return GenerateNormal(40, 15);
                case Position.DM:
                    return GenerateNormal(50, 15);
                case Position.AM:
                    return GenerateNormal(65, 15);
                case Position.CM:
                    return GenerateNormal(60, 13);
                case Position.LM:
                    return GenerateNormal(70, 15);
                case Position.RM:
                    return GenerateNormal(70, 15);
                case Position.RW:
                    return GenerateNormal(72, 12);
                case Position.LW:
                    return GenerateNormal(72, 12);;
                case Position.ST:
                    return GenerateNormal(65, 12);
                default:
                    return GenerateNormal(50, 25);
            }
        }

        static int GenerateFinishing(Position p){
            var rand = new Random();

            switch (p){
                case Position.GK:
                    return GenerateNormal(15, 5);
                case Position.LB:
                    return GenerateNormal(35, 10);
                case Position.RB:
                    return GenerateNormal(35, 10);
                case Position.CB:
                    return GenerateNormal(30, 10);
                case Position.DM:
                    return GenerateNormal(45, 15);
                case Position.AM:
                    return GenerateNormal(65, 20);
                case Position.CM:
                    return GenerateNormal(55, 15);
                case Position.LM:
                    return GenerateNormal(70, 10);
                case Position.RM:
                    return GenerateNormal(70, 10);
                case Position.RW:
                    return GenerateNormal(75, 10);
                case Position.LW:
                    return GenerateNormal(75, 10);
                case Position.ST:
                    return GenerateNormal(85, 5);
                default:
                    return GenerateNormal(50, 25);
            }
        }

        static int GenerateDefence(Position p){
            var rand = new Random();

            switch (p){
                case Position.GK:
                    return GenerateNormal(75, 12);
                case Position.LB:
                    return GenerateNormal(70, 10);
                case Position.RB:
                    return GenerateNormal(70, 10);
                case Position.CB:
                    return GenerateNormal(80, 10);
                case Position.DM:
                    return GenerateNormal(80, 10);
                case Position.AM:
                    return GenerateNormal(45, 15);
                case Position.CM:
                    return GenerateNormal(60, 10);
                case Position.LM:
                    return GenerateNormal(57, 8);
                case Position.RM:
                    return GenerateNormal(57, 8);
                case Position.RW:
                    return GenerateNormal(53, 8);
                case Position.LW:
                    return GenerateNormal(53, 8);
                case Position.ST:
                    return GenerateNormal(40, 10);
                default:
                    return GenerateNormal(50, 25);
            }
        }

        static int GeneratePassing(Position p){
            var rand = new Random();

            return GenerateNormal(60, 25);
        }

        static int GenerateAssisting(Position p){
            var rand = new Random();

            return GenerateNormal(60, 25);
        }

        public static void PrintIntroduction(object filename) {
            var f = File.ReadAllLines(string.Format("csvs/ENG1/Manchester Utd/{0}",filename));
            //var f = open("csvs/Manchester Utd/{0}".format(filename), "r");
            //var info = f.readline().rstrip("\n").split(",");
            var info = f[0].Split(',');
            var forename = info[1];
            //forename = str(unicodedata.normalize('NFKD', info[1]))
            var surname = info[0];
            //surname = unicodedata.normalize('NFKD', info[0])
            var fullname = "";
            if (forename == "") {
                fullname = surname;
            } else {
                //fullname = "{0} {1}".format(forename, surname);
                fullname = forename + " " + surname;
            }
            var shirtname = "";
            if (forename == "") {
                shirtname = surname;
            } else {
                //shirtname = "{0}. {1}".format(forename[0], surname);
                shirtname = forename[0] + ". " + surname;
            }
            var team = info[2];
            //var positions = f.readline().rstrip("\n").split(",");
            var positions = f[1].Split(',');
            var pos1 = positions[0];
            var pos2 = positions[1];
            var pos3 = positions[2];
            var introduction = "";
            if (pos2 == "" && pos3 == "") {
                //introduction = "{0} plays for {1} as a {2}".format(fullname, team, pos1);
                introduction = String.Format("{0} plays for {1} as a {2}", fullname, team, pos1);
            } else if (pos3 == "") {
                //introduction = "{0} plays for {1} as a {2}, but can also play as a {3}".format(fullname, team, pos1, pos2);
                introduction = String.Format("{0} plays for {1} as a {2}, but can also play as a {3}", fullname, team, pos1, pos2);
            } else {
                //introduction = "{0} plays for {1} as a {2}, but can also play as a {3} or a {4}".format(fullname, team, pos1, pos2, pos3);
                introduction = String.Format("{0} plays for {1} as a {2}, but can also play as a {3} or a {4}", fullname, team, pos1, pos2, pos3);
            }
            Console.WriteLine(introduction);
        }
        
        public static int GetValue(string s, List<(string, int)> stats) {
            var l = (from x in stats
                where x.Item1 ==s
                select x).ToList();
            try {
                return Convert.ToInt32(l[0].Item2);
            } catch {
                return 0;
            }
        }
        
        static Position ConvertToPosition(string s){
            switch (s){
                case "GK": return Position.GK;
                case "LB": return Position.LB;
                case "RB": return Position.RB;
                case "CB": return Position.CB;
                case "LCB": return Position.CB;
                case "RCB": return Position.CB;
                case "LM": return Position.LM;
                case "RM": return Position.RM;
                case "CM": return Position.CM;
                case "AM": return Position.AM;
                case "DM": return Position.DM;
                case "RAM": return Position.AM;
                case "RDM": return Position.DM;
                case "LAM": return Position.AM;
                case "LDM": return Position.DM;
                case "RW": return Position.RW;
                case "LW": return Position.LW;
                case "ST": return Position.ST;
                case "CF": return Position.ST;
                case "DF": return Position.DF;
                case "MF": return Position.MF;
                case "FW": return Position.FW;
                case "WM": return Position.LM;
                case "WB": return Position.LB;
                case "WF": return Position.LW;
                default: return Position.MF;
            }
        }

        public static Player GenerateRealPlayer(string filepath) {
            var f = File.ReadAllLines(filepath);
            var info = f[0].Split(',');
            var forename = info[1];
            var surname = info[0];
            var fullname = "";
            if (forename == "") {
                fullname = surname;
            } else {
                fullname = forename + " " + surname;
            }
            var shirtname = "";
            if (forename == "") {
                shirtname = surname;
            } else {
                shirtname = forename[0] + ". " + surname;
            }
            fullname = fullname.Normalize();
            var team = info[2];
            var positions = f[1].Split(',');
            var pos1 = positions[0];
            var pos2 = positions[1];
            var pos3 = positions[2];
            
            int FW_TOT = 0;
            int FW_AVG = 0;
            int MF_TOT = 0;
            int MF_AVG = 0;
            int DF_TOT = 0;
            int DF_AVG = 0;
            int GK_TOT = 0;
            int GK_AVG = 0;

            int DRIBBLING = 0;
            int PASSING = 0;
            int ASSISTING = 0;
            int FINISHING = 0;
            int TACKLING = 0;
            int GOAL_PREVENTION = 0;
            // Problem:  How to approach mentality? It was a measure for how likely a player was to give away a foul,
            //           but with the stats available here there doesn't seem to be a good analogue to that
            //           Lengthy solution would be to re-scrape all the data and retrieve the super detailed stats,
            //           but that seems like a lot of effort (for now)
            //       For now this could be solved by just using the inverse of the tackling stat, so worse tacklers give fouls away more often
            List<(string, int)> stats = new List<(string, int)>();

            int i = 3;
            while (i < f.Length){
                string[] line = f[i].Split(',');
                if (FW_DATA.Contains(line[0])) {
                    FW_TOT += Convert.ToInt32(line[2]);
                } else if (MF_DATA.Contains(line[0])) {
                    MF_TOT += Convert.ToInt32(line[2]);
                } else if (DF_DATA.Contains(line[0])) {
                    DF_TOT += Convert.ToInt32(line[2]);
                } else if (GK_DATA.Contains(line[0])) {
                    GK_TOT += Convert.ToInt32(line[2]);
                } else {
                    // Nothing
                }

                (string,int) t = (line[0], Convert.ToInt32(line[2]));
                if (DATA.Contains(line[0])) {
                    stats.Add(t);
                }
                i++;
            }

            // TODO:
            // Stats need to be weighted based on the positions they are applying to
            //  e.g. A low percentile for a defensive stat on a defender would still warrant a relatively high stat value, 
            //      whereas a low defensive stat on an attacking player would warrant a low stat value
            //  Especially for goalkeepers: As they rely heavily on the Goal Prevention stat, low percentiles for values that combine to 
            //      create that stat should not mean they have a low final stat, but rather the final value should be weighted to take into account
            //      that a low percentile for a goalkeeper still means their goalkeeping skills are potentially decent, just compared to better players
            //  Alternatively a function could be used to bound these values based on what is expected: for example goalkeepers are useless with 
            //      low Goal Prevention (e.g. Aaron Ramsdale has 38), despite any premier league goalkeeper actually being perfectly capable of shotstopping
            //  This is likely the reason for the absurd number of goals per game, as because the goal chance system relies on the similarity of values,
            //      exceedingly low Goal Prevention stats (e.g. 38) against a high Finishing stat (e.g. 85) means that the attacker will score almost every time,
            //      which is not realistic
            //  The alternate approach is to adjust the goalscoring algorithm to rely less on similar values, however as discovered, seasons simulated with
            //      completely random values tend to have a fairly realistic number of goals scored, so the problem is more likely in the stat conversion rather
            //      than in the decision algorithms
            if (pos1 == "GK") {
                GK_AVG = (int)Math.Round((double)(GK_TOT / 13),2);
                //Console.WriteLine("Average GK percentile: {0}"+(GK_AVG));
                DRIBBLING = (int)Math.Round(0.5 * GetValue("DEFENSIVE_ACTIONS_OUTSIDE_AREA", stats));
                PASSING = (int)Math.Round(0.25 * GetValue("TOUCHES", stats) + 0.25 * GetValue("LAUNCH_PERCENTAGE", stats) + 0.25 * GetValue("GOAL_KICKS", stats) + 0.25 * GetValue("AVG_GOAL_KICK_DISTANCE", stats));
                ASSISTING = (int)Math.Round(0.5 * GetValue("GOAL_KICKS", stats) + 0.5 * GetValue("AVG_GOAL_KICK_DISTANCE", stats));
                FINISHING = (int)Math.Round(0.5 * GetValue("AVG_GOAL_KICK_DISTANCE", stats));
                TACKLING = (int)Math.Round(0.4 * GetValue("CROSSES_STOPPED_PERCENTAGE", stats) + 0.4 * GetValue("DEFENSIVE_ACTIONS_OUTSIDE_AREA", stats) + 0.2 * GetValue("AVG_DISTANCE_OF_DEFENSIVE_ACTIONS", stats));
                GOAL_PREVENTION = (int)Math.Round(0.15 * GetValue("POST_SHOT_XG-CONCEDED", stats) + 0.15 * GetValue("GOALS_CONCEDED", stats) + 0.25 * GetValue("SAVE_PERCENTAGE", stats) + 0.15 * GetValue("POST_SHOT_XG_PER_SHOT_ON_TARGET", stats) + 0.1 * GetValue("PENALTY_SAVE_PERCENTAGE", stats) + 0.2 * GetValue("CLEAN_SHEET_PERCENTAGE", stats));
            } else {
                FW_AVG = (int)Math.Round((double)(FW_TOT / 7), 2);
                MF_AVG = (int)Math.Round((double)(MF_TOT / 7), 2);
                DF_AVG = (int)Math.Round((double)(DF_TOT / 6), 2);
                //Console.WriteLine("Average FW percentile: "+(FW_AVG));
                //Console.WriteLine("Average MF percentile: "+(MF_AVG));
                //Console.WriteLine("Average DF percentile: "+(DF_AVG));
                //for s in stats: print(s)
                //print([item for item in stats if "XA" in item])
                DRIBBLING = (int)Math.Round(0.4 * GetValue("PROGRESSIVE CARRIES", stats) + 0.4 * GetValue("DRIBBLES_COMPLETED", stats) + 0.1 * GetValue("TOUCHES_IN_AREA", stats) + 0.1 * GetValue("PROGRESSIVE_PASSES_RECIEVED", stats));
                PASSING = (int)Math.Round(0.3 * GetValue("PASSES_ATTEMPTED", stats) + 0.4 * GetValue("PASS_COMPLETION_PERCENTAGE", stats) + 0.3 * GetValue("PROGRESSIVE_PASSES", stats));
                ASSISTING = (int)Math.Round(0.2 * GetValue("ASSISTS", stats) + 0.3 * GetValue("XA", stats) + 0.1 * GetValue("PROGRESSIVE_PASSES", stats) + 0.3 * GetValue("SHOT_CREATIONS", stats) + 0.1 * GetValue("NON_PEN_XG+XA", stats));
                FINISHING = (int)Math.Round(0.4 * GetValue("NON_PEN_GOALS", stats) + 0.3 * GetValue("NON_PEN_XG", stats) + 0.2 * GetValue("TOTAL_SHOTS", stats) + 0.1 * GetValue("NON_PEN_XG+XA", stats));
                TACKLING = (int)Math.Round(0.15 * GetValue("PRESSURES", stats) + 0.35 * GetValue("INTERCEPTIONS", stats) + 0.35 * GetValue("TACKLES", stats) + 0.15 * GetValue("AERIAL_DUELS_WON", stats));
                GOAL_PREVENTION = (int)Math.Round(0.5 * GetValue("BLOCKS", stats) + 0.5 * GetValue("CLEARANCES", stats));
            }
            //Console.WriteLine(String.Format("\nDribbling: {0}\nPassing: {1}\nAssisting: {2}\nFinishing: {3}\nTackling: {4}\nGoal Prevention: {5}",DRIBBLING, PASSING, ASSISTING, FINISHING, TACKLING, GOAL_PREVENTION));

            Player p = new Player(fullname, 0, ConvertToPosition(pos1), DRIBBLING, FINISHING, TACKLING, PASSING, ASSISTING, GOAL_PREVENTION);
            p.Position2 = ConvertToPosition(pos2);
            return p;
        }

        public static void PrintStats(string filepath) {
            //var f = File.ReadAllLines(string.Format("csvs/Manchester Utd/{0}",filename));
            var f = File.ReadAllLines(filepath);
            //var f = open("csvs/Manchester Utd/{0}".format(filename), "r");
            //var info = f.readline().rstrip("\n").split(",");
            var info = f[0].Split(',');
            var forename = info[1];
            //forename = str(unicodedata.normalize('NFKD', info[1]))
            var surname = info[0];
            //surname = unicodedata.normalize('NFKD', info[0])
            var fullname = "";
            if (forename == "") {
                fullname = surname;
            } else {
                //fullname = "{0} {1}".format(forename, surname);
                fullname = forename + " " + surname;
            }
            var shirtname = "";
            if (forename == "") {
                shirtname = surname;
            } else {
                //shirtname = "{0}. {1}".format(forename[0], surname);
                shirtname = forename[0] + ". " + surname;
            }
            var team = info[2];
            //var positions = f.readline().rstrip("\n").split(",");
            var positions = f[1].Split(',');
            var pos1 = positions[0];
            var pos2 = positions[1];
            var pos3 = positions[2];
            var introduction = "";
            if (pos2 == "" && pos3 == "") {
                //introduction = "{0} plays for {1} as a {2}".format(fullname, team, pos1);
                introduction = String.Format("{0} plays for {1} as a {2}", fullname, team, pos1);
            } else if (pos3 == "") {
                //introduction = "{0} plays for {1} as a {2}, but can also play as a {3}".format(fullname, team, pos1, pos2);
                introduction = String.Format("{0} plays for {1} as a {2}, but can also play as a {3}", fullname, team, pos1, pos2);
            } else {
                //introduction = "{0} plays for {1} as a {2}, but can also play as a {3} or a {4}".format(fullname, team, pos1, pos2, pos3);
                introduction = String.Format("{0} plays for {1} as a {2}, but can also play as a {3} or a {4}", fullname, team, pos1, pos2, pos3);
            }
            Console.WriteLine(introduction);
            //var line = f.readline();
            
            int FW_TOT = 0;
            int FW_AVG = 0;
            int MF_TOT = 0;
            int MF_AVG = 0;
            int DF_TOT = 0;
            int DF_AVG = 0;
            int GK_TOT = 0;
            int GK_AVG = 0;

            // New Stat Ideas (For outfield players)
            int DRIBBLING = 0;
            //   Combination of [PROGRESSIVE CARRIES, DRIBBLES_COMPLETED, TOUCHES_IN_AREA, PROGRESSIVE_PASSES_RECIEVED]
            //   0.4*PROGRESSIVE CARRIES + 0.4*DRIBBLES_COMPLETED + 0.1*TOUCHES_IN_AREA + 0.1*PROGRESSIVE_PASSES_RECIEVED
            int PASSING = 0;
            //   Combination of [PASSES_ATTEMPTED, PASS_COMPLETION_PERCENTAGE, PROGRESSIVE_PASSES]
            //   0.3*PASSES_ATTEMPTED4 + 0.4*PASS_COMPLETION_PERCENTAGE + 0.3*PROGRESSIVE_PASSES
            int ASSISTING = 0;
            //   Combination of [ASSISTS, XA, PROGRESSIVE_PASSES, SHOT_CREATIONS, NON_PEN_XG+XA]
            //   0.2*ASSISTS + 0.3*XA + 0.1*PROGRESSIVE_PASSES + 0.3*SHOT_CREATIONS + 0.1*NON_PEN_XG+XA
            int FINISHING = 0;
            //   Combination of [NON_PEN_GOALS, NON_PEN_XG, TOTAL_SHOTS, NON_PEN_XG+XA]
            //   0.4*NON_PEN_GOALS + 0.3*NON_PEN_XG + 0.2*TOTAL_SHOTS + 0.1*NON_PEN_XG+XA
            int TACKLING = 0;
            //   Combination of [TACKLES, INTERCEPTIONS, AERIAL_DUELS_WON, PRESSURES]
            //   0.15*PRESSURES + 0.35*TACKLES + 0.35*INTERCEPTIONS + 0.15*AERIAL_DUELS_WON
            int GOAL_PREVENTION = 0;
            //   Combination of [BLOCKS, CLEARANCES]
            //   0.5*BLOCKS + 0.5*CLEARANCES
            // Problem:  How to approach mentality? It was a measure for how likely a player was to give away a foul,
            //           but with the stats available here there doesn't seem to be a good analogue to that
            //           Lengthy solution would be to re-scrape all the data and retrieve the super detailed stats,
            //           but that seems like a lot of effort (for now)
            //       For now this could be solved by just using the inverse of the tackling stat, so worse tacklers give fouls away more often
            List<(string, int)> stats = new List<(string, int)>();

            int i = 3;
            while (i < f.Length){
                string[] line = f[i].Split(',');
                if (FW_DATA.Contains(line[0])) {
                    //print("FW STAT: {0}".format(line))
                    FW_TOT += Convert.ToInt32(line[2]);
                } else if (MF_DATA.Contains(line[0])) {
                    //print("MF STAT: {0}".format(line))
                    MF_TOT += Convert.ToInt32(line[2]);
                } else if (DF_DATA.Contains(line[0])) {
                    //print("DF STAT: {0}".format(line))
                    DF_TOT += Convert.ToInt32(line[2]);
                } else if (GK_DATA.Contains(line[0])) {
                    GK_TOT += Convert.ToInt32(line[2]);
                } else {
                    // Nothing
                }
                //print(line)
                (string,int) t = (line[0], Convert.ToInt32(line[2]));
                if (DATA.Contains(line[0])) {
                    stats.Add(t);
                }
                //line = f.readline().rstrip("\n").split(",");
                i++;
            }

            /*while (line != new List<object> {
                ""
            }) {
                if (FW_DATA.Contains(line[0])) {
                    //print("FW STAT: {0}".format(line))
                    FW_TOT += Convert.ToInt32(line[2]);
                } else if (MF_DATA.Contains(line[0])) {
                    //print("MF STAT: {0}".format(line))
                    MF_TOT += Convert.ToInt32(line[2]);
                } else if (DF_DATA.Contains(line[0])) {
                    //print("DF STAT: {0}".format(line))
                    DF_TOT += Convert.ToInt32(line[2]);
                } else if (GK_DATA.Contains(line[0])) {
                    GK_TOT += Convert.ToInt32(line[2]);
                } else {
                    int x;
                }
                //print(line)
                (string,int) t = (line[0], line[2]);
                if (DATA.Contains(line[0])) {
                    stats.Add(t);
                }
                line = f.readline().rstrip("\n").split(",");
            }*/
            if (pos1 == "GK") {
                GK_AVG = (int)Math.Round((double)(GK_TOT / 13),2);
                Console.WriteLine("Average GK percentile: {0}"+(GK_AVG));
                DRIBBLING = (int)Math.Round(0.5 * GetValue("DEFENSIVE_ACTIONS_OUTSIDE_AREA", stats));
                PASSING = (int)Math.Round(0.25 * GetValue("TOUCHES", stats) + 0.25 * GetValue("LAUNCH_PERCENTAGE", stats) + 0.25 * GetValue("GOAL_KICKS", stats) + 0.25 * GetValue("AVG_GOAL_KICK_DISTANCE", stats));
                ASSISTING = (int)Math.Round(0.5 * GetValue("GOAL_KICKS", stats) + 0.5 * GetValue("AVG_GOAL_KICK_DISTANCE", stats));
                FINISHING = (int)Math.Round(0.5 * GetValue("AVG_GOAL_KICK_DISTANCE", stats));
                TACKLING = (int)Math.Round(0.4 * GetValue("CROSSES_STOPPED_PERCENTAGE", stats) + 0.4 * GetValue("DEFENSIVE_ACTIONS_OUTSIDE_AREA", stats) + 0.2 * GetValue("AVG_DISTANCE_OF_DEFENSIVE_ACTIONS", stats));
                GOAL_PREVENTION = (int)Math.Round(0.15 * GetValue("POST_SHOT_XG-CONCEDED", stats) + 0.15 * GetValue("GOALS_CONCEDED", stats) + 0.25 * GetValue("SAVE_PERCENTAGE", stats) + 0.15 * GetValue("POST_SHOT_XG_PER_SHOT_ON_TARGET", stats) + 0.1 * GetValue("PENALTY_SAVE_PERCENTAGE", stats) + 0.2 * GetValue("CLEAN_SHEET_PERCENTAGE", stats));
            } else {
                FW_AVG = (int)Math.Round((double)(FW_TOT / 7), 2);
                MF_AVG = (int)Math.Round((double)(MF_TOT / 7), 2);
                DF_AVG = (int)Math.Round((double)(DF_TOT / 6), 2);
                Console.WriteLine("Average FW percentile: "+(FW_AVG));
                Console.WriteLine("Average MF percentile: "+(MF_AVG));
                Console.WriteLine("Average DF percentile: "+(DF_AVG));
                //for s in stats: print(s)
                //print([item for item in stats if "XA" in item])
                DRIBBLING = (int)Math.Round(0.4 * GetValue("PROGRESSIVE CARRIES", stats) + 0.4 * GetValue("DRIBBLES_COMPLETED", stats) + 0.1 * GetValue("TOUCHES_IN_AREA", stats) + 0.1 * GetValue("PROGRESSIVE_PASSES_RECIEVED", stats));
                PASSING = (int)Math.Round(0.3 * GetValue("PASSES_ATTEMPTED", stats) + 0.4 * GetValue("PASS_COMPLETION_PERCENTAGE", stats) + 0.3 * GetValue("PROGRESSIVE_PASSES", stats));
                ASSISTING = (int)Math.Round(0.2 * GetValue("ASSISTS", stats) + 0.3 * GetValue("XA", stats) + 0.1 * GetValue("PROGRESSIVE_PASSES", stats) + 0.3 * GetValue("SHOT_CREATIONS", stats) + 0.1 * GetValue("NON_PEN_XG+XA", stats));
                FINISHING = (int)Math.Round(0.4 * GetValue("NON_PEN_GOALS", stats) + 0.3 * GetValue("NON_PEN_XG", stats) + 0.2 * GetValue("TOTAL_SHOTS", stats) + 0.1 * GetValue("NON_PEN_XG+XA", stats));
                TACKLING = (int)Math.Round(0.15 * GetValue("PRESSURES", stats) + 0.35 * GetValue("INTERCEPTIONS", stats) + 0.35 * GetValue("TACKLES", stats) + 0.15 * GetValue("AERIAL_DUELS_WON", stats));
                GOAL_PREVENTION = (int)Math.Round(0.5 * GetValue("BLOCKS", stats) + 0.5 * GetValue("CLEARANCES", stats));
            }
            Console.WriteLine(String.Format("\nDribbling: {0}\nPassing: {1}\nAssisting: {2}\nFinishing: {3}\nTackling: {4}\nGoal Prevention: {5}",DRIBBLING, PASSING, ASSISTING, FINISHING, TACKLING, GOAL_PREVENTION));
        }
        
        public static void PrintAll() {
            var directory_in_str = "csvs/ENG1/Manchester Utd/";
            string[] files = Directory.GetFiles(directory_in_str);
            //var directory = os.fsencode(directory_in_str);
            /*foreach (var file in os.listdir(directory)) {
                var filename = os.fsdecode(file);
                if (filename.endswith(".csv")) {
                    // print(os.path.join(directory, filename))
                    //PrintIntroduction(filename)
                    PrintStats(filename);
                    Console.WriteLine();
                    continue;
                } else {
                    continue;
                }
            }*/
            foreach (var file in files){
                PrintStats(Path.GetFileName(file));
            }
        }
        
        public static void PrintOne() {
            //f = csv.writer(open("csvs/Manchester Utd/Fred.csv","r"))
            Console.WriteLine("Enter a Manchester Utd player to print stats for: ");
            string p = Console.ReadLine();
            p = "csvs/ENG1/Manchester Utd/" + p + ".csv";
            //p = "{0}.csv".format(p);
            PrintStats(p);
            //PrintStats("Fred.csv")
            //PrintAll()
        }
    }
}
