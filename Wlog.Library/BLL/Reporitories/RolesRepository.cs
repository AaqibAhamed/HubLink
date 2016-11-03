﻿//******************************************************************************
// <copyright file="license.md" company="Wlog project  (https://github.com/arduosoft/wlog)">
// Copyright (c) 2016 Wlog project  (https://github.com/arduosoft/wlog)
// Wlog project is released under LGPL terms, see license.md file.
// </copyright>
// <author>Daniele Fontani, Emanuele Bucaelli</author>
// <autogenerated>true</autogenerated>
//******************************************************************************
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wlog.BLL.Entities;
using Wlog.DAL.NHibernate.Helpers;
using Wlog.Library.BLL.Classes;
using Wlog.Library.BLL.DataBase;
using Wlog.Library.BLL.Enums;
using Wlog.Library.BLL.Interfaces;

namespace Wlog.Library.BLL.Reporitories
{

    public class RolesRepository : EntityRepository
    {
        private Logger _logger => LogManager.GetCurrentClassLogger();

        public RolesRepository()
        {
        }

        public List<RolesEntity> GetAllApplicationRoles(ApplicationEntity applicationEntity)
        {

            using (IUnitOfWork uow = BeginUnitOfWork())
            {
                uow.BeginTransaction();
                if (applicationEntity != null)
                {

                    List<Guid> ids = uow.Query<AppUserRoleEntity>()
                        .Where(x => x.ApplicationId.Equals(applicationEntity.IdApplication))
                        .Select(x => x.RoleId).ToList();
                    return uow.Query<RolesEntity>().Where(x => ids.Contains(x.Id)).ToList();
                }
            }
            return null;
        }

        public List<AppUserRoleEntity> GetApplicationRolesForUser(UserEntity userEntity)
        {

            using (IUnitOfWork uow = BeginUnitOfWork())
            {
                uow.BeginTransaction();
                return uow.Query<AppUserRoleEntity>().Where(c => c.UserId.Equals(userEntity.Id)).ToList();
            }
        }

        public RolesEntity GetRoleByName(string rolename)
        {

            using (IUnitOfWork uow = BeginUnitOfWork())
            {
                uow.BeginTransaction();
                return uow.Query<RolesEntity>().FirstOrDefault(x => x.RoleName == rolename);
            }
        }

        public bool Save(RolesEntity rolesEntity)
        {
            try
            {

                using (IUnitOfWork uow = BeginUnitOfWork())
                {
                    uow.BeginTransaction();
                    uow.SaveOrUpdate(rolesEntity);
                    uow.Commit();
                }

                return true;
            }
            catch (Exception err)
            {
                _logger.Error(err);
            }

            return false;
        }

        public List<RolesEntity> GetAllRoles()
        {
            return GetAllRoles(RoleScope.All);
        }


        public List<RolesEntity> GetAllRoles(RoleScope scope)
        {
            List<RolesEntity> result = new List<RolesEntity>();
            try
            {

                using (IUnitOfWork uow = BeginUnitOfWork())
                {
                    var query = uow.Query<RolesEntity>();
                    if (scope == RoleScope.None)
                    {
                        return new List<RolesEntity>();
                    }

                    if (scope != RoleScope.All)
                    {
                        if (scope == RoleScope.Application)
                        {
                            query = query.Where(x => x.ApplicationScope == true);
                        }

                        if (scope == RoleScope.Global)
                        {
                            query = query.Where(x => x.GlobalScope == true);
                        }
                    }
                    result.AddRange(query.ToList());
                }
            }
            catch (Exception err)
            {
                _logger.Error(err);
            }

            return result;
        }

        public List<ProfilesRolesEntity> GetProfilesRolesForUser(UserEntity userEntity)
        {
            using (IUnitOfWork uow = BeginUnitOfWork())
            {
                uow.BeginTransaction();
                return uow.Query<ProfilesRolesEntity>().Where(c => c.ProfileId.Equals(userEntity.ProfileId)).ToList();
            }
        }

        public List<RolesEntity> GetAllRolesForUser(UserEntity userEntity)
        {
            List<RolesEntity> result = new List<RolesEntity>();

            try
            {
                var rolesIds = GetProfilesRolesForUser(userEntity).Select(x => x.RoleId);

                using (IUnitOfWork uow = BeginUnitOfWork())
                {
                    result.AddRange(uow.Query<RolesEntity>().Where(x => rolesIds.Contains(x.Id)).ToList());
                }
            }
            catch (Exception err)
            {
                _logger.Error(err);
            }

            return result;
        }
    }
}
