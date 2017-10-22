﻿using AspNetCore.Identity.MongoDbCore.Interfaces;
using AspNetCore.Identity.MongoDbCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace AspNetCore.Identity.MongoDbCore.Extensions
{
    /// <summary>
    /// The extensions for an object that holds claims.
    /// </summary>
    public static class ClaimHolderExtensions
    {
        public static MongoClaim ToMongoClaim(this Claim claim)
        {
            return new MongoClaim
            {
                Type = claim.Type,
                Value = claim.Value,
                Issuer = claim.Issuer
            };
        }

        public static Claim ToClaim(this MongoClaim claim)
        {
            return new Claim(claim.Type, claim.Value, null, claim.Issuer);
        }

        /// <summary>
        /// Adds a claim to a claim holder, implementing <see cref="IClaimHolder"/>.
        /// </summary>
        /// <param name="claimHolder">The object holding claims.</param>
        /// <param name="claim">The claim you want to add.</param>
        /// <returns>Returns true if the claim was added.</returns>
        public static bool AddClaim(this IClaimHolder claimHolder, Claim claim)
        {
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            // prevent adding duplicate claims
            if (claimHolder.HasClaim(claim))
            {
                return false;
            }

            claimHolder.Claims.Add(claim.ToMongoClaim());
            return true;
        }

        /// <summary>
        /// Replaces a claim on a claim holder, implementing <see cref="IClaimHolder"/>.
        /// </summary>
        /// <param name="claimHolder">The object holding claims.</param>
        /// <param name="claim">The claim you want to replace.</param>
        /// <param name="newClaim">The new claim you want to set.</param>
        /// <returns>Returns true if the claim was replaced.</returns>
        public static bool ReplaceClaim(this IClaimHolder claimHolder, Claim claim, Claim newClaim)
        {
            var replaced = false;
            claimHolder.Claims.Where(uc => uc.Value == claim.Value && uc.Type == claim.Type).ToList()
                       .ForEach(oldClaim => {
                           oldClaim.Type = newClaim.Type;
                           oldClaim.Value = newClaim.Value;
                           oldClaim.Issuer = newClaim.Issuer;
                           replaced |= true;
                       });
            return replaced;
        }

        /// <summary>
        /// Checks if an object implementing <see cref="IClaimHolder"/> has a claim.
        /// </summary>
        /// <param name="claimHolder">The object holding claims.</param>
        /// <param name="claim">The claim you want to replace.</param>
        /// <returns>Returns true if the claim is present, false otherwise.</returns>
        public static bool HasClaim(this IClaimHolder claimHolder, Claim claim)
        {
            if(claimHolder.Claims == null)
            {
                claimHolder.Claims = new List<MongoClaim>();
            }
            return claimHolder.Claims.Any(e => e.Value == claim.Value && e.Type == claim.Type);
        }

        public static bool RemoveClaim(this IClaimHolder claimHolder, Claim claim)
        {
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }
            var exists = claimHolder.Claims
                                    .FirstOrDefault(e => e.Value == claim.Value 
                                                      && e.Type == claim.Type);
            if (exists != null)
            {
                claimHolder.Claims.Remove(exists);
                return true;
            }
            return false;
        }

        public static bool RemoveClaims(this IClaimHolder claimHolder, IEnumerable<Claim> claims)
        {
            var someClaimsRemoved = false;
            foreach (var claim in claims)
            {
                var matchedClaims = claimHolder.Claims.Where(uc => uc.Value == claim.Value && uc.Type == claim.Type)
                                               .ToList();

                foreach (var c in matchedClaims)
                {
                    claimHolder.Claims.Remove(c);
                    someClaimsRemoved |= true;
                }
            }
            return someClaimsRemoved;
        }
    }
}
