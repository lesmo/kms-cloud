namespace Kms.Cloud.Api.Models.ResponseModels {
    public class FriendScoreResponse {
        /// <summary>
        ///     Objeto del Amigo que representa éste elemento de la lista.
        /// </summary>
        public FriendResponse Friend { get; set; }

        /// <summary>
        ///     Distancia Total recorrida por el Usuario representado en el
        ///     objeto Friend EN CENTÍMETROS.
        /// </summary>
        public double TotalDistance { get; set; }

        /// <summary>
        ///     Indica si el objeto Friend de éste elemento en la lista es
        ///     el Usuario de la sesión actual. Para facilitar resaltarlo
        ///     en las listas o lo que quieras.
        /// </summary>
        public bool IsMe { get; set; }
    }
}