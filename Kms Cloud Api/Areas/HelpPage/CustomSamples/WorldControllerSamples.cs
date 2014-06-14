using Kms.Cloud.Api.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Kms.Cloud.Api.Areas.HelpPage.CustomSamples {
    public static class WorldControllerSamples {
        public static void Register(Dictionary<Type, object> typeSamples, HttpConfiguration config) {
            var worldResponseSample = new WorldCountryResponse[] {
                new WorldCountryResponse {
                    Code = DefaultValues.CountryCode1,
                    Name = DefaultValues.CountryName1 ,
                    Subdivisions = new WorldSubdivisionResponse[] {
                        new WorldSubdivisionResponse {
                            Code = DefaultValues.SubdivisionCode1,
                            Name = DefaultValues.SubdivisionName1
                        },
                        new WorldSubdivisionResponse {
                            Code = DefaultValues.SubdivisionCode2,
                            Name = DefaultValues.SubdivisionName2
                        }
                    }     
                },
                new WorldCountryResponse {
                    Code = DefaultValues.CountryCode2,
                    Name = DefaultValues.CountryName2 ,
                    Subdivisions = new WorldSubdivisionResponse[] {
                        new WorldSubdivisionResponse {
                            Code = DefaultValues.SubdivisionCode3,
                            Name = DefaultValues.SubdivisionName3
                        },
                        new WorldSubdivisionResponse {
                            Code = DefaultValues.SubdivisionCode4,
                            Name = DefaultValues.SubdivisionName4
                        }
                    }     
                }
            };

            typeSamples.Add(typeof(IEnumerable<WorldCountryResponse>), worldResponseSample);
        }
    }
}