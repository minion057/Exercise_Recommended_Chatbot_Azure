using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;       //Add for List<>
using GreatWall.Dialogs;

using System.Data;
using System.Data.SqlClient; //db�� ����
using GreatWall.Model;
using GreatWall.Helpers;

namespace GreatWall
{
    [Serializable]
    public class UserDialog : IDialog<string>
    {
        protected int count = 1;
        string strMessage;
        int age;
        float height, weight;
        string gender;
        string status = "";

        int modify = 0; // 0:���� ���� x
        //1: ���̼��� / 2: ���� ���� / 3: Ű ���� / 4: ������ ����
        //���������̶�� �κ��� �˷��ִ� ����

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        { //����
            Activity activity = await result as Activity;
            string strage = activity.Text.Trim();
            if (strage.ToUpper() == "EXIT") await tofirstAsync(context);
            try
            {
                age = Convert.ToInt32(strage);
                if(age < 13)
                {
                    strMessage = "���� ��õ�帮�� ��� 13�� �̻���� �ϱ� �����մϴ�.\nó������ ���ư��ϴ�.";
                    await context.PostAsync(strMessage);

                    context.Done("");
                    return;
                }
                if (modify == 1) await modifyselectMessageAsync(context);
                else
                {
                    await genderselectMessageAsync(context);
                }
            }
            catch (Exception)
            {
                strMessage = "�߸� �Է��ϼ̽��ϴ�.\n����� �����̸� �˷��ּ���.\n(���ڷθ� �Է����ּ���.)";
                await context.PostAsync(strMessage);
                context.Wait(MessageReceivedAsync);
            }           
        }

        public async Task genderselectMessageAsync(IDialogContext context)
        {
            strMessage = "����� ������ �˷��ּ���.";
            await context.PostAsync(strMessage);
            var message = context.MakeMessage();        //Create message
            var actions = new List<CardAction>();       //Create List

            actions.Add(new CardAction() { Title = "1. ����", Value = "1", Type = ActionTypes.ImBack });
            actions.Add(new CardAction() { Title = "2. ����", Value = "2", Type = ActionTypes.ImBack });

            message.Attachments.Add(
                new HeroCard { Title = "���� ����", Buttons = actions }.ToAttachment()
            );

            await context.PostAsync(message);
            context.Wait(genderMessageAsync);
        }

        public async Task genderMessageAsync(IDialogContext context, IAwaitable<object> result)
        {
            Activity activity = await result as Activity;
            string strgender = activity.Text.Trim();
            if (strgender.ToUpper() == "EXIT") await tofirstAsync(context);

            if (strgender == "1" || strgender=="����" || strgender.ToUpper() == "WOMAN")
            {
                gender = "����";
            }
            else if(strgender == "2" || strgender == "����" || strgender.ToUpper() == "MAN")
            {
                gender = "����";
            }
            else
            {
                strMessage = "�߸� �Է��ϼ̽��ϴ�.\n����� ������ �˷��ּ���.";
                await context.PostAsync(strMessage);
                await genderselectMessageAsync(context);
                return;
            }

            if (modify == 2) await modifyselectMessageAsync(context);
            else
            {
                strMessage = "����� Ű�� �˷��ּ���.\n(���ڷθ� �Է��ϼ���.)";
                await context.PostAsync(strMessage);
                context.Wait(heightMessageAsync);
            }
        }

        public async Task heightMessageAsync(IDialogContext context, IAwaitable<object> result)
        { //Ű
            Activity activity = await result as Activity;
            string strheight = activity.Text.Trim();
            if (strheight.ToUpper() == "EXIT") await tofirstAsync(context);
            try
            {
                height = Convert.ToSingle(strheight);
                if (height < 110 || height > 250)
                {
                    strMessage = "Ű�� �ʹ� �۰ų� Ů�ϴ�(110cm ~ 250cm).\n��¥ ����� Ű�� �˷��ּ���.\n(���ڷθ� �Է����ּ���.)";
                    await context.PostAsync(strMessage);
                    context.Wait(heightMessageAsync);
                    return;
                }
                if (modify == 3) await modifyselectMessageAsync(context);
                else
                {
                    strMessage = "����� �������� �˷��ּ���.\n(���ڷθ� �Է��ϼ���.)";
                    await context.PostAsync(strMessage);
                    context.Wait(weightMessageAsync);
                }
            }
            catch(Exception )
            {
                strMessage = "�߸� �Է��ϼ̽��ϴ�.\n����� Ű�� �˷��ּ���.\n(���ڷθ� �Է��ϼ���.)";
                await context.PostAsync(strMessage);
                context.Wait(heightMessageAsync);
            }
        }

