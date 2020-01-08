using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;       //Add for List<>
using AdaptiveCards;
using GreatWall.Helpers;

namespace GreatWall
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        protected int count = 1;
        string strMessage;
        private string strServerUrl = "http://localhost:3984/Images/";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }   
        
        public async Task MessageReceivedAsync(IDialogContext context, 
                                               IAwaitable<object> result)
        {
            var message = context.MakeMessage();

            List<CardImage> auto01_images = new List<CardImage>();   //Create image object
            auto01_images.Add(new CardImage() { Url = "https://pds.joins.com/news/component/htmlphoto_mmdata/201608/04/htm_2016080484837486184.jpg" });

            List<CardAction> auto01_Button = new List<CardAction>();   //Create Button object
            auto01_Button.Add(new CardAction() { Title = "1. FAQ (도움말)", Value = "1", Type = ActionTypes.ImBack });
            auto01_Button.Add(new CardAction() { Title = "2. 자동추천", Value = "2", Type = ActionTypes.ImBack });
            auto01_Button.Add(new CardAction() { Title = "3, 부위선택", Value = "3", Type = ActionTypes.ImBack });

            //Create Hero Card-01
            HeroCard memu01_Card = new HeroCard()
            {
                Title = "안녕하세요. 운동법을 알려주는 '마요미'입니다.",
                Subtitle = "어떻게 운동 자세를 알려드릴까요?\n(버튼을 클릭해주세요)",
                Images = auto01_images,
                Buttons = auto01_Button
            };
            message.Attachments.Add(memu01_Card.ToAttachment());
            await context.PostAsync(message);
            context.Wait(SendWelcomeMessageAsync);
        }

        public async Task SendWelcomeMessageAsync(IDialogContext context,
                                               IAwaitable<object> result)
        {
            Activity activity = await result as Activity;
            string strSelected = activity.Text.Trim();
            
            if(strSelected == "1")
            {
                strMessage = "[FAQ] 질문을 입력해주세요.\n 종료는 exit를 입력해주세요.";
                await context.PostAsync(strMessage);

                context.Call(new FAQDialog(), DialogResumeAfter);
            }
            else if(strSelected == "2")
            {
                strMessage = "[자동추천]을 선택하셨습니다.\n체형에 맞는 운동을 위해 BMI 지수를 계산하겠습니다.";
                await context.PostAsync(strMessage);
                strMessage = "당신의 만나이를 알려주세요.\n(숫자로만 입력해주세요.)\n 종료는 exit를 입력해주세요.";
                await context.PostAsync(strMessage);

                context.Call(new UserDialog(), DialogResumeAfter);
            }
            else if (strSelected == "3")
            {
                strMessage = "[부위선택]을 선택하셨습니다.\n";
                await context.PostAsync(strMessage);

                strMessage = "어떤 부분을 운동하고싶으신가요?\n(버튼을 클릭해주세요)";
                await context.PostAsync(strMessage);


                var message = context.MakeMessage();


                message.Attachments.Add(CardHelper.GetHero2Card("상체", this.strServerUrl + "dongsuk1.jpg", "상체", "1"));
                message.Attachments.Add(CardHelper.GetHero2Card("하체",
                                        this.strServerUrl + "dongsuk2.jpg", "하체", "2"));
                message.Attachments.Add(CardHelper.GetHero2Card("복부",
                                        this.strServerUrl + "dongsuk3.jpg", "복부", "3"));
                message.Attachments.Add(CardHelper.GetHero2Card("직접입력",
                                        this.strServerUrl + "dongsuk4.jpg", "직접입력", "4"));


                message.AttachmentLayout = "carousel";
                await context.PostAsync(message);

                context.Call(new OrderDialog(), DialogResumeAfter);

            }
            else
            {
                strMessage = "잘못 선택하셨습니다.\n다시 선택해주세요.";
                await context.PostAsync(strMessage);
                context.Wait(SendWelcomeMessageAsync);
            }     
        }

        public async Task DialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                strMessage = await result;

                //await context.PostAsync(WelcomeMessage); ;
                await this.MessageReceivedAsync(context, result);
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("Error occurred....");
            }
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }
    }
}

