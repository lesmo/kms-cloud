using Kilometros_WebAPI.Exceptions;
using Kilometros_WebAPI.Helpers;
using Kilometros_WebAPI.Models.ResponseModels;
using Kilometros_WebAPI.Security;
using Kilometros_WebGlobalization.API;
using KilometrosDatabase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kilometros_WebAPI.Controllers {
    [Authorize]
    public class TipsController : IKMSController {
        [HttpGet]
        [Route("tips/categories")]
        public IEnumerable<TipCategoryResponse> GetTipsCategories() {
            // --- Verificar si se tiene la cabecera {If-Modified-Since} ---
            DateTimeOffset? ifModifiedSince
                = Request.Headers.IfModifiedSince;

            if ( ifModifiedSince.HasValue ) {
                TipCategoryGlobalization lastTipCategoryGlobalization
                    = Database.TipCategoryGlobalizationStore.GetFirst(
                        orderBy: o =>
                            o.OrderBy(b => b.CreationDate)
                    );

                if ( lastTipCategoryGlobalization != null && ifModifiedSince.Value.UtcDateTime > lastTipCategoryGlobalization.CreationDate )
                    throw new HttpNotModifiedException();
            }

            // --- Obtener las categorías ---
            IEnumerable<TipCategory> tipCategories
                = Database.TipCategoryStore.GetAll();
            List<TipCategoryResponse> tipCategoriesResponse
                = new List<TipCategoryResponse>();

            // --- Obtener los textos en el Idioma + Cultura actuales ---
            foreach ( TipCategory tipCategory in tipCategories ) {
                tipCategoriesResponse.Add(
                    new TipCategoryResponse() {
                        TipCategoryId
                            = tipCategory.Guid.ToBase64String(),
                        Name
                            = tipCategory.GetGlobalization().Name,
                        Description
                            = tipCategory.GetGlobalization().Description
                    }
                );
            }

            return tipCategoriesResponse;
        }

        [HttpGet]
        [Route("tips/history")]
        public IEnumerable<TipResponse> GetTipsHistory(int page = 0, int perPage = 20) {
            // --- Verificar si se tiene la cabecera {If-Modified-Since} ---
            DateTimeOffset? ifModifiedSince
                = Request.Headers.IfModifiedSince;

            if ( ifModifiedSince.HasValue ) {
                UserTipHistory lastTipHistory
                = Database.UserTipHistoryStore.GetFirst(
                    filter: f =>
                        f.User.Guid == CurrentUser.Guid,
                    orderBy: o =>
                        o.OrderBy(b => b.CreationDate)
                );

                if ( lastTipHistory != null && ifModifiedSince.Value.UtcDateTime > lastTipHistory.CreationDate )
                    throw new HttpNotModifiedException();
            }

            // --- Obtener los Tips conseguidos ---
            IEnumerable<UserTipHistory> tipsHistory
                = Database.UserTipHistoryStore.GetAll(
                    filter: f =>
                        f.User.Guid == CurrentUser.Guid,
                    orderBy: o =>
                        o.OrderByDescending(b => b.CreationDate),
                    extra: x =>
                        x.Skip(page * perPage).Take(perPage),
                    include:
                        new string[] { "Tip.TipCategory" }
                );

            // --- Preparar respuesta ---
            List<TipResponse> response
                = new List<TipResponse>();
            Dictionary<Guid, TipCategoryResponse> tipCategoryLocales
                = new Dictionary<Guid,TipCategoryResponse>();

            foreach ( UserTipHistory tipHistory in tipsHistory ) {
                // Obtener Tip en el Idioma actual
                TipGlobalization tipGlobalization
                    = tipHistory.Tip.GetGlobalization();
                
                // Añadir el Tip sólo si se tienen textos en idioma solicitado
                if ( string.IsNullOrEmpty(tipGlobalization.Text) ) 
                    continue;

                // Obtener Categoría de Tip
                TipCategory tipCategory
                    = tipHistory.Tip.TipCategory;

                TipCategoryResponse tipCategoryResponse;

                if ( ! tipCategoryLocales.ContainsKey(tipCategory.Guid) ) {
                    tipCategoryLocales.Add(
                        tipCategory.Guid,
                        new TipCategoryResponse {
                            TipCategoryId
                                = tipCategory.Guid.ToBase64String(),
                            Name
                                = tipCategory.GetGlobalization().Name,
                            Description
                                = tipCategory.GetGlobalization().Description,
                        }
                    );
                }
                
                tipCategoryResponse
                    = tipCategoryLocales[tipCategory.Guid];
                
                response.Add(new TipResponse() {
                    TipId
                        = tipHistory.Guid.ToBase64String(),
                    TipCategory
                        = tipCategoryResponse,

                    Timestamp
                        = tipHistory.CreationDate,
                    Text
                        = tipGlobalization.Text,
                    Source
                        = tipGlobalization.Source
                });
            }

            return response;
        }

        [HttpGet]
        [Route("tips/{tipGuidBase64}")]
        public TipResponse GetTip(string tipId) {
            // --- Obtener Tip Guid e intentar buscarlo ---
            UserTipHistory tip
                = Database.UserTipHistoryStore.Get(tipId);

            if ( tip == null )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning801_TipNotFound
                );
            
            // --- Obtener Categoría de Tip ---
            TipCategory tipCategory
                = tip.Tip.TipCategory;
            
            // --- Preparar y enviar la respuesta ---
            return new TipResponse() {
                TipId
                    = tip.Guid.ToBase64String(),
                TipCategory
                    = new TipCategoryResponse {
                        TipCategoryId
                            = tipCategory.Guid.ToBase64String(),
                        Name
                            = tipCategory.GetGlobalization().Name,
                        Description
                            = tipCategory.GetGlobalization().Description
                    },

                Timestamp
                    = tip.CreationDate,
                Text
                    = tip.Tip.GetGlobalization().Text,
                Source
                    = tip.Tip.GetGlobalization().Source
            };
        }
    }
}
