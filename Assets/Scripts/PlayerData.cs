using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

public class PlayerData : MonoBehaviour {
    public Dictionary<string, General> generals, playerShopGenerals; //playershotgenerals is a sorted list where player's generals appear first
    public BinaryData2 playerData;
    public List<Sprite> photos;
    public Dictionary<string, Sprite> generalPhotos;
    //general perks' sprites
    public List<Sprite> generalPerkSprites; //goes by enum of generalperk


    public static PlayerData instance;


    void Start() {
        DontDestroyOnLoad(gameObject);
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        playerData = null;
        playerData = BinaryPlayerSave.LoadData();
        
        FixGeneralNames();
        FixMapNames();
//#if !UNITY_ANDROID && !UNITY_IOS
        if (playerData == null) {
            playerData = new BinaryData2();
            Debug.LogWarning("data error");
        }
//#endif
        AddGenerals();
    }
    void AddGenerals() {
        generalPhotos = new Dictionary<string, Sprite>();
        generalPhotos.Add("default", photos[3]);
        generalPhotos.Add("Rommel", photos[0]);
        generalPhotos.Add("Manstein", photos[1]);
        generalPhotos.Add("Guderian", photos[2]);
        generalPhotos.Add("Rundstedt", photos[4]);
        generalPhotos.Add("Doenitz", photos[5]);
        generalPhotos.Add("Zhukov", photos[6]);
        generalPhotos.Add("Konev", photos[7]);
        generalPhotos.Add("Rokossovsky", photos[8]);
        generalPhotos.Add("Patton", photos[25]);
        generalPhotos.Add("Montgomery", photos[26]);
        generalPhotos.Add("Bradley", photos[24]);

        generalPhotos.Add("Lin Biao", photos[48]);
        generalPhotos.Add("Du Yuming", photos[12]);
        generalPhotos.Add("Mao Zedong", photos[9]);
        generalPhotos.Add("Chiang Kaishek", photos[10]);
        generalPhotos.Add("Wang Yaowu", photos[11]);
        generalPhotos.Add("Bai Chongxi", photos[13]);
        generalPhotos.Add("Fan Hanjie", photos[14]);
        generalPhotos.Add("Chen Shaokuan", photos[15]);
        generalPhotos.Add("Zhou Zhirou", photos[16]);
        generalPhotos.Add("Xue Yue", photos[17]);
        generalPhotos.Add("Sun Liren", photos[18]);
        generalPhotos.Add("Yan Xishan", photos[19]);
        generalPhotos.Add("Sheng Shicai", photos[20]);
        generalPhotos.Add("Ma Bufang", photos[21]);
        generalPhotos.Add("Long Yun", photos[22]);
        generalPhotos.Add("Peng Dehuai", photos[23]);


        generalPhotos.Add("Zhang Zizhong", photos[27]);
        generalPhotos.Add("Fu Zuoyi", photos[28]);
        generalPhotos.Add("Liu Xiang", photos[29]);
        generalPhotos.Add("Liu Wenhui", photos[30]);
        generalPhotos.Add("Luo Zhuoying", photos[31]);
        generalPhotos.Add("Gu Zhutong", photos[32]);
        generalPhotos.Add("Zhang Xueliang", photos[33]);
        generalPhotos.Add("Sun Lianzhong", photos[34]);
        generalPhotos.Add("Zhu Shaoliang", photos[35]);
        generalPhotos.Add("Yu Hanmou", photos[36]);
        generalPhotos.Add("Liu Zhi", photos[37]);
        generalPhotos.Add("Li Pinxian", photos[38]);
        generalPhotos.Add("Jiang Dingwen", photos[39]);
        generalPhotos.Add("Feng Yuxiang", photos[40]);
        generalPhotos.Add("He Yingqin", photos[41]);
        generalPhotos.Add("Ma Hongkui", photos[42]);
        generalPhotos.Add("Ma Hongbin", photos[43]);
        generalPhotos.Add("Emperor Puyi", photos[44]);
        generalPhotos.Add("Zhang Jinghui", photos[45]);
        generalPhotos.Add("Wei Lihuang", photos[46]);
        generalPhotos.Add("Li Zongren", photos[47]);
        generalPhotos.Add("Lin Yisheng", photos[49]);
        generalPhotos.Add("Paulus", photos[50]);
        generalPhotos.Add("Kleist", photos[51]);
        generalPhotos.Add("Kruger", photos[52]);
        generalPhotos.Add("Keitel", photos[53]);
        generalPhotos.Add("Kesselring", photos[54]);
        generalPhotos.Add("Bock", photos[55]);
        generalPhotos.Add("Seeckt", photos[56]);
        generalPhotos.Add("Kuckler", photos[57]);
        generalPhotos.Add("Brauchitsch", photos[58]);
        generalPhotos.Add("Goering", photos[59]);
        generalPhotos.Add("Student", photos[60]);
        generalPhotos.Add("Manteuffel", photos[61]);
        generalPhotos.Add("List", photos[62]);
        generalPhotos.Add("Glommer", photos[63]);
        generalPhotos.Add("Heinrici", photos[64]);
        generalPhotos.Add("Witzleben", photos[65]);
        generalPhotos.Add("Schoerner", photos[66]);
        generalPhotos.Add("Model", photos[67]);
        generalPhotos.Add("Fegelein", photos[68]);
        generalPhotos.Add("Meyer", photos[69]);
        generalPhotos.Add("Leeb", photos[70]);
        generalPhotos.Add("Raeder", photos[71]);
        generalPhotos.Add("Weichs", photos[72]);
        generalPhotos.Add("Eisonhower", photos[73]);
        generalPhotos.Add("Clark", photos[74]);
        generalPhotos.Add("Smith", photos[75]);
        generalPhotos.Add("Stilwell", photos[76]);
        generalPhotos.Add("Halsey", photos[77]);
        generalPhotos.Add("Ike", photos[78]);
        generalPhotos.Add("Nimitz", photos[79]);
        generalPhotos.Add("Fletcher", photos[80]);
        generalPhotos.Add("Devers", photos[81]);
        generalPhotos.Add("Spruance", photos[82]);
        generalPhotos.Add("Lee", photos[83]);
        generalPhotos.Add("Doolitle", photos[84]);
        generalPhotos.Add("Mitchell", photos[85]);
        generalPhotos.Add("Kinkaid", photos[86]);
        generalPhotos.Add("King", photos[87]);
        generalPhotos.Add("Arnold", photos[88]);
        generalPhotos.Add("Chennault", photos[89]);
        generalPhotos.Add("McArthur", photos[90]);
        generalPhotos.Add("McCain", photos[91]);
        generalPhotos.Add("McAuliffe", photos[92]);

        generalPhotos.Add("Toyoda", photos[93]);
        generalPhotos.Add("Inoue", photos[94]);
        generalPhotos.Add("Nagumo", photos[95]);
        generalPhotos.Add("Koga", photos[96]);
        generalPhotos.Add("Terauchi", photos[97]);
        generalPhotos.Add("Ozawa", photos[98]);
        generalPhotos.Add("Koiso", photos[99]);
        generalPhotos.Add("Yamashita", photos[100]);
        generalPhotos.Add("Yamaguchi", photos[101]);
        generalPhotos.Add("Yamamoto", photos[102]);
        generalPhotos.Add("Yamada", photos[103]);
        generalPhotos.Add("Okumura", photos[104]);
        generalPhotos.Add("Kimura", photos[105]);
        generalPhotos.Add("Sugiyama", photos[106]);
        generalPhotos.Add("Matsui", photos[107]);
        generalPhotos.Add("Itagaki", photos[108]);
        generalPhotos.Add("Kuribayashi", photos[109]);
        generalPhotos.Add("Kurita", photos[110]);
        generalPhotos.Add("Umezu", photos[111]);
        generalPhotos.Add("Nagata", photos[112]);
        generalPhotos.Add("Nagano", photos[113]);
        generalPhotos.Add("Fuchida", photos[114]);
        generalPhotos.Add("Ushijima", photos[115]);
        generalPhotos.Add("Hata", photos[116]);
        generalPhotos.Add("Ishiwara", photos[117]);
        generalPhotos.Add("Isogai", photos[118]);
        generalPhotos.Add("Kusaka", photos[119]);
        generalPhotos.Add("Araki", photos[120]);
        generalPhotos.Add("Tani", photos[121]);
        generalPhotos.Add("Kondo", photos[122]);
        generalPhotos.Add("Endo", photos[123]);
        generalPhotos.Add("Suzuki", photos[124]);
        generalPhotos.Add("Hasegawa", photos[125]);
        generalPhotos.Add("Abe", photos[126]);
        generalPhotos.Add("Kesago", photos[127]);

        //needs translating
        generalPhotos.Add("Bagramyan", photos[128]);
        generalPhotos.Add("Budyonny", photos[129]);
        generalPhotos.Add("Chuikov", photos[130]);
        generalPhotos.Add("Govorov", photos[131]);
        generalPhotos.Add("Lukin", photos[132]);
        generalPhotos.Add("Malinovsky", photos[133]);
        generalPhotos.Add("Kuznetsov", photos[134]);
        generalPhotos.Add("Shaposhnikov", photos[135]);
        generalPhotos.Add("Timoshenko", photos[136]);
        generalPhotos.Add("Vasilevsky", photos[137]);
        generalPhotos.Add("Vatutin", photos[138]);
        generalPhotos.Add("Voroshilov", photos[139]);

        generalPhotos.Add("Blamey", photos[140]);
        generalPhotos.Add("Cunningham", photos[141]);
        generalPhotos.Add("Dempsey", photos[142]);
        generalPhotos.Add("Dowding", photos[143]);
        generalPhotos.Add("Grace", photos[144]);
        generalPhotos.Add("Alexander", photos[145]);
        generalPhotos.Add("Dill", photos[146]);
        generalPhotos.Add("Mountbatten", photos[147]);
        generalPhotos.Add("Portal", photos[148]);

        generalPhotos.Add("Pound", photos[149]);
        generalPhotos.Add("Slim", photos[150]);

        generalPhotos.Add("Wavell", photos[151]);
        generalPhotos.Add("Wingate", photos[152]);

        generalPhotos.Add("Darlan", photos[153]);
        generalPhotos.Add("Bartus", photos[154]);
        generalPhotos.Add("Leclerc", photos[155]);
        generalPhotos.Add("De Gaulle", photos[156]);
        generalPhotos.Add("Juin", photos[157]);
        generalPhotos.Add("Auboyneau", photos[158]);
        generalPhotos.Add("Biver", photos[159]);
        generalPhotos.Add("Valin", photos[160]);
        generalPhotos.Add("Gamelin", photos[161]);
        generalPhotos.Add("Bene", photos[162]);
        generalPhotos.Add("Petain", photos[163]);
        generalPhotos.Add("Martin", photos[164]);
        generalPhotos.Add("Auchinleck", photos[165]);

        generalPhotos.Add("Antonescu", photos[166]);
        generalPhotos.Add("Boris", photos[167]);
        generalPhotos.Add("Christian", photos[168]);
        generalPhotos.Add("Franco", photos[169]);
        generalPhotos.Add("Horthy", photos[170]);
        generalPhotos.Add("Inonu", photos[171]);
        generalPhotos.Add("Leopold", photos[172]);
        generalPhotos.Add("Mahdi", photos[173]);
        generalPhotos.Add("Mannerheim", photos[174]);
        generalPhotos.Add("Nasser", photos[175]);
        generalPhotos.Add("Olav", photos[176]);
        generalPhotos.Add("Papagos", photos[177]);
        generalPhotos.Add("Phibun", photos[178]);
        generalPhotos.Add("Smigly", photos[179]);
        generalPhotos.Add("Tito", photos[180]);
        generalPhotos.Add("Winkleman", photos[181]);

        generalPhotos.Add("Campioni", photos[182]);
        generalPhotos.Add("Messe", photos[183]);
        generalPhotos.Add("Graziani", photos[184]);
        generalPhotos.Add("Balbo", photos[185]);
        generalPhotos.Add("Badoglio", photos[186]);
        generalPhotos.Add("Enrico", photos[187]);
        generalPhotos.Add("Cavallero", photos[188]);
        generalPhotos.Add("Liu Bocheng", photos[189]);
        generalPhotos.Add("Xu Shiyou", photos[190]);
        generalPhotos.Add("Xu Xiangqian", photos[191]);
        generalPhotos.Add("Zhu De", photos[192]);

        generalPhotos.Add("He Long", photos[193]);
        generalPhotos.Add("Xiao Jinguang", photos[194]);
        generalPhotos.Add("Xu Haidong", photos[195]);


        generalPhotos.Add("Ye Jianying", photos[196]);
        generalPhotos.Add("Su Yu", photos[197]);
        generalPhotos.Add("Chen Yi", photos[198]);
        generalPhotos.Add("Chen Xilian", photos[199]);
        generalPhotos.Add("Wang Jingwei", photos[200]);
        generalPhotos.Add("Herring", photos[201]);
        generalPhotos.Add("Giap", photos[202]);
        generalPhotos.Add("Sudirman", photos[203]);

        generalPhotos.Add("Jodl", photos[204]);
        generalPhotos.Add("Krebs", photos[205]);
        generalPhotos.Add("Steiner", photos[206]);

        generalPhotos.Add("Burgdorf", photos[207]);
        generalPhotos.Add("Crerar", photos[208]);
        generalPhotos.Add("Simonds", photos[209]);

        generalPhotos.Add("Roux", photos[210]);
        generalPhotos.Add("Castro", photos[211]);
        generalPhotos.Add("Charusathien", photos[212]);
        generalPhotos.Add("Kunaev", photos[213]);
        generalPhotos.Add("Freyberg", photos[214]);
        generalPhotos.Add("Gunichi", photos[215]);
        generalPhotos.Add("Choibalsan", photos[216]);
        generalPhotos.Add("Kim", photos[217]);
        generalPhotos.Add("Paik", photos[218]);
        generalPhotos.Add("Pervizi", photos[219]);
        generalPhotos.Add("Jaujard", photos[220]);

        //custom
        generalPhotos.Add("DeSantis", photos[221]);


        generals = new Dictionary<string, General>();
        generals.Add("default", new General());


        generals.Add("Zhukov", new General("Soviet", General.GeneralType.SSInfantryArtillery, hideGeneral: false, perk1: General.GeneralPerk.Artillery, perk2: General.GeneralPerk.Infantry, perk3: General.GeneralPerk.Training));
        generals.Add("Konev", new General("Soviet", General.GeneralType.SSArtillery, hideGeneral: false, perk1: General.GeneralPerk.Artillery, perk2: General.GeneralPerk.Mechanic, perk3: General.GeneralPerk.Plains));
        generals.Add("Rokossovsky", new General("Soviet", General.GeneralType.SSInfantryPanzer, hideGeneral: false, perk1: General.GeneralPerk.Armor, perk2: General.GeneralPerk.Blitzkrieg, perk3: General.GeneralPerk.Plains));
        generals.Add("Vasilevsky", new General("Soviet", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Logistics));
        generals.Add("Vatutin", new General("Soviet", General.GeneralType.SPanzer, hideGeneral: false, perk1: General.GeneralPerk.Armor, perk2: General.GeneralPerk.Plains));
        generals.Add("Chuikov", new General("Soviet", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.DefenseExpert, perk2: General.GeneralPerk.City));
        generals.Add("Govorov", new General("Soviet", General.GeneralType.SArtillery, hideGeneral: false, perk1: General.GeneralPerk.Artillery, perk2: General.GeneralPerk.Plains));
        generals.Add("Lukin", new General("Soviet", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Guerilla, perk2: General.GeneralPerk.City));
        generals.Add("Kuznetsov", new General("Soviet", General.GeneralType.SNavy, hideGeneral: false, perk1: General.GeneralPerk.Navy));
        generals.Add("Malinovsky", new General("Soviet", General.GeneralType.AInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Training));
        generals.Add("Shaposhnikov", new General("Soviet", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Training));
        generals.Add("Timoshenko", new General("Soviet", General.GeneralType.APanzer, perk1: General.GeneralPerk.Armor));
        generals.Add("Voroshilov", new General("Soviet", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Training));
        generals.Add("Bagramyan", new General("Soviet", General.GeneralType.AInfantry, hideGeneral: false, perk1: General.GeneralPerk.Plains));
        generals.Add("Budyonny", new General("Soviet", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Infantry));
        generals.Add("Kunaev", new General("Soviet", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Plains));


        generals.Add("Manstein", new General("German", General.GeneralType.SSInfantryPanzer, hideGeneral: false, perk1: General.GeneralPerk.Armor, perk2: General.GeneralPerk.Blitzkrieg, perk3: General.GeneralPerk.Mechanic));
        generals.Add("Rommel", new General("German", General.GeneralType.SSPanzer, hideGeneral: false, perk1: General.GeneralPerk.Armor, perk2: General.GeneralPerk.Blitzkrieg, perk3: General.GeneralPerk.Desert));
        generals.Add("Guderian", new General("German", General.GeneralType.SSInfantryPanzer, hideGeneral: false, perk1: General.GeneralPerk.Armor, perk2: General.GeneralPerk.Blitzkrieg, perk3: General.GeneralPerk.Plains));
        generals.Add("Bock", new General("German", General.GeneralType.SSInfantry, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Logistics, perk3: General.GeneralPerk.Training));
        generals.Add("Goering", new General("German", General.GeneralType.SSAir, perk1: General.GeneralPerk.Logistics, perk2: General.GeneralPerk.Training));
        generals.Add("Doenitz", new General("German", General.GeneralType.SSNavy, hideGeneral: false, perk1: General.GeneralPerk.Navy, perk2: General.GeneralPerk.Mechanic, perk3: General.GeneralPerk.Training));
        generals.Add("Model", new General("German", General.GeneralType.SSInfantry, hideGeneral: false, perk1: General.GeneralPerk.DefenseExpert, perk2: General.GeneralPerk.ShelterExpert, perk3: General.GeneralPerk.Logistics));
        generals.Add("Leeb", new General("German", General.GeneralType.SSArtillery, hideGeneral: false, perk1: General.GeneralPerk.Artillery, perk2: General.GeneralPerk.Plains, perk3: General.GeneralPerk.Mechanic));
        generals.Add("Kleist", new General("German", General.GeneralType.SPanzer, perk1: General.GeneralPerk.Blitzkrieg, perk2: General.GeneralPerk.Plains));
        generals.Add("Rundstedt", new General("German", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.DefenseExpert, perk3: General.GeneralPerk.Training));
        generals.Add("Paulus", new General("German", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.City));
        generals.Add("Keitel", new General("German", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Logistics, perk2: General.GeneralPerk.Mechanic));
        generals.Add("Kesselring", new General("German", General.GeneralType.SAir, perk1: General.GeneralPerk.Training));
        generals.Add("Seeckt", new General("German", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Training));
        generals.Add("Brauchitsch", new General("German", General.GeneralType.SArtillery, perk1: General.GeneralPerk.Artillery, perk2: General.GeneralPerk.Mechanic));
        generals.Add("Schoerner", new General("German", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Logistics));
        generals.Add("Jodl", new General("German", General.GeneralType.SPanzer, perk1: General.GeneralPerk.Mechanic, perk2: General.GeneralPerk.Training));
        generals.Add("List", new General("German", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Armor, perk2: General.GeneralPerk.Plains));
        generals.Add("Student", new General("German", General.GeneralType.SAir, perk1: General.GeneralPerk.Logistics));
        generals.Add("Kuckler", new General("German", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Training));
        generals.Add("Steiner", new General("German", General.GeneralType.APanzer, perk1: General.GeneralPerk.Mechanic));
        generals.Add("Kruger", new General("German", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Plains));
        generals.Add("Manteuffel", new General("German", General.GeneralType.APanzer, hideGeneral: false, perk1: General.GeneralPerk.Mechanic, perk2: General.GeneralPerk.Plains));
        generals.Add("Glommer", new General("German", General.GeneralType.AAir));
        generals.Add("Heinrici", new General("German", General.GeneralType.AInfantry));
        generals.Add("Witzleben", new General("German", General.GeneralType.AInfantry));
        generals.Add("Fegelein", new General("German", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Training));
        generals.Add("Meyer", new General("German", General.GeneralType.AArtillery));
        generals.Add("Raeder", new General("German", General.GeneralType.AArtillery));
        generals.Add("Weichs", new General("German", General.GeneralType.AArtillery, hideGeneral: false, perk1: General.GeneralPerk.Artillery));
        generals.Add("Burgdorf", new General("German", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Training));
        generals.Add("Krebs", new General("German", General.GeneralType.AInfantry, perk1: General.GeneralPerk.ShelterExpert));

        generals.Add("Badoglio", new General("Italy", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Mountains));
        generals.Add("Graziani", new General("Italy", General.GeneralType.SPanzer, hideGeneral: false, perk1: General.GeneralPerk.Armor, perk2: General.GeneralPerk.Mountains));
        generals.Add("Campioni", new General("Italy", General.GeneralType.ANavy, perk1: General.GeneralPerk.Navy));
        generals.Add("Messe", new General("Italy", General.GeneralType.APanzer, hideGeneral: false, perk1: General.GeneralPerk.Mechanic, perk2: General.GeneralPerk.Mountains));
        generals.Add("Balbo", new General("Italy", General.GeneralType.AAir));
        generals.Add("Enrico", new General("Italy", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Guerilla));
        generals.Add("Cavallero", new General("Italy", General.GeneralType.AInfantry));


        generals.Add("Eisonhower", new General("USA", General.GeneralType.SSAirNavy, perk1: General.GeneralPerk.Navy, perk2: General.GeneralPerk.Mechanic, perk3: General.GeneralPerk.Training));
        generals.Add("Nimitz", new General("USA", General.GeneralType.SSAirNavy, hideGeneral: false, perk1: General.GeneralPerk.Navy, perk2: General.GeneralPerk.Training));
        generals.Add("Fletcher", new General("USA", General.GeneralType.SSNavy, perk1: General.GeneralPerk.Navy, perk2: General.GeneralPerk.Training));
        generals.Add("Halsey", new General("USA", General.GeneralType.SSNavy, hideGeneral: false, perk1: General.GeneralPerk.Navy, perk2: General.GeneralPerk.Mechanic));
        generals.Add("Patton", new General("USA", General.GeneralType.SSPanzer, hideGeneral: false, perk1: General.GeneralPerk.Armor, perk2: General.GeneralPerk.Mechanic, perk3: General.GeneralPerk.Plains));
        generals.Add("McArthur", new General("USA", General.GeneralType.SSInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Forest, perk3: General.GeneralPerk.Logistics));
        generals.Add("Doolitle", new General("USA", General.GeneralType.SAir, hideGeneral: false, perk1: General.GeneralPerk.Training));
        generals.Add("Arnold", new General("USA", General.GeneralType.SAir, perk1: General.GeneralPerk.Training));
        generals.Add("Mitchell", new General("USA", General.GeneralType.SAir));
        generals.Add("Kinkaid", new General("USA", General.GeneralType.SNavy, hideGeneral: false, perk1: General.GeneralPerk.Navy));
        generals.Add("Bradley", new General("USA", General.GeneralType.SPanzer, hideGeneral: false, perk1: General.GeneralPerk.Armor, perk2: General.GeneralPerk.Mechanic));
        generals.Add("Chennault", new General("USA", General.GeneralType.SAir, perk1: General.GeneralPerk.Training));
        generals.Add("Clark", new General("USA", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Training));
        generals.Add("Spruance", new General("USA", General.GeneralType.SNavy, perk1: General.GeneralPerk.Training));
        generals.Add("Stilwell", new General("USA", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Training));
        generals.Add("Ike", new General("USA", General.GeneralType.AAir));
        generals.Add("Smith", new General("USA", General.GeneralType.AArtillery));
        generals.Add("Devers", new General("USA", General.GeneralType.APanzer, hideGeneral: false, perk1: General.GeneralPerk.Armor, perk2: General.GeneralPerk.Training));
        generals.Add("Lee", new General("USA", General.GeneralType.AArtillery, perk1: General.GeneralPerk.Artillery));
        generals.Add("King", new General("USA", General.GeneralType.ANavy, perk1: General.GeneralPerk.Training));
        generals.Add("McCain", new General("USA", General.GeneralType.ANavy, perk1: General.GeneralPerk.Mechanic));
        generals.Add("McAuliffe", new General("USA", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Mechanic));


        generals.Add("Montgomery", new General("UK", General.GeneralType.SSPanzer, hideGeneral: false, perk1: General.GeneralPerk.Armor, perk2: General.GeneralPerk.Mechanic, perk3: General.GeneralPerk.Desert));
        generals.Add("Dowding", new General("UK", General.GeneralType.SSAir, hideGeneral: false, perk1: General.GeneralPerk.Logistics, perk2: General.GeneralPerk.Training));
        generals.Add("Mountbatten", new General("UK", General.GeneralType.SSNavy, hideGeneral: false, perk1: General.GeneralPerk.Navy, perk2: General.GeneralPerk.Mechanic, perk3: General.GeneralPerk.Training));
        generals.Add("Auchinleck", new General("UK", General.GeneralType.SPanzer, hideGeneral: false, perk1: General.GeneralPerk.Armor));
        generals.Add("Alexander", new General("UK", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Infantry));
        generals.Add("Dill", new General("UK", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Training));
        generals.Add("Wingate", new General("UK", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Guerilla));
        generals.Add("Portal", new General("UK", General.GeneralType.SAir, perk1: General.GeneralPerk.Infantry));
        generals.Add("Cunningham", new General("UK", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Infantry));
        generals.Add("Slim", new General("UK", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Training));

        generals.Add("Dempsey", new General("UK", General.GeneralType.AInfantry));

        generals.Add("Pound", new General("UK", General.GeneralType.ANavy));
        generals.Add("Wavell", new General("UK", General.GeneralType.AInfantry));

        generals.Add("Crerar", new General("Canada", General.GeneralType.AArtillery, hideGeneral: false, perk1: General.GeneralPerk.Artillery));
        generals.Add("Simonds", new General("Canada", General.GeneralType.AInfantry, hideGeneral: false, perk1: General.GeneralPerk.Logistics));

        generals.Add("Herring", new General("Australia", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Forest));
        generals.Add("Blamey", new General("Australia", General.GeneralType.AInfantry, hideGeneral: false, perk1: General.GeneralPerk.Training));
        generals.Add("Grace", new General("Australia", General.GeneralType.ANavy));
        generals.Add("Freyberg", new General("Australia", General.GeneralType.AArtillery));

        generals.Add("Castro", new General("Cuba", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Guerilla, perk3: General.GeneralPerk.Logistics));
        generals.Add("Antonescu", new General("Romania", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Plains, hideGeneral:true));
        generals.Add("Boris", new General("Bulgaria", General.GeneralType.AInfantry));
        generals.Add("Christian", new General("Denmark", General.GeneralType.AInfantry));
        generals.Add("Franco", new General("FascistSpain", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Logistics, perk2: General.GeneralPerk.Training));
        generals.Add("Horthy", new General("Hungary", General.GeneralType.AInfantry));
        generals.Add("Inonu", new General("Turkey", General.GeneralType.SInfantry));
        generals.Add("Mahdi", new General("Iraq", General.GeneralType.AInfantry));
        generals.Add("Mannerheim", new General("Finland", General.GeneralType.SSInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Guerilla, perk3: General.GeneralPerk.Snow));
        generals.Add("Nasser", new General("Egypt", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Desert));
        generals.Add("Olav", new General("Norway", General.GeneralType.AInfantry));
        generals.Add("Papagos", new General("Greece", General.GeneralType.AInfantry));
        generals.Add("Tito", new General("Yugoslavia", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Guerilla));
        generals.Add("Winkleman", new General("Netherlands", General.GeneralType.AInfantry));
        generals.Add("Smigly", new General("Poland", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Logistics));
        generals.Add("Leopold", new General("Belgium", General.GeneralType.AArtillery));
        generals.Add("Pervizi", new General("Albania", General.GeneralType.SInfantry));
        generals.Add("Giap", new General("Vietnam", General.GeneralType.SArtillery, hideGeneral: false, perk1: General.GeneralPerk.Armor, perk2: General.GeneralPerk.Artillery, perk3: General.GeneralPerk.Guerilla));
        generals.Add("Sudirman", new General("Indonesia", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Guerilla));
        generals.Add("Choibalsan", new General("Mongolia", General.GeneralType.AInfantry));
        generals.Add("Kim", new General("NorthKorea", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Training));
        generals.Add("Paik", new General("SouthKorea", General.GeneralType.APanzer, perk1: General.GeneralPerk.Plains));
        generals.Add("Charusathien", new General("Thailand", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Logistics));
        generals.Add("Phibun", new General("Thailand", General.GeneralType.AInfantry));


        generals.Add("Leclerc", new General("France", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Training));
        generals.Add("De Gaulle", new General("France", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Logistics, perk2: General.GeneralPerk.Training));
        generals.Add("Petain", new General("France", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Training));
        generals.Add("Jaujard", new General("France", General.GeneralType.SNavy, hideGeneral: false, perk1: General.GeneralPerk.Navy));
        generals.Add("Gamelin", new General("France", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Infantry));
        generals.Add("Valin", new General("France", General.GeneralType.SAir, perk1: General.GeneralPerk.Training));
        generals.Add("Auboyneau", new General("France", General.GeneralType.SNavy));
        generals.Add("Darlan", new General("France", General.GeneralType.AInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry));
        generals.Add("Bartus", new General("France", General.GeneralType.ANavy));
        generals.Add("Juin", new General("France", General.GeneralType.AArtillery));
        generals.Add("Biver", new General("France", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Plains));
        generals.Add("Bene", new General("France", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Logistics));
        generals.Add("Martin", new General("France", General.GeneralType.AInfantry));
        generals.Add("Roux", new General("France", General.GeneralType.ANavy));



        generals.Add("Mao Zedong", new General("PRCNew", General.GeneralType.SSInfantry, perk1: General.GeneralPerk.Logistics, perk2: General.GeneralPerk.Guerilla, perk3: General.GeneralPerk.Training));
        generals.Add("Liu Bocheng", new General("PRCNew", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Guerilla));
        generals.Add("Xu Shiyou", new General("PRCNew", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Guerilla, perk2: General.GeneralPerk.Logistics));
        generals.Add("Xu Xiangqian", new General("PRCNew", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Guerilla));
        generals.Add("Zhu De", new General("PRCNew", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Logistics, perk2: General.GeneralPerk.Training));
        generals.Add("Lin Biao", new General("PRCNew", General.GeneralType.SArtillery, hideGeneral: false, perk1: General.GeneralPerk.Artillery, perk2: General.GeneralPerk.ShelterExpert, perk3: General.GeneralPerk.Training));
        generals.Add("Peng Dehuai", new General("PRCNew", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Guerilla, perk3: General.GeneralPerk.Logistics));
        generals.Add("He Long", new General("PRCNew", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Plains, perk3: General.GeneralPerk.Training));
        generals.Add("Ye Jianying", new General("PRCNew", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.DefenseExpert, perk3: General.GeneralPerk.Training));
        generals.Add("Su Yu", new General("PRCNew", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.DefenseExpert, perk3: General.GeneralPerk.Logistics));
        generals.Add("Chen Yi", new General("PRCNew", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Logistics));
        generals.Add("Chen Xilian", new General("PRCNew", General.GeneralType.AArtillery, hideGeneral: false, perk1: General.GeneralPerk.Artillery));
        generals.Add("Xiao Jinguang", new General("PRCNew", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Logistics));
        generals.Add("Xu Haidong", new General("PRCNew", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Training));

        

        generals.Add("Chiang Kaishek", new General("ROC", General.GeneralType.SSInfantry, perk1: General.GeneralPerk.Logistics, perk2: General.GeneralPerk.Training, perk3: General.GeneralPerk.Plains));
        generals.Add("Sun Liren", new General("ROC", General.GeneralType.SSInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Forest, perk3: General.GeneralPerk.Logistics));
        generals.Add("Wang Yaowu", new General("ROC", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Guerilla));
        generals.Add("Du Yuming", new General("ROC", General.GeneralType.SPanzer, hideGeneral: false, perk1: General.GeneralPerk.Armor, perk2: General.GeneralPerk.Blitzkrieg));
        generals.Add("Zhang Zizhong", new General("ROC", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.City, perk2: General.GeneralPerk.DefenseExpert));
        generals.Add("Zhou Zhirou", new General("ROC", General.GeneralType.SAir, hideGeneral: false, perk1: General.GeneralPerk.Training));
        generals.Add("Xue Yue", new General("ROC", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.City, perk3: General.GeneralPerk.DefenseExpert));
        generals.Add("Yan Xishan", new General("ROC", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Guerilla, perk2: General.GeneralPerk.Mountains));
        generals.Add("Sheng Shicai", new General("ROC", General.GeneralType.BInfantry, perk1: General.GeneralPerk.Infantry));
        generals.Add("Ma Bufang", new General("ROC", General.GeneralType.AInfantry));
        generals.Add("Long Yun", new General("ROC", General.GeneralType.AInfantry));
        generals.Add("Feng Yuxiang", new General("ROC", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Mountains));
        generals.Add("He Yingqin", new General("ROC", General.GeneralType.AInfantry, hideGeneral: false, perk1: General.GeneralPerk.Logistics));
        generals.Add("Fu Zuoyi", new General("ROC", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Training));
        generals.Add("Wei Lihuang", new General("ROC", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Infantry));
        generals.Add("Lin Yisheng", new General("ROC", General.GeneralType.AInfantry, hideGeneral: false, perk1: General.GeneralPerk.City));
        generals.Add("Li Zongren", new General("ROC", General.GeneralType.AInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.City));
        generals.Add("Bai Chongxi", new General("ROC", General.GeneralType.AInfantry, hideGeneral: false, perk1: General.GeneralPerk.Logistics, perk2: General.GeneralPerk.City));
        generals.Add("Gu Zhutong", new General("ROC", General.GeneralType.AInfantry, perk1: General.GeneralPerk.City));
        generals.Add("Zhang Xueliang", new General("ROC", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Guerilla));
        generals.Add("Chen Shaokuan", new General("ROC", General.GeneralType.ANavy, hideGeneral: false, perk1: General.GeneralPerk.Training));
        generals.Add("Liu Xiang", new General("ROC", General.GeneralType.BInfantry));
        generals.Add("Liu Wenhui", new General("ROC", General.GeneralType.BInfantry));
        generals.Add("Luo Zhuoying", new General("ROC", General.GeneralType.BInfantry));
        generals.Add("Sun Lianzhong", new General("ROC", General.GeneralType.BInfantry));
        generals.Add("Zhu Shaoliang", new General("ROC", General.GeneralType.BInfantry));
        generals.Add("Yu Hanmou", new General("ROC", General.GeneralType.BInfantry));
        generals.Add("Liu Zhi", new General("ROC", General.GeneralType.BInfantry));
        generals.Add("Li Pinxian", new General("ROC", General.GeneralType.BInfantry));
        generals.Add("Jiang Dingwen", new General("ROC", General.GeneralType.BInfantry, hideGeneral: false));
        generals.Add("Ma Hongkui", new General("ROC", General.GeneralType.BInfantry));
        generals.Add("Ma Hongbin", new General("ROC", General.GeneralType.BInfantry));
        generals.Add("Fan Hanjie", new General("ROC", General.GeneralType.BInfantry, hideGeneral: false));
        generals.Add("Zhang Jinghui", new General("ROC", General.GeneralType.BInfantry));


        generals.Add("Emperor Puyi", new General("Manchukuo", General.GeneralType.AInfantry));
        generals.Add("Wang Jingwei", new General("PuppetROC", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Logistics));


        generals.Add("Yamamoto", new General("Japan", General.GeneralType.SSAirNavy, hideGeneral: false, perk1: General.GeneralPerk.Navy, perk2: General.GeneralPerk.ShelterExpert, perk3: General.GeneralPerk.Training));
        generals.Add("Yamashita", new General("Japan", General.GeneralType.SSInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Forest, perk3: General.GeneralPerk.Guerilla));
        generals.Add("Inoue", new General("Japan", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Infantry, perk2: General.GeneralPerk.Guerilla));
        generals.Add("Nagumo", new General("Japan", General.GeneralType.SNavy, hideGeneral: false, perk1: General.GeneralPerk.Navy, perk2: General.GeneralPerk.Mechanic));
        generals.Add("Yamaguchi", new General("Japan", General.GeneralType.SNavy, perk1: General.GeneralPerk.Navy, perk2: General.GeneralPerk.Training));
        generals.Add("Okumura", new General("Japan", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.City, perk2: General.GeneralPerk.Logistics));
        generals.Add("Koga", new General("Japan", General.GeneralType.SNavy, perk1: General.GeneralPerk.Training));
        generals.Add("Nagano", new General("Japan", General.GeneralType.SNavy, hideGeneral: false, perk1: General.GeneralPerk.Mechanic));
        generals.Add("Terauchi", new General("Japan", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Logistics));
        generals.Add("Ozawa", new General("Japan", General.GeneralType.SNavy, perk1: General.GeneralPerk.Training));
        generals.Add("Kuribayashi", new General("Japan", General.GeneralType.SInfantry, hideGeneral: false, perk1: General.GeneralPerk.Infantry));
        generals.Add("Kurita", new General("Japan", General.GeneralType.SNavy, perk1: General.GeneralPerk.Navy));
        generals.Add("Umezu", new General("Japan", General.GeneralType.SInfantry, perk1: General.GeneralPerk.Infantry));
        generals.Add("Koiso", new General("Japan", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Logistics));
        generals.Add("Toyoda", new General("Japan", General.GeneralType.ANavy));
        generals.Add("Yamada", new General("Japan", General.GeneralType.AInfantry));
        generals.Add("Kimura", new General("Japan", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Training));
        generals.Add("Sugiyama", new General("Japan", General.GeneralType.AInfantry));
        generals.Add("Matsui", new General("Japan", General.GeneralType.AInfantry));
        generals.Add("Itagaki", new General("Japan", General.GeneralType.BPanzer, perk1: General.GeneralPerk.Mechanic));
        generals.Add("Nagata", new General("Japan", General.GeneralType.AInfantry));
        generals.Add("Fuchida", new General("Japan", General.GeneralType.AAir));
        generals.Add("Ushijima", new General("Japan", General.GeneralType.AInfantry));
        generals.Add("Hata", new General("Japan", General.GeneralType.BArtillery));
        generals.Add("Ishiwara", new General("Japan", General.GeneralType.AInfantry));
        generals.Add("Isogai", new General("Japan", General.GeneralType.BInfantry));
        generals.Add("Kusaka", new General("Japan", General.GeneralType.ANavy));
        generals.Add("Araki", new General("Japan", General.GeneralType.AInfantry));
        generals.Add("Tani", new General("Japan", General.GeneralType.AInfantry));
        generals.Add("Kondo", new General("Japan", General.GeneralType.ANavy));
        generals.Add("Endo", new General("Japan", General.GeneralType.AInfantry, perk1: General.GeneralPerk.Logistics));
        generals.Add("Suzuki", new General("Japan", General.GeneralType.BInfantry));
        generals.Add("Hasegawa", new General("Japan", General.GeneralType.BNavy));
        generals.Add("Abe", new General("Japan", General.GeneralType.BInfantry));
        generals.Add("Kesago", new General("Japan", General.GeneralType.AInfantry));
        generals.Add("Gunichi", new General("Japan", General.GeneralType.ANavy, hideGeneral: false, perk1: General.GeneralPerk.Navy));

        //custom charlie
        generals.Add("DeSantis", new General("USA", General.GeneralType.SSInfantryPanzer, perk1: General.GeneralPerk.Armor, perk2: General.GeneralPerk.Plains, perk3: General.GeneralPerk.Training));



        foreach (KeyValuePair<string, General> kp in generals) {
            generals[kp.Key].name = kp.Key;
        }

        SortGenerals();
    }
    public void SortGenerals() {
        Dictionary<string, General> sortedGenerals = new Dictionary<string, General>();
        //sortedGenerals.Add("default", generals["default"]);
        foreach (KeyValuePair<string, General> g in generals) {
            if (g.Key == "default")
                continue;
            if (playerData.generals.ContainsKey(g.Key)) {
//                print(g.Key);
                sortedGenerals.Add(g.Key, g.Value);
            }
        }
        foreach (KeyValuePair<string, General> g in generals) {
            if (g.Key == "default")
                continue;

            if (playerData.generals.ContainsKey(g.Key) || g.Value.hideGeneral) {
                continue;
            }
            sortedGenerals.Add(g.Key, g.Value);
        }

        playerShopGenerals = new Dictionary<string, General>(sortedGenerals);
    }
    //fix wrong names of past versions
    void FixGeneralNames() {
        foreach (KeyValuePair<string, string> i in CustomFunctions.GeneralUpdatedNames) {
            if (playerData.generals.ContainsKey(i.Key) && !playerData.generals.ContainsKey(i.Value)) {
                playerData.generals.Add(i.Value, playerData.generals[i.Key]);
                playerData.generals.Remove(i.Key);
            }
        }
        saveFile();
    }

