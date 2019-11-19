using System.Collections.Generic;
using UnityEngine;

public class AIStrat : StateMachineBehaviour {
    #region Variables
    private List<ObstacleConstructor> m_listObstacles;
    private List<int> m_obstaclesConstructedIndList = new List<int> ();
    private List<int> m_obstaclesConstructedByBotIndList = new List<int> ();
    private List<SpawnUnits> m_botSpawnUnits;
    private List<CheckpointBase> m_listCheckpoints;
    private List<int> m_pricesBuildList;
    private List<ActiveObstacle.ObstacleType> m_TypeBuildList;
    private List<Constant.SpawnTypeUnit> m_listeTypeUnits;
    private List<int> m_pricesList;
    private List<int> m_listIndex;

    private AIBuildObstacle m_aiBuildObstacles;
    private AIDestroyObstacle m_aiDestroyObstacle;
    private AICreateTeam m_aiCreateTeam;
    private AIFindCkOnPath m_aiFindCkOnPath;
    private AIRandomActions m_aiRandomActions;
    private AIGroupOfUnits m_aiGroupOfUnits;
    private AIFindClosestBuild m_aiFindClosestBuild;

    private int m_indexSpawn;
    private bool m_enoughGold;
    private bool m_enoughBuildGold;
    private bool m_isGigaBusSpawned = false;
    private bool m_isEveryBuldDestroyed = false;
    private bool m_isEveryBuldDestroyedForTower = false;
    private bool m_isTakingTower = false;
    private bool m_isWaitingForBuild = true;
    private bool m_isSpawning = false;
    private bool m_isUnbunkering = false;
    private bool m_isBaseTowerTaken = false;
    private bool m_isStrat = false;
    private int m_counter = 0;
    private int m_nbTtraps = 0;

    #endregion

    #region main functions
    override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        m_aiBuildObstacles = animator.GetBehaviour<AIBuildObstacle> ();
        m_aiDestroyObstacle = animator.GetBehaviour<AIDestroyObstacle> ();
        m_aiCreateTeam = animator.GetBehaviour<AICreateTeam> ();
        m_aiFindCkOnPath = animator.GetBehaviour<AIFindCkOnPath> ();
        m_aiRandomActions = animator.GetBehaviour<AIRandomActions> ();
        m_aiGroupOfUnits = animator.GetBehaviour<AIGroupOfUnits> ();
        m_aiFindClosestBuild = animator.GetBehaviour<AIFindClosestBuild> ();

