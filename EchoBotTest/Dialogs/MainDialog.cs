using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using LegalBotTest.Services;
using System.Text.RegularExpressions;



namespace LegalBotTest.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly BotStateService _botStateService;


        public MainDialog(BotStateService botStateService) : base(nameof(MainDialog))
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            //Create Waterfall Steps
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinallStepAsync
            };

            //Add Named Dialogs
            //AddDialog(new SurveyDialog($"{nameof(MainDialog)}.survey", _botStateService));
            AddDialog(new UserRegistrationDialog($"{nameof(MainDialog)}.userRegistration", _botStateService));

            AddDialog(new WaterfallDialog(nameof(MainDialog), waterfallSteps));

            //Set the starting Dialog
            InitialDialogId = nameof(MainDialog);

        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
          
                return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.userRegistration", null, cancellationToken);
         
        }

        private async Task<DialogTurnResult> FinallStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }


    }
}
