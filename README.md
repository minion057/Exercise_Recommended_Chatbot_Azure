# 운동 추천 챗봇

Azure기반으로 유튜브에서 운동영상을 자동으로 추천해주는 챗봇입니다.  
자동추천은 사용자의 정보를 입력받아 BMI를 고려해 추천하고, 부위 선택은 사용자가 선택한 혹은 입력한 부위 운동을 추천해줍니다.  
추천받은 운동이 마음에 들지 않는다면, 다시 추천받을 수 있습니다.  
이때, 사용자가 입력하는 자연어 처리는 LUIS를 이용하였고, 도움말은 Azure에서 제공하는 QnA Maker를 이용하여 처리하였습니다.  

## 첫 실행화면   
![00.시작화면](/md_img/00.시작화면.png)     
  
## FAQ (Azure의 QnA Maker 이용)     
![01.FAQ](/md_img/01.FAQ.png)   
  
## 자동추천 (사용자 BMI 계산 후 추천)      
**<사용자 정보 입력>**     
![02.자동추천-사용자정보%입력](/md_img/02.자동추천-사용자정보%20입력.PNG)       
   
**<BMI계산 후 추천>**   
![03.자동추천-결과](/md_img/03.자동추천-결과.PNG)    
  
## 부위선택 (사용자가 입력하는 자연어는 LUIS를 이용하여 처리)  
**<사용자가 부위 선택>**  
![04.부위선택](/md_img/04.부위선택.PNG)
   
**<사용자가 직접 부위 입력>**       
![05.부위선택-사용자입력](/md_img/05.부위선택-사용자입력.png)  
 
