using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


#if UNITY_IOS || UNITY_TVOS
namespace Prime31
{
	public enum SKProductStorePromotionVisibility
	{
		Default,
		Show,
		Hide
	}

	public class StoreKitBinding
	{
		[DllImport("__Internal")]
		private static extern bool _storeKitCanMakePayments();

		/// <summary>
		/// Checks to see if the currently logged in user can make payments
		/// </summary>
		public static bool canMakePayments()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _storeKitCanMakePayments();
			return false;
		}


		[DllImport("__Internal")]
		private static extern void _storeKitSetApplicationUsername( string applicationUserName );

		/// <summary>
		/// This is used to help the store detect irregular activity.
		/// The recommended implementation is to use a one-way hash of the user's account name to calculate the value for this property.
		/// </summary>
	    public static void setApplicationUsername( string applicationUserName )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_storeKitSetApplicationUsername( applicationUserName );
	    }


	    [DllImport("__Internal")]
	    private static extern string _storeKitGetAppStoreReceiptUrl();

		/// <summary>
		/// Returns the location of the App Store receipt file. If called on an older iOS version it returns null.
		/// </summary>
	    public static string getAppStoreReceiptLocation()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _storeKitGetAppStoreReceiptUrl();

			return null;
	    }


		[DllImport("__Internal")]
		private static extern void _storeKitRefreshReceipt();

		/// <summary>
		/// Refreshes the app store receipt. Resuts in the refreshReceiptSucceeded/Failed event firing.
		/// </summary>
		public static void refreshReceipt()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_storeKitRefreshReceipt();
		}


		[DllImport("__Internal")]
		private static extern void _storeKitSendTransactionUpdateEvents( bool sendTransactionUpdateEvents );

		/// <summary>
		/// By default, the transactionUpdatedEvent will not be called to avoid excessive string allocations. If you pass true to this method it will be called.
		/// </summary>
		public static void setShouldSendTransactionUpdateEvents( bool sendTransactionUpdateEvents )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_storeKitSendTransactionUpdateEvents( sendTransactionUpdateEvents );
		}


		[DllImport("__Internal")]
		private static extern void _storeKitEnableHighDetailLogs( bool shouldEnable );

		/// <summary>
		/// Enables/disables high detail logs
		/// </summary>
		public static void enableHighDetailLogs( bool shouldEnable )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_storeKitEnableHighDetailLogs( shouldEnable );
		}


		[DllImport("__Internal")]
		private static extern void _storeKitRequestProductData( string productIdentifier );

		/// <summary>
		/// Accepts an array of product identifiers. All of the products you have for sale should be requested in one call.
		/// </summary>
	    public static void requestProductData( string[] productIdentifiers )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_storeKitRequestProductData( string.Join( ",", productIdentifiers ) );
	    }


	    [DllImport("__Internal")]
	    private static extern void _storeKitPurchaseProduct( string productIdentifier, int quantity );

		/// <summary>
		/// Purchases the given product and quantity
		/// </summary>
	    public static void purchaseProduct( string productIdentifier, int quantity )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_storeKitPurchaseProduct( productIdentifier, quantity );
	    }


	    [DllImport("__Internal")]
	    private static extern void _storeKitFinishPendingTransactions();

		/// <summary>
		/// Finishes any pending transactions that were being tracked
		/// </summary>
	    public static void finishPendingTransactions()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_storeKitFinishPendingTransactions();
	    }


	    [DllImport("__Internal")]
	    private static extern void _storeKitFinishPendingTransaction( string transactionIdentifier );

		/// <summary>
		/// Finishes the pending transaction identified by the transactionIdentifier
		/// </summary>
	    public static void finishPendingTransaction( string transactionIdentifier )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_storeKitFinishPendingTransaction( transactionIdentifier );
	    }


	    [DllImport("__Internal")]
	    private static extern void _storeKitPauseDownloads();

		/// <summary>
		/// Pauses any pending downloads
		/// </summary>
	    public static void pauseDownloads()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_storeKitPauseDownloads();
	    }


	    [DllImport("__Internal")]
	    private static extern void _storeKitResumeDownloads();

		/// <summary>
		/// Resumes any pending paused downloads
		/// </summary>
	    public static void resumeDownloads()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_storeKitResumeDownloads();
	    }


	    [DllImport("__Internal")]
	    private static extern void _storeKitCancelDownloads();

		/// <summary>
		/// Cancels any pending downloads
		/// </summary>
	    public static void cancelDownloads()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_storeKitCancelDownloads();
	    }


	    [DllImport("__Internal")]
	    private static extern void _storeKitRestoreCompletedTransactions();
	    [DllImport("__Internal")]
	    private static extern void _storeKitRestoreCompletedTransactionsWithApplicationUsername( string applicationUsername );

		/// <summary>
		/// Restores all previous transactions. This is used when a user gets a new device and they need to restore their old purchases.
		/// DO NOT call this on every launch. It will prompt the user for their password. Each transaction that is restored will have the normal
		/// purchaseSuccessfulEvent fire for when restoration is complete.
		/// The applicationUsername parameter should only be used in cases where setApplicationUsername has been called.
		/// </summary>
	    public static void restoreCompletedTransactions( string applicationUsername = null )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
			{
				if( applicationUsername != null )
					_storeKitRestoreCompletedTransactionsWithApplicationUsername( applicationUsername );
				else
					_storeKitRestoreCompletedTransactions();
			}
	    }


	    [DllImport("__Internal")]
	    private static extern string _storeKitGetAllSavedTransactions();

		/// <summary>
		/// Returns a list of all the transactions that occured on this device.  They are stored in the Document directory.
		/// </summary>
	    public static List<StoreKitTransaction> getAllSavedTransactions()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
			{
				// Grab the transactions and parse them out
				var json = _storeKitGetAllSavedTransactions();
				return StoreKitTransaction.transactionsFromJson( json );
			}

			return new List<StoreKitTransaction>();
	    }


	    [DllImport("__Internal")]
	    private static extern string _storeKitGetAllCurrentTransactions();

		/// <summary>
		/// Returns a list of all the transactions that are currently in the queue
		/// </summary>
	    public static List<StoreKitTransaction> getAllCurrentTransactions()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
			{
				// Grab the transactions and parse them out
				var json = _storeKitGetAllCurrentTransactions();
				return StoreKitTransaction.transactionsFromJson( json );
			}

			return new List<StoreKitTransaction>();
	    }


		[DllImport("__Internal")]
		private static extern void _storeKitDisplayStoreWithProductId( string productId, string affiliateToken );

		/// <summary>
		/// Displays the App Store with the given productId in app. The affiliateToken parameter will only work on iOS 8+.
		/// </summary>
		public static void displayStoreWithProductId( string productId, string affiliateToken = null )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_storeKitDisplayStoreWithProductId( productId, affiliateToken );
		}