    void FixMapNames() {
        foreach (KeyValuePair<string, string> i in CustomFunctions.MissionUpdatedNames) {
            if (playerData.completedLevels.Contains(i.Key) && !playerData.completedLevels.Contains(i.Value)) {
                playerData.completedLevels.Add(i.Value);
                playerData.completedLevels.Remove(i.Key);
            }
            if (playerData.levelsUnlocked.Contains(i.Key) && !playerData.levelsUnlocked.Contains(i.Value)) {
                playerData.levelsUnlocked.Add(i.Value);
                playerData.levelsUnlocked.Remove(i.Key);
            }
        }
        saveFile();
    }

    public void saveFile() {
        if (playerData != null)
            BinaryPlayerSave.SaveData(playerData);
    }

    void Update() {
#if UNITY_EDITOR
    //WARNING: TEST CODE
        //if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.E)) {
        //    playerData = new BinaryData2();
        //    saveFile();
        //}
#endif

    }
}
[System.Serializable]
public class General {
    public float[] infAtk, armorAtk, artilleryAtk, airAtk, navyAtk, healthBonus;
    public int[] maxCmdSize, cost, movement;
    public GeneralPerk perk1 = GeneralPerk.None, perk2 = GeneralPerk.None, perk3 = GeneralPerk.None;
    public string name, country;
    public bool hideGeneral;
    public bool ContainsPerk(GeneralPerk perk) {
        if (perk1 == perk || perk2 == perk || perk3 == perk) return true;
        return false;
    }
    public enum GeneralPerk {
        None, Infantry, Armor, Artillery, Navy, Plains,
        Forest, Mountains, Snow, Desert, City, Mechanic, Logistics, Blitzkrieg, Guerilla,
        Training, ShelterExpert, DefenseExpert
    }
    public enum GeneralType {
        SSInfantry, SInfantry, AInfantry, BInfantry,
        SSPanzer, SPanzer, APanzer, BPanzer,
        SSArtillery, SArtillery, AArtillery, BArtillery,
        SSNavy, SNavy, ANavy, BNavy,
        SSAir, SAir, AAir, BAir,
        SSInfantryArtillery, SSInfantryPanzer, SSAirNavy
    }
    public enum GeneralBranch { Infantry, Panzer, Artillery, Navy, Air, InfantryArtillery, InfantryPanzer, AirNavy }
    public GeneralBranch skillBranch;
    public General(string country, GeneralType ty, bool hideGeneral= true, GeneralPerk perk1= GeneralPerk.None, GeneralPerk perk2 = GeneralPerk.None, GeneralPerk perk3 = GeneralPerk.None) {
        this.hideGeneral = hideGeneral;
        //NOTE: AIRDEF is now for general health
        this.country = country;
        this.perk1 = perk1;
        this.perk2 = perk2;
        this.perk3 = perk3;
        switch (ty) {
        case GeneralType.SSInfantry:
            Assign(infAtk: new float[] { 30, 35, 35, 40, 45 }, armorAtk: new float[] { 15, 20, 20, 25, 30 }, artilleryAtk: new float[] { 20, 25, 25, 30, 30 }, airAtk: new float[] { 15, 15, 20, 20, 25 }, navyAtk: new float[] { 0, 0, 0, 0, 0 }, healthBonus: new float[] { 35, 40, 45, 50, 55 }, cost: new int[] { 1500, 750, 850, 950, 1050 }, movement: new int[] { 3, 3, 4, 4, 5 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Infantry;
            break;
        case GeneralType.SInfantry:
            Assign(infAtk: new float[] { 25, 30, 30, 35, 35 }, armorAtk: new float[] { 10, 15, 20, 20, 25 }, artilleryAtk: new float[] { 5, 10, 15, 20, 25 }, airAtk: new float[] { 10, 10, 15, 15, 20 }, navyAtk: new float[] { 0, 0, 0, 0, 0 }, healthBonus: new float[] { 30, 35, 40, 45, 50 }, cost: new int[] { 1250, 550, 650, 700, 750 }, movement: new int[] { 2, 3, 3, 4, 4 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Infantry;
            break;
        case GeneralType.AInfantry:
            Assign(infAtk: new float[] { 20, 20, 25, 25, 30 }, armorAtk: new float[] { 5, 10, 10, 15, 20 }, artilleryAtk: new float[] { 5, 10, 15, 15, 20 }, airAtk: new float[] { 5, 5, 10, 10, 15 }, navyAtk: new float[] { 0, 0, 0, 0, 0 }, healthBonus: new float[] { 20, 25, 30, 35, 40 }, cost: new int[] { 900, 500, 550, 600, 700 }, movement: new int[] { 1, 1, 2, 2, 3 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Infantry;
            break;
        case GeneralType.BInfantry:
            Assign(infAtk: new float[] { 15, 20, 20, 25, 25 }, armorAtk: new float[] { 5, 5, 10, 10, 15 }, artilleryAtk: new float[] { 5, 5, 10, 10, 15 }, airAtk: new float[] { 5, 5, 5, 10, 10 }, navyAtk: new float[] { 0, 0, 0, 0, 0 }, healthBonus: new float[] { 10, 15, 20, 25, 30 }, cost: new int[] { 550, 350, 400, 450, 500 }, movement: new int[] { 0, 1, 1, 1, 2 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Infantry;
            break;
        case GeneralType.SSPanzer:
            Assign(infAtk: new float[] { 20, 25, 25, 25, 30 }, armorAtk: new float[] { 30, 30, 35, 40, 45 }, artilleryAtk: new float[] { 20, 20, 25, 25, 30 }, airAtk: new float[] { 15, 15, 20, 20, 25 }, navyAtk: new float[] { 0, 0, 0, 0, 0 }, healthBonus: new float[] { 35, 40, 45, 50, 55 }, cost: new int[] { 1500, 750, 850, 950, 1050 }, movement: new int[] { 3, 3, 4, 4, 5 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Panzer;
            break;
        case GeneralType.SPanzer:
            Assign(infAtk: new float[] { 10, 15, 20, 20, 25 }, armorAtk: new float[] { 25, 25, 30, 35, 35 }, artilleryAtk: new float[] { 5, 10, 15, 20, 25 }, airAtk: new float[] { 10, 10, 15, 15, 20 }, navyAtk: new float[] { 0, 0, 0, 0, 0 }, healthBonus: new float[] { 30, 35, 40, 45, 50 }, cost: new int[] { 1250, 550, 650, 700, 750 }, movement: new int[] { 2, 3, 3, 4, 4 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Panzer;
            break;
        case GeneralType.APanzer:
            Assign(infAtk: new float[] { 5, 10, 10, 15, 20 }, armorAtk: new float[] { 20, 20, 25, 35, 30 }, artilleryAtk: new float[] { 5, 10, 15, 15, 20 }, airAtk: new float[] { 5, 5, 10, 10, 15 }, navyAtk: new float[] { 0, 0, 0, 0, 0 }, healthBonus: new float[] { 20, 25, 30, 35, 40 }, cost: new int[] { 900, 500, 550, 600, 700 }, movement: new int[] { 1, 1, 2, 2, 3 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Panzer;
            break;
        case GeneralType.BPanzer:
            Assign(infAtk: new float[] { 5, 5, 10, 10, 15 }, armorAtk: new float[] { 15, 20, 20, 25, 25 }, artilleryAtk: new float[] { 5, 5, 10, 10, 15 }, airAtk: new float[] { 5, 5, 5, 10, 10 }, navyAtk: new float[] { 0, 0, 0, 0, 0 }, healthBonus: new float[] { 10, 15, 20, 25, 30 }, cost: new int[] { 550, 350, 400, 450, 500 }, movement: new int[] { 0, 1, 1, 1, 2 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Panzer;
            break;
        case GeneralType.SSArtillery:
            Assign(infAtk: new float[] { 20, 20, 25, 25, 30 }, armorAtk: new float[] { 20, 20, 25, 25, 25 }, artilleryAtk: new float[] { 25, 30, 30, 35, 40 }, airAtk: new float[] { 10, 15, 20, 20, 25 }, navyAtk: new float[] { 0, 0, 0, 0, 0 }, healthBonus: new float[] { 35, 40, 45, 50, 55 }, cost: new int[] { 1500, 750, 850, 950, 1050 }, movement: new int[] { 3, 3, 4, 4, 5 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Artillery;
            break;
        case GeneralType.SArtillery:
            Assign(infAtk: new float[] { 15, 15, 20, 20, 20 }, armorAtk: new float[] { 10, 15, 20, 20, 25 }, artilleryAtk: new float[] { 20, 25, 30, 35, 35 }, airAtk: new float[] { 10, 10, 15, 15, 20 }, navyAtk: new float[] { 0, 0, 0, 0, 0 }, healthBonus: new float[] { 30, 35, 40, 45, 50 }, cost: new int[] { 1250, 550, 650, 700, 750 }, movement: new int[] { 2, 3, 3, 4, 4 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Artillery;
            break;
        case GeneralType.AArtillery:
            Assign(infAtk: new float[] { 10, 10, 15, 15, 20 }, armorAtk: new float[] { 5, 10, 10, 15, 20 }, artilleryAtk: new float[] { 20, 25, 25, 30, 30 }, airAtk: new float[] { 5, 5, 10, 10, 15 }, navyAtk: new float[] { 0, 0, 0, 0, 0 }, healthBonus: new float[] { 20, 25, 30, 35, 40 }, cost: new int[] { 900, 500, 550, 600, 700 }, movement: new int[] { 1, 1, 2, 2, 3 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Artillery;
            break;
        case GeneralType.BArtillery:
            Assign(infAtk: new float[] { 5, 5, 10, 10, 15 }, armorAtk: new float[] { 5, 5, 10, 10, 15 }, artilleryAtk: new float[] { 15, 20, 20, 25, 25 }, airAtk: new float[] { 5, 5, 5, 10, 10 }, navyAtk: new float[] { 0, 0, 0, 0, 0 }, healthBonus: new float[] { 10, 15, 20, 25, 30 }, cost: new int[] { 550, 350, 400, 450, 500 }, movement: new int[] { 0, 1, 1, 1, 2 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Artillery;
            break;
        case GeneralType.SSAir:
            Assign(infAtk: new float[] { 15, 15, 20, 20, 25 }, armorAtk: new float[] { 20, 20, 25, 25, 25 }, artilleryAtk: new float[] { 20, 20, 25, 25, 30 }, airAtk: new float[] { 35, 35, 40, 40, 45 }, navyAtk: new float[] { 20, 20, 25, 25, 30 }, healthBonus: new float[] { 35, 40, 45, 50, 55 }, cost: new int[] { 1500, 750, 850, 950, 1050 }, movement: new int[] { 3, 3, 4, 4, 5 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Air;
            break;
        case GeneralType.SAir:
            Assign(infAtk: new float[] { 15, 15, 20, 20, 20 }, armorAtk: new float[] { 10, 15, 20, 20, 25 }, artilleryAtk: new float[] { 10, 10, 15, 15, 20 }, airAtk: new float[] { 20, 25, 30, 30, 35 }, navyAtk: new float[] { 5, 10, 15, 15, 20 }, healthBonus: new float[] { 30, 35, 40, 45, 50 }, cost: new int[] { 1250, 550, 650, 700, 750 }, movement: new int[] { 2, 3, 3, 4, 4 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Air;
            break;
        case GeneralType.AAir:
            Assign(infAtk: new float[] { 10, 10, 15, 15, 20 }, armorAtk: new float[] { 5, 10, 10, 15, 20 }, artilleryAtk: new float[] { 5, 5, 10, 10, 15 }, airAtk: new float[] { 20, 25, 25, 30, 30 }, navyAtk: new float[] { 5, 5, 10, 10, 15 }, healthBonus: new float[] { 20, 25, 30, 35, 40 }, cost: new int[] { 900, 500, 550, 600, 700 }, movement: new int[] { 1, 1, 2, 2, 3 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Air;
            break;
        case GeneralType.BAir:
            Assign(infAtk: new float[] { 5, 10, 10, 15, 15 }, armorAtk: new float[] { 5, 5, 10, 10, 15 }, artilleryAtk: new float[] { 5, 5, 10, 10, 15 }, airAtk: new float[] { 15, 20, 20, 25, 25 }, navyAtk: new float[] { 0, 0, 5, 5, 10 }, healthBonus: new float[] { 10, 15, 20, 25, 30 }, cost: new int[] { 550, 350, 400, 450, 500 }, movement: new int[] { 0, 1, 1, 1, 2 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Air;
            break;
        case GeneralType.SSNavy:
            Assign(infAtk: new float[] { 15, 15, 20, 20, 25 }, armorAtk: new float[] { 20, 20, 25, 25, 25 }, artilleryAtk: new float[] { 20, 20, 25, 25, 30 }, airAtk: new float[] { 20, 20, 25, 25, 30 }, navyAtk: new float[] { 35, 35, 40, 40, 45 }, healthBonus: new float[] { 35, 40, 45, 50, 55 }, cost: new int[] { 1500, 750, 850, 950, 1050 }, movement: new int[] { 3, 3, 4, 4, 5 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Navy;
            break;
        case GeneralType.SNavy:
            Assign(infAtk: new float[] { 15, 15, 20, 20, 20 }, armorAtk: new float[] { 10, 15, 20, 20, 25 }, artilleryAtk: new float[] { 10, 10, 15, 15, 20 }, airAtk: new float[] { 5, 10, 15, 15, 20 }, navyAtk: new float[] { 20, 25, 30, 30, 35 }, healthBonus: new float[] { 30, 35, 40, 45, 50 }, cost: new int[] { 1250, 550, 650, 700, 750 }, movement: new int[] { 2, 3, 3, 4, 4 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Navy;
            break;
        case GeneralType.ANavy:
            Assign(infAtk: new float[] { 10, 10, 15, 15, 20 }, armorAtk: new float[] { 5, 10, 10, 15, 20 }, artilleryAtk: new float[] { 5, 5, 10, 10, 15 }, airAtk: new float[] { 5, 5, 10, 10, 15 }, navyAtk: new float[] { 20, 25, 25, 30, 30 }, healthBonus: new float[] { 20, 25, 30, 35, 40 }, cost: new int[] { 900, 500, 550, 600, 700 }, movement: new int[] { 1, 1, 2, 2, 3 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Navy;
            break;
        case GeneralType.BNavy:
            Assign(infAtk: new float[] { 5, 10, 10, 15, 15 }, armorAtk: new float[] { 5, 5, 10, 10, 15 }, artilleryAtk: new float[] { 5, 5, 10, 10, 15 }, airAtk: new float[] { 0, 0, 5, 5, 10 }, navyAtk: new float[] { 15, 20, 20, 25, 25 }, healthBonus: new float[] { 10, 15, 20, 25, 30 }, cost: new int[] { 550, 350, 400, 450, 500 }, movement: new int[] { 0, 1, 1, 1, 2 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.Navy;
            break;
        case GeneralType.SSInfantryArtillery:
            Assign(infAtk: new float[] { 30, 35, 35, 40, 45 }, armorAtk: new float[] { 20, 20, 25, 25, 25 }, artilleryAtk: new float[] { 30, 30, 35, 40, 45 }, airAtk: new float[] { 15, 15, 20, 20, 25 }, navyAtk: new float[] { 0, 0, 0, 0, 0 }, healthBonus: new float[] { 45, 50, 55, 60, 65 }, cost: new int[] { 1750, 850, 900, 950, 1050 }, movement: new int[] { 3, 3, 4, 4, 5 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.InfantryArtillery;
            break;
        case GeneralType.SSInfantryPanzer:
            Assign(infAtk: new float[] { 30, 30, 35, 40, 45 }, armorAtk: new float[] { 30, 35, 35, 40, 45 }, artilleryAtk: new float[] { 20, 20, 25, 25, 25 }, airAtk: new float[] { 15, 15, 20, 20, 25 }, navyAtk: new float[] { 0, 0, 0, 0, 0 }, healthBonus: new float[] { 45, 50, 55, 60, 65 }, cost: new int[] { 1750, 850, 900, 950, 1050 }, movement: new int[] { 3, 3, 4, 4, 5 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.InfantryPanzer;
            break;
        case GeneralType.SSAirNavy:
            Assign(infAtk: new float[] { 15, 15, 20, 20, 25 }, armorAtk: new float[] { 20, 20, 25, 25, 25 }, artilleryAtk: new float[] { 20, 20, 25, 25, 30 }, airAtk: new float[] { 35, 40, 45, 45, 50 }, navyAtk: new float[] { 35, 40, 40, 45, 50 }, healthBonus: new float[] { 45, 50, 55, 60, 65 }, cost: new int[] { 1750, 850, 900, 950, 1050 }, movement: new int[] { 3, 3, 4, 4, 5 }, maxCmdSize: new int[] { 1, 1, 1, 1, 1 });
            skillBranch = GeneralBranch.AirNavy;
            break;
        }
    }
    void Assign(float[] infAtk, float[] armorAtk, float[] artilleryAtk, float[] airAtk, float[] navyAtk, float[] healthBonus, int[] maxCmdSize, int[] cost, int[] movement, string name = "") {
        this.infAtk = infAtk;
        this.armorAtk = armorAtk;
        this.artilleryAtk = artilleryAtk;
        this.airAtk = airAtk;
        this.navyAtk = navyAtk;
        this.healthBonus = healthBonus;
        this.maxCmdSize = maxCmdSize;
        this.cost = cost;

        if (perk1 != GeneralPerk.None) { //+$50 for second perk per level
            for (int i = 0; i < this.cost.Length; i++) {
                this.cost[i] += 50;
            }
        }
        if (perk2 != GeneralPerk.None) { //+$150 for second perk per level
            for (int i = 0; i < this.cost.Length; i++) {
                this.cost[i] += 150;
            }
        }
        if (perk3 != GeneralPerk.None) { //+$350 for third perk per level
            for (int i = 0; i < this.cost.Length; i++) {
                this.cost[i] += 350; 
            }
        }
        this.name = name;
        this.movement = movement;
    }
    public General() {
        this.infAtk = new float[] { 0 };
        this.armorAtk = new float[] { 0 };
        this.artilleryAtk = new float[] { 0 };
        this.airAtk = new float[] { 0 };
        this.navyAtk = new float[] { 0 };
        this.healthBonus = new float[] { 0 };
        this.maxCmdSize = new int[] { 0 };
        this.cost = new int[] { 0 };
        this.name = "";
        this.movement = new int[] { 0 };
    }

}
public static class BinaryPlayerSave {
    public static void SaveData(BinaryData2 data) {
        BinaryFormatter bf = new BinaryFormatter();

        //note: two files are used in case one is corrupted whilst in write mode
        using FileStream file = File.Create(MyPlayerPrefs.GetDataPath() + "/playerdata2.sav"); bf.Serialize(file, data);
        SaveBackupData(data);
    }
    public static void SaveBackupData(BinaryData2 data) {
        BinaryFormatter bf = new BinaryFormatter();

        //note: two files are used in case one is corrupted whilst in write mode
        using FileStream backupFile = File.Create(MyPlayerPrefs.GetDataPath() + "/playerdata2backup.sav"); bf.Serialize(backupFile, data);

    }
    //see if player has matching tech tree (if not, return new list with six zeroes)
    public static List<int> CheckTechDictionary(List<int> list) {
        if (list == null || list.Count != 6) {
            Debug.LogWarning("warning: tech tree information is not found");
            return new List<int>() { 0, 0, 0, 0, 0, 0 };
        }
        return list;
    }
    public static BinaryData2 LoadBackupData() {
        if (File.Exists(MyPlayerPrefs.GetDataPath() + "/playerdata2backup.sav")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(MyPlayerPrefs.GetDataPath() + "/playerdata2backup.sav", FileMode.Open);
            BinaryData2 data;
            try {
                data = bf.Deserialize(stream) as BinaryData2;
                data.techLevels = CheckTechDictionary(data.techLevels);
                stream.Close();
            } catch {
                stream.Close();
                Debug.LogWarning("backup error; NO SAVE FILE SO RESETTING PROGRESS");

                return new BinaryData2();
            }
            return data;

        } else {
            Debug.LogWarning("no backup found; SAVE IS PROBABLY ALREADY LOST");
            return new BinaryData2();
        }
    }
    public static BinaryData2 LoadData() {
        if (File.Exists(MyPlayerPrefs.GetDataPath() + "/playerdata2.sav")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(MyPlayerPrefs.GetDataPath() + "/playerdata2.sav", FileMode.Open);

            BinaryData2 data;
            
            try {
                data = bf.Deserialize(stream) as BinaryData2;
                data.techLevels = CheckTechDictionary(data.techLevels);

                string a = data.levelsUnlocked[0]; //test this to see if it is here

                stream.Close();
            } catch {
                stream.Close();
                Debug.LogWarning("data error, trying backup");

                return LoadBackupData();
            }
            //data retrieved successfully

            //since current save can be found, backup will be overridden to this save
            SaveBackupData(data);

            return data;
        } else if (File.Exists(MyPlayerPrefs.GetDataPath() + "/playerdata2backup.sav")) {
            return LoadBackupData();
        } else {
            MyPlayerPrefs.instance.SetFloat("realSounds", 0.3f);
            MyPlayerPrefs.instance.SetFloat("realMusics", 0.31f);
            Debug.Log("new data");
            try {
                if (Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseTraditional) {
                    MyPlayerPrefs.instance.SetString("language", "Chinese");
                }
                if (Application.systemLanguage == SystemLanguage.French) {
                    MyPlayerPrefs.instance.SetString("language", "French");
                }
                if (Application.systemLanguage == SystemLanguage.Spanish) {
                    MyPlayerPrefs.instance.SetString("language", "Spanish");
                }
                if (Application.systemLanguage == SystemLanguage.Japanese) {
                    MyPlayerPrefs.instance.SetString("language", "Japanese");
                }
                if (Application.systemLanguage == SystemLanguage.Russian) {
                    MyPlayerPrefs.instance.SetString("language", "Russian");
                }
                if (Application.systemLanguage == SystemLanguage.English) {
                    MyPlayerPrefs.instance.SetString("language", "English");
                }
            } catch {
                Debug.Log("can't get language");
            }
            return new BinaryData2();
        }
    }
}


[System.Serializable]
public class ExportBinaryData {
    public int money;
    public bool removedAds;

    //new tech levels
    public List<int> techLevels;

    public List<string> completedLevels;
    public List<string> generals; //name/level
    public List<int> generalsLevels;
    public List<string> generalsInUse;
    public List<int> generalsLevelsInUse;
    public List<string> levelsUnlocked;

    //player prefs saves
    public List<string> ppIntKeys, ppFloatKeys, ppStringKeys, ppStringValues;
    public List<int> ppIntValues;
    public List<float> ppFloatValues;

    //hashes money, generals, and levels unlocked to compare to make sure they weren't doctored
    public string hashValue;
    public ExportBinaryData(BinaryData2 bf, PlayerPrefsData pf) {
        if (bf == null) {
            Debug.LogError("no data found");
            return;
        }

        money = bf.money;
        techLevels = bf.techLevels;
        completedLevels = bf.completedLevels;
        generals = new List<string>(bf.generals.Keys);
        generalsLevels = new List<int>(bf.generals.Values);
        if (bf.generalsInUse != null) {
            generalsInUse = new List<string>(bf.generalsInUse.Keys);
            generalsLevelsInUse = new List<int>(bf.generalsInUse.Values);
        } else {
            generalsInUse = new List<string>();
            generalsLevelsInUse = new List<int>();
        }
        levelsUnlocked = bf.levelsUnlocked;
        hashValue = CalculateHash();
        try {
            ppIntKeys = new List<string>(pf.intDict.Keys);
            ppFloatKeys = new List<string>(pf.floatDict.Keys);
            ppStringKeys = new List<string>(pf.stringDict.Keys);
            ppIntValues = new List<int>(pf.intDict.Values);
            ppFloatValues = new List<float>(pf.floatDict.Values);
            ppStringValues = new List<string>(pf.stringDict.Values);
        } catch (System.Exception e) {
            Debug.LogWarning(e);
        }

    }
    string CalculateHash() {
        return Hash128.Compute((money + generals.Count + levelsUnlocked.Count).ToString() + System.DateTime.Now.DayOfYear).ToString();
    }
    public bool CheckJsonHash() {
        return hashValue == CalculateHash();
    }
}
[System.Serializable]
public class BinaryData2 {
    public int money;

    //new tech levels
    public List<int> techLevels;

    public bool removedAds;
    public List<string> completedLevels;
    public Dictionary<string, int> generals; //name/level
    public Dictionary<string, int> generalsInUse;
    public List<string> levelsUnlocked;
    
    public BinaryData2(ExportBinaryData bf) {
        money = bf.money;

        techLevels = bf.techLevels;

        completedLevels = bf.completedLevels;
        generals = bf.generals.Zip(bf.generalsLevels, (k, v) => new { k, v })
              .ToDictionary(x => x.k, x => x.v);
        generalsInUse = bf.generalsInUse.Zip(bf.generalsLevelsInUse, (k, v) => new { k, v })
              .ToDictionary(x => x.k, x => x.v);
        levelsUnlocked = bf.levelsUnlocked;
    }
    public BinaryData2() {
        removedAds = false;
        generals = new Dictionary<string, int>();
        generalsInUse = new Dictionary<string, int>();
        generalsInUse.Add("default", 2);
        money = 500;
        completedLevels = new List<string>();
        completedLevels.Add("");
        levelsUnlocked = new List<string>();
        levelsUnlocked.Add("Battle of Khalkhin Gol");
        levelsUnlocked.Add("Spanish Civil War");
        //levelsUnlocked.Add("Greek Civil War"); //note: make it so that first element is always accessable
        levelsUnlocked.Add("Europe 1939");
        levelsUnlocked.Add("Asia 1937");
        //Debug.Log("new save");
        techLevels = new List<int>() {0, 0, 0, 0, 0, 0};
    }

}
