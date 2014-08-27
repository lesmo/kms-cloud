namespace Kms.Cloud.Api.Models.ResponseModels {
    public class ContactInfoResponse : IModifiedDate {
        /// <summary>
        ///     Teléfono de Casa del Usuario.
        /// </summary>
        public string HomePhone { get; set; }

        /// <summary>
        ///     Teléfono Móvil del Usuario.
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        ///     Teléfono de Trabajo del Usuario.
        /// </summary>
        public string WorkPhone { get; set; }
    }
}