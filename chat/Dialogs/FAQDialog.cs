using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading.Tasks;           //Add to process Async Task
using Microsoft.Bot.Connector;          //Add for Activity Class
using Microsoft.Bot.Builder.Dialogs;    //Add for Dialog Class
using QnAMakerDialog;
using QnAMakerDialog.Models;
using System.Threading;

namespace GreatWall
{
    [Serializable]
    //[QnAMakerService(Host, EndpointKey, knowledgebases, MaxAnswers = 0)]
    [QnAMakerService("https://greatwallqna2017.azurewebsites.net/qnamaker",
        "c0cfbd47-316c-4b3b-955c-750b9ff8fb8f", "f85a97fc-9a00-4ff7-ba50-59322eda81fa",
        MaxAnswers = 5)]

    public class FAQDialog : QnAMakerDialog<string>
    {
        //This method is called automatically when there are no results for the question
        public override async Task NoMatchHandler(IDialogContext context, string originalQueryText)
        {
            await context.PostAsync($"Sorry, I couldn't find an answer for'{originalQueryText}'.");
            context.Wait(MessageReceived);
        }

        //This method is called automatically when there is a result for the question
        public override async Task DefaultMatchHandler(IDialogContext context, 
                                            string originalQueryText, QnAMakerResult result)
        {
            if(originalQueryText.ToUpper() == "EXIT")
            {
                context.Done("");
                return;
            } await context.PostAsync(result.Answers.First().Answer);
            context.Wait(MessageReceived);
        }

        [QnAMakerResponseHandler(0.5)] //1:100% 0.5:50%
        //This method is called when there is a low-order result
        public async Task LowScoreHandler(IDialogContext context, string originalQueryText,
                                                    QnAMakerResult result)
        {
            if (originalQueryText.ToUpper() == "EXIT")
            {
                context.Done("");
                return;
            }
            var messageActivity = ProcessResultAndCreateMessageActivity(context, ref result);
            messageActivity.Text = $"I found an answer that might help..." + $"{result.Answers.First().Answer}.";
            await context.PostAsync(messageActivity);
            context.Wait(MessageReceived);
        }
    }
}