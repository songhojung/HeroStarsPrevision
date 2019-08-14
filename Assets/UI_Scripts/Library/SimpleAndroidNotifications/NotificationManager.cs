using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID && !UNITY_EDITOR

using System.Linq;

#endif

namespace Assets.SimpleAndroidNotifications
{
    public static class NotificationManager
    {
		private static int id = 0;
        #if UNITY_ANDROID && !UNITY_EDITOR

        private const string FullClassName = "com.hippogames.simpleandroidnotifications.Controller";
       // private const string MainActivityClassName = "com.unity3d.player.UnityPlayerActivity";
        private const string MainActivityClassName = "com.cle.dy.Suddenground.MainActivity";

		#endif

		//private static bool isNotiInit = false;
		public static void Init_Notification()
		{
#if UNITY_IOS
			if (isNotiInit == false) 
			{
				isNotiInit = true;
			
				UnityEngine.iOS.NotificationServices.RegisterForNotifications (
					UnityEngine.iOS.NotificationType.Alert |
					UnityEngine.iOS.NotificationType.Badge |
					UnityEngine.iOS.NotificationType.Sound
				);
				UserEditor.Getsingleton.EditLog("로컬푸시 초기화완료");
			}
#endif
		}


		/// <summary>
        /// Schedule simple notification without app icon.
        /// </summary>
        /// <param name="smallIcon">List of build-in small icons: notification_icon_bell (default), notification_icon_clock, notification_icon_heart, notification_icon_message, notification_icon_nut, notification_icon_star, notification_icon_warning.</param>
        public static int SendPush(TimeSpan delay, string title, string message, int id =  1,NotificationIcon smallIcon = NotificationIcon.sug)
        {
            return SendPushCustom(new NotificationParams
            {
				Id = id,
                Delay = delay,
                Title = title,
                Message = message,
                Ticker = message,
                Sound = true,
                Vibrate = true,
                Light = true,
                SmallIcon = smallIcon,
                SmallIconColor = DefineKey.Blue,
                LargeIcon = ""
            });
        }

        /// <summary>
        /// Schedule notification with app icon.
        /// </summary>
        /// <param name="smallIcon">List of build-in small icons: notification_icon_bell (default), notification_icon_clock, notification_icon_heart, notification_icon_message, notification_icon_nut, notification_icon_star, notification_icon_warning.</param>
        public static int SendPushWithAppIcon(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = 0)
        {
            return SendPushCustom(new NotificationParams
            {
                Id = UnityEngine.Random.Range(0, int.MaxValue),
                Delay = delay,
                Title = title,
                Message = message,
                Ticker = message,
                Sound = true,
                Vibrate = true,
                Light = true,
                SmallIcon = smallIcon,
                SmallIconColor = smallIconColor,
                LargeIcon = "app_icon"
            });
        }

        /// <summary>
        /// Schedule customizable notification.
        /// </summary>
        public static int SendPushCustom(NotificationParams notificationParams)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR

			UserEditor.Getsingleton.EditLog("START AOS LOCAL PUSH");

            var p = notificationParams;
            var delay = (long) p.Delay.TotalMilliseconds;
			NotificationManager.id	 = p.Id;

            new AndroidJavaClass(FullClassName).CallStatic("SetNotification", p.Id, delay, p.Title, p.Message, p.Ticker,
                p.Sound ? 1 : 0, p.Vibrate ? 1 : 0, p.Light ? 1 : 0, p.LargeIcon, GetSmallIconName(p.SmallIcon), ColotToInt(p.SmallIconColor), MainActivityClassName);

#elif UNITY_IOS
			//Init_Notification();
			UserEditor.Getsingleton.EditLog("START IOS LOCAL PUSH");
			
			UnityEngine.iOS.LocalNotification noti = new UnityEngine.iOS.LocalNotification();
			noti.alertAction = notificationParams.Title;
			noti.alertBody = notificationParams.Message;
			noti.applicationIconBadgeNumber = 0;

			IDictionary userInfo = new Dictionary<string, int>(1);
			userInfo["id"] = notificationParams.Id;
			noti.userInfo = userInfo;

			noti.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
			UserEditor.Getsingleton.EditLog("now : " + TimeManager.Instance.Get_nowTime());
			UserEditor.Getsingleton.EditLog("notificationParams.Delay : " + notificationParams.Delay);

			DateTime addingTime =DateTime.Now.Add(notificationParams.Delay);

			UserEditor.Getsingleton.EditLog("adding : " + addingTime);

			//DateTime a = new DateTime(addingTime.Year,addingTime.Month,addingTime.Hour,addingTime.Minute,addingTime.Second);
			noti.fireDate =  DateTime.Now;
			UserEditor.Getsingleton.EditLog("noti.fireDate11 : " + noti.fireDate);
			noti.fireDate = noti.fireDate.Add(notificationParams.Delay);
			UserEditor.Getsingleton.EditLog("noti.fireDate22: " + noti.fireDate);
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(noti);

#else



			Debug.LogWarning("Simple Android Notifications are not supported for current platform. Build and play this scene on android device!");

            #endif

            return notificationParams.Id;
        }

        /// <summary>
        /// Cancel notification by id.
        /// </summary>
        public static void Cancel(int id)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR

			UserEditor.Getsingleton.EditLog("AOS LOCAL PUSH CLEAR : " + id);

            new AndroidJavaClass(FullClassName).CallStatic("CancelScheduledNotification", id);

#elif UNITY_IOS
			UserEditor.Getsingleton.EditLog("UnityEngine.iOS.NotificationServices.localNotifications COUNT : " + UnityEngine.iOS.NotificationServices.localNotifications);
			foreach (var notis in UnityEngine.iOS.NotificationServices.localNotifications)
			{
				UserEditor.Getsingleton.EditLog("notis.userInfo.Count : " + notis.userInfo.Count);

				foreach (int item in notis.userInfo.Values)
				{
					UserEditor.Getsingleton.EditLog("item : " + item);
					if (id == item)
					{
						UserEditor.Getsingleton.EditLog("IOS PUSH CLEAR  : " + id);
						UnityEngine.iOS.NotificationServices.CancelLocalNotification(notis);
					}
				}
			}

#endif


		}

        /// <summary>
        /// Cancel all notifications.
        /// </summary>
        public static void CancelAll()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR

            new AndroidJavaClass(FullClassName).CallStatic("CancelAllScheduledNotifications");

			#elif UNITY_IOS
			UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
			UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
			#endif


		}

        private static int ColotToInt(Color color)
        {
            var smallIconColor = (Color32) color;
            
            return smallIconColor.r * 65536 + smallIconColor.g * 256 + smallIconColor.b;
        }

        private static string GetSmallIconName(NotificationIcon icon)
        {
            return "anp_" + icon.ToString().ToLower();
        }
    }
}