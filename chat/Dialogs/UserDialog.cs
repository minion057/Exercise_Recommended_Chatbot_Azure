using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;       //Add for List<>
using GreatWall.Dialogs;

using System.Data;
using System.Data.SqlClient; //db를 위함
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

        int modify = 0; // 0:수정 사항 x
        //1: 나이수정 / 2: 성별 수정 / 3: 키 수정 / 4: 몸무게 수정
        //수정사항이라는 부분을 알려주는 변수

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        { //나이
            Activity activity = await result as Activity;
            string strage = activity.Text.Trim();
            if (strage.ToUpper() == "EXIT") await tofirstAsync(context);
            try
            {
                age = Convert.ToInt32(strage);
                if(age < 13)
                {
                    strMessage = "저희가 추천드리는 운동은 13세 이상부터 하길 권장합니다.\n처음으로 돌아갑니다.";
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
                strMessage = "잘못 입력하셨습니다.\n당신의 만나이를 알려주세요.\n(숫자로만 입력해주세요.)";
                await context.PostAsync(strMessage);
                context.Wait(MessageReceivedAsync);
            }           
        }

        public async Task genderselectMessageAsync(IDialogContext context)
        {
            strMessage = "당신의 성별을 알려주세요.";
            await context.PostAsync(strMessage);
            var message = context.MakeMessage();        //Create message
            var actions = new List<CardAction>();       //Create List

            actions.Add(new CardAction() { Title = "1. 여자", Value = "1", Type = ActionTypes.ImBack });
            actions.Add(new CardAction() { Title = "2. 남자", Value = "2", Type = ActionTypes.ImBack });

            message.Attachments.Add(
                new HeroCard { Title = "성별 선택", Buttons = actions }.ToAttachment()
            );

            await context.PostAsync(message);
            context.Wait(genderMessageAsync);
        }

        public async Task genderMessageAsync(IDialogContext context, IAwaitable<object> result)
        {
            Activity activity = await result as Activity;
            string strgender = activity.Text.Trim();
            if (strgender.ToUpper() == "EXIT") await tofirstAsync(context);

            if (strgender == "1" || strgender=="여자" || strgender.ToUpper() == "WOMAN")
            {
                gender = "여자";
            }
            else if(strgender == "2" || strgender == "남자" || strgender.ToUpper() == "MAN")
            {
                gender = "남자";
            }
            else
            {
                strMessage = "잘못 입력하셨습니다.\n당신의 성별을 알려주세요.";
                await context.PostAsync(strMessage);
                await genderselectMessageAsync(context);
                return;
            }

            if (modify == 2) await modifyselectMessageAsync(context);
            else
            {
                strMessage = "당신의 키를 알려주세요.\n(숫자로만 입력하세요.)";
                await context.PostAsync(strMessage);
                context.Wait(heightMessageAsync);
            }
        }

        public async Task heightMessageAsync(IDialogContext context, IAwaitable<object> result)
        { //키
            Activity activity = await result as Activity;
            string strheight = activity.Text.Trim();
            if (strheight.ToUpper() == "EXIT") await tofirstAsync(context);
            try
            {
                height = Convert.ToSingle(strheight);
                if (height < 110 || height > 250)
                {
                    strMessage = "키가 너무 작거나 큽니다(110cm ~ 250cm).\n진짜 당신의 키를 알려주세요.\n(숫자로만 입력해주세요.)";
                    await context.PostAsync(strMessage);
                    context.Wait(heightMessageAsync);
                    return;
                }
                if (modify == 3) await modifyselectMessageAsync(context);
                else
                {
                    strMessage = "당신의 몸무게을 알려주세요.\n(숫자로만 입력하세요.)";
                    await context.PostAsync(strMessage);
                    context.Wait(weightMessageAsync);
                }
            }
            catch(Exception )
            {
                strMessage = "잘못 입력하셨습니다.\n당신의 키를 알려주세요.\n(숫자로만 입력하세요.)";
                await context.PostAsync(strMessage);
                context.Wait(heightMessageAsync);
            }
        }

        public async Task weightMessageAsync(IDialogContext context, IAwaitable<object> result)
        { //몸무게
            Activity activity = await result as Activity;
            string strweight = activity.Text.Trim();
            if (strweight.ToUpper() == "EXIT") await tofirstAsync(context);
            try
            {
                weight = Convert.ToSingle(strweight);
                if (weight < 30 || weight > 300)
                {
                    strMessage = "몸무게가 너무 작거나 큽니다(30kg ~ 300kg).\n진짜 당신의 몸무게를 알려주세요.\n(숫자로만 입력해주세요.)";
                    await context.PostAsync(strMessage);
                    context.Wait(weightMessageAsync);
                    return;
                }

                await modifyselectMessageAsync(context);
            }
            catch (Exception )
            {
                strMessage = "잘못 입력하셨습니다.\n당신의 몸무게을 알려주세요.\n(숫자로만 입력하세요.)";
                await context.PostAsync(strMessage);
                context.Wait(weightMessageAsync);
            }
        }

        public async Task modifyselectMessageAsync(IDialogContext context)
        {
            strMessage = "당신의 정보\n나이 : " + age + "세\n성별 : " + gender + "\n키 : " + height + "cm\n몸무게 : " + weight + "kg";
            await context.PostAsync(strMessage);

            var message = context.MakeMessage();        //Create message
            var actions = new List<CardAction>();       //Create List

            actions.Add(new CardAction() { Title = "1. 나이 수정", Value = "1", Type = ActionTypes.ImBack });
            actions.Add(new CardAction() { Title = "2. 성별 수정", Value = "2", Type = ActionTypes.ImBack });
            actions.Add(new CardAction() { Title = "3. 키 수정", Value = "3", Type = ActionTypes.ImBack });
            actions.Add(new CardAction() { Title = "4. 몸무게 수정", Value = "4", Type = ActionTypes.ImBack });
            actions.Add(new CardAction() { Title = "5. 수정사항 없음", Value = "5", Type = ActionTypes.ImBack });

            message.Attachments.Add(
                new HeroCard { Title = "사용자 정보 수정 선택", Buttons = actions }.ToAttachment()
            );

            await context.PostAsync(message);
            context.Wait(bmiMessageAsync);
        }

        public async Task bmiMessageAsync(IDialogContext context, IAwaitable<object> result)
        { //몸무게
            Activity activity = await result as Activity;
            string strselect = activity.Text.Trim();
            if (strselect.ToUpper() == "EXIT") await tofirstAsync(context);

            if (strselect == "1" || strselect == "나이" || strselect == "나이 수정" || strselect == "1. 나이 수정")
            {
                modify = 1;
                strMessage = "수정할 나이를 알려주세요.\n(숫자로만 입력하세요.)";
                await context.PostAsync(strMessage);
                context.Wait(MessageReceivedAsync);
            }
            else if (strselect == "2" || strselect == "성별" || strselect == "성별 수정" || strselect == "2. 성별 수정")
            {
                modify = 2;
                strMessage = "수정할 성별을 알려주세요.";
                await context.PostAsync(strMessage);
                await genderselectMessageAsync(context);
            }
            else if (strselect == "3" || strselect == "키" || strselect == "키 수정" || strselect == "3. 키 수정")
            {
                modify = 3;
                strMessage = "수정할 키를 알려주세요.\n(숫자로만 입력하세요.)";
                await context.PostAsync(strMessage);
                context.Wait(heightMessageAsync);
            }
            else if (strselect == "4" || strselect == "몸무게" || strselect == "몸무게 수정" || strselect == "4. 몸무게 수정")
            {
                //몸무게 입력 후 modify로 가니까 여기서 수정사항이라는 부분을 알려주는 modify를 설정할 필요가 없음
                strMessage = "수정할 몸무게를 알려주세요.\n(숫자로만 입력하세요.)";
                await context.PostAsync(strMessage);
                context.Wait(weightMessageAsync);
            }
            else if (strselect == "5" || strselect == "없어" || strselect == "수정사항 없음" || strselect == "5. 수정사항 없음")
            {
                //사용자 정보 입력 완료
                height = Convert.ToSingle(height * 0.01);
                float bmi = weight / (height * height);
                strMessage = "BMI 지수 : " + bmi;
                

                if (bmi <= 18.5)
                {
                    strMessage += "\n저체중입니다.";
                    status = "저체중";
                }
                else if (bmi <= 23)
                {
                    strMessage += "\n정상입니다.";
                    status = "정상";
                }
                else if (bmi <= 25)
                {
                    strMessage += "\n과체중입니다.";
                    status = "과체중";
                }
                else if (bmi <= 30)
                {
                    strMessage += "\n비만입니다.";
                    status = "비만";
                }
                else
                {
                    strMessage += "\n고도비만입니다.";
                    status = "고도비만";
                }
                await context.PostAsync(strMessage);
                strMessage = "BMI 지수는 키와 체중을 이용하여 체지방량의 정도로 비만도를 판단하는 것입니다.";
                strMessage += "\n단, 근육과 성별을 구분해서 계산할 수 없으니 단순히 참고용으로만 생각해주세요.";
                await context.PostAsync(strMessage);

                //context.Call(new AutoDialog(), DialogResumeAfter);
                await autoMessageAsync(context);
            }
            else
            {
                strMessage = "잘못 입력하셨습니다.";
                await context.PostAsync(strMessage);
                await modifyselectMessageAsync(context);
            }
        }

        public async Task autoMessageAsync(IDialogContext context)
        { //자동추천
            
            String strSQL = "SELECT * FROM BMI ";
            switch (this.status)
            {
                case "고도비만":
                    strSQL += "WHERE bmi = N'고도비만'";
                    break;
                case "비만":
                    strSQL += "WHERE bmi = N'비만'";
                    break;
                case "정상":
                    strSQL += "WHERE bmi = N'정상'";
                    break;
                case "저체중":
                    strSQL += "WHERE bmi = N'저체중'";
                    break;
                default:
                    strMessage = "bmi가 이상합니다";
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
            img.Add(links[random].Substring(links[random].IndexOf('=') + 1, links[random].Length - (links[random].IndexOf('=') + 1))); //이미지를 위해 필요한 주소를 잘라냄
            img.Add(links[random+1].Substring(links[random+1].IndexOf('=') + 1, links[random+1].Length - (links[random+1].IndexOf('=') + 1))); //이미지를 위해 필요한 주소를 잘라냄
            img.Add(links[random+2].Substring(links[random+2].IndexOf('=') + 1, links[random+2].Length - (links[random+2].IndexOf('=') + 1))); //이미지를 위해 필요한 주소를 잘라냄
            String ex = "클릭시 영상으로 이동";
            var message = context.MakeMessage();
            message.Attachments.Add(CardHelper.GetVideoCard(status+"추천 1번", ex, "https://i.ytimg.com/vi/" + img[0] + "/hqdefault.jpg", status+"추천 1번", links[random]));
            message.Attachments.Add(CardHelper.GetVideoCard(status+"추천 2번", ex, "https://i.ytimg.com/vi/" + img[1] + "/hqdefault.jpg", status+"추천 2번", links[random+1]));
            message.Attachments.Add(CardHelper.GetVideoCard(status+"추천 3번", ex, "https://i.ytimg.com/vi/" + img[2] + "/hqdefault.jpg", status+"추천 3번", links[random+2]));
            message.Attachments.Add(CardHelper.GetHeroCard("Again", "Again", "http://download.seaicons.com/icons/icons8/windows-8/512/Computer-Hardware-Restart-icon.png", "다시 추천받기", "Again"));
            message.Attachments.Add(CardHelper.GetHeroCard("Exit", "Exit", "https://cdn.pixabay.com/photo/2016/01/20/18/35/x-1152114_960_720.png", "처음으로 돌아가기", "Exit"));
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
            strMessage = "종료를 입력하셨습니다.\n처음으로 돌아갑니다.";
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

