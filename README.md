# 유니티 포트폴리오 (C#서버 연동)
## 1. 플레이 영상
- [플레이 영상](https://youtu.be/8DtsXO9Uk0E)
## 2. 개요
- C#서버와 Unity 엔진을 활용하여 제작한 멀티플레이 3D RPG 게임입니다.
- 개발 인원 : 1인
- 개발 기간 : 2024.05.03 ~ 2024.05.22 (약 3주)
- Repository에는 클라이언트 코드만 올라가 있습니다.
## 3. 개발 환경
- C#
- Unity 2022.3.27f1
- Google Protobuf
## 4. 구현 기능
- System
  - 채팅
  - 파티
- Monster
  - 기본 몬스터
    - 슬라임, 뿔버섯
  - 보스 몬스터(드래곤)
- Player
  - 이동 동기화
  - 일반공격(콤보공격)
  - 스킬(지면강타, 검기방출)
  - 대쉬
- Item
  - 장비 아이템(투구, 갑옷, 무기, 장신구)
  - 소비 아이템(Hp회복, Mp회복)
- UI
  - Hud
    - 채팅
    - 현재 상태
    - 퀵슬롯
    - 파티Hp(파티가 있다면)
  - 파티
    - 파티찾기, 내 파티, 파티 만들기
  - 인벤토리
    - 장비, 소비, 스킬
    - 스탯
  - 상점
## 5. 인게임 이미지
![1](https://github.com/kjhcocomi/Unity_RPG_Scripts/blob/main/Images/1.png)
![2](https://github.com/kjhcocomi/Unity_RPG_Scripts/blob/main/Images/2.png)
![3](https://github.com/kjhcocomi/Unity_RPG_Scripts/blob/main/Images/3.png)
![4](https://github.com/kjhcocomi/Unity_RPG_Scripts/blob/main/Images/4.png)
![5](https://github.com/kjhcocomi/Unity_RPG_Scripts/blob/main/Images/5.png)
![6](https://github.com/kjhcocomi/Unity_RPG_Scripts/blob/main/Images/6.png)
![7](https://github.com/kjhcocomi/Unity_RPG_Scripts/blob/main/Images/7.png)
![8](https://github.com/kjhcocomi/Unity_RPG_Scripts/blob/main/Images/8.png)
## 6. 문서
- [이동 동기화, 콤보공격](https://docs.google.com/presentation/d/1vx5eWriDmBu2DHMmPzD-YKcuhqhExtZAgpVnUIIViMs/edit?usp=sharing)
- [Blog](https://kjhcocomi.tistory.com/category/%EC%9C%A0%EB%8B%88%ED%8B%B0/C%23%20%EC%84%9C%EB%B2%84%20%EC%97%B0%EB%8F%99%20%EC%9C%A0%EB%8B%88%ED%8B%B0%20%ED%8F%AC%ED%8A%B8%ED%8F%B4%EB%A6%AC%EC%98%A4)
