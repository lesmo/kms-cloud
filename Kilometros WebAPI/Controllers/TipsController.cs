﻿using Kilometros_WebAPI.Exceptions;
using Kilometros_WebAPI.Helpers;
using Kilometros_WebAPI.Models.HttpGet.TipsController;
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
    public class TipsController : ApiController {
        KilometrosDatabase.Abstraction.WorkUnit Database
            = new KilometrosDatabase.Abstraction.WorkUnit();

        [HttpGet]
        [Route("tips/categories")]
        public TipCategoryResponse[] GetTipsCategories() {
            /** Obtener el último Idioma + Cultura añadido **/
            TipCategoryGlobalization lastTipCategoryGlobalization
                = Database.TipCategoryGlobalizationStore.GetFirst(
                    orderBy: tcg => tcg.OrderBy(o => o.CreationDate)
                );

            /** Verificar si se tiene la cabecera {If-Modified-Since} **/
            DateTimeOffset? ifModifiedSince
                = Request.Headers.IfModifiedSince;

            if ( ifModifiedSince.HasValue && lastTipCategoryGlobalization != null ) {
                if ( ifModifiedSince.Value.UtcDateTime > lastTipCategoryGlobalization.CreationDate )
                    throw new HttpNotModifiedException();
            }

            /** Obtener las categorías **/
            IEnumerable<TipCategory> tipCategories
                = Database.TipCategoryStore.GetAll();
            List<TipCategoryResponse> tipCategoriesResponse
                = new List<TipCategoryResponse>();

            /** Obtener los textos en el Idioma + Cultura actuales **/
            foreach ( TipCategory tipCategory in tipCategories ) {
                // Obtener Categoría en el Idioma actual
                //   - Buscar match exacto de Idioma + Culture
                //   - Buscar match sólo de Idioma
                TipCategoryGlobalization tipCategoryLocale
                    = Database.TipCategoryStore.GetGlobalization(
                        tipCategory
                    ) as TipCategoryGlobalization;

                if ( tipCategoryLocale == null ) {
                    tipCategoriesResponse.Add(
                        new TipCategoryResponse() {
                            TipCategoryId
                                = MiscHelper.Base64FromGuid(tipCategory.Guid),
                            Name
                                = null,
                            Description
                                = null
                        }
                    );
                } else {
                    tipCategoriesResponse.Add(
                        new TipCategoryResponse() {
                            TipCategoryId
                                = MiscHelper.Base64FromGuid(tipCategory.Guid),
                            Name
                                = tipCategoryLocale.Name,
                            Description 
                                = tipCategoryLocale.Description
                        }
                    );
                }
            }

            return tipCategoriesResponse.ToArray();
        }

        [HttpGet]
        [Route("tips/history")]
        public TipResponse[] GetTipsHistory(int page = 1) {
            KmsIdentity identity
                = (KmsIdentity)User.Identity;
            User user
                = identity.UserData;

            /** Obtener el último Tip conseguido **/
            UserTipHistory lastTipHistory
                = Database.UserTipHistoryStore.GetFirst(
                    t => t.User == user,
                    o => o.OrderBy(by => by.CreationDate)
                );

            /** Verificar si se tiene la cabecera {If-Modified-Since} **/
            DateTimeOffset? ifModifiedSince
                = Request.Headers.IfModifiedSince;

            if ( ifModifiedSince.HasValue && lastTipHistory != null ) {
                if ( ifModifiedSince.Value.UtcDateTime > lastTipHistory.CreationDate )
                    throw new HttpNotModifiedException();
            }

            /** Obtener los Tips conseguidos **/
            IEnumerable<UserTipHistory> tipsHistory
                = (
                    from tip in user.UserTipHistory
                    orderby tip.CreationDate
                    select tip
                ).Skip(10 * page).Take(10);

            /** Preparar respuesta **/
            List<TipResponse> response
                = new List<TipResponse>();
            Dictionary<Guid, TipCategoryResponse> tipCategoryLocales
                = new Dictionary<Guid,TipCategoryResponse>();

            foreach ( UserTipHistory tipHistory in tipsHistory ) {
                // Obtener Tip en el Idioma actual
                TipGlobalization tipLocale
                    = Database.UserTipHistoryStore.GetGlobalization(
                        tipHistory
                    ) as TipGlobalization;
                
                // Añadir el Tip sólo si se tienen textos en idioma solicitado
                if ( tipLocale == null ) 
                    continue;

                // Obtener Categoría de Tip
                TipCategory tipCategory
                    = tipHistory.Tip.TipCategory;

                TipCategoryResponse tipCategoryResponse;
                if ( tipCategoryLocales.ContainsKey(tipCategory.Guid) ) {
                    tipCategoryResponse = tipCategoryLocales[tipCategory.Guid];
                } else {
                    TipCategoryGlobalization tipCategoryLocale
                        = Database.TipCategoryStore.GetGlobalization(
                            tipCategory
                        ) as TipCategoryGlobalization;
                    
                    if ( tipCategoryLocale == null ) {
                        tipCategoryResponse
                            = new TipCategoryResponse() {
                                TipCategoryId
                                    = MiscHelper.Base64FromGuid(tipCategory.Guid),
                                Name
                                    = null,
                                Description
                                    = null
                            };
                    } else {
                        tipCategoryResponse
                            = new TipCategoryResponse() {
                                TipCategoryId
                                    = MiscHelper.Base64FromGuid(tipCategory.Guid),
                                Name    
                                    = tipCategoryLocale.Name,
                                Description 
                                    = tipCategoryLocale.Description
                            };
                    }

                    tipCategoryLocales.Add(
                        tipCategory.Guid,
                        tipCategoryResponse
                    );
                }

                response.Add(new TipResponse() {
                    TipId
                        = MiscHelper.Base64FromGuid(tipHistory.Guid),
                    TipCategory
                        = tipCategoryResponse,

                    Timestamp
                        = tipHistory.CreationDate,
                    Text
                        = tipLocale.Text,
                    Source
                        = tipLocale.Source
                });
            }

            return response.ToArray();
        }

        [HttpGet]
        [Route("tips/{tipGuidBase64}")]
        public TipResponse GetTip(string tipGuidBase64) {
            /** Obtener Tip Guid e intentar buscarlo **/
            Guid? tipGuid
                = MiscHelper.GuidFromBase64(tipGuidBase64);

            if ( ! tipGuid.HasValue )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning801_TipNotFound
                );

            UserTipHistory tip
                = Database.UserTipHistoryStore.Get(tipGuid.Value);

            if ( tip == null )
                throw new HttpNotFoundException(
                    ControllerStrings.Warning801_TipNotFound
                );

            /** Obtener Tip en el Idioma actual **/
            TipGlobalization tipLocale
                = Database.TipStore.GetGlobalization(tip.Tip) as TipGlobalization;

            /** Obtener Categoría de Tip **/
            TipCategory tipCategory
                = tip.Tip.TipCategory;
            TipCategoryGlobalization tipCategoryLocale
                = Database.TipCategoryStore.GetGlobalization(tipCategory) as TipCategoryGlobalization;

            TipCategoryResponse tipCategoryResponse;
            if ( tipCategoryLocale == null ) {
                tipCategoryResponse
                    = new TipCategoryResponse() {
                        TipCategoryId
                            = MiscHelper.Base64FromGuid(tipCategory.Guid),
                        Name
                            = null,
                        Description 
                            = null
                    };
            } else {
                tipCategoryResponse
                    = new TipCategoryResponse() {
                        TipCategoryId
                            = MiscHelper.Base64FromGuid(tipCategory.Guid),
                        Name
                            = tipCategoryLocale.Name,
                        Description
                            = tipCategoryLocale.Description
                    };
            }

            /** Preparar y enviar la respuesta **/
            TipResponse tipResponse
                = new TipResponse() {
                    TipId
                        = MiscHelper.Base64FromGuid(tip.Guid),
                    TipCategory
                        = tipCategoryResponse,

                    Timestamp
                        = tip.CreationDate,
                    Text
                        = tipLocale.Text,
                    Source
                        = tipLocale.Source
                };

            return tipResponse;
        }
    }
}