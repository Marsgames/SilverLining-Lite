#region Author
///////////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
using System.Collections.Generic;
#endregion
public static class Constant
{
    public enum ELayer
    {
        Default = 0,
        NavMesh = 8
    }

    public enum SpawnTypeUnit
    {
        Warrior,
        Range,
        Tank,
        Scout
    }

    public struct ListOfObject
    {
        public static readonly string s_navMeshSurface = "NavMeshSurface";
        public static readonly string s_wallSideCollider = "SideCollider";
    }

    public struct LobbyString
    {
        public static readonly string s_player2 = "Joueur 2";
        public static readonly string s_computeur = "Ordinateur";
        public static readonly string s_player2Text = "TextPlayer2";
        public static readonly string s_play = "JOUER";
        public static readonly string s_waiting = "EN ATTENTE";
        public static readonly string s_ready = "PRÊT";
        public static readonly string s_textLauncGame = "Lancement de la partie : ";
        public static readonly string s_loading = "Chargement en cours...";
    }

    public struct ListOfTag
    {
        public static readonly string s_unit = "Unit";
        public static readonly string s_neutralUnit = "NeutralUnit";
        public static readonly string s_spawnUnit1 = "SpawnUnit1";
        public static readonly string s_spawnUnit2 = "SpawnUnit2";
        public static readonly string s_bombCD = "BombCD";
        public static readonly string s_checkpoint = "Checkpoint";
        public static readonly string s_obstacle = "Obstacle";
    }

    public struct ListOfShortcut
    {
        public static readonly string s_focusSpawnUnit1 = "FocusSpawnUnit1";
        public static readonly string s_focusSpawnUnit2 = "FocusSpawnUnit2";
        public static readonly string s_openCloseCheckpoint = "OpenCloseCheckpoint";
        public static readonly string s_releaseUnit1 = "ReleaseUnit1";
        public static readonly string s_releaseUnit2 = "ReleaseUnit2";
        public static readonly string s_releaseUnit3 = "ReleaseUnit3";
        public static readonly string s_releaseUnit4 = "ReleaseUnit4";
        public static readonly string s_releaseUnit5 = "ReleaseUnit5";
        public static readonly string s_spellWarrior = "SpellWarrior";
        public static readonly string s_spellTank = "SpellTank";
        public static readonly string s_spellScout = "SpellScout";
        public static readonly string s_spellRange = "SpellRange";
    }

    public struct ListOfUI
    {
        public static readonly string s_canvas = "Canvas";
        public static readonly string s_canvasText = "CanvasText";
        public static readonly string s_obstacleContext = "ObstaclesContext";
        public static readonly string s_detailsCanvas = "DetailsCanvas";
        public static readonly string s_openCloseCheckpoint = "OpenCloseCanvas";
        public static readonly string s_spawnCanvas = "SpawnCanvas";
        public static readonly string s_spellCanvas = "SpellCanvas";
        public static readonly string s_spellImageCD = "SpellImageCD";
        public static readonly string s_spellStack = "SpellStack";
        public static readonly string s_bombImageCD = "BombImageCD";
        public static readonly string s_healthBarsCanvas = "HealthBarsCanvas";
        public static readonly string s_timerCanvas = "TimerCanvas";

        public static readonly string s_stockUnit = "StockUnit";
        public static readonly string s_openCloseButton = "OpenCloseButton";
        public static readonly string s_countStockUnit = "CountStockUnit";
        public static readonly string s_stockUnitText = "StockUnitText";
        public static readonly string s_uiOpenClose = "UIOpenClose";
        public static readonly string s_openImage = "OpenImage";
        public static readonly string s_closeImage = "CloseImage";
        public static readonly string s_cd = "CD";

        public static readonly string s_victoryText = "VictoryText";
        public static readonly string s_victoryScreen = "VictoryScreen";
        public static readonly string s_containerUnit = "ContainerUnit";
        public static readonly string s_containerObstacle = "ContainerObstacle";

        public static readonly string s_homeButton = "HomeButton";
    }

    public struct ListOfLayer
    {
        public static readonly string s_ui = "UI";
    }

    public struct ListOfAnimTrigger
    {
        public static readonly string s_run = "Run";
        public static readonly string s_attack = "Attack";
        public static readonly string s_idle = "Idle";
        public static readonly string s_death = "Death";
        public static readonly string s_isActive = "IsActive";
    }

    public struct ListOfScene
    {
        public static readonly string s_startGame = "StartGame";
        public static readonly string s_networkLobby = "NetworkLobby";
    }

    public struct ListOfText
    {
        public static readonly string s_victory = "Victoire";
        public static readonly string s_defeat = "Défaite";
        public static readonly string s_warningUnit = "Impéryolithe insuffisante";
        public static readonly string s_warningObstacle = "Concentration insuffisante";
        public static readonly string s_warningTerritory = "Vous n'avez pas le contrôle de cette zone";
    }

    public struct BotTransition
    {
        public static readonly string s_randomActions = "RandomActions";
        public static readonly string s_mirrorPlayer = "MirrorPlayer";
        public static readonly string s_groupOfUnits = "GroupOfUnits";
        public static readonly string s_groupOfUnitsTrigger = "GroupOfUnitsTrigger";
        public static readonly string s_findCkOnPath = "FindCkOnPath";
        public static readonly string s_findCkOnPathtrig = "FindCkOnPathTrig";
        public static readonly string s_findClosestBuild = "FindClosestBuild";
        public static readonly string s_createTeam = "CreateTeam";
        public static readonly string s_spawnUnit = "SpawnUnit";
        public static readonly string s_buildObstacle = "BuildObstacle";
        public static readonly string s_destroyObstacle = "DestroyObstacle";
        public static readonly string s_releaseUnits = "ReleaseUnits";
        public static readonly string s_return = "Return";
        public static readonly string s_strat = "Strat";
        public static readonly string s_onlyRandomActions = "OnlyRandomActions";
    }

    public struct ListOfMisc
    {
        public static readonly string s_Image = "Image";
        public static readonly string s_Grey = "Grey";
        public static readonly string s_CostText = "CostText";
    }

    public struct ListOfTerritories
    {
        public static readonly string s_territoryBaseP1 = "Territory-Base-P1";
        public static readonly string s_territorySide1 = "Territory-Side-1";
        public static readonly string s_territoryCenter = "Territory-Center";
        public static readonly string s_territorySide2 = "Territory-Side-2";
        public static readonly string s_territoryBaseP2 = "Territory-Base-P2";

    }

}