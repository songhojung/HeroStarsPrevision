using UnityEngine;
using System;
using System.Collections.Generic;
using Prime31;


#if UNITY_IOS || UNITY_TVOS

namespace Prime31
{
	public class StoreKitManager : AbstractManager
	{
		public static bool autoConfirmTransactions = true;


		/// <summary>
		/// Fired when a call to refreshReceipt succeeds
		/// </summary>
		public static event Action refreshReceiptSucceededEvent;

		/// <summary>
		/// Fired when a call to refreshReceipt fails
		/// </summary>
		public static event Action<string> refreshReceiptFailedEvent;

		/// <summary>
		/// Fired when the product list your required returns. Automatically serializes the productString into StoreKitProduct's.
		/// </summary>
		public static event Action<List<StoreKitProduct>> productListReceivedEvent;

		/// <summary>
		/// Fired when requesting product data fails
		/// </summary>
		public static event Action<string> productListRequestFailedEvent;

		/// <summary>
		/// Fired anytime Apple updates a transaction if you called setShouldSendTransactionUpdateEvents with true. Check the transaction.transactionState to
		/// know what state the transaction is currently in.
		/// </summary>
		public static event Action<StoreKitTransaction> transactionUpdatedEvent;

		/// <summary>
		/// Fired when a product purchase has returned from Apple's servers and is awaiting completion. By default the plugin will finish transactions for you.
		/// You can change that behavior by setting autoConfirmTransactions to false which then requires that you call StoreKitBinding.finishPendingTransaction
		/// to complete a purchase.
		/// </summary>
		public static event Action<StoreKitTransaction> productPurchaseAwaitingConfirmationEvent;

		/// <summary>
		/// Fired when a product is successfully paid for. The event will provide a StoreKitTransaction object that holds the productIdentifer and receipt of the purchased product.
		/// </summary>
		public static event Action<StoreKitTransaction> purchaseSuccessfulEvent;

		/// <summary>
		/// Fired when a product purchase fails. This differs from the purchaseFailedEvent in that it returns a full P31Error object as opposed to just the error message.
		/// </summary>
		public static event Action<P31Error> purchaseErrorEvent;

		/// <summary>
		/// Fired when a product purchase fails
		/// </summary>
		public static event Action<string> purchaseFailedEvent;

		/// <summary>
		/// Fired when a product purchase is cancelled by the user or system
		/// </summary>
		public static event Action<string> purchaseCancelledEvent;

		/// <summary>
		/// Fired when all transactions from the user's purchase history have successfully been added back to the queue. Note that this event will almost always
		/// fire before each individual transaction is processed.
		/// </summary>
		public static event Action restoreTransactionsFinishedEvent;

		/// <summary>
		/// Fired when an error is encountered while adding transactions from the user's purchase history back to the queue
		/// </summary>
		public static event Action<string> restoreTransactionsFailedEvent;

		/// <summary>
		/// Fired when any SKDownload objects are updated by Apple. If using hosted content you should not be confirming the transaction until all downloads are complete.
		/// </summary>
		public static event Action<List<StoreKitDownload>> paymentQueueUpdatedDownloadsEvent;

		/// <summary>
		/// Fired when a call to fetchStorePromotionVisibilityForProduct succeeds. Includes the product identifier and visibility.
		/// </summary>
		public static event Action<string,SKProductStorePromotionVisibility> fetchStorePromotionVisibilitySucceededEvent;

		/// <summary>
		/// Fired when a call to fetchStorePromotionVisibilityForProduct fails. Includes the error message.
		/// </summary>
		public static event Action<string> fetchStorePromotionVisibilityFailedEvent;

		/// <summary>
		/// Fired when a call to updateStorePromotionVisibilityForProduct succeeds. Includes the product identifier.
		/// </summary>
		public static event Action<string> updateStorePromotionVisibilitySucceededEvent;

