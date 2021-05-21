using System.Collections.Generic;
using InfoEducatie.Contest.Judging.Results;
using InfoEducatie.Main.Dashboard;
using InfoEducatie.Main.Data;
using InfoEducatie.Main.InfoEducatieAdmin;
using InfoEducatie.Main.InfoEducatieAdmin.Diplomas;
using InfoEducatie.Main.Pages;
using InfoEducatie.Main.Seminars;
using MCMS.Admin.Users;
using MCMS.Base.Builder;
using MCMS.Base.Data.Seeder;
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
                c.SiteCopyright = "Copyright &copy; TDR 2021";
                c.FaviconPath = "~/favicon.ico";
            });

            services.AddScoped<ImportService>();
            services.AddScoped<DiplomasService>();

            services.AddOptions<LayoutIncludesOptions>().Configure(c => { c.AddForPages("InfoeducatieIncludes"); });

            services.AddOptions<SeedSources>().Configure(ss =>
                ss.Add((typeof(InfoEducatieSpecifications).Assembly, "seed-ie.json")));
        }

        private void ConfigureMenu(MenuConfig config)
        {
            config.Add(new MenuSection
            {
                Name = " ",
                Index = 0,
                Items = new List<IMenuItemBase>
                {
                    new MenuLink("Acasă", typeof(DashboardController)).WithIconClasses("fas fa-home"),
                    new MenuSection
                    {
                        Name = "Administrare",
                        IsCollapsable = true,
                        Items = new List<IMenuItemBase>
                        {
                            new MenuLink("Panou Administrare", typeof(InfoEducatieAdminController),
                                    nameof(InfoEducatieAdminController.Index)).WithIconClasses("fas fa-tools")
                                .RequiresRoles("Admin"),
                            new MenuSection
                            {
                                Name = "Conținut",
                                IsCollapsable = true,
                                Items = new List<IMenuItemBase>
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
                            new MenuLink("Results", typeof(ResultsController), nameof(ResultsController.Index))
                                .WithIconClasses("fas fa-list-ol").RequiresRoles("Jury", "Moderator"),
                            new MenuLink("Detailed results", typeof(ResultsController),
                                    nameof(ResultsController.DetailedResults))
                                .WithIconClasses("fas fa-search").RequiresRoles("Jury", "Moderator"),
                        }
                    }.WithIconClasses("fas fa-tools").RequiresRoles("Admin", "Moderator")
                }
            });
            // config.Add(new MenuLink("Seminarii", typeof(SeminarsController),
                // nameof(SeminarsController.Index)));
            config.Add(new MenuLink("Program", "/Pages/program") {Index = 20});
        }
    }
}