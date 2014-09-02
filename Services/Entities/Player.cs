using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPS_Challenge.Services.Entities {
    /// <summary>
    /// Representa un Jugador.
    /// </summary>
    public class Player {
        /// <summary>
        /// Nombre del Jugador.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Estrategia del Jugador.
        /// R: Piedra, P: Papel, S: Tijera
        /// </summary>
        public string Strategy { get; set; }

        #region Player
        /// <summary>
        /// Crea e inicializa una instancia del tipo <see cref="RPS_Challenge.Services.Entities.Player"/>
        /// </summary>
        /// <param name="name">Nombre del Jugador.</param>
        /// <param name="strategy">Estrategia del Jugador (R, P, S)</param>
        public Player(string name, string strategy) {
            this.Name = name;
            this.Strategy = strategy.ToUpper();
        }
        #endregion Player
    }
}
