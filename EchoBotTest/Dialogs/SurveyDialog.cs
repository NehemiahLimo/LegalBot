using LegalBotTest.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegalBotTest.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace LegalBotTest.Dialogs
{
    public class SurveyDialog : ComponentDialog
    {
        private readonly BotStateService _botStateService;

        public SurveyDialog(string dialogId, BotStateService botStateService) : base(dialogId)
        {

            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            { 
                //Create Waterfall steps
               ServiceMenuStepAsync,
               ServiceSubMenuStepAsync,
               QuestionStepAsync,
               ProvisionServiceStepAsync,
               AccessibilityStepAsync,
               ImproveAccessStepAsync,
               RecommendationStepAsync,
               OverallexperienceStepAsync,
               ImproveExperienceStepAsync,
               SurveySummaryStepAsync
            };

            //Add Named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(SurveyDialog)}.mainFlow", waterfallSteps));
            AddDialog(new ChoicePrompt($"{nameof(SurveyDialog)}.serviceMenu"));
            AddDialog(new ChoicePrompt($"{nameof(SurveyDialog)}.serviceSubMenu"));
            AddDialog(new ChoicePrompt($"{nameof(SurveyDialog)}.question"));
            AddDialog(new TextPrompt($"{nameof(SurveyDialog)}.provisionService"));
            AddDialog(new ChoicePrompt($"{nameof(SurveyDialog)}.accessibility"));
            AddDialog(new TextPrompt($"{nameof(SurveyDialog)}.improveAccess"));
            AddDialog(new ChoicePrompt($"{nameof(SurveyDialog)}.recommend"));
            AddDialog(new ChoicePrompt($"{nameof(SurveyDialog)}.overallExperience"));
            AddDialog(new TextPrompt($"{nameof(SurveyDialog)}.improveExperience"));
            AddDialog(new ChoicePrompt($"{nameof(SurveyDialog)}.surveySummary"));

            //set the starting Dialog
            InitialDialogId = $"{nameof(SurveyDialog)}.mainFlow";

        }

        //waterfall steps
  
        //Get User Name

        private async Task<DialogTurnResult> ServiceMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            //Get the current profile object from user state
            var userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            return await stepContext.PromptAsync($"{nameof(SurveyDialog)}.serviceMenu", new PromptOptions
            {
                Prompt = MessageFactory.Text(string.Format("Hi {0}, Welcome back.Please choose from:", userProfile.Name)),
                Choices = ChoiceFactory.ToChoices(new List<string> { "1. EMPLOYMENT LAW", " 2. PROPERTY LAW", "3. DOMESTIC LAW" }),
            }, cancellationToken);
        }

        //Get county Location

        private async Task<DialogTurnResult> ServiceSubMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["serviceMenu"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync($"{nameof(SurveyDialog)}.serviceSubMenu", new PromptOptions

            {
                Prompt = MessageFactory.Text("Please Choose from:"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "1. INFORMATION", " 2. REQUEST A LAWYER", "3. GIVE FEEDBACK" }),
            }, cancellationToken);

        }

        //get sub-county location

        private async Task<DialogTurnResult> QuestionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["serviceSubMenu"] = ((FoundChoice)stepContext.Result).Value;

            if (stepContext.Values["serviceSubMenu"] == "GIVE FEEDBACK")
            {
                return await stepContext.PromptAsync($"{nameof(SurveyDialog)}.question", new PromptOptions

                {
                    Prompt = MessageFactory.Text("Answer a few questions to tell us how we can improve. How useful was the information or service you received?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "1. USEFUL", "2. NOT USEFUL" }),
                }, cancellationToken);
            }
            else
            {
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text("Select a service to proceed"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "INFORMATION", "NEWS", "REFERAL", "SURVEY", "UPDATE PROFILE", "SHARE" }),
                };
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.subMenu",promptOptions, cancellationToken);
            }

           
        }

        //get ward location

        private async Task<DialogTurnResult> ProvisionServiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["question"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync($"{nameof(SurveyDialog)}.provisionService", new PromptOptions

            {
                Prompt = MessageFactory.Text("Okay, what information or service would you like us to provide? please type your response."),
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> AccessibilityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["provisionService"] = (string)stepContext.Result;

            return await stepContext.PromptAsync($"{nameof(SurveyDialog)}.accessibility", new PromptOptions

            {
                Prompt = MessageFactory.Text("Well noted. How easy was it for you to access the information you required? Please choose:"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "1. EASY", "2. HARD" }),
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> ImproveAccessStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["accessibility"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync($"{nameof(SurveyDialog)}.improveAccess", new PromptOptions

            {
                Prompt = MessageFactory.Text("Almost done! How can I make it easier? Please type your Response"),
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> RecommendationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["improveAccess"] = (string)stepContext.Result;

            return await stepContext.PromptAsync($"{nameof(SurveyDialog)}.recommend", new PromptOptions

            {
                Prompt = MessageFactory.Text("How likely are you to recommend the service to your family and friends? 0-not likely, 10-very likely"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "0","1","2","3","4","5","6","7","8","9", "10" }),
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> OverallexperienceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["recommend"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync($"{nameof(SurveyDialog)}.overallExperience", new PromptOptions

            {
                Prompt = MessageFactory.Text("Thank you.how would you rate your overall experience with our service? 0-not likely, 10-very likely"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" }),
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> ImproveExperienceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["overallExperience"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync($"{nameof(SurveyDialog)}.improveExperience", new PromptOptions

            {
                Prompt = MessageFactory.Text("Lastly how can I make your overall experience better? please type your response."),
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> SurveySummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["improveExperience"] = (string)stepContext.Result;

            //saving feedback data in json file
            //Get the current profile object from user state
            var surveyData = await _botStateService.SurveyDataAccessor.GetAsync(stepContext.Context, () => new SurveyData(), cancellationToken);

            //var Location = await _botStateService.LocationAccessor.GetAsync(stepContext.Context, () => new Location(), cancellationToken);

            //save all of the data inside the user profile
            surveyData.Question = (string)stepContext.Values["question"];
            surveyData.ProvisionService = (string)stepContext.Values["provisionService"];
            surveyData.Accesibility = (string)stepContext.Values["accessibility"];
            surveyData.ImproveAccess = (string)stepContext.Values["improveAccess"];
            surveyData.Recommend = (string)stepContext.Values["recommend"];
            surveyData.OverallExperience= (string)stepContext.Values["overallExperience"];
            surveyData.ImproveExperience = (string)stepContext.Values["improveExperience"];

           

            //save data in userstate
            await _botStateService.SurveyDataAccessor.SetAsync(stepContext.Context, surveyData);

            //Write user details to a json file
            var feedbackData = JsonConvert.SerializeObject(surveyData, Formatting.Indented);
            var filePath = @".\Resources\feedback.json";

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, feedbackData);
            }
            else
            {
                File.AppendAllText(filePath, feedbackData);
            }

            //end of saving feedback data in json file

            return await stepContext.PromptAsync($"{nameof(SurveyDialog)}.surveySummary", new PromptOptions

            {
                Prompt = MessageFactory.Text("Thank you very much for your feedback.we appreciate your support.please choose from. MAIN MENU to continue using the service"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "1. INFORMATION", " 2. NEWS", "3. REFERAL", "4. SURVEY", "5. UPDATE PROFILE", "6. SHARE" }),
            }, cancellationToken);

            stepContext.Values["surveySummary"] = ((FoundChoice)stepContext.Result).Value;

            //waterfallStep always finishes with the end of the waterfall or with another dialog here it is the end

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);



        }

    }

}

