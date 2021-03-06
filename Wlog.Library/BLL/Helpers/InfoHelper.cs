﻿//******************************************************************************
// <copyright file="license.md" company="Wlog project  (https://github.com/arduosoft/wlog)">
// Copyright (c) 2016 Wlog project  (https://github.com/arduosoft/wlog)
// Wlog project is released under LGPL terms, see license.md file.
// </copyright>
// <author>Daniele Fontani, Emanuele Bucaelli</author>
// <autogenerated>true</autogenerated>
//******************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NLog;
using Wlog.Library.BLL.Classes;

namespace Wlog.Library.BLL.Helpers
{
    /// <summary>
    /// Helper class to get application info from assembli and files using naming conventions
    /// </summary>
    public static class InfoHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static string GetAssemblyAttribute<T>(Func<T, string> value, Assembly a) where T : Attribute
        {
            if (a == null)
            {
                return string.Empty;
            }
            logger.Debug("[ih] entering GetAssemblyAttribute");
            T attribute = (T)Attribute.GetCustomAttribute(a, typeof(T));
            return value.Invoke(attribute);
        }


        public static AssemblyMetadata GetAssemblyMetadata(Assembly assembly)
        {
            if (assembly == null) return null;
            logger.Debug("[ih] entering GetAssemblyMetadata");
            AssemblyMetadata aa = new AssemblyMetadata();
            aa.Title = GetAssemblyAttribute<AssemblyTitleAttribute>(a => (a != null) ? a.Title : String.Empty, assembly);
            aa.Copyright = GetAssemblyAttribute<AssemblyCopyrightAttribute>(a => (a != null) ? a.Copyright : String.Empty, assembly);
            aa.Version = GetAssemblyAttribute<AssemblyVersionAttribute>(a => (a != null) ? a.Version : String.Empty, assembly);
            aa.Description = GetAssemblyAttribute<AssemblyDescriptionAttribute>(a => (a != null) ? a.Description : String.Empty, assembly);

            aa.FullName = assembly.FullName;
            AssemblyName an = assembly.GetName();
            aa.Culture = an.CultureInfo.DisplayName;
            aa.Architecture = an.ProcessorArchitecture.ToString();
            aa.PublicKeyToken = System.Text.Encoding.ASCII.GetString(an.GetPublicKeyToken());
            aa.Version = (String.IsNullOrEmpty(aa.Version)) ? an.Version.ToString() : aa.Version;
            return aa;
        }

        public static List<AssemblyMetadata> GetLoadedAssemblies()
        {
            logger.Debug("[ih] entering GetLoadedAssemblies");
            List<AssemblyMetadata> result = new List<AssemblyMetadata>();
            foreach (System.Reflection.AssemblyName an in System.Reflection.Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                System.Reflection.Assembly asm = System.Reflection.Assembly.Load(an.ToString());

                result.Add(GetAssemblyMetadata(asm));
            }
            return result;
        }


        public static string GetMarkdownContent(string path)
        {

            logger.Debug("[ih] entering GetMarkdownContent");

            string content = File.ReadAllText(path);

            var settings = CommonMark.CommonMarkSettings.Default.Clone();
            settings.OutputFormat = CommonMark.OutputFormat.Html;

            return CommonMark.CommonMarkConverter.Convert(content, settings);
        }

        public static InfoPageModel GetInfoPage(InfoPageConfiguration conf)
        {
            logger.Debug("[ih] entering GetInfoPage");

            InfoPageModel ip = new InfoPageModel();
            ip.MainAssembly = GetAssemblyMetadata(conf.MainAssembly);
            ip.LoadedAssemblies = GetLoadedAssemblies();
            ip.Conf = conf;
            if (conf.ShowInfo)
            {
                ip.Info = GetMarkdownContent(conf.InfoPath);
            }

            if (conf.ShowLicense)
            {
                ip.License = GetMarkdownContent(conf.LicensePath);
            }

            if (conf.ShowChangeLog)
            {
                ip.ChangeLogs = GetMarkdownContent(conf.ChangeLogPath);
            }

            return ip;
        }

        public static string ResolveUrl(string path)
        {
            logger.Debug("[ih] entering ResolveUrl({0})", path);
            return HttpContext.Current.Server.MapPath(path);
        }
    }
}