        public async Task weightMessageAsync(IDialogContext context, IAwaitable<object> result)
        { //������
            Activity activity = await result as Activity;
            string strweight = activity.Text.Trim();
            if (strweight.ToUpper() == "EXIT") await tofirstAsync(context);
            try
            {
                weight = Convert.ToSingle(strweight);
                if (weight < 30 || weight > 300)
                {
                    strMessage = "�����԰� �ʹ� �۰ų� Ů�ϴ�(30kg ~ 300kg).\n��¥ ����� �����Ը� �˷��ּ���.\n(���ڷθ� �Է����ּ���.)";
                    await context.PostAsync(strMessage);
                    context.Wait(weightMessageAsync);
                    return;
                }

                await modifyselectMessageAsync(context);
            }
            catch (Exception )
            {
                strMessage = "�߸� �Է��ϼ̽��ϴ�.\n����� �������� �˷��ּ���.\n(���ڷθ� �Է��ϼ���.)";
                await context.PostAsync(strMessage);
                context.Wait(weightMessageAsync);
            }
        }

        public async Task modifyselectMessageAsync(IDialogContext context)
        {
            strMessage = "����� ����\n���� : " + age + "��\n���� : " + gender + "\nŰ : " + height + "cm\n������ : " + weight + "kg";
            await context.PostAsync(strMessage);

            var message = context.MakeMessage();        //Create message
            var actions = new List<CardAction>();       //Create List

            actions.Add(new CardAction() { Title = "1. ���� ����", Value = "1", Type = ActionTypes.ImBack });
            actions.Add(new CardAction() { Title = "2. ���� ����", Value = "2", Type = ActionTypes.ImBack });
            actions.Add(new CardAction() { Title = "3. Ű ����", Value = "3", Type = ActionTypes.ImBack });
            actions.Add(new CardAction() { Title = "4. ������ ����", Value = "4", Type = ActionTypes.ImBack });
            actions.Add(new CardAction() { Title = "5. �������� ����", Value = "5", Type = ActionTypes.ImBack });

            message.Attachments.Add(
                new HeroCard { Title = "����� ���� ���� ����", Buttons = actions }.ToAttachment()
            );

            await context.PostAsync(message);
            context.Wait(bmiMessageAsync);
        }

        public async Task bmiMessageAsync(IDialogContext context, IAwaitable<object> result)
        { //������
            Activity activity = await result as Activity;
            string strselect = activity.Text.Trim();
            if (strselect.ToUpper() == "EXIT") await tofirstAsync(context);

            if (strselect == "1" || strselect == "����" || strselect == "���� ����" || strselect == "1. ���� ����")
            {
                modify = 1;
                strMessage = "������ ���̸� �˷��ּ���.\n(���ڷθ� �Է��ϼ���.)";
                await context.PostAsync(strMessage);
                context.Wait(MessageReceivedAsync);
            }
            else if (strselect == "2" || strselect == "����" || strselect == "���� ����" || strselect == "2. ���� ����")
            {
                modify = 2;
                strMessage = "������ ������ �˷��ּ���.";
                await context.PostAsync(strMessage);
                await genderselectMessageAsync(context);
            }
            else if (strselect == "3" || strselect == "Ű" || strselect == "Ű ����" || strselect == "3. Ű ����")
            {
                modify = 3;
                strMessage = "������ Ű�� �˷��ּ���.\n(���ڷθ� �Է��ϼ���.)";
                await context.PostAsync(strMessage);
                context.Wait(heightMessageAsync);
            }
            else if (strselect == "4" || strselect == "������" || strselect == "������ ����" || strselect == "4. ������ ����")
            {
                //������ �Է� �� modify�� ���ϱ� ���⼭ ���������̶�� �κ��� �˷��ִ� modify�� ������ �ʿ䰡 ����
                strMessage = "������ �����Ը� �˷��ּ���.\n(���ڷθ� �Է��ϼ���.)";
                await context.PostAsync(strMessage);
                context.Wait(weightMessageAsync);
            }
            else if (strselect == "5" || strselect == "����" || strselect == "�������� ����" || strselect == "5. �������� ����")
            {
                //����� ���� �Է� �Ϸ�
                height = Convert.ToSingle(height * 0.01);
                float bmi = weight / (height * height);
                strMessage = "BMI ���� : " + bmi;
                

                if (bmi <= 18.5)
                {
                    strMessage += "\n��ü���Դϴ�.";
                    status = "��ü��";
                }
                else if (bmi <= 23)
                {
                    strMessage += "\n�����Դϴ�.";
                    status = "����";
                }
                else if (bmi <= 25)
                {
                    strMessage += "\n��ü���Դϴ�.";
                    status = "��ü��";
                }
                else if (bmi <= 30)
                {
                    strMessage += "\n���Դϴ�.";
                    status = "��";
                }
                else
                {
                    strMessage += "\n�����Դϴ�.";
                    status = "����";
                }
                await context.PostAsync(strMessage);
                strMessage = "BMI ������ Ű�� ü���� �̿��Ͽ� ü���淮�� ������ �񸸵��� �Ǵ��ϴ� ���Դϴ�.";
                strMessage += "\n��, ������ ������ �����ؼ� ����� �� ������ �ܼ��� ��������θ� �������ּ���.";
                await context.PostAsync(strMessage);

                //context.Call(new AutoDialog(), DialogResumeAfter);
                await autoMessageAsync(context);
            }
            else
            {
                strMessage = "�߸� �Է��ϼ̽��ϴ�.";
                await context.PostAsync(strMessage);
                await modifyselectMessageAsync(context);
            }
        }

