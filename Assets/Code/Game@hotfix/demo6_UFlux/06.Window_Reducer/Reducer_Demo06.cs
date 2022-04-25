﻿using System;
using System.Net;
using System.Threading.Tasks;
using BDFramework.UFlux.Contains;
using BDFramework.UFlux.Reducer;
using LitJson;

namespace BDFramework.UFlux.Test
{
    /// <summary>
    /// Reducer 函数处理的集合
    /// </summary>
    public class Reducer_Demo06 : AReducers<Server_HeroData>
    {
        public enum Reducer06
        {
            //同步请求
            RequestHeroDataSynchronization,
            //异步请求
            RequestHeroDataAsync,
            //回调请求
            RequestHeroCallback,
        }
        
        /// <summary>
        /// 注册所有的Recucer
        /// </summary>
        public override void RegisterReducers()
        {
            base.RegisterReducers();
            //这里用显式注册，避免函数签名错误
            //同步方法
            this.AddRecucer(Reducer06.RequestHeroDataSynchronization, RequestServer);
            //异步方法
            this.AddAsyncRecucer(Reducer06.RequestHeroDataAsync, RequestServerByAsync);
            //回调需要考虑乱序问题
            this.AddCallbackReducer(Reducer06.RequestHeroCallback, RequestServerByCallback);
        }
        
        /// <summary>
        /// url
        /// </summary>
        readonly public string url = "https://1843236967254885.cn-shanghai.fc.aliyuncs.com/2016-08-15/proxy/BDFramework/DemoForUFlux/";

        /// <summary>
        /// 同步网络请求
        /// </summary>
        /// <param name="old"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private Server_HeroData RequestServer(Server_HeroData old, object @param)
        {
            var api = url + "api/bdframework/getherodata";
            WebClient  wc=new WebClient();
            string ret =  wc.DownloadString(api);
            var hero = JsonMapper.ToObject<Server_HeroData>(ret);
            return hero;
        }
        
        /// <summary>
        /// 异步网络请求
        /// </summary>
        /// <param name="old"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        async private Task<Server_HeroData>RequestServerByAsync(Server_HeroData old, object @param)
        {
            var api = url + "api/bdframework/getherodata";
            WebClient  wc=new WebClient();
            string ret = await  wc.DownloadStringTaskAsync(api);
            var hero = JsonMapper.ToObject<Server_HeroData>(ret);
            return hero;
        }
        

        
        /// <summary>
        /// 网络请求回调
        /// </summary>
        /// <param name="old"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private void RequestServerByCallback (Store<Server_HeroData>.GetState getStateFunc, object @params = null, Action<Server_HeroData> callback = null)
        {
            var api = url + "api/bdframework/getherodata";
            WebClient  wc=new WebClient();
            //提前注册回调
            wc.DownloadStringCompleted += (ret,e) =>
            {
                var hero = JsonMapper.ToObject<Server_HeroData>(ret.ToString());
                callback?.Invoke(hero);
            };
            //开始异步下载
            wc.DownloadStringAsync(new Uri(api));
        }
        
    }
}