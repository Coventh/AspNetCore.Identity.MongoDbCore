﻿using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;

namespace AspNetCore.Identity.MongoDbCore.Models
{
    /// <summary>
    /// A <see cref="MongoIdentityRole{TKey}"/> where TKey is a <see cref="string"/>
    /// </summary>
    public class MongoDbIdentityRole : MongoIdentityRole<string>
    {
        /// <summary>
        /// The constructor for a <see cref="MongoDbIdentityRole"/>
        /// </summary>
        public MongoDbIdentityRole() : base()
        {
        }

        /// <summary>
        /// The constructor for a <see cref="MongoDbIdentityRole"/>, taking a role name.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        public MongoDbIdentityRole(string roleName) : base(roleName)
        {
        }
    }

    /// <summary>
    /// A <see cref="MongoIdentityRole{TKey}"/> where TKey is a <see cref="Guid"/>
    /// </summary>
    public class MongoIdentityRole : MongoIdentityRole<Guid>
    {
        /// <summary>
        /// The constructor for a <see cref="MongoIdentityRole"/>
        /// </summary>
        public MongoIdentityRole() : base()
        {
        }

        /// <summary>
        /// The constructor for a <see cref="MongoIdentityRole"/>, taking a role name.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        public MongoIdentityRole(string roleName) : base(roleName)
        {
        }
    }

    /// <summary>
    /// A document representing an <see cref="IdentityRole{TKey}"/> document.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    public class MongoIdentityRole<TKey> : IdentityRole<TKey>, IDocument<TKey>, IClaimHolder
        where TKey : IEquatable<TKey>
    {

        private void InitializeFields()
        {
            Version = 1;
            Claims = new List<MongoClaim>();
            Guid guidValue = Guid.NewGuid();
            var idTypeName = typeof(TKey).Name;
            switch (idTypeName)
            {
                case "Guid":
                    Id = (TKey)(object)guidValue;
                    break;
                case "Int16":
                    Id = (TKey)(object)GlobalVariables.Random.Next(1, short.MaxValue);
                    break;
                case "Int32":
                    Id = (TKey)(object)GlobalVariables.Random.Next(1, int.MaxValue);
                    break;
                case "Int64":
                    Id = (TKey)(object)(GlobalVariables.Random.NextLong(1, long.MaxValue));
                    break;
                case "String":
                    Id = (TKey)(object)guidValue.ToString();
                    break;
            }
        }

        /// <summary>
        /// The constructor for a <see cref="MongoIdentityRole{TKey}"/>
        /// </summary>
        public MongoIdentityRole()
        {
            InitializeFields();
        }

        /// <summary>
        /// The constructor for a <see cref="MongoIdentityRole{TKey}"/>, taking a role name.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        public MongoIdentityRole(string roleName)
        {
            Name = roleName;
            InitializeFields();
        }

        /// <summary>
        /// The constructor for a <see cref="MongoIdentityRole{TKey}"/>, taking a role name and a primary key value.
        /// </summary>
        /// <param name="name">The name of the role.</param>
        /// <param name="key">The value of the primary key</param>
        public MongoIdentityRole(string name, TKey key)
        {
            InitializeFields();
            Id = key;
            Name = Name;
        }

        /// <summary>
        /// The version of the role schema
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// The claims associated to the role
        /// </summary>
        public List<MongoClaim> Claims { get; set; }
    }
}
