using PubnubApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWebapi
{
    public static class PubNub
    {

        public static void Initialise(Pubnub objPubnub)
        {
            //PNConfiguration pnConfiguration = new PNConfiguration();
            //pnConfiguration.SubscribeKey = "sub-c-f956c704-63f5-11e8-90b6-8e3ee2a92f04";
            //pnConfiguration.PublishKey = "pub-c-ee31281d-c11a-4bd8-895f-cca3f587ce5a";
            //pnConfiguration.SecretKey = "sec-c-ZDMzZWY3MGUtNzExZi00ZDA3LWFiMTktYTFmMzMwOTVjZTc3";
            //pnConfiguration.LogVerbosity = PNLogVerbosity.BODY;
            //pnConfiguration.Uuid = "PubNubCSharpExample";
            //objPubnub = new Pubnub(pnConfiguration);

            //Dictionary<string, string> message = new Dictionary<string, string>();
            //message.Add("Quick Health", "Quick Health");


            //SubscribeCallbackExt subscribeCallback = new SubscribeCallbackExt(
            //        (pubnubObj, messageResult) => {
            //            if (messageResult != null)
            //            {
            //                System.Diagnostics.Debug.WriteLine("In Example, SusbcribeCallback received PNMessageResult");
            //                System.Diagnostics.Debug.WriteLine("In Example, SusbcribeCallback messsage channel = " + messageResult.Channel);
            //                string jsonString = messageResult.Message.ToString();
            //                Dictionary<string, string> msg = objPubnub.JsonPluggableLibrary.DeserializeToObject<Dictionary<string, string>>(jsonString);
            //                System.Diagnostics.Debug.WriteLine("msg: " + msg["msg"]);
            //            }
            //        },
            //        (pubnubObj, presencResult) => {
            //            if (presencResult != null)
            //            {
            //                System.Diagnostics.Debug.WriteLine("In Example, SusbcribeCallback received PNPresenceEventResult");
            //                System.Diagnostics.Debug.WriteLine(presencResult.Channel + " " + presencResult.Occupancy + " " + presencResult.Event);
            //            }
            //        },
            //        (pubnubObj, statusResult) => {
            //            if (statusResult.Category == PNStatusCategory.PNConnectedCategory)
            //            {
            //                objPubnub.Publish()
            //                .Channel("my_channel")
            //                .Message(message)
            //                .Async(new PNPublishResultExt((publishResult, publishStatus) => {
            //                    if (!publishStatus.Error)
            //                    {
            //                        System.Diagnostics.Debug.WriteLine(string.Format("DateTime {0}, In Publish Example, Timetoken: {1}", DateTime.UtcNow, publishResult.Timetoken));
            //                    }
            //                    else
            //                    {
            //                        System.Diagnostics.Debug.WriteLine(publishStatus.Error);
            //                        System.Diagnostics.Debug.WriteLine(publishStatus.ErrorData.Information);
            //                    }
            //                }));
            //            }
            //        }
            //    );

            //objPubnub.AddListener(subscribeCallback);

            //objPubnub.Subscribe<string>()
            //    .Channels(new string[]{
            //"TEST:QUICK_HEALTH"
            //    }).Execute();


            //objPubnub.History()
            //.Channel("TEST:QUICK_HEALTH")
            //.Count(100)
            //.Async(new PNHistoryResultExt(
            //    (result, status) => {
            //    }
            //));
            //objPubnub.Publish();






            PNConfiguration pnConfiguration = new PNConfiguration();
            pnConfiguration.SubscribeKey = "sub-c-f956c704-63f5-11e8-90b6-8e3ee2a92f04";
            pnConfiguration.PublishKey = "pub-c-ee31281d-c11a-4bd8-895f-cca3f587ce5a";
            pnConfiguration.SecretKey = "sec-c-ZDMzZWY3MGUtNzExZi00ZDA3LWFiMTktYTFmMzMwOTVjZTc3";
            Pubnub pubnub = new Pubnub(pnConfiguration);
            pubnub.Subscribe<string>()
            .Channels(new string[] {
// subscribe to channels
"TEST:QUICK_HEALTH"
            })
            .Execute();
            string[] arrayMessage = new string[] {
"Hello",
"world!"
};
            pubnub.Publish()
            .Message(arrayMessage.ToList())
            .Channel("TEST:QUICK_HEALTH")
            .ShouldStore(true)
            .UsePOST(true)
            .Async(new PNPublishResultExt(
            (result, status) => {
    // Check whether request successfully completed or not.
    if (status.Error)
                {
        // something bad happened.
        Console.WriteLine("error happened while publishing: " + pubnub.JsonPluggableLibrary.SerializeToJsonString(status));
                }
                else
                {
                    Console.WriteLine("publish worked! timetoken: " + result.Timetoken.ToString());
                }
            }
            ));









        }// class ending bracket
    }
}