        animator.SetBool (Constant.BotTransition.s_randomActions, false);
    }

    override public void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        SetListOfObstaclesConstructed ();

        if (m_listCheckpoints[15].GetPlayerOwner () == PlayerEntity.Player.Bot && m_isTakingTower) {
            m_isTakingTower = false;
        }
        if (m_listCheckpoints[19].GetNbUnitsStocked () == 3 && m_isSpawning) {
            m_isSpawning = false;
        }

        if (m_isUnbunkering && !IsObstaclesBuilt (new List<int> () { 0, 1, 3 })) {
            m_isUnbunkering = false;
        }

        if (m_listCheckpoints[19].GetNbUnitsStocked () == 3) {
            m_isBaseTowerTaken = true;
        }

        if (m_counter <= 16) {
            TakeFirstMines (animator);
        } else if (m_counter >= 17 && m_counter <= 21) {
            FarmGolemGoldAndTakeMiddleMine (animator);
        } else if (m_counter >= 22 && m_counter <= 27) {
            TakeSideMines (animator);
        } else if (!m_isStrat && !(PlayerEntity.Player.Bot == m_listCheckpoints[14].GetPlayerOwner () && m_listCheckpoints[14].GetNbUnitsStocked () >= 3)) {
            TakeHardestMines (animator);
        } else if (m_isStrat) {
            Debug.Log ("final strat");
            if (isPlayerBunkered () || m_isUnbunkering) {
                Debug.Log ("try to destroy the bunker");
                UnbunkerPlayer (animator);
            } else if (m_listCheckpoints[19].GetNbUnitsStocked () < 3 && !m_isSpawning) {
                Debug.Log ("take base tower");
                TakeBaseTower (animator);
            } else if (EnoughGold (new List<int> () { 6, 10, 3, 0 }) || m_isGigaBusSpawned) {
                Debug.Log ("kill the great golem");
                KillTheGreatGolem (animator);
            } else if (m_listCheckpoints[15].GetPlayerOwner () != PlayerEntity.Player.Bot && !m_isTakingTower && m_isBaseTowerTaken) {
                Debug.Log ("take mdl tower");
                TakeMiddleTower (animator);
            } else if (m_listCheckpoints[11].GetPlayerOwner () != PlayerEntity.Player.Bot && m_listCheckpoints[12].GetPlayerOwner () != PlayerEntity.Player.Bot ||
                m_listCheckpoints[11].GetNbUnitsStocked () < 3 || m_listCheckpoints[12].GetNbUnitsStocked () < 3) {
                Debug.Log ("take mdl mines");
                TakeMiddleMines (animator);
            } else if (GameManager.Instance.GetPlayer (PlayerEntity.Player.Bot).GetMaxBuildGold () < 6) {
                Debug.Log ("random actions");
                CallRandomActions (0.4f, 0f, 0f, 0f, animator);
            } else if (GameManager.Instance.GetPlayer (PlayerEntity.Player.Bot).GetMaxBuildGold () >= 6) {
                Debug.Log ("farm golem");

                if (IsObstaclesBuilt (new List<int> () { 30, 31, 32, 33, 34, 35 })) {
                    CallRandomActions (0.5f, 0f, 0f, 0f, animator);
                } else {
                    FarmGolem (animator);
                }
            }
        }
    }
    #endregion

    #region differents strat
    private void TakeFirstMines (Animator animator) {
        if (m_counter <= 16 && m_counter >= 6) {
            if (m_listObstacles[17].GetTerritory ().IsAvailable (PlayerEntity.Player.Bot) && !IsObstaclesBuilt (new List<int> () { 17 })) {
                CallBuildObstacle (17, ActiveObstacle.ObstacleType.SlopeRight, animator);
            } else if (m_listObstacles[18].GetTerritory ().IsAvailable (PlayerEntity.Player.Bot) && !IsObstaclesBuilt (new List<int> () { 18 })) {
                CallBuildObstacle (18, ActiveObstacle.ObstacleType.SlopeRight, animator);
            } else {
                List<int> listeNbUnits = new List<int> () { 0, 0, 0, 1 };

                if (m_counter <= 16 && m_counter >= 14) {
                    CallCreateTeam (new List<int> () { 0, 0, 1, 0 }, 0, animator);
                } else if (m_counter <= 13 && m_counter >= 11) {
                    CallCreateTeam (new List<int> () { 0, 0, 1, 0 }, 1, animator);
                } else {
                    CallCreateTeam (listeNbUnits, 0, animator);
                }
            }
        } else if (m_counter >= 3 && m_counter <= 5) {
            List<int> listeNbUnits = new List<int> () { 0, 0, 1, 0 };
            if (EnoughGold (listeNbUnits)) {
                CallCreateTeam (listeNbUnits, 0, animator);
            }
        } else if (m_counter == 1 || m_counter == 2) {
            List<int> listeNbUnits = new List<int> () { 0, 0, 0, 1 };
            CallCreateTeam (listeNbUnits, 1, animator);
        } else if (m_counter == 0) {
            List<int> listeNbUnits = new List<int> () { 0, 0, 3, 1 };
            CallCreateTeam (listeNbUnits, 1, animator);
        }
    }

    private void FarmGolemGoldAndTakeMiddleMine (Animator animator) {
        List<int> listeNbUnits = new List<int> () { 0, 0, 0, 4 };
        if (m_counter == 17 && IsObstaclesBuilt (new List<int> () { 18 }) && GameManager.Instance.GetPlayer (PlayerEntity.Player.Bot).GetUnitGold () >= 500) {
            CallDestroyObstacle (18, animator);
        } else if (m_counter == 18 && IsObstaclesBuilt (new List<int> () { 17 }) && !IsObstaclesBuilt (new List<int> () { 18 })) {
            CallDestroyObstacle (17, animator);
        } else if (m_counter == 19 && GameManager.Instance.GetPlayer (PlayerEntity.Player.Bot).GetUnitGold () > 700) {
            CallBuildObstacle (35, ActiveObstacle.ObstacleType.SimpleWall, animator);
        } else if (m_counter == 20 && IsObstaclesBuilt (new List<int> { 35 })) {
            CallBuildObstacle (33, ActiveObstacle.ObstacleType.SimpleWall, animator);
        } else if (m_counter == 21) {
            CallCreateTeam (listeNbUnits, 1, animator);
        }
    }

    private void TakeSideMines (Animator animator) {
        if (ObstacleConstructor.EState.Built == m_listObstacles[35].GetCurrentState ()) {
            CallDestroyObstacle (35, animator);
        } else if (ObstacleConstructor.EState.Built == m_listObstacles[33].GetCurrentState ()) {
            CallDestroyObstacle (33, animator);
        } else if (m_counter == 24) {
            CallBuildObstacle (30, ActiveObstacle.ObstacleType.SimpleWall, animator);
        } else if (m_counter == 25) {
            CallBuildObstacle (31, ActiveObstacle.ObstacleType.SimpleWall, animator);
        } else if (m_counter == 26 && IsObstaclesBuilt (new List<int> { 31 })) {
            CallBuildObstacle (32, ActiveObstacle.ObstacleType.SimpleWall, animator);
        } else if (m_counter >= 27) {
            List<int> listeNbUnits = new List<int> () { 0, 0, 0, 3 };
            if (EnoughGold (listeNbUnits) && m_listObstacles[31].GetCurrentRestingTime () < 20f) {
                CallCreateTeam (listeNbUnits, 1, animator);
            }
        }
    }

    private void TakeCaserne (Animator animator) {
        if (IsObstaclesBuilt (new List<int> { 32 }) && m_counter == 28) {
            List<int> listeNbUnits = new List<int> () { 0, 0, 1, 0 };
            CallCreateTeam (listeNbUnits, 1, animator);
        } else if (m_counter == 29) {
            CallBuildObstacle (28, ActiveObstacle.ObstacleType.SimpleWall, animator);
        }
    }

    private void TakeMiddleTower (Animator animator) {
        if (m_obstaclesConstructedByBotIndList.Count > 0 && !m_isEveryBuldDestroyedForTower) {
            DestroyAllBotsObstaclesAndPlayersInList (new List<int> () { 30, 31, 35, 19 }, animator);
        } else {
            m_isEveryBuldDestroyedForTower = true;
            if (!IsObstaclesBuilt (new List<int> { 30 })) {
                CallBuildObstacle (30, ActiveObstacle.ObstacleType.SimpleWall, animator);
            } else if (!IsObstaclesBuilt (new List<int> { 31 }) && IsObstaclesBuilt (new List<int> { 30 })) {
                CallBuildObstacle (31, ActiveObstacle.ObstacleType.SimpleWall, animator);
            } else if (!IsObstaclesBuilt (new List<int> { 35 }) && IsObstaclesBuilt (new List<int> { 31 })) {
                CallBuildObstacle (35, ActiveObstacle.ObstacleType.SimpleWall, animator);
            } else if (!IsObstaclesBuilt (new List<int> { 19 }) && IsObstaclesBuilt (new List<int> { 35 })) {
                CallBuildObstacle (19, ActiveObstacle.ObstacleType.SimpleWall, animator);
            } else {
                List<int> listeNbUnits = new List<int> () { 0, 0, 0, 6 };
                if (EnoughGold (listeNbUnits)) {
                    m_isEveryBuldDestroyedForTower = false;
                    m_isTakingTower = true;
                    CallCreateTeam (listeNbUnits, 1, animator);
                }
            }
        }
    }

    private void TakeHardestMines (Animator animator) {
        List<int> listInd = new List<int> () { 27, 28 };

        if (IsObstaclesBuilt (listInd) || m_listObstacles[30].GetCurrentState () == ObstacleConstructor.EState.Invulnerable) {
            CallRandomActions (1f, 0f, 0f, 0f, animator);
        } else if (IsObstaclesBuilt (new List<int> () { 30 })) {
            CallDestroyObstacle (30, animator);
        } else if (!IsObstaclesBuilt (new List<int> () { 30 }) && !IsObstaclesBuilt (new List<int> () { 28 })) {
            CallBuildObstacle (28, ActiveObstacle.ObstacleType.SimpleWall, animator);
        } else if (m_listObstacles[28].GetCurrentRestingTime () < 20f) {
            CallBuildObstacle (27, ActiveObstacle.ObstacleType.SimpleWall, animator);
            m_isStrat = true;
        }
    }

    private void TakeSideMinesFromPlayer (Animator animator) {
        if (m_obstaclesConstructedByBotIndList.Count > 0 && !m_isEveryBuldDestroyed) {
            DestroyAllBotsObstaclesAndPlayersInList (new List<int> () { 19, 30, 31, 32 }, animator);
        } else {
            m_isEveryBuldDestroyed = true;
            if (!IsObstaclesBuilt (new List<int> { 19 })) {
                CallBuildObstacle (19, ActiveObstacle.ObstacleType.SimpleWall, animator);
            } else if (IsObstaclesBuilt (new List<int> { 19 })) {
                CallBuildObstacle (30, ActiveObstacle.ObstacleType.SimpleWall, animator);
            } else if (IsObstaclesBuilt (new List<int> { 30 })) {
                CallBuildObstacle (31, ActiveObstacle.ObstacleType.SimpleWall, animator);
            } else if (IsObstaclesBuilt (new List<int> { 31 })) {
                m_isEveryBuldDestroyed = false;
                CallBuildObstacle (32, ActiveObstacle.ObstacleType.SimpleWall, animator);
            }
        }
    }

    private void FarmGolem (Animator animator) {
        List<int> listInd = new List<int> () { 30, 31, 32, 33, 34, 35 };
        DestroyAllBotsObstaclesAndPlayersInList (listInd, animator);

        for (int i = 0; i < listInd.Count; i++) {
            if (i == 0) {
                CallBuildObstacle (listInd[i], ActiveObstacle.ObstacleType.SimpleWall, animator);
            } else if (!IsObstaclesBuilt (new List<int> () { listInd[i] }) && IsObstaclesBuilt (new List<int> () { listInd[i - 1] })) {
                CallBuildObstacle (listInd[i], ActiveObstacle.ObstacleType.SimpleWall, animator);
            }
        }
    }

    private void KillTheGreatGolem (Animator animator) {
        if (m_obstaclesConstructedByBotIndList.Count == 0) {
            m_isEveryBuldDestroyed = true;
        }
        if (m_isWaitingForBuild) {
            IsWaitingForBuilding (m_obstaclesConstructedIndList, animator);
        } else if (!m_isGigaBusSpawned) {
            m_isWaitingForBuild = true;
            m_isGigaBusSpawned = true;
            List<int> listeNbUnits = new List<int> () { 6, 10, 3, 0 };
            CallCreateTeam (listeNbUnits, 1, animator);
        } else if (m_obstaclesConstructedByBotIndList.Count > 0 && !m_isEveryBuldDestroyed) {
            DestroyAllBotsObstaclesAndPlayersInList (new List<int> () { 14, 25, 30, 31, 35 }, animator);
        } else if (m_isGigaBusSpawned && m_isEveryBuldDestroyed) {
            m_isWaitingForBuild = false;
            if (!IsObstaclesBuilt (new List<int> { 35 })) {
                CallBuildObstacle (35, ActiveObstacle.ObstacleType.SlopeRight, animator);
            } else if (!IsObstaclesBuilt (new List<int> { 30 }) && IsObstaclesBuilt (new List<int> { 35 })) {
                CallBuildObstacle (30, ActiveObstacle.ObstacleType.SlopeRight, animator);
            } else if (!IsObstaclesBuilt (new List<int> { 31 }) && IsObstaclesBuilt (new List<int> { 30 })) {
                CallBuildObstacle (31, ActiveObstacle.ObstacleType.SlopeRight, animator);
            } else if (!IsObstaclesBuilt (new List<int> { 25 }) && IsObstaclesBuilt (new List<int> { 31 }) && m_listObstacles[31].GetCurrentRestingTime () < 15f) {
                CallBuildObstacle (25, ActiveObstacle.ObstacleType.SimpleWall, animator);
            } else if (!IsObstaclesBuilt (new List<int> { 14 }) && IsObstaclesBuilt (new List<int> { 25 })) {
                m_isGigaBusSpawned = false;
                m_isEveryBuldDestroyed = false;
                CallBuildObstacle (14, ActiveObstacle.ObstacleType.SimpleWall, animator);
            }
        }
    }

    private void UnbunkerPlayer (Animator animator) {
        m_isUnbunkering = true;
        IsWaitingForBuilding (new List<int> () { 1 }, animator);
        if (!m_isWaitingForBuild) {
            CallDestroyObstacle (1, animator);
        }
    }

    private void TakeBaseTower (Animator animator) {
        DestroyAllBotsObstaclesAndPlayersInList (m_obstaclesConstructedByBotIndList, animator);
        if (IsObstaclesBuilt (new List<int> () { 38 }) && null == m_listObstacles[38].transform.Find ("SlopeRight_A(Clone)")) {
            CallDestroyObstacle (38, animator);
        } else if (IsObstaclesBuilt (new List<int> () { 38 }) && null != m_listObstacles[38].transform.Find ("SlopeRight_A(Clone)") && EnoughGold (new List<int> () { 3, 0, 0, 0 })) {
            m_isSpawning = true;
            CallCreateTeam (new List<int> () { 3, 0, 0, 0 }, 1, animator);
        } else if (!IsObstaclesBuilt (new List<int> () { 38 })) {
            DestroyForBuildGold (animator, 1);
            if (EnoughBuildGold (ActiveObstacle.ObstacleType.SlopeRight)) {
                CallBuildObstacle (38, ActiveObstacle.ObstacleType.SlopeRight, animator);
            }
        }
    }

    private void TakeMiddleMines (Animator animator) {
        IsWaitingForBuilding (new List<int> () { 35, 30, 31, 23, 24 }, animator);
        if (!m_isWaitingForBuild) {
            DestroyAllBotsObstaclesAndPlayersInList (new List<int> () { 35, 30, 31, 23, 24 }, animator);
        } else if (!IsObstaclesBuilt (new List<int> () { 35, 30, 31, 23, 24 })) {
            CallRandomActions (0.5f, 0f, 0f, 0f, animator);
        }
    }

    #endregion
    #region callfunction
    private void CallCreateTeam (List<int> listNbUnits, int indexSpawn, Animator animator) {
        m_enoughGold = EnoughGold (listNbUnits);
        if (m_enoughGold) {
            m_counter++;
            m_aiCreateTeam.SetSpawnList (m_listeTypeUnits, listNbUnits);
            m_aiCreateTeam.SetSpawnUnitsIndex (indexSpawn);
            animator.SetTrigger (Constant.BotTransition.s_createTeam);
        }
    }

    private void CallFindCkOnPath (int indexOfCk, Animator animator, int indexOfSpawnUnits) {
        if (EnoughBuildGold (ActiveObstacle.ObstacleType.SimpleWall)) {
            m_counter++;
            m_aiFindCkOnPath.SetCkAreaCollider (m_listCheckpoints[indexOfCk].GetComponentsInChildren<CheckpointCollider> ());
            m_aiFindCkOnPath.SetSpawnUnit (m_botSpawnUnits[indexOfSpawnUnits]);
            animator.SetTrigger (Constant.BotTransition.s_findCkOnPathtrig);
        }
    }

    private void CallBuildObstacle (int indexOfObstacleConstructor, ActiveObstacle.ObstacleType wallsType, Animator animator) {
        if (EnoughBuildGold (wallsType) && m_listObstacles[indexOfObstacleConstructor].GetCurrentState () == ObstacleConstructor.EState.Buildable) {
            m_counter++;
            m_aiBuildObstacles.SetIndex (indexOfObstacleConstructor);
            m_aiBuildObstacles.SetTypeBuild (wallsType);
            animator.SetTrigger (Constant.BotTransition.s_buildObstacle);
        }
    }

    private void CallDestroyObstacle (int indexOfObstacleConstructor, Animator animator) {
        m_counter++;
        m_aiDestroyObstacle.SetIndex (indexOfObstacleConstructor);
        animator.SetTrigger (Constant.BotTransition.s_destroyObstacle);
    }

    private void CallRandomActions (float unitSpawnRatio, float obstaclesRatio, float checkpointsReleaseAllRatio, float checkpointsReleaseRatio, Animator animator) {
        m_aiRandomActions.SetRatio (unitSpawnRatio, obstaclesRatio, checkpointsReleaseAllRatio, checkpointsReleaseRatio);
        animator.SetBool (Constant.BotTransition.s_randomActions, true);
    }
    #endregion

    #region useful functions

    private void DestroyForBuildGold (Animator animator, int buildGoldAim) {
        if (GameManager.Instance.GetPlayer (PlayerEntity.Player.Bot).GetBuildGold () < buildGoldAim && m_obstaclesConstructedByBotIndList.Count > 0) {
            int aleaInd = (int) UnityEngine.Random.Range (0f, m_obstaclesConstructedByBotIndList.Count);
            CallDestroyObstacle (m_obstaclesConstructedByBotIndList[aleaInd], animator);
        }
    }

    private void DestroyAllBotsObstaclesAndPlayersInList (List<int> listOfIndToDestroy, Animator animator) {
        List<int> copy = m_obstaclesConstructedByBotIndList;
        IsWaitingForBuilding (m_obstaclesConstructedIndList, animator);
        if (IsEnoughBombToDestroy (listOfIndToDestroy)) {
            if (copy.Count > 0) {
                CallDestroyObstacle (copy[0], animator);
            } else {
                for (int i = 0; i < listOfIndToDestroy.Count; i++) {
                    if (IsObstaclesBuilt (new List<int> () { listOfIndToDestroy[i] })) {
                        CallDestroyObstacle (listOfIndToDestroy[i], animator);
                    }
                }
            }
        }
    }

    private int FindLongestCurrentCoolTime () {
        int maxTimeInd = 0;
        for (int k = 0; k < m_listObstacles.Count; k++) {
            if (m_listObstacles[maxTimeInd].GetCurrentRestingTime () < m_listObstacles[k].GetCurrentRestingTime () && !IsObstaclesBuilt (new List<int> () { k })) {
                maxTimeInd = k;
            }
        }
        return maxTimeInd;
    }

    private int GetBotsMines () {
        int nb = 0;
        for (int i = 0; i < m_listCheckpoints.Count; i++) {
            if (null != m_listCheckpoints[i].GetComponentInChildren<Mine> () && m_listCheckpoints[i].GetPlayerOwner () == PlayerEntity.Player.Bot) {
                nb++;
            }
        }
        return nb;
    }

    #endregion

    #region checks
    private bool EnoughGold (List<int> m_listeNbUnits) {
        int m_totalPrice = 0;

        for (int j = 0; j < m_listeNbUnits.Count; j++) {
            m_totalPrice += m_pricesList[j] * m_listeNbUnits[j];
        }
        return m_totalPrice <= GameManager.Instance.GetPlayer (PlayerEntity.Player.Bot).GetUnitGold ();
    }

    private bool EnoughBuildGold (ActiveObstacle.ObstacleType wallType) {
        return GameManager.Instance.GetPlayer (PlayerEntity.Player.Bot).GetBuildGold () >= m_pricesBuildList[m_TypeBuildList.IndexOf (wallType)];
    }

    private bool IsThereABus (Animator animator, int minNbUnitForBus) {
        UnitController[] unitControllers = FindObjectsOfType<UnitController> ();
        List<UnitController> playerUnits = new List<UnitController> ();
        foreach (UnitController unit in unitControllers) {
            if (unit.GetPlayerNumber () == PlayerEntity.Player.Player1) {
                playerUnits.Add (unit);
            }
        }

        int maxNbUnits = 0;
        int indexUnit = 0;
        float rayon = 10f;
        if (playerUnits.Count > 0) {
            maxNbUnits = 0;
            for (int i = 0; i < playerUnits.Count; i++) {
                int nbUnits = 0;
                for (int j = 0; j < playerUnits.Count; j++) {
                    Vector2 point1 = new Vector2 (playerUnits[i].transform.position.x, playerUnits[i].transform.position.z);
                    Vector2 point2 = new Vector2 (playerUnits[j].transform.position.x, playerUnits[j].transform.position.z);
                    float distance = Vector2.Distance (point1, point2);
                    if (playerUnits[i].transform.position.y == playerUnits[j].transform.position.y && distance < rayon) {
                        nbUnits += 1;
                    }
                }
                if (maxNbUnits < nbUnits) {
                    indexUnit = i;
                    maxNbUnits = nbUnits;
                }
            }
            m_aiFindClosestBuild.SetUnitController (playerUnits[indexUnit]);
            m_aiFindClosestBuild.SetBuildType (ActiveObstacle.ObstacleType.Trap);
        }
        return (maxNbUnits > minNbUnitForBus);
    }

    private bool IsObstaclesBuilt (List<int> listIndObstacles) {
        bool isObstaclesBuilt = false;
        for (int i = 0; i < listIndObstacles.Count; i++) {
            if (m_listObstacles[listIndObstacles[i]].GetCurrentState () == ObstacleConstructor.EState.Built || m_listObstacles[listIndObstacles[i]].GetCurrentState () == ObstacleConstructor.EState.Invulnerable) {
                isObstaclesBuilt = true;
            } else {
                isObstaclesBuilt = false;
                break;
            }
        }
        return isObstaclesBuilt;
    }

    private void IsWaitingForBuilding (List<int> listind, Animator animator) {
        for (int i = 0; i < listind.Count; i++) {
            if (m_listObstacles[listind[i]].GetCurrentRestingTime () > 0f) {
                //Debug.Log("random resting");
                m_isWaitingForBuild = true;
                CallRandomActions (0.5f, 0f, 0f, 0f, animator);
                return;
            }
        }
        m_isWaitingForBuild = false;
    }

    private bool IsEnoughBombToDestroy (List<int> listIndObstacles) {
        int nbPlayersObstacles = 0;
        for (int i = 0; i < listIndObstacles.Count; i++) {
            if (IsObstaclesBuilt (new List<int> () { listIndObstacles[i] })) {
                ActiveObstacle activeObstacle = m_listObstacles[listIndObstacles[i]].transform.GetComponentInChildren<ActiveObstacle> ();
                if (null == activeObstacle) {
                    continue;
                }
                PlayerEntity.Player playerNumber = activeObstacle.GetPlayerNumber ();
                if (playerNumber != PlayerEntity.Player.Bot) {
                    nbPlayersObstacles++;
                }
            }
        }
        return nbPlayersObstacles <= GameManager.Instance.GetPlayer (PlayerEntity.Player.Bot).GetComponent<Bomb> ().GetBombStack ();
    }

    private bool isPlayerBunkered () {
        return IsObstaclesBuilt (new List<int> () { 0, 1, 3 }) && IsEnoughBombToDestroy (new List<int> () { 1 }) && null != m_listObstacles[1].transform.Find ("Pillar_S(Clone)");
    }

    #endregion

    #region setters
    public void SetObstacleConstructorList (List<ObstacleConstructor> listeObstacleConstructor) {
        m_listObstacles = listeObstacleConstructor;
    }

    public void SetCheckpointsList (List<CheckpointBase> listeCheckpoints) {
        m_listCheckpoints = listeCheckpoints;
    }

    public void SetPricesList (List<int> listPrices) {
        m_pricesList = listPrices;
    }

    public void SetPricesBuildList (List<int> listPricesBuild) {
        m_pricesBuildList = listPricesBuild;
    }

    public void SetBuildTypesList (List<ActiveObstacle.ObstacleType> listTypesBuild) {
        m_TypeBuildList = listTypesBuild;
    }
    public void SetListTypesUnits (List<Constant.SpawnTypeUnit> listeTypes) {
        m_listeTypeUnits = listeTypes;
    }

    public void SetBotSpawnUnitsList (List<SpawnUnits> list) {
        m_botSpawnUnits = list;
    }
    private void SetListOfObstaclesConstructed () {
        m_nbTtraps = 0;
        m_obstaclesConstructedIndList.Clear ();
        m_obstaclesConstructedByBotIndList.Clear ();
        for (int i = 0; i < m_listObstacles.Count; i++) {
            if (IsObstaclesBuilt (new List<int> () { i })) {
                m_obstaclesConstructedIndList.Add (i);
                ActiveObstacle activeObstacle = m_listObstacles[i].transform.GetComponentInChildren<ActiveObstacle> ();

                if (activeObstacle == null) {
                    //Debug.Log("activeObstacle est null");
                    return;
                }
                PlayerEntity.Player playerNumber = activeObstacle.GetPlayerNumber ();
                if (playerNumber == PlayerEntity.Player.Bot) {

                    if (null != m_listObstacles[i].transform.Find ("Trap_A(Clone)")) {
                        m_nbTtraps++;
                    }
                    m_obstaclesConstructedByBotIndList.Add (i);
                }
            }
        }
    }

    #endregion
}