#if UNITY_IOS
		[DllImport("__Internal")]
		private static extern void _storeKitAskForReview();
#endif

		/// <summary>
		/// iOS 10.3+ only! Displays the App Store review page or does nothing, depending on Apple's mood. There are no callbacks and no
		/// indications if this method does anything at all because that is how Apple operates.
		/// </summary>
		public static void askForReview()
		{
#if UNITY_IOS
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_storeKitAskForReview();
#endif
		}


		/// <summary>
		/// WARNING: Please do not do this on device! It is very, very easy to hack any on device receipt validation! This should be done on a secure server!
		/// useSandboxServers should be true when accessing the iTunes sandbox servers. password is the Shared Secret available in iTunes Connect. Returns a
		/// null StoreKitAppReceipt on request failure.
		/// </summary>
		public static IEnumerator validateReceipt( bool useSandboxServers, string password = null, Action<StoreKitAppReceipt> completionHandler = null )
		{
			var receiptPath = getAppStoreReceiptLocation().Replace( "file://", string.Empty );

			if( !System.IO.File.Exists( receiptPath ) )
			{
				Debug.LogError( "Receipt file does not exist on disk. Try refreshing the receipt first to ensure it is present." );
				if( completionHandler != null )
					completionHandler( null );
				yield break;
			}

			var receiptData = System.IO.File.ReadAllBytes( receiptPath );
			var receiptBase64 = System.Convert.ToBase64String( receiptData );

			// form the JSON for the request
			var dict = new Dictionary<string,object>();
			dict.Add( "receipt-data", receiptBase64 );

			if( password != null )
				dict.Add( "password", password );

			var json = Json.encode( dict );
			var postBytes = System.Text.ASCIIEncoding.UTF8.GetBytes( json );

			// use the proper URL based on if we are hitting the sandbox or not
			var www = new WWW( useSandboxServers ? "https://sandbox.itunes.apple.com/verifyReceipt" : "https://buy.itunes.apple.com/verifyReceipt", postBytes );
			yield return www;

			if( www.error == null )
			{
				// why must we dump the JSON to disk to get it to parse properly? Who knows.
				var receiptJsonFile = System.IO.Path.Combine( Application.persistentDataPath, "receipt.json" );
				System.IO.File.WriteAllText( receiptJsonFile, www.text );
				var receipt = Json.decode<StoreKitAppReceipt>( System.IO.File.ReadAllText( receiptJsonFile ) );
				System.IO.File.Delete( receiptJsonFile );

				if( completionHandler != null )
					completionHandler( receipt );
			}
			else
			{
				Debug.LogError( www.error );
				if( completionHandler != null )
					completionHandler( null );
			}
		}


		#region Promoted In App Purchases

	    [DllImport("__Internal")]
	    private static extern void _storeKitSetShouldDeferStorePayment( bool shouldDefer );

		/// <summary>
		/// When a promoted purchase occurs, this will control whether it is deferred or processed immediately. By default, all payments
		/// will be processed immediately unless you call this method with true for shouldDefer. You should call this on your games first
		/// launch so the value can be stored and used when needed.
		/// </summary>
	    public static void setShouldDeferStorePayment( bool shouldDefer )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_storeKitSetShouldDeferStorePayment( shouldDefer );
	    }


		[DllImport("__Internal")]
		private static extern bool _storeKitIsDeferredPaymentCached();

		/// <summary>
		/// Checks to see if their is a deferred payment cached that needs to be added to the payment queue
		/// </summary>
		public static bool isDeferredPaymentCached()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _storeKitIsDeferredPaymentCached();
			return false;
		}
		
		
		[DllImport("__Internal")]
		private static extern string _storeKitGetDeferredPaymentProductIdentifier();

		/// <summary>
		/// If a defrerred payment is available, returns the product identifier of the product else return snull.
		/// </summary>
		public static string getDeferredPaymentProductIdentifier()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _storeKitGetDeferredPaymentProductIdentifier();
			return null;
		}


		[DllImport("__Internal")]
		private static extern void _storeKitAddDeferredPaymentToQueue();

		/// <summary>
		/// If there is a deferred payment available, calling this method will add it to the payment queue.
		/// </summary>
		public static void addDeferredPaymentToQueue()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_storeKitAddDeferredPaymentToQueue();
		}

		#endregion


		#region SKProductStorePromotionController

		[DllImport("__Internal")]
		private static extern void _storeKitFetchStorePromotionVisibilityForProduct( string productIdentifier );

		/// <summary>
		/// iOS 11+ only! Fetches the store promotion visibility of a product. Results in the fetchStorePromotionVisibilitySucceeded/FailedEvent
		/// firing. Note that you must first call requestProductData for this to have a product to look up!
		/// </summary>
		public static void fetchStorePromotionVisibilityForProduct( string productIdentifier )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_storeKitFetchStorePromotionVisibilityForProduct( productIdentifier );
		}


		[DllImport("__Internal")]
		private static extern void _storeKitUpdateStorePromotionVisibilityForProduct( string productIdentifier, int visibility );

		/// <summary>
		/// iOS 11+ only! Updates the store promotion visibility of a product. Results in the updateStorePromotionVisibilitySucceeded/FailedEvent
		/// firing. Note that you must first call requestProductData for this to have a product to look up!
		/// </summary>
		public static void updateStorePromotionVisibilityForProduct( string productIdentifier, SKProductStorePromotionVisibility visibility )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_storeKitUpdateStorePromotionVisibilityForProduct( productIdentifier, (int)visibility );
		}

		#endregion

	}

}
#endif