		/// <summary>
		/// Fired when a call to updateStorePromotionVisibilityForProduct fails. Includes the error message.
		/// </summary>
		public static event Action<string> updateStorePromotionVisibilityFailedEvent;



	    static StoreKitManager()
	    {
			AbstractManager.initialize( typeof( StoreKitManager ) );

			// we ignore the results of this call because our only purpose is to trigger the creation of the required listener on the native side for transaction processing.
			StoreKitBinding.canMakePayments();
	    }



		void refreshReceiptSucceeded( string empty )
		{
			if( refreshReceiptSucceededEvent != null )
				refreshReceiptSucceededEvent();
		}


		void refreshReceiptFailed( string error )
		{
			if( refreshReceiptFailedEvent != null )
				refreshReceiptFailedEvent( error );
		}


		void transactionUpdated( string json )
		{
			if( transactionUpdatedEvent != null )
				transactionUpdatedEvent( StoreKitTransaction.transactionFromJson( json ) );
		}


		void productPurchaseAwaitingConfirmation( string json )
		{
			if( productPurchaseAwaitingConfirmationEvent != null )
				productPurchaseAwaitingConfirmationEvent( StoreKitTransaction.transactionFromJson( json ) );

			if( autoConfirmTransactions )
				StoreKitBinding.finishPendingTransactions();
		}


		void productPurchased( string json )
		{
			if( purchaseSuccessfulEvent != null )
				purchaseSuccessfulEvent( StoreKitTransaction.transactionFromJson( json ) );
		}


		void productPurchaseError( string json )
		{
			if( purchaseErrorEvent != null )
				purchaseErrorEvent( P31Error.errorFromJson( json ) );
		}


		void productPurchaseFailed( string error )
		{
			if( purchaseFailedEvent != null )
				purchaseFailedEvent( error );
		}


		void productPurchaseCancelled( string error )
		{
			if( purchaseCancelledEvent != null )
				purchaseCancelledEvent( error );
		}


		void productsReceived( string json )
		{
			if( productListReceivedEvent != null )
				productListReceivedEvent( StoreKitProduct.productsFromJson( json ) );
		}


		void productsRequestDidFail( string error )
		{
			if( productListRequestFailedEvent != null )
				productListRequestFailedEvent( error );
		}


		void restoreCompletedTransactionsFailed( string error )
		{
			if( restoreTransactionsFailedEvent != null )
				restoreTransactionsFailedEvent( error );
		}


		void restoreCompletedTransactionsFinished( string empty )
		{
			if( restoreTransactionsFinishedEvent != null )
				restoreTransactionsFinishedEvent();
		}


		void paymentQueueUpdatedDownloads( string json )
		{
			if( paymentQueueUpdatedDownloadsEvent != null )
				paymentQueueUpdatedDownloadsEvent( StoreKitDownload.downloadsFromJson( json ) );

		}


		void fetchStorePromotionVisibilitySucceeded( string json )
		{
			if( fetchStorePromotionVisibilitySucceededEvent != null )
			{
				var dict = Json.decode<Dictionary<string,object>>( json );
				var visibility = int.Parse( dict["visibility"].ToString() );
				fetchStorePromotionVisibilitySucceededEvent( dict["productId"].ToString(), (SKProductStorePromotionVisibility)visibility );
			}
		}


		void fetchStorePromotionVisibilityFailed( string error )
		{
			if( fetchStorePromotionVisibilityFailedEvent != null )
				fetchStorePromotionVisibilityFailedEvent( error );
		}


		void updateStorePromotionVisibilitySucceeded( string productId )
		{
			if( updateStorePromotionVisibilitySucceededEvent != null )
				updateStorePromotionVisibilitySucceededEvent( productId );
		}


		void updateStorePromotionVisibilityFailed( string error )
		{
			if( updateStorePromotionVisibilityFailedEvent != null )
				updateStorePromotionVisibilityFailedEvent( error );
		}

	}

}
#endif
