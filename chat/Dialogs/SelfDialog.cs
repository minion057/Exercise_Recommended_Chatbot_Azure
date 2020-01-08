using GreatWall.Helpers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models; //Microsoft.Cognitive.Luis
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;

namespace GreatWall.Dialogs
{
    //[LuisModel(App IO, Subcription Key)]
    [LuisModel("ddf98f94-a1d1-460e-9855-a78e02323f68", "6ff1d2e436b4499da45c6b4798a43b1c")]

    [Serializable]
    public class SelfDialog : LuisDialog<string>
    {

        string strMessage;
        String status;

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"없는 부위를 선택했습니다.";
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }
        [LuisIntent("Exit")]
        public async Task Exit(IDialogContext context, LuisResult result)
        {
            strMessage = "종료를 입력하셨습니다.\n처음으로 돌아가려면 아무 단어나 입력해주세요";
            await context.PostAsync(strMessage);
            context.Call(new RootDialog(), DialogResumeAfter);

        }

        private Task DialogResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

        [LuisIntent("Parts")]
        public async Task Parts(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            EntityRecommendation whichpartEntityRecommendation;
            string whichpart = "";

            if (result.TryFindEntity("whichpart", out whichpartEntityRecommendation))
            {
                whichpart = whichpartEntityRecommendation.Entity.Replace(" ", "");

            }
            else
            {
                await context.PostAsync("없는 부위를 선택했습니다.");
                context.Wait(this.MessageReceived);
                return;
            }

            await context.PostAsync($"{whichpart}를 선택하셨습니다.");
            //context.Wait(this.MessageReceived);
            await context.PostAsync($"{whichpart} 운동 추천을 시작합니다.");

            this.status = whichpart;
            await autoMessageAsync(context);
        }

        public async Task autoMessageAsync(IDialogContext context)
        {
            String strSQL = "SELECT * FROM PartofBody ";
            switch (this.status)
            {
                case "어깨":
                    strSQL += "WHERE detail = N'어깨'";
                    //strMessage = "어깨 운동 추천을 시작합니다.";
                    await context.PostAsync(strMessage);
                    break;
                case "승모근":
                    strSQL += "WHERE detail = N'승모근'";
                    //strMessage = "승모근 운동 추천을 시작합니다.";
                    await context.PostAsync(strMessage);
                    break;
                case "목":
                    strSQL += "WHERE detail = N'목'";
                    //strMessage = "목 운동 추천을 시작합니다.";
                    await context.PostAsync(strMessage);
                    break;
                case "팔":
                    strSQL += "WHERE detail = N'팔'";
                    //strMessage = "팔 운동 추천을 시작합니다.";
                    await context.PostAsync(strMessage);
                    break;
                case "가슴":
                    strSQL += "WHERE detail = N'가슴'";
                    //strMessage = "가슴 운동 추천을 시작합니다.";
                    await context.PostAsync(strMessage);
                    break;
                case "종아리":
                    strSQL += "WHERE detail = N'종아리'";
                    //strMessage = "종아리 운동 추천을 시작합니다.";
                    await context.PostAsync(strMessage);
                    break;
                case "허벅지":
                    strSQL += "WHERE detail = N'허벅지'";
                    //strMessage = "허벅지 운동 추천을 시작합니다.";
                    await context.PostAsync(strMessage);
                    break;
                case "복근":
                    strSQL += "WHERE detail = N'복근'";
                    //strMessage = "복근 운동 추천을 시작합니다.";
                    await context.PostAsync(strMessage);
                    break;
                case "힙업":
                    strSQL += "WHERE detail = N'힙업'";
                    //strMessage = "힙업 운동 추천을 시작합니다.";
                    await context.PostAsync(strMessage);
                    break;
                case "뱃살":
                    strSQL += "WHERE detail = N'뱃살'";
                    //strMessage = "뱃살 운동 추천을 시작합니다.";
                    await context.PostAsync(strMessage);
                    break;
                case "발목":
                    strSQL += "WHERE detail = N'발목'";
                    //strMessage = "발목 운동 추천을 시작합니다.";
                    await context.PostAsync(strMessage);
                    break;
                default:
                    strMessage = "ERROR";
                    await context.PostAsync(strMessage);
                    context.Done("");
                    return;
            }


            DataSet DB_DS = SQLHelper.RunSQL(strSQL);
            List<String> links = new List<string>();
            foreach (DataRow row in DB_DS.Tables[0].Rows)
            {
                links.Add(row["link"].ToString());
            }
            Random r = new Random();
            int random = r.Next(1, DB_DS.Tables[0].Rows.Count - 3);
            List<String> img = new List<string>();
            img.Add(links[random].Substring(links[random].IndexOf('e') + 1, links[random].Length - (links[random].IndexOf('e') + 1))); //ee이미지를 위해 필요한 주소를 잘라냄
            img.Add(links[random + 1].Substring(links[random + 1].IndexOf('e') + 1, links[random + 1].Length - (links[random + 1].IndexOf('e') + 1))); //ee이미지를 위해 필요한 주소를 잘라냄
            img.Add(links[random + 2].Substring(links[random + 2].IndexOf('e') + 1, links[random + 2].Length - (links[random + 2].IndexOf('e') + 1))); //이미지를 위해 필요한 주소를 잘라냄
            String ex = "클릭시 영상으로 이동";
            var messagecard = context.MakeMessage();
            messagecard.Attachments.Add(CardHelper.GetVideoCard("추천 1번", ex, "https://i.ytimg.com/vi" + img[0] + "/hqdefault.jpg", "추천 1번",  links[random]));
            messagecard.Attachments.Add(CardHelper.GetVideoCard("추천 2번", ex, "https://i.ytimg.com/vi" + img[1] + "/hqdefault.jpg", "추천 2번",  links[random + 1]));
            messagecard.Attachments.Add(CardHelper.GetVideoCard("추천 3번", ex, "https://i.ytimg.com/vi" + img[2] + "/hqdefault.jpg", "추천 3번", links[random + 2]));
            messagecard.Attachments.Add(CardHelper.GetHeroCard("Again", "Again", "http://download.seaicons.com/icons/icons8/windows-8/512/Computer-Hardware-Restart-icon.png", "다시 추천받기", "Again"));
            messagecard.Attachments.Add(CardHelper.GetHeroCard("Again_part", "Again_part", "https://png.pngtree.com/element_origin_min_pic/17/07/12/b284bfdc72087ed360213418b69d1616.jpg", "다시 부위선택", "Again_part"));
            messagecard.AttachmentLayout = "carousel";
            await context.PostAsync(messagecard);
            context.Wait(lastAsync);
        }

        public async Task lastAsync(IDialogContext context, IAwaitable<object> result)
        {
            Activity activity = await result as Activity;
            string strselect = activity.Text.Trim();
            if (strselect.ToUpper() == "AGAIN_PART") context.Wait(this.MessageReceived);
            else await autoMessageAsync(context);

        }
    }
}