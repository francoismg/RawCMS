﻿//******************************************************************************
// <copyright file="license.md" company="RawCMS project  (https://github.com/arduosoft/RawCMS)">
// Copyright (c) 2019 RawCMS project  (https://github.com/arduosoft/RawCMS)
// RawCMS project is released under GPL3 terms, see LICENSE file on repository root at  https://github.com/arduosoft/RawCMS .
// </copyright>
// <author>Daniele Fontani, Emanuele Bucarelli, Francesco Mina'</author>
// <autogenerated>true</autogenerated>
//******************************************************************************
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RawCMS.Library.DataModel;
using RawCMS.Library.Service;
using System.Threading;
using System.Threading.Tasks;
using IdentityRole = RawCMS.Plugins.Core.Model.IdentityRole;

namespace RawCMS.Plugins.Core.Stores
{
    public class RawRoleStore : IRoleStore<IdentityRole>
    {
        private readonly ILogger logger;
        private readonly CRUDService service;
        private const string collection = "_roles";

        public RawRoleStore(CRUDService service, ILogger logger)
        {
            this.service = service;
            this.logger = logger;
        }

        public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            role.RoleId = role.RoleId.ToLower();
            //TODO: Add check to avoid duplicates
            service.Insert(collection, JObject.FromObject(role));
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            service.Delete(collection, role.RoleId);
            return IdentityResult.Success;
        }

        public void Dispose()
        {
        }

        public async Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            DataQuery query = new DataQuery
            {
                //TODO: Check if serialization esclude null values
                RawQuery = JsonConvert.SerializeObject(new IdentityRole()
                {
                    RoleId = roleId
                })
            };
            //TODO: check for result count
            return service.Query(collection, query).Items.First.First.ToObject<IdentityRole>();
        }

        public async Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return await FindByIdAsync(normalizedRoleName, cancellationToken);
        }

        public async Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            return role.RoleId;
        }

        public async Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            return role.RoleId;
        }

        public async Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            return role.RoleId;
        }

        public async Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.RoleId = normalizedName;
        }

        public async Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
        {
            role.RoleId = roleName;
        }

        public async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            service.Update(collection, JObject.FromObject(role), true);
            return IdentityResult.Success;
        }
    }
}