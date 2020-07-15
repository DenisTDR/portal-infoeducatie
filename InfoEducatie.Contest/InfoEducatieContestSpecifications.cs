using System.Collections.Generic;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using MCMS.Admin.Users;
using MCMS.Base.Builder;
using MCMS.Controllers;
using MCMS.Display.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest
{
    public class InfoEducatieContestSpecifications : MSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<MenuConfig>().Configure(ConfigureMenu);
        }

        private void ConfigureMenu(MenuConfig config)
        {
            config.Items.Add(new MenuSection
            {
                Name = "Concurs",
                IsCollapsed = true,
                Items = new List<IMenuItem>
                {
                    new MenuLink("Categorii", typeof(CategoriesAdminController),
                        nameof(CategoriesAdminController.Index)),
                    new MenuLink("Criterii Jurizare", typeof(JudgingCriteriaAdminController),
                        nameof(JudgingCriteriaAdminController.Index))
                }
            }.RequiresRoles("Moderator"));
        }
    }
}