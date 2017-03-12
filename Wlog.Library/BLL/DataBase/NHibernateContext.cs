﻿//******************************************************************************
// <copyright file="license.md" company="Wlog project  (https://github.com/arduosoft/wlog)">
// Copyright (c) 2016 Wlog project  (https://github.com/arduosoft/wlog)
// Wlog project is released under LGPL terms, see license.md file.
// </copyright>
// <author>Daniele Fontani, Emanuele Bucaelli</author>
// <autogenerated>true</autogenerated>
//******************************************************************************
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using NLog;

namespace Wlog.DAL.NHibernate.Helpers
{
    public class NHibernateContext
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Get a configuration using settings in web.config and map entities in this assembly
        /// </summary>
        /// <returns></returns>
        public static Configuration GetConfiguration()
        {
            logger.Debug("[NH] getting configuration. This should happen only once.");

            Configuration cfg = new Configuration();

            //if (System.Configuration.ConfigurationManager.AppSettings["_override_connectionstring"] != null)
            //{
            //    cfg.SetProperty("_override_connectionstring", System.Configuration.ConfigurationManager.AppSettings["_override_connectionstring"]);
            //}


            //if (System.Configuration.ConfigurationManager.AppSettings["_override_connectionstring"] != null)
            //{
            //    cfg.SetProperty("_override_connectionstring", System.Configuration.ConfigurationManager.AppSettings["_override_connectionstring"]);
            //}


            //ModelMapper mapper = new ModelMapper();
            //mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());

            //HbmMapping domainMapping =
            //  mapper.CompileMappingForAllExplicitlyAddedEntities();
            //cfg.AddMapping(domainMapping);
            cfg.Configure();

            return cfg;
        }

        public Configuration Configuration { get; set; }
        public ISessionFactory SessionFactory { get; set; }

        private static NHibernateContext CreateNewContext()
        {
            logger.Debug("[NH] generating NHibernateContext. This should happen only once at startup.");
            NHibernateContext ctx = new NHibernateContext();
            ctx.Configuration = GetConfiguration();
            ctx.SessionFactory = ctx.Configuration.BuildSessionFactory();
            return ctx;
        }

        public static void ApplySchemaChanges()
        {
            logger.Debug("[NH] ApplySchemaChanges. This should happen only once at startup. changes should be done only at firt run or after upgrade.");

            Configuration cfg = GetConfiguration();

            SchemaMetadataUpdater.QuoteTableAndColumns(cfg);
            //NHibernate.Tool.hbm2ddl.SchemaExport schema = new NHibernate.Tool.hbm2ddl.SchemaExport(cfg);
            
            //schema.Create(false, true);
            var update = new SchemaUpdate(cfg);
           
            update.Execute(true, true);

            foreach (var v in update.Exceptions)
            {
                logger.Error(v);
            }

        }


        private static NHibernateContext current;
        public static NHibernateContext Current
        {
            get
            {
              
                return current ?? (current = CreateNewContext());
            }
        }

    }
}