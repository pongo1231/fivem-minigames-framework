﻿using CitizenFX.Core;
using GamemodesServer.Utils;
using System;
using System.Collections.Generic;

namespace GamemodesServer.Core
{
    /// <summary>
    /// Player scooter manager class
    /// </summary>
    public class PlayerScooterManager : GmScript
    {
        /// <summary>
        /// Scooter player class
        /// </summary>
        private class ScooterPlayer
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_player">Player</param>
            public ScooterPlayer(Player _player)
            {
                Player = _player;
            }

            /// <summary>
            /// Player
            /// </summary>
            public Player Player { get; private set; }

            /// <summary>
            /// Scooter
            /// </summary>
            public Vehicle Scooter { get; set; }
        }

        /// <summary>
        /// List of scooter players
        /// </summary>
        private static List<ScooterPlayer> s_scooterPlayers = new List<ScooterPlayer>();

        /// <summary>
        /// Current scooter vehicle to spawn for clients
        /// </summary>
        private static string s_scooterVehicle;

        /// <summary>
        /// Player dropped function
        /// </summary>
        /// <param name="_player">Player</param>
        [PlayerDropped]
        private void OnPlayerDropped(Player _player)
        {
            // Get scooter player from player
            ScooterPlayer scooterPlayer = s_scooterPlayers.Find(_scooterPlayer => _scooterPlayer.Player == _player);

            // Check if scooter player exists
            if (scooterPlayer != null)
            {
                // Delete scooter
                scooterPlayer.Scooter.Delete();

                // Clean up from list
                s_scooterPlayers.Remove(scooterPlayer);
            }
        }

        /// <summary>
        /// Request scooter event by client
        /// </summary>
        /// <param name="_player">Player</param>
        /// <param name="_pos">Position</param>
        /// <param name="_rot">Rotation</param>
        [EventHandler("gamemodes:sv_cl_requestscooter")]
        private async void OnClientRequestScooter([FromSource]Player _player, Vector3 _pos, Vector3 _rot)
        {
            // Don't get event handler unregistered when client sends garbage to server
            try
            {
                // Abort if no vehicle model has been specified yet
                if (s_scooterVehicle == null)
                {
                    return;
                }

                // Check whether player is not registered already
                if (s_scooterPlayers.Find(_scooterPlayer => _scooterPlayer.Player == _player) == null)
                {
                    // Create new scooter player
                    ScooterPlayer scooterPlayer = new ScooterPlayer(_player);

                    // Add player to list
                    s_scooterPlayers.Add(scooterPlayer);

                    // Wait a bit
                    await Delay(4000);

                    // Set player position to scooter position
                    _player.Character.Position = _pos;

                    // Wait a bit
                    await Delay(4000);

                    // Spawn scooter
                    scooterPlayer.Scooter = await EntityPool.CreateVehicle(s_scooterVehicle, _pos, _rot);

                    // Make client aware of scooter
                    _player.TriggerEvent("gamemodes:cl_sv_spawnedscooter", scooterPlayer.Scooter.NetworkId);
                }
            }
            catch (Exception _e)
            {
                Log.WriteLine($"{_e}");
            }
        }

        /// <summary>
        /// Allow clients to spawn scooters
        /// </summary>
        /// <param name="_vehicleModel">Vehicle model to spawn for clients</param>
        public static void Enable(string _vehicleModel)
        {
            // Clear list
            s_scooterPlayers.Clear();

            // Set current model to specified model
            s_scooterVehicle = _vehicleModel;
        }

        /// <summary>
        /// Disallow clients to spawn scooters anymore
        /// </summary>
        public static void Disable()
        {
            s_scooterVehicle = null;
        }
    }
}