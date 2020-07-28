using System.Collections.Generic;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Judging.Judges;
using InfoEducatie.Contest.Judging.Judging;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection;
using InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints;
using InfoEducatie.Contest.Judging.Results;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using InfoEducatie.Contest.Participants.ProjectParticipant;
using MCMS.Base.Builder;
using MCMS.Display.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest
{
    public class InfoEducatieContestSpecifications : MSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<MenuConfig>().Configure(ConfigureMenu);
            services.AddScoped<JudgingService>();
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
                        nameof(JudgingController.Index)).WithIconClasses("fas fa-gavel").RequiresRoles("Jury"),
                    new MenuLink("Judges", typeof(JudgesAdminController),
                        nameof(JudgesAdminController.Index)).RequiresRoles("Moderator"),
                    new MenuLink("Judging criteria", typeof(JudgingCriteriaAdminController),
                        nameof(JudgingCriteriaAdminController.Index)).RequiresRoles("Moderator"),
                    new MenuLink("Judging criteria sections", typeof(JudgingCriteriaSectionsAdminController),
                        nameof(JudgingCriteriaSectionsAdminController.Index)).RequiresRoles("Moderator"),
                    new MenuLink("Judging criteria points", typeof(ProjectJudgingCriterionPointsAdminController),
                        nameof(ProjectJudgingCriterionPointsAdminController.Index)).RequiresRoles("God"),
                    new MenuLink("Results", typeof(ResultsController),
                            nameof(ResultsController.Index)).WithIconClasses("fas fa-list-ol")
                        .RequiresRoles("Jury", "Moderator"),
                }
            }.RequiresRoles("Moderator", "Jury"));
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
                    new MenuLink("Project Participants", typeof(ProjectParticipantsAdminController),
                        nameof(ProjectParticipantsAdminController.Index)).RequiresRoles("God"),
                    new MenuLink("Categories", typeof(CategoriesAdminController),
                        nameof(CategoriesAdminController.Index)),
                }
            }.RequiresRoles("Moderator"));
        }
    }
}