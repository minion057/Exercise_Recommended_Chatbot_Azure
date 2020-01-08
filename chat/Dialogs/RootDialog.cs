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
            auto01_Button.Add(new CardAction() { Title = "1. FAQ (����)", Value = "1", Type = ActionTypes.ImBack });
            auto01_Button.Add(new CardAction() { Title = "2. �ڵ���õ", Value = "2", Type = ActionTypes.ImBack });
            auto01_Button.Add(new CardAction() { Title = "3, ��������", Value = "3", Type = ActionTypes.ImBack });

            //Create Hero Card-01
            HeroCard memu01_Card = new HeroCard()
            {
                Title = "�ȳ��ϼ���. ����� �˷��ִ� '�����'�Դϴ�.",
                Subtitle = "��� � �ڼ��� �˷��帱���?\n(��ư�� Ŭ�����ּ���)",
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
                strMessage = "[FAQ] ������ �Է����ּ���.\n ����� exit�� �Է����ּ���.";
                await context.PostAsync(strMessage);

                context.Call(new FAQDialog(), DialogResumeAfter);
            }
            else if(strSelected == "2")
            {
                strMessage = "[�ڵ���õ]�� �����ϼ̽��ϴ�.\nü���� �´� ��� ���� BMI ������ ����ϰڽ��ϴ�.";
                await context.PostAsync(strMessage);
                strMessage = "����� �����̸� �˷��ּ���.\n(���ڷθ� �Է����ּ���.)\n ����� exit�� �Է����ּ���.";
                await context.PostAsync(strMessage);

                context.Call(new UserDialog(), DialogResumeAfter);
            }
            else if (strSelected == "3")
            {
                strMessage = "[��������]�� �����ϼ̽��ϴ�.\n";
                await context.PostAsync(strMessage);

                strMessage = "� �κ��� ��ϰ�����Ű���?\n(��ư�� Ŭ�����ּ���)";
                await context.PostAsync(strMessage);


                var message = context.MakeMessage();


                message.Attachments.Add(CardHelper.GetHero2Card("��ü", this.strServerUrl + "dongsuk1.jpg", "��ü", "1"));
                message.Attachments.Add(CardHelper.GetHero2Card("��ü",
                                        this.strServerUrl + "dongsuk2.jpg", "��ü", "2"));
                message.Attachments.Add(CardHelper.GetHero2Card("����",
                                        this.strServerUrl + "dongsuk3.jpg", "����", "3"));
                message.Attachments.Add(CardHelper.GetHero2Card("�����Է�",
                                        this.strServerUrl + "dongsuk4.jpg", "�����Է�", "4"));


                message.AttachmentLayout = "carousel";
                await context.PostAsync(message);

                context.Call(new OrderDialog(), DialogResumeAfter);

            }
            else
            {
                strMessage = "�߸� �����ϼ̽��ϴ�.\n�ٽ� �������ּ���.";
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

