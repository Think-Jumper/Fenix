
/*
 * (c)2020 Sekkit.com
 * Fenix��һ������Actor����ģ�͵ķֲ�ʽ��Ϸ������
 * server��ͨ�Ŷ�����tcp
 * server/client֮�������tcp/kcp/websockets
 */
 
using Fenix;
using Fenix.Config;  
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Server.Config;
using CommandLine;
using MessagePack.Resolvers;
using MessagePack;

namespace Server
{
    public class Options
    {
        [Option('a', "AppName", Required = false, HelpText = "AppName")]
        public string AppName { get; set; }

        [Option('c', "Config", Required = false, HelpText = "Config")]
        public string Config { get; set; }
    }

    class App
    { 
        static void Main(string[] args)
        {
            StaticCompositeResolver.Instance.Register(
                 MessagePack.Resolvers.ClientAppResolver.Instance,
                 MessagePack.Resolvers.FenixRuntimeResolver.Instance,
                 MessagePack.Resolvers.SharedResolver.Instance,
                 MessagePack.Unity.UnityResolver.Instance,
                 MessagePack.Unity.Extension.UnityBlitResolver.Instance,
                 MessagePack.Unity.Extension.UnityBlitWithPrimitiveArrayResolver.Instance,
                 MessagePack.Resolvers.StandardResolver.Instance
            );

            if (args.Length == 0)
            {
                var cfgList = new List<RuntimeConfig>();

                var obj = new RuntimeConfig();
                obj.ExternalIP = "auto";
                obj.InternalIP = "auto";
                obj.Port = 17777; //auto
                obj.AppName = "Login.App";
                obj.HeartbeatIntervalMS = 5000;
                obj.ClientNetwork = NetworkType.TCP;
                obj.DefaultActorNames = new List<string>()
                {
                    "LoginService"
                };

                cfgList.Add(obj);

                obj = new RuntimeConfig();
                obj.ExternalIP = "auto";
                obj.InternalIP = "auto";
                obj.Port = 17778; //auto
                obj.AppName = "Match.App";
                obj.HeartbeatIntervalMS = 5000;
                obj.ClientNetwork = NetworkType.TCP;
                obj.DefaultActorNames = new List<string>()
                {
                    "MatchService"
                };

                cfgList.Add(obj);

                obj = new RuntimeConfig();
                obj.ExternalIP = "auto";
                obj.InternalIP = "auto";
                obj.Port = 17779; //auto
                obj.AppName = "Master.App";
                obj.HeartbeatIntervalMS = 5000;
                obj.ClientNetwork = NetworkType.TCP;
                obj.DefaultActorNames = new List<string>()
                {
                    "MasterService"
                };

                cfgList.Add(obj);

                obj = new RuntimeConfig();
                obj.ExternalIP = "auto";
                obj.InternalIP = "auto";
                obj.Port = 17780; //auto
                obj.AppName = "Zone.App";
                obj.HeartbeatIntervalMS = 5000;
                obj.ClientNetwork = NetworkType.TCP;
                obj.DefaultActorNames = new List<string>()
                {
                    "ZoneService"
                };

                cfgList.Add(obj);
                
                using (var sw = new StreamWriter("app.json", false, Encoding.UTF8))
                {
                    var content = JsonConvert.SerializeObject(cfgList, Formatting.Indented);
                    sw.Write(content);
                }
                
                //for Debug purpose
                Bootstrap.StartSingleProcess(new Assembly[] { typeof(UModule.Avatar).Assembly }, cfgList, OnInit); //������ģʽ
            }
            else
            {
                Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       //�������в��������õ����̵Ļ�������
                       Environment.SetEnvironmentVariable("AppName", o.AppName);

                       using (var sr = new StreamReader(o.Config))
                       {
                           var cfgList = JsonConvert.DeserializeObject<List<RuntimeConfig>>(sr.ReadToEnd());
                           foreach (var cfg in cfgList)
                               if(cfg.AppName == o.AppName)
                                   Bootstrap.StartMultiProcess(new Assembly[] { typeof(UModule.Avatar).Assembly }, cfg, OnInit); //�ֲ�ʽ
                       }
                   });
            }
        }

        static void OnInit()
        {
            DbConfig.Init();

            Global.DbManager.LoadDb(DbConfig.account_db);
            Global.DbManager.LoadDb(DbConfig.seq_db);
        }
    }
}
