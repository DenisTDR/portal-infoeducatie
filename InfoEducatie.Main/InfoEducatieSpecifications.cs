using System.Collections.Generic;
using InfoEducatie.Main.Dashboard;
using InfoEducatie.Main.Data;
using InfoEducatie.Main.Seminars;
using MCMS.Admin.Users;
using MCMS.Base.Builder;
using MCMS.Builder;
using MCMS.Common.Translations.Translations;
using MCMS.Controllers;
using MCMS.Data;
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
                c.SiteName = "InfoEducație";
                c.SiteCopyright = "Copyright &copy; TDR 2020";
            });
        }

        private void ConfigureMenu(MenuConfig config)
        {
            // var typeConfig = new BaseEntityTypeConfiguration<User>();
            config.Items.Add(new MenuSection
            {
                Name = "Main",
                Index = 0,
                Items = new List<IMenuItem>
                {
                    new MenuLink("Home", typeof(DashboardController)).WithIconClasses(
                        "fas fa-tachometer-alt"),
                    new MenuSection
                    {
                        Name = "Administrare",
                        IsCollapsed = true,
                        Items = new List<IMenuItem>
                        {
                            new MenuLink("Panou Administrare", typeof(AdminDashboardController),
                                nameof(AdminDashboardController.Index)).WithIconClasses("fas fa-tools"),
                            new MenuSection
                            {
                                Name = "Conținut",
                                IsCollapsed = true,
                                Items = new List<IMenuItem>
                                {
                                    new MenuLink("Seminarii", typeof(SeminarsAdminController),
                                        nameof(SeminarsAdminController.Index))
                                }
                            }.WithIconClasses("fas fa-file-contract").RequiresRoles("Admin", "Moderator"),
                            new MenuLink("Utilizatori", typeof(AdminUsersController),
                                nameof(AdminUsersController.Index)).WithIconClasses("fas fa-users"),
                            new MenuLink("Texte / Traduceri", typeof(TranslationsController),
                                nameof(TranslationsController.Index)).WithIconClasses("fas fa-globe"),
                            new MenuLink("Fișiere", typeof(FilesController), nameof(FilesController.Index))
                                .WithIconClasses("fas fa-copy"),
                        }
                    }.WithIconClasses("fas fa-tools").RequiresRoles("Admin")
                }
            });
            config.Items.Add(new MenuLink("Seminarii", typeof(SeminarsController),
                nameof(SeminarsController.Index)));
        }
    }
}