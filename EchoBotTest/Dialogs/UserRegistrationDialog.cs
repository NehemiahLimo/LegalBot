using LegalBotTest.Models;
using LegalBotTest.Services;
using Microsoft.AspNetCore.Internal;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace LegalBotTest.Dialogs
{
    public class UserRegistrationDialog : ComponentDialog
    {
        #region Variables
        private readonly BotStateService _botStateService;
        #endregion

        public UserRegistrationDialog(string dialogId, BotStateService botStateService) : base(dialogId)
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            //Create Waterfall Steps
            var waterfallSteps = new WaterfallStep[]
            {
                LanguageStepAsync,
                NameStepAsync,
                CountyStepAsync,
                SubCountyStepAsync,
                WardStepAsync,
                //SummaryStepAsync,
                MainMenuStepAsync

            };

            //Add Named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(UserRegistrationDialog)}.mainFlow", waterfallSteps));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.language"));
            AddDialog(new TextPrompt($"{nameof(UserRegistrationDialog)}.name"));  //, ServiceDateValidationAsync
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.county")); //ServiceTimeValidationAsync));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.subCounty")); //PurchaseAmountValidationAsync));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.ward"));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.mainMenu"));



            //Set the starting Dialog
            InitialDialogId = $"{nameof(UserRegistrationDialog)}.mainFlow";

        }

        private async Task<DialogTurnResult> LanguageStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            
            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.language",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Hujambo, karibu katika huduma yetu. Tafadhali chagua lugha inayokufaa (1.KISWAHILI),(2.KINGEREZA)\n" +
                    "Hello, welcome to our service. Please choose your preferred language.(1.KISWAHILI),(2. ENGLISH)"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "KISWAHILI/KISWAHILI","KINGEREZA/ENGLISH"}),
                }, cancellationToken);
        }
        private async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["language"] = (FoundChoice)stepContext.Result;

            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.name",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Welcome to our service, we would like to ask you a few questions for the purpose of registration. What is your full name?"),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> CountyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //Get location information
             


            //Filter for administrative location 


            //We'll need present list to user
            
            
            
            
            
            
            
            stepContext.Values["name"] = (string)stepContext.Result;

            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.county",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Which county do you live in?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Nairobi", "Mombasa", "Kilifi", "Kiambu","Nyanza","Kajiado"}),
                }, cancellationToken);
        }

            private async Task<DialogTurnResult> SubCountyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["county"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.subCounty",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Which sub-county do you are you located at?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> {"Kamkunji","Embakasi West","Westlands","Dagoretti South","Kibra","Mathare"}),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> WardStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["subCounty"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.ward",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Which ward do you live in?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> {"Eastleigh North","Eastleigh South","Pumwani/Shauri Moyo","Califonia","Airbase"}),
                }, cancellationToken);
        }

       //private List<string> GetLocations(string administrativeKey, string filterKey = null, string filterValue  = null)
       // {
       //     //Source
       //     var file = AppContext.BaseDirectory + "/files/locations.json";
       //     //Read values and convert to json
       //     var locations = JsonConvert.DeserializeObject<JArray>(File.ReadAllText(file));

       //     var json = @"{
       //                     "id": "554",
       //                     "ward": "NGENDA",
       //                     "constituency": "GATUNDU SOUTH",
       //                     "county": "KIAMBU",
       //                     "country": "Kenya",
       //                     "lat": "-1.00777276516811",
       //                     "long": "36.8982576650519",
       //                     "sublocation": "Kiminyu"
       //                 }"

       //     var items = new List<string>();

       //     //Loop though array
       //     foreach(var obj in locations)
       //     {
       //         if (filterKey != null && obj.Value<sring>(filterKey) != filterValue)
       //                continue;

       //         items.Add(obj.Value<string>(administrativeKey));
             
       //     }
       //     return items;
       // }




        private async Task<DialogTurnResult> MainMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["ward"] = ((FoundChoice)stepContext.Result).Value;

            //Get the current profile object from user state.
            var userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            //Save all of the data inside the user profile
            userProfile.Name = (string)stepContext.Values["name"];
            userProfile.County = (string)stepContext.Values["county"];
            userProfile.SubCounty = (string)stepContext.Values["subCounty"];
            userProfile.Ward = (string)stepContext.Values["ward"];

            //Show summary to the user
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Here is the summary of your personal details:"), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Name: {0}", userProfile.Name)), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("County: {0}", userProfile.County)), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Subcounty: {0}", userProfile.SubCounty)), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Ward: {0}", userProfile.Ward)), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Congratulations {0}! You are now registered to use our service. Please choose (1.MAIN MENU ) to continue using the service.",userProfile.Name)), cancellationToken);

            //save data in userstate
            await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);

            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.mainMenu",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("MAIN MENU"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "INFORMATION", "NEWS", "REFERAL", "SURVEY", "UPDATE PROFILE","SHARE" }),
                }, cancellationToken);

            stepContext.Values["mainMenu"] = (FoundChoice)stepContext.Result;

            //WaterfallStep always finishes with the end with the end of the Waterfall or with another dialog, here it is in the end
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }



    }
}
