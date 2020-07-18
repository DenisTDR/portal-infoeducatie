using System.Collections.Generic;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.Judge;
using InfoEducatie.Contest.Judging.Judging;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
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
                Name = "Judging",
                IsCollapsed = true,
                Items = new List<IMenuItem>
                {
                    new MenuLink("Judging", typeof(JudgingController),
                        nameof(JudgingController.Index)).WithIconClasses("fas fa-gavel"),
                    new MenuLink("Judges", typeof(JudgesAdminController),
                        nameof(JudgesAdminController.Index)),
                    new MenuLink("Judging criteria", typeof(JudgingCriteriaAdminController),
                        nameof(JudgingCriteriaAdminController.Index)),
                    new MenuLink("Judging criteria points", typeof(ProjectJudgingCriterionPointsAdminController),
                        nameof(ProjectJudgingCriterionPointsAdminController.Index)),
                }
            }.RequiresRoles("Moderator"));
            config.Items.Add(new MenuSection
            {
                Name = "Contest",
                IsCollapsed = true,
                Items = new List<IMenuItem>
                {
                    new MenuLink("Participants", typeof(ParticipantsAdminController),
                        nameof(ParticipantsAdminController.Index)),
                    new MenuLink("Projects", typeof(ProjectsAdminController),
                        nameof(ProjectsAdminController.Index)),
                    new MenuLink("Categories", typeof(CategoriesAdminController),
                        nameof(CategoriesAdminController.Index)),
                }
            }.RequiresRoles("Moderator"));
        }
    }
}