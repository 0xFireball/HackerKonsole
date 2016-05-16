/*
 */
using System;
using System.IO;
using OmniBean.PowerCrypt4;
using System.Net;
using System.Net.Sockets;

namespace HackerKonsole.ConnectionServices
{
	/// <summary>
	/// A virtualization layer over TcpClient that provides end-to-end encryption.
	/// </summary>
	public class CryptTcpClient
	{
		TcpClient _tcpClient;
		StreamWriter _outputStream;
		StreamReader _inputStream;
		
		public PowerRSA RSAKeyPair;
		public string SessionKey;
		
		private const int _keyExchangeTimeoutMillis = 5000;
		private const string _keyExchangeBanner = "<==KEY EXCHANGE==>";
		
		public CryptTcpClient(TcpClient existingTcpClient)
		{
			_tcpClient = existingTcpClient;
			_outputStream = new StreamWriter(new BufferedStream(_tcpClient.GetStream()));
			_inputStream = new StreamReader(_tcpClient.GetStream());
		}
		
		public void GenerateAsymmetricKeys()
		{
			RSAKeyPair = new PowerRSA(8192); //Initialize RSA Key pair
		}
		
		public void ServerPerformKeyExchange()
		{
			//Set reasonable timeouts
			SetReceiveTimeout(_keyExchangeTimeoutMillis);
			SetSendTimeout(_keyExchangeTimeoutMillis);
			_outputStream.WriteLine(_keyExchangeBanner);
			_outputStream.WriteLine(RSAKeyPair.PublicKey);
			const int retryCount = 3;
			bool gotBanner = false;
			for (int tries = 0; tries <= retryCount; tries++)
			{
				string data = _inputStream.ReadLine();
				if (data == _keyExchangeBanner)
				{
					gotBanner = true;
					break;
				}
			}
			if (!gotBanner)
			{
				throw new ApplicationException("Remote host did not send the banner. Exceeded retry count of "+retryCount.ToString());
			}
			//Got banner, proceed
			var clientPubkey = new PowerRSA(_inputStream.ReadLine(), 8192); //their key (public)
			string myChallenge = PowerAES.GenerateRandomString(64) + "|" + PowerAES.GenerateRandomString(64);
			string[] myChallengeHalves = myChallenge.Split('|');
			_outputStream.WriteLine(clientPubkey.EncryptStringWithPublicKey(myChallengeHalves[0])); //send first piece of challenge
			string theirChallengeHalf1 = RSAKeyPair.DecryptStringWithPrivateKey(_inputStream.ReadLine()); //get first piece of their challenge
			_outputStream.WriteLine(clientPubkey.EncryptStringWithPublicKey(myChallengeHalves[1])); //send second piece of challenge
			string theirChallengeHalf2 = RSAKeyPair.DecryptStringWithPrivateKey(_inputStream.ReadLine()); //get second piece of their challenge
			string theirChallenge = theirChallengeHalf1 + theirChallengeHalf2; //Piece together their challenge
			_outputStream.WriteLine(theirChallenge); //Send back their challenge
			string theChallengeTheySent = _inputStream.ReadLine();
			if (theChallengeTheySent != myChallenge)
			{
				//Challenge validation failed.
				throw new ApplicationException("Challenge validation failed.");
			}
		}
		
		public void ClientPerformKeyExchange()
		{
			//Set reasonable timeouts
			SetReceiveTimeout(_keyExchangeTimeoutMillis);
			SetSendTimeout(_keyExchangeTimeoutMillis);
			const int retryCount = 3;
			bool gotBanner = false;
			for (int tries = 0; tries <= retryCount; tries++)
			{
				string data = _inputStream.ReadLine();
				if (data == _keyExchangeBanner)
				{
					gotBanner = true;
					break;
				}
			}
			if (!gotBanner)
			{
				throw new ApplicationException("Remote host did not send the banner. Exceeded retry count of "+retryCount.ToString());
			}
			//Got banner, proceed
			string publicKey = _inputStream.ReadLine();
			RSAKeyPair = new PowerRSA(publicKey, 8192); //Server key pair (public)
			SessionKey = PowerAES.GenerateRandomString(64); //Generate Session Key
			var tempRsaKey = new PowerRSA(8192); //my key pair (private/public)
			_outputStream.WriteLine(_keyExchangeBanner);
			_outputStream.WriteLine(tempRsaKey.PublicKey);
			
			string myChallenge = PowerAES.GenerateRandomString(64) + "|" + PowerAES.GenerateRandomString(64);
			string[] myChallengeHalves = myChallenge.Split('|');
			string theirChallengeHalf1 = RSAKeyPair.DecryptStringWithPrivateKey(_inputStream.ReadLine()); //get first piece of their challenge
			_outputStream.WriteLine(RSAKeyPair.EncryptStringWithPublicKey(myChallengeHalves[0])); //send first piece of challenge
			string theirChallengeHalf2 = RSAKeyPair.DecryptStringWithPrivateKey(_inputStream.ReadLine()); //get second piece of their challenge
			_outputStream.WriteLine(RSAKeyPair.EncryptStringWithPublicKey(myChallengeHalves[1])); //send second piece of challenge
			string theirChallenge = theirChallengeHalf1 + theirChallengeHalf2; //Piece together their challenge
			_outputStream.WriteLine(theirChallenge); //Send back their challenge
			string theChallengeTheySent = _inputStream.ReadLine();
			if (theChallengeTheySent != myChallenge)
			{
				//Challenge validation failed.
				throw new ApplicationException("Challenge validation failed.");
			}
		}
		
		public NetworkStream GetStream()
		{
			return _tcpClient.GetStream();
		}
		public void SetReceiveTimeout(int milliseconds)
		{
			_tcpClient.ReceiveTimeout = milliseconds;
		}
		public void SetSendTimeout(int milliseconds)
		{
			_tcpClient.SendTimeout = milliseconds;
		}
		public void Close()
		{
			_tcpClient.Close();
		}
	}
}
