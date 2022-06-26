using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using Cysharp.Threading.Tasks;

using System.Linq;

public class LoginScript : MonoBehaviour
{
    [SerializeField] private bool debugMode;
    [Space(20)]
    //data
    [SerializeField] private GetPlayerCombinedInfoRequestParams InfoRequestParams;

    //privateData
    private bool isLogin = false;
    private const string PASSWORD_CHARS = "0123456789ABCDEFGHIJKLMNPQRSTUVWXYZ";
    private string sceneName;

    //UI
    [Header("UI")]
    [SerializeField] private Text startText;

    [SerializeField] private GameObject content_BlockObj;
    [SerializeField] private GameObject parent_BlockObj;
    [SerializeField] private GameObject ParticleSystem;

    public void Login()
    {
        if (!isLogin)
        {
            isLogin = true;

            PlayFabAuthService.Instance.InfoRequestParams = InfoRequestParams;
            PlayFabAuthService.OnLoginSuccess += PlayFabLogin_OnLoginSuccess;
            PlayFabAuthService.Instance.Authenticate(Authtypes.Silent);
        }
    }

    private void PlayFabLogin_OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Success!");

        //"Tap to Start"の文字を"Now Loading"に変更
        startText.text = "Now Loading";

        //タイトルデータ（マスタデータ）の読み込み
        DataList.mcDatas = PlayFabSimpleJson.DeserializeObject<List<MasterData_Character>>(result.InfoResultPayload.TitleData["CharacterData"]);
        DataList.msDatas = PlayFabSimpleJson.DeserializeObject<List<MasterData_Stage>>(result.InfoResultPayload.TitleData["StageData"]);

        #region キャラセット：デバッグモード
        if (debugMode)
        {
            DataList.isTutorialCompleted = "True";

            var user = DataList.userData;

            user.user_Name = "demoPlayer";
            user.user_Rank = 1;
            user.user_NickName0 = "上級";
            user.user_NickName1 = "プレイヤー";
            user.user_ExpPoint = 0;
            user.user_UntilRankUp = Mathf.FloorToInt(4425 / 105 * user.user_Rank);
            user.user_Money = 100000000;
            user.user_MagicStone = 10000;

            for (int i = 0; i < 11; i++)
            {
                Data_Character chara = new Data_Character();

                switch (i)
                {
                    case 0: chara.c_Id = 1345; break;
                    case 1: chara.c_Id = 1135; break;
                    case 2: chara.c_Id = 1221; break;
                    case 3: chara.c_Id = 1090; break;
                    case 4: chara.c_Id = 1487; break;
                    case 5: chara.c_Id = 1146; break;
                    case 6: chara.c_Id = 1433; break;
                    case 7: chara.c_Id = 1339; break;
                    case 8: chara.c_Id = 1076; break;
                    case 9: chara.c_Id = 1472; break;
                    case 10: chara.c_Id = 1504; break;
                }

                chara.c_Level = 1;

                var sb = new System.Text.StringBuilder(12);

                for (int j = 0; j < 16; j++)
                {
                    int pos = Random.Range(0, PASSWORD_CHARS.Length);
                    char c = PASSWORD_CHARS[pos];
                    sb.Append(c);
                }

                chara.c_UniqueId = sb.ToString();
                sb = null;

                chara.c_CostumeId = 0;
                chara.c_Uses = 0;

                var charaData = DataList.mcDatas.Find(x => x.mc_Id == chara.c_Id);

                chara.c_UntilLevelUp = Mathf.FloorToInt(((100 + (50 - charaData.mc_RiseValue)) * 0.01f) *  chara.c_Level      * (100 + (50 - charaData.mc_RiseValue)));
                chara.c_ExpPoint = 10;

                chara.c_HP = Mathf.FloorToInt(charaData.mc_HP * 2.4f / charaData.mc_RiseValue) * chara.c_Level + charaData.mc_HP;

                float A = 1.0f;
                float D = 1.0f;

                switch (charaData.mc_BattleType)
                {
                    case 0:
                        break;
                    case 1:
                        A = 1.1f;
                        D = 0.9f;
                        break;
                    case 2:
                        D = 1.1f;
                        A = 0.9f;
                        break;
                }
                chara.c_Attack = Mathf.FloorToInt((charaData.mc_Attack * 2.4f / charaData.mc_RiseValue) * A) * chara.c_Level + charaData.mc_Attack;
                chara.c_Defense = Mathf.FloorToInt((charaData.mc_Defense * 2.4f / charaData.mc_RiseValue) * D) * chara.c_Level + charaData.mc_Defense;

                chara.c_Rarity = charaData.mc_Rarity;
                chara.c_Attribute = charaData.mc_Attribute;
                chara.c_BattleType = charaData.mc_BattleType;
                chara.c_DicePower = charaData.mc_DicePower;

                if (!DataList.userData.user_PictureBook.Contains(chara.c_Id))
                {
                    DataList.userData.user_PictureBook.Add(chara.c_Id);
                }

                DataList.userData.user_Character.Add(chara);
            }

            for (var i = 0; i < 4; i++)
            {
                DataList.userData.user_Formations[i] = DataList.userData.user_Character[i].c_UniqueId;

                var a = DataList.userData.user_Character.Find(x=>x.c_UniqueId == DataList.userData.user_Formations[i]);
            }

            DataList.userData.user_FavoriteChara = DataList.userData.user_Character[0];

            DataList.userData.user_DataCostume.cos_Valkyrie[0] = true;

            DataList.userData.user_DataCostume.cos_Frist[0] = true;
        }
        #endregion

        //アカウントが既に作成されており、チュートリアルも完了しているかチェック
        if (result.NewlyCreated)
        {
            AccountCreation();
        }
        else
        {
            sceneName = "HomeScene";

            DataList.isTutorialCompleted = result.InfoResultPayload.UserData["isTutorialCompleted"].Value;
            if(DataList.isTutorialCompleted == "false")
            {
                LoadScene();
            }
            else
            {
                LoadScene();
            }
        }
    }

    private void AccountCreation()
    {
        //プレイヤーデータを登録する処理
        PlayFabClientAPI.UpdateUserData(
            new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>()
                {
                    {"isTutorialCompleted", "false"}
                }
            },
            //データの登録が完了した場合
            result =>
            {
                Debug.Log("アカウント作成+チュートリアル完了フラッグセット");

                DataList.isTutorialCompleted = "false";
                sceneName = "TutorialScene";
                LoadScene();
            },
            //データの登録中にエラーが発生した場合
            error => { Debug.Log(error.GenerateErrorReport()); });
    }

    private async void LoadScene()
    {
        await UniTask.Delay(1500);

        var asnycScene = SceneManager.LoadSceneAsync(sceneName);
        asnycScene.allowSceneActivation = false;

        foreach (Transform obj in parent_BlockObj.transform)
        {
            Destroy(obj.gameObject);
        }

        for(int i = 0; i < 15; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                var content = Instantiate(content_BlockObj, parent_BlockObj.transform);
                Vector3 pos = content.transform.localPosition;

                pos.x = -566 + (j * 141.1f);
                pos.y = 990.5f - (i * 141.1f);

                content.transform.localPosition = pos;

                await UniTask.Delay(35);
            }
        }

        ParticleSystem.SetActive(false);
        await UniTask.Delay(1500);
        asnycScene.allowSceneActivation = true;
    }
}
