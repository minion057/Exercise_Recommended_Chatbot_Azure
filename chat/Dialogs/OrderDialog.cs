using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;       //Add for List<>
using GreatWall.Dialogs;
using System.Data;
using System.Data.SqlClient;
using GreatWall.Helpers;


namespace GreatWall
{
    [Serializable]
    public class OrderDialog : IDialog<string>
    {
        protected int count = 1;
        string strMessage, status;
        private string strServerUrl = "http://localhost:3984/Images/";


        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            Activity activity = await result as Activity;
            string strpart = activity.Text.Trim();
            if (strpart.ToUpper() == "EXIT")
            {
                await tofirstAsync(context);
                return;
            }
            switch (strpart)
            {
                case "1":
                    strMessage = "상체 운동 추천을 시작합니다.";
                    status = "상체";
                    await context.PostAsync(strMessage);
                    break;
                case "2":
                    strMessage = "하체 운동 추천을 시작합니다.";
                    status = "하체";
                    await context.PostAsync(strMessage);
                    break;
                case "3":
                    strMessage = "복부 운동 추천을 시작합니다.";
                    status = "복부";
                    await context.PostAsync(strMessage);
                    break;
                case "4":
                    strMessage = "상세 부위 추천을 시작합니다.";
                    strMessage = "원하시는 부위를  입력해주세요.";
                    await context.PostAsync(strMessage);
                    context.Call(new SelfDialog(), DialogResumeAfter);
                    return;
                default:
                    strMessage = "ERROR";
                    await context.PostAsync(strMessage);
                    context.Done("");
                    return;
            }
            await autoMessageAsync(context);
        }

        public async Task autoMessageAsync(IDialogContext context)
        { //자동추천
            String strSQL = "SELECT * FROM PartofBody ";
            switch (this.status)
            {
                case "상체":
                    strSQL += "WHERE tmb = N'상체'";
                    break;
                case "하체":
                    strSQL += "WHERE tmb = N'하체'";
                    break;
                case "복부":
                    strSQL += "WHERE tmb = N'복부'";
                    break;
                default:
                    strMessage = "부위 선택이 이상합니다";
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
            int random = r.Next(1, DB_DS.Tables[0].Rows.Count - 3);
            List<String> img = new List<string>();
            img.Add(links[random].Substring(links[random].IndexOf('e') + 1, links[random].Length - (links[random].IndexOf('e') + 1))); //ee이미지를 위해 필요한 주소를 잘라냄
            img.Add(links[random + 1].Substring(links[random + 1].IndexOf('e') + 1, links[random + 1].Length - (links[random + 1].IndexOf('e') + 1))); //ee이미지를 위해 필요한 주소를 잘라냄
            img.Add(links[random + 2].Substring(links[random + 2].IndexOf('e') + 1, links[random + 2].Length - (links[random + 2].IndexOf('e') + 1))); //이미지를 위해 필요한 주소를 잘라냄
            String ex = "클릭시 영상으로 이동";
            var message = context.MakeMessage();
            message.Attachments.Add(CardHelper.GetVideoCard("추천 1번", ex, "https://i.ytimg.com/vi" + img[0] + "/hqdefault.jpg", "추천 1번", links[random]));
            message.Attachments.Add(CardHelper.GetVideoCard("추천 2번", ex, "https://i.ytimg.com/vi" + img[1] + "/hqdefault.jpg", "추천 2번", links[random + 1]));
            message.Attachments.Add(CardHelper.GetVideoCard("추천 3번", ex, "https://i.ytimg.com/vi" + img[2] + "/hqdefault.jpg", "추천 3번", links[random + 2]));
            message.Attachments.Add(CardHelper.GetHeroCard("Again", "Again", "http://download.seaicons.com/icons/icons8/windows-8/512/Computer-Hardware-Restart-icon.png", "다시 추천받기", "Again"));
            message.Attachments.Add(CardHelper.GetHeroCard("Again_part", "Again_part", "https://png.pngtree.com/element_origin_min_pic/17/07/12/b284bfdc72087ed360213418b69d1616.jpg", "다시 부위선택", "Again_part"));
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
            else if(strselect.ToUpper() == "AGAIN_PART") await partselectMessageAsync(context);
            else await autoMessageAsync(context);

        }

        public async Task partselectMessageAsync(IDialogContext context)
        {
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
            context.Wait(MessageReceivedAsync);
        }

        private Task MessageReceived(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
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
