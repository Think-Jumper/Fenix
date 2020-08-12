using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Shared.Protocol;
using System;
using Shared.DataModel;
using Server.DataModel;
using System.Collections.Generic;

namespace Server.UModule
{
    [ActorType(AType.SERVER)] 
    [AccessLevel(ALevel.CLIENT_AND_SERVER)]
    [PersistentData(typeof(User))]
    public partial class Avatar : Actor
    {
        public new Client.AvatarRef Client => (Client.AvatarRef)this.clientActor;

        public Avatar()
        {
            
        }

        public Avatar(string uid) : base(uid)
        {

        }

        protected override void onLoad()
        {
            
        }

        protected override void onClientEnable()
        {
            base.onClientEnable();

            //��ͻ��˷���Ϣ��ǰ���ǣ��Ѿ�����ClientAvatarRef,����һ��Actor��ClientRef����ȫ�ֿɼ��ģ�ֻ���ڸ�host�����ϵ���
            this.Client.client_on_api_test("hello", (code) =>
            {
                Log.Info("client_on_api_test", code);
            });
        }

        [ServerApi]
        public void ChangeName(string name, Action<ErrCode> callback)
        {
            Get<Account>().uid = name;

            callback(ErrCode.OK);
        }

        [ServerOnly]
        public void OnMatchOk()
        {

        }
    }
}
