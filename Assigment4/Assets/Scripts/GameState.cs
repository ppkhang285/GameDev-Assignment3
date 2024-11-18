using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameState
{
    public int Turn { get; set; } // Current turn in the game
    public (int, int, int)[][] Cells { get; private set; } // Item1 = -1 means empty cell, Item2 = -1 means Lord, first item indicates the player that owns the chess, second item is the type of the chess, third item is its current hp
    public PlayerData[] Players { get; private set; } // list of players 
    public bool[] Defeated { get; private set; }

    public GameState(int turn, (int, int, int) [][] cells, PlayerData[] players, bool[] defeated)
    {
        Turn = turn;
        Cells = cells;
        Players = players;
        Defeated = defeated;
    }

    public bool HasLost(int currentPlayer)
    {
        if (Turn > GameConstants.TurnReceiveEnergy) // No more energy is generated
            if (Players[currentPlayer].IsDead())
            {
                ClearBoard(currentPlayer);
                Debug.Log("Player " + currentPlayer.ToString() + " lost");
                return true;
            }
            else
                return false;
        else
        {
            if (Players[currentPlayer].LordHP <= 0)
            {
                ClearBoard(currentPlayer);
                Debug.Log("Player " + currentPlayer.ToString() + " lost");
                return true;
            }
            else
                return false;
        }
    }

    public bool HasWon(int currentPlayer)
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (i != currentPlayer)
            {
                if (!Defeated[i]) return false; 
            }
        }
        return true;
    }

    public void CheckDefeatedPlayer()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Defeated[i]) continue;
            if (HasLost(i)) Defeated[i] = true;
        }
    }

    public void ChangeTurn(int player, int energyAmount)
    {
        if ((Turn - 2 - player) / Players.Length < GameConstants.TurnReceiveEnergy)
            Players[player].ReceiveEnergy(energyAmount);
        Players[player].RestoreAP();
    }

    public void ClearBoard(int player) // Clear the cells that has player's lord or character 
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            for (int j = 0; j < Cells[i].Length; j++)
            {
                if (Cells[i][j].Item1 == player)
                {
                    Cells[i][j] = (-1, -1, 0);
                }
            }
        }
        Players[player].ClearBoard();
    }

    public List<Move> GetLegalSpawn(int currentPlayer, float dropChance)
    {
        //if (defeated[currentPlayer]) return new List<Move>(); // If player is defeated, cannot do anything
        PlayerData player = Players[currentPlayer];
        List<Move> moves = new List<Move>();

        // Iterate through spawning locations
        foreach (Vector2Int spawnLocation in player.SpawnLocations)
        {
            // Spawning location is occupied by other chess 
            if (Cells[spawnLocation.y][spawnLocation.x].Item1 != -1) continue;
            for (int i = 0; i < player.Characters.Length; i++)
            {
                CharacterData chessPiece = player.Characters[i];
                if (!chessPiece.Spawned && chessPiece.characterStats.cost <= player.Energy) // Chess is not spawned and enough energy to spawn
                {
                    Vector2Int spawnLoc = new Vector2Int(spawnLocation.x, spawnLocation.y);
                    moves.Add(new Move(new Vector2Int(currentPlayer, i), spawnLoc, MoveType.Spawn));
                }
            }
        }

        return moves;
    }

    public List<Move> GetLegalMoves(int currentPlayer, float dropChance)
    {
        //if (defeated[currentPlayer]) return new List<Move>(); // If player is defeated, cannot do anything
        PlayerData player = Players[currentPlayer];
        List<Move> moves = new List<Move>();

        // Spawn chess
        moves.AddRange(this.GetLegalSpawn(currentPlayer, dropChance));

        // Move or attack
        for (int i = 0; i < player.Characters.Length; i++)
        {
            CharacterData chessPiece = player.Characters[i];
            if (chessPiece.Spawned && !chessPiece.Dead && chessPiece.AP > 0) // Chess is spawned and not dead and still has AP
            {
                Vector2Int location = chessPiece.Location;
                int movementRange = chessPiece.characterStats.movementRange;
                int attackRange = chessPiece.characterStats.attackRange;

                // Possible cells to move
                for (int j = -movementRange; j <= movementRange; j++)
                {
                    int remain = movementRange - Mathf.Abs(j);
                    for (int k = -remain; k <= remain; k++)
                    {
                        if (location.x + j < 0 || location.x + j >= Cells.Length || location.y + k < 0 || location.y + k >= Cells.Length) continue; // Out of the board
                        if (j == 0 && k == 0) continue; // The chess's current cell
                        if (Cells[location.y + k][location.x + j].Item1 == -1) // Cell is empty
                        {
                            Vector2Int target = new Vector2Int(location.x + j, location.y + k);
                            Vector2Int loc = new Vector2Int(location.x, location.y);
                            Move newMove = new Move(loc, target, MoveType.CharMove);
                            if (Utils.ManhattanDistance(loc, Players[currentPlayer].LordLocation) > Utils.ManhattanDistance(target, Players[currentPlayer].LordLocation))
                            {
                                bool random = UnityEngine.Random.value >= dropChance;
                                if (random)
                                    moves.Add(newMove);
                            } else
                            {
                                moves.Add(newMove);
                            }
                        }
                    }
                }

                // Possible cells to attack
                for (int j = -attackRange; j <= attackRange; j++)
                {
                    int remain = attackRange - Mathf.Abs(j);
                    for (int k = -remain; k <= remain; k++)
                    {
                        if (location.x + j < 0 || location.x + j >= Cells.Length || location.y + k < 0 || location.y + k >= Cells.Length) continue; // Out of the board
                        if (j == 0 && k == 0) continue; // The chess's current cell
                        if (Cells[location.y + k][location.x + j].Item1 != -1 && Cells[location.y + k][location.x + j].Item1 != currentPlayer) // Cell is not empty and the piece in the cell is of enemy team 
                        {
                            Vector2Int target = new Vector2Int(location.x + j, location.y + k);
                            Vector2Int loc = new Vector2Int(location.x, location.y);
                            Move newMove = new Move(loc, target, MoveType.CharAttack);                           
                            moves.Add(newMove);
                        }
                    }
                }
            }
        }

        // Doing nothing is also an option
        moves.Add(new Move(new Vector2Int(-1, -1), new Vector2Int(-1, -1), MoveType.Idle));

        return moves;
    }

    public void ApplyMove(Move move)
    {
        if (move.Type == MoveType.CharMove)
            ApplyCharMove(move);
        else if (move.Type == MoveType.CharAttack)
            ApplyCharAttack(move);
        else if (move.Type == MoveType.Spawn)
            ApplySpawn(move);
        else;
        CheckDefeatedPlayer();
    }

    public void ApplyCharMove(Move move)
    {
        (int, int, int) sourceCell = Cells[move.Source.y][move.Source.x];
        PlayerData player = Players[sourceCell.Item1]; 
        int chessIndex = sourceCell.Item2;
        player.CharMove(new Vector2Int(move.Target.x, move.Target.y), chessIndex);

        Cells[move.Target.y][move.Target.x] = (Cells[move.Source.y][move.Source.x].Item1, Cells[move.Source.y][move.Source.x].Item2, Cells[move.Source.y][move.Source.x].Item3);
        Cells[move.Source.y][move.Source.x] = (-1, -1, 0); // Source becomes empty

        for (int i = 0; i < Players.Length; i++)
        {
            // Chess ends turn at opponent's spawn location
            if (i != player.PlayerNo) 
            {
                foreach (Vector2Int spawnLocation in Players[i].SpawnLocations)
                {
                    if (spawnLocation.x == move.Target.x && spawnLocation.y == move.Target.y)
                        Players[i].LostSpawnLocationAt(spawnLocation);
                    break;
                }
            }
        }
        
    }

    public void ApplySpawn(Move move)
    {
        PlayerData player = Players[move.Source.x];
        int chessIndex = move.Source.y;
        Cells[move.Target.y][move.Target.x] = (move.Source.x, chessIndex, player.Characters[chessIndex].characterStats.hp);
        player.SpawnChar(new Vector2Int(move.Target.x, move.Target.y), chessIndex);
    }

    public void ApplyCharAttack(Move move)
    {
        (int, int, int) sourceCell = Cells[move.Source.y][move.Source.x];
        PlayerData attackPlayer = Players[sourceCell.Item1];
        int attackerIndex = sourceCell.Item2;
        CharacterData attacker = attackPlayer.Characters[attackerIndex];
        attackPlayer.CharAttack(attackerIndex);

        (int, int, int) targetCell = Cells[move.Target.y][move.Target.x];
        PlayerData targetPlayer = Players[targetCell.Item1];
        // If target is not Lord
        if (targetCell.Item2 != -1)
        {
            int targetIndex = targetCell.Item2;
            CharacterData target = targetPlayer.Characters[targetIndex];
            targetPlayer.CharTakeDmg(attacker.characterStats.damage, targetIndex);
            if (target.CurrentHP <= 0)
            {
                Cells[move.Target.y][move.Target.x] = (-1, -1, 0); // Target is dead, cell becomes empty
            } else
            {
                Cells[move.Target.y][move.Target.x].Item3 = target.CurrentHP; // Change the cell state
            }
        }
        else         // If target is Lord
        {
            targetPlayer.LordHP -= attacker.characterStats.damage;
            Cells[move.Target.y][move.Target.x].Item3 = targetPlayer.LordHP;
            attackPlayer.CharTakeDmg(GameConstants.LordDmg, attackerIndex);
            if (attacker.CurrentHP <= 0)
            {
                Cells[move.Source.y][move.Source.x] = (-1, -1, 0); // Attacker is dead, cell becomes empty
            } else
            {
                Cells[move.Source.y][move.Source.x].Item3 = attacker.CurrentHP;
            }
        }
    }

    public List<float> ToFeatures()
    {
        List<float> features = new List<float>();

        features.Add(Turn);

        foreach (var row in Cells)
        {
            foreach (var cell in row)
            {
                features.Add(cell.Item1); // Owner of the chess
            }
        }

        foreach (PlayerData player in Players)
        {
            features.AddRange(player.ToFeatures());
        }

        return features;
    }
}
