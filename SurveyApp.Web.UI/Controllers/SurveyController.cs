﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Core.DTO;
using SurveyApp.Domain.Entities;
using SurveyApp.Domain.RepositoryContracts;
using SurveyApp.Web.UI.Models;

namespace SurveyApp.Web.UI.Controllers
{
    public class SurveyController(IUnitOfWork unitOfWork, IMapper mapper) : Controller
    {

        [HttpGet]
        [Route("/{survey_id:int}")]
        public async Task<IActionResult> TakeSurvey(int survey_id)
        {
            var survey = await unitOfWork.Surveys.GetAsync(r => r.Id == survey_id);
            if (survey == null)
                return NotFound();
            var model = new SurveyViewModel()
            {
                Request = new() { SurveyId = survey.Id },
                Survey = mapper.Map<SurveyResponse>(survey)
            };
            return View(model);
        }
        [HttpPost]
        [Route("/{survey_id:int}")]
        public async Task<IActionResult> TakeSurvey(int survey_id, SurveyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var survey = await unitOfWork.Surveys.GetAsync(r => r.Id == survey_id);
                model.Survey = mapper.Map<SurveyResponse>(survey);
                return View(model);
            }

            var request = mapper.Map<Submission>(model.Request);
            request.CreatedOn = DateTime.Now;
            await unitOfWork.Submissions.CreateAsync(request);
            //await unitOfWork.SaveAsync();
            TempData["success"] = "Survey submitted successfully";
            return RedirectToAction(nameof(TakeSurvey), new { survey_id });
        }
        [HttpGet]
        [Route("/statistics/{survey_id:int}")]
        public IActionResult ShowSurveyStatistics(int survey_id)
        {
            return View("SurveyStatistics");
        }
    }
}
