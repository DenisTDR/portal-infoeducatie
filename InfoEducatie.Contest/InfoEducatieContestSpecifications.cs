using System.Collections.Generic;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Exports;
using InfoEducatie.Contest.Judging.Judges;
using InfoEducatie.Contest.Judging.Judging;
using InfoEducatie.Contest.Judging.JudgingCriteria;
using InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection;
using InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints;
using InfoEducatie.Contest.Judging.Results;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Builder;
using MCMS.Display.Menu;
using MCMS.Display.Link;
using Microsoft.Extensions.DependencyInjection;

namespace InfoEducatie.Contest
{
    public class InfoEducatieContestSpecifications : MSpecifications
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<MenuConfig>().Configure(ConfigureMenu);
            services.AddScoped<JudgingService>();
            services.AddScoped<ResultsService>();
            services.AddScoped<FinalXlsxExportService>();
            services.AddScoped<FinalXlsxExportServiceWithCeilings>();
        }

        private void ConfigureMenu(MenuConfig config)
        {
            config.Add(new MenuSection
            {
                Name = "Judging",
                IsCollapsable = true,
                Items = new List<IMenuItemBase>
                {
                    new MenuLink("Judging Projects", typeof(JudgingController), nameof(JudgingController.Judging))
                        .WithIconClasses("fas fa-gavel").RequiresRoles("Jury").WithValues(new {type = "project"}),
                    new MenuLink("Judging Open", typeof(JudgingController), nameof(JudgingController.Judging))
                        .WithIconClasses("fas fa-gavel").RequiresRoles("Jury").WithValues(new {type = "open"}),
                    new MenuLink("Judges", typeof(JudgesAdminController),
                        nameof(JudgesAdminController.Index)).RequiresRoles("Moderator"),
                    new MenuLink("Judging criteria", typeof(JudgingCriteriaAdminController),
                        nameof(JudgingCriteriaAdminController.Index)).RequiresRoles("Moderator"),
                    new MenuLink("Judging criteria sections", typeof(JudgingCriteriaSectionsAdminController),
                        nameof(JudgingCriteriaSectionsAdminController.Index)).RequiresRoles("Moderator"),
                    new MenuLink("Judging criteria points", typeof(ProjectJudgingCriterionPointsAdminController),
                        nameof(ProjectJudgingCriterionPointsAdminController.Index)).RequiresRoles("God"),
                    // new MenuLink("Results", typeof(ResultsController), nameof(ResultsController.Index))
                    //     .WithIconClasses("fas fa-list-ol").RequiresRoles("Jury", "Moderator"),
                    // new MenuLink("Detailed results", typeof(ResultsController),
                    //         nameof(ResultsController.DetailedResults))
                    //     .WithIconClasses("fas fa-search").RequiresRoles("Jury", "Moderator"),
                }
            }.RequiresRoles("Moderator", "Jury"));
            config.Add(new MenuSection
            {
                Name = "Contest",
                IsCollapsable = true,
                Items = new List<IMenuItemBase>
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