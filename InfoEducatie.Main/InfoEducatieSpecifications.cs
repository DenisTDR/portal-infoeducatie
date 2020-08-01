using System.Collections.Generic;
using InfoEducatie.Contest.Exports;
using InfoEducatie.Main.Dashboard;
using InfoEducatie.Main.Data;
using InfoEducatie.Main.InfoEducatieAdmin;
using InfoEducatie.Main.Pages;
using InfoEducatie.Main.Seminars;
using MCMS.Admin.Users;
using MCMS.Base.Builder;
using MCMS.Builder;
using MCMS.Common.Translations.Translations;
using MCMS.Data;
using MCMS.Display.Link;
using MCMS.Display.Menu;
using MCMS.Files.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Main
{
    public class InfoEducatieSpecifications : MSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<BaseDbContext, ApplicationDbContext>();
            services.AddOptions<MenuConfig>().Configure(ConfigureMenu);
            services.AddOptions<SiteConfig>().Configure(c =>
            {
                c.SiteName = "Portal InfoEducație";
                c.SiteCopyright = "Copyright &copy; TDR 2020";
            });

            services.AddScoped<ImportService>();
            services.AddScoped<FinalXlsxExportService>();
        }

        private void ConfigureMenu(MenuConfig config)
        {
            // var typeConfig = new BaseEntityTypeConfiguration<User>();
            config.Items.Add(new MenuSection
            {
                Name = " ",
                Index = 0,
                Items = new List<IMenuItem>
                {
                    new MenuLink("Acasă", typeof(DashboardController)).WithIconClasses("fas fa-home"),
                    new MenuSection
                    {
                        Name = "Administrare",
                        IsCollapsed = true,
                        Items = new List<IMenuItem>
                        {
                            new MenuLink("Panou Administrare", typeof(InfoEducatieAdminController),
                                    nameof(InfoEducatieAdminController.Index)).WithIconClasses("fas fa-tools")
                                .RequiresRoles("Admin"),
                            new MenuSection
                            {
                                Name = "Conținut",
                                IsCollapsed = true,
                                Items = new List<IMenuItem>
                                {
                                    new MenuLink("Seminarii", typeof(SeminarsAdminController),
                                        nameof(SeminarsAdminController.Index)),
                                    new MenuLink("Pagini", typeof(PagesAdminController),
                                        nameof(PagesAdminController.Index))
                                }
                            }.WithIconClasses("fas fa-file-contract").RequiresRoles("Moderator"),
                            new MenuLink("Utilizatori", typeof(AdminUsersController),
                                    nameof(AdminUsersController.Index)).WithIconClasses("fas fa-users")
                                .RequiresRoles("Moderator", "Admin"),
                            new MenuLink("Texte / Traduceri", typeof(TranslationsController),
                                    nameof(TranslationsController.Index)).WithIconClasses("fas fa-globe")
                                .RequiresRoles("Admin"),
                            new MenuLink("Fișiere", typeof(FilesController), nameof(FilesController.Index))
                                .WithIconClasses("fas fa-copy").RequiresRoles("Admin"),
                        }
                    }.WithIconClasses("fas fa-tools").RequiresRoles("Admin", "Moderator")
                }
            });
            config.Items.Add(new MenuLink("Seminarii", typeof(SeminarsController),
                nameof(SeminarsController.Index)));
            config.Items.Add(new MenuLink("Program", "/Pages/program-2020") {Index = 20});
        }
    }
}