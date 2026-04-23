using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RPS_Challenge.Core.Entities;

namespace RPS_Challenge.Core.Entities {
    /// <summary>
    /// Representa un juego o Round.
    /// </summary>
    public class Round {
        /// <summary>
        /// Lista de jugadores que se enfrentan en el Round actual.
        /// </summary>
        public List<Player> Players { get; set; }

        /// <summary>
        /// Jugador ganador del Torneo.
        /// </summary>
        public Player Winner { get; set; }

        #region Round
        /// <summary>
        /// Crea e inicializa una instancia del tipo <see cref="RPS_Challenge.Core.Entities.Round"/>
        /// </summary>
        /// <param name="playerList">Lista de objetos de tipo <see cref=" RPS_Challenge.Core.Entities.Player"/></param>
        public Round(List<Player> playerList) {
            this.Players = playerList;
        }
        #endregion Round
    }
}