        public async Task autoMessageAsync(IDialogContext context)
        { //�ڵ���õ
            
            String strSQL = "SELECT * FROM BMI ";
            switch (this.status)
            {
                case "����":
                    strSQL += "WHERE bmi = N'����'";
                    break;
                case "��":
                    strSQL += "WHERE bmi = N'��'";
                    break;
                case "����":
                    strSQL += "WHERE bmi = N'����'";
                    break;
                case "��ü��":
                    strSQL += "WHERE bmi = N'��ü��'";
                    break;
                default:
                    strMessage = "bmi�� �̻��մϴ�";
                    await context.PostAsync(strMessage);
                    context.Done("");
                    return;
            }
            Random r = new Random();
            DataSet DB_DS = SQLHelper.RunSQL(strSQL);

            List<String> links = new List<string>();
            foreach (DataRow row in DB_DS.Tables[0].Rows)
            {
                links.Add(row["link"].ToString());
            }
            int random = r.Next(1, DB_DS.Tables[0].Rows.Count-3);
            List<String> img = new List<string>();
            img.Add(links[random].Substring(links[random].IndexOf('=') + 1, links[random].Length - (links[random].IndexOf('=') + 1))); //�̹����� ���� �ʿ��� �ּҸ� �߶�
            img.Add(links[random+1].Substring(links[random+1].IndexOf('=') + 1, links[random+1].Length - (links[random+1].IndexOf('=') + 1))); //�̹����� ���� �ʿ��� �ּҸ� �߶�
            img.Add(links[random+2].Substring(links[random+2].IndexOf('=') + 1, links[random+2].Length - (links[random+2].IndexOf('=') + 1))); //�̹����� ���� �ʿ��� �ּҸ� �߶�
            String ex = "Ŭ���� �������� �̵�";
            var message = context.MakeMessage();
            message.Attachments.Add(CardHelper.GetVideoCard(status+"��õ 1��", ex, "https://i.ytimg.com/vi/" + img[0] + "/hqdefault.jpg", status+"��õ 1��", links[random]));
            message.Attachments.Add(CardHelper.GetVideoCard(status+"��õ 2��", ex, "https://i.ytimg.com/vi/" + img[1] + "/hqdefault.jpg", status+"��õ 2��", links[random+1]));
            message.Attachments.Add(CardHelper.GetVideoCard(status+"��õ 3��", ex, "https://i.ytimg.com/vi/" + img[2] + "/hqdefault.jpg", status+"��õ 3��", links[random+2]));
            message.Attachments.Add(CardHelper.GetHeroCard("Again", "Again", "http://download.seaicons.com/icons/icons8/windows-8/512/Computer-Hardware-Restart-icon.png", "�ٽ� ��õ�ޱ�", "Again"));
            message.Attachments.Add(CardHelper.GetHeroCard("Exit", "Exit", "https://cdn.pixabay.com/photo/2016/01/20/18/35/x-1152114_960_720.png", "ó������ ���ư���", "Exit"));
            message.AttachmentLayout = "carousel";              //Setting Menu Layout Format
            await context.PostAsync(message);                   //Output message 

            context.Wait(lastAsync);
        }

        public async Task lastAsync(IDialogContext context, IAwaitable<object> result)
        {
            Activity activity = await result as Activity;
            string strselect = activity.Text.Trim();
            if (strselect.ToUpper() == "EXIT") await tofirstAsync(context);
            else await autoMessageAsync(context);


        }

        public async Task tofirstAsync(IDialogContext context)
        {
            strMessage = "���Ḧ �Է��ϼ̽��ϴ�.\nó������ ���ư��ϴ�.";
            await context.PostAsync(strMessage);
            context.Done("");
            return;
        }

        public async Task DialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                strMessage = await result;
                
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

