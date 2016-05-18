/*
 */

using System;
using System.IO;
using System.Net.Sockets;
using OmniBean.PowerCrypt4;

namespace HackerKonsole.ConnectionServices
{
    /// <summary>
    ///     A virtualization layer over TcpClient that provides end-to-end encryption.
    /// </summary>
    public class CryptTcpClient
    {
        #region Public Constructors

        public CryptTcpClient(TcpClient existingTcpClient)
        {
            _tcpClient = existingTcpClient;
            _outputStream = new StreamWriter(new BufferedStream(_tcpClient.GetStream()));
            _inputStream = new StreamReader(_tcpClient.GetStream());
        }

        #endregion Public Constructors

        #region Public Fields

        public PowerRSA RSAKeyPair;
        public string SessionKey;

        #endregion Public Fields

        #region Private Fields

        private const int ChallengeHalfSize = 12;
        private const string KeyExchangeBanner = "<==KEY EXCHANGE==>";
        private const int KeyExchangeTimeoutMillis = 20000; //20s
        private const int RSAKeySize = 2048;
        private readonly StreamReader _inputStream;
        private readonly StreamWriter _outputStream;
        private readonly TcpClient _tcpClient;

        #endregion Private Fields

        #region Public Methods

        public void ClientPerformKeyExchange()
        {
            //Set reasonable timeouts
            SetReceiveTimeout(KeyExchangeTimeoutMillis);
            SetSendTimeout(KeyExchangeTimeoutMillis);
            const int retryCount = 3;
            var gotBanner = false;
            for (var tries = 0; tries <= retryCount; tries++)
            {
                var data = _inputStream.ReadLine();
                if (data == KeyExchangeBanner)
                {
                    gotBanner = true;
                    break;
                }
            }
            if (!gotBanner)
            {
                throw new ApplicationException("Remote host did not send the banner. Exceeded retry count of " +
                                               retryCount);
            }
            //Got banner, proceed
            var publicKey = _inputStream.ReadLine();
            RSAKeyPair = new PowerRSA(publicKey, RSAKeySize); //Server key pair (public)
            SessionKey = PowerAES.GenerateRandomString(64); //Generate Session Key
            var tempRsaKey = new PowerRSA(RSAKeySize); //my key pair (private/public)
            _outputStream.WriteLine(KeyExchangeBanner);
            _outputStream.WriteLine(tempRsaKey.PublicKey);
            _outputStream.Flush();

            var myChallenge = PowerAES.GenerateRandomString(ChallengeHalfSize) + "|" + PowerAES.GenerateRandomString(ChallengeHalfSize);
            var myChallengeHalves = myChallenge.Split('|');
            myChallenge = myChallenge.Replace("|", ""); //Remove the split character
            var theirChallengeHalf1 = tempRsaKey.DecryptStringWithPrivateKey(_inputStream.ReadLine());
            //get first piece of their challenge
            _outputStream.WriteLine(RSAKeyPair.EncryptStringWithPublicKey(myChallengeHalves[0]));
            //send first piece of challenge
            _outputStream.Flush();
            var theirChallengeHalf2 = tempRsaKey.DecryptStringWithPrivateKey(_inputStream.ReadLine());
            //get second piece of their challenge
            _outputStream.WriteLine(RSAKeyPair.EncryptStringWithPublicKey(myChallengeHalves[1]));
            //send second piece of challenge
            _outputStream.Flush();
            var theirChallenge = theirChallengeHalf1 + theirChallengeHalf2; //Piece together their challenge
            _outputStream.WriteLine(RSAKeyPair.EncryptStringWithPublicKey(theirChallenge)); //Send back their challenge
            _outputStream.Flush();
            var theChallengeTheySent = tempRsaKey.DecryptStringWithPrivateKey(_inputStream.ReadLine());
            if (theChallengeTheySent != myChallenge)
            {
                //Challenge validation failed.
                throw new ApplicationException("Challenge validation failed.");
            }
            //Use a hash of the challenge as the session key
            SessionKey = PowerAES.SHA512Hash(myChallenge);
        }

        public void Close()
        {
            _tcpClient.Close();
        }

        public void Flush()
        {
            _outputStream.Flush();
        }

        public void GenerateAsymmetricKeys()
        {
            RSAKeyPair = new PowerRSA(RSAKeySize); //Initialize RSA Key pair
        }

        /*
        public NetworkStream GetStream()
        {
            return _tcpClient.GetStream();
        }
        */

        public NetworkStream GetUnencryptedStream()
        {
            return _tcpClient.GetStream();
        }

        public string ReadLineCrypto()
        {
            var rawData = _inputStream.ReadLine();
            return PowerAES.Decrypt(rawData, SessionKey);
        }

        public void ServerPerformKeyExchange()
        {
            //Set reasonable timeouts
            SetReceiveTimeout(KeyExchangeTimeoutMillis);
            SetSendTimeout(KeyExchangeTimeoutMillis);
            _outputStream.WriteLine(KeyExchangeBanner);
            _outputStream.WriteLine(RSAKeyPair.PublicKey);
            _outputStream.Flush();
            const int retryCount = 3;
            var gotBanner = false;
            for (var tries = 0; tries <= retryCount; tries++)
            {
                var data = _inputStream.ReadLine();
                if (data == KeyExchangeBanner)
                {
                    gotBanner = true;
                    break;
                }
            }
            if (!gotBanner)
            {
                throw new ApplicationException("Remote host did not send the banner. Exceeded retry count of " +
                                               retryCount);
            }
            //Got banner, proceed
            var clientPubkey = new PowerRSA(_inputStream.ReadLine(), RSAKeySize); //their key (public)
            var myChallenge = PowerAES.GenerateRandomString(ChallengeHalfSize) + "|" + PowerAES.GenerateRandomString(ChallengeHalfSize);
            var myChallengeHalves = myChallenge.Split('|');
            myChallenge = myChallenge.Replace("|", ""); //Remove the split character
            _outputStream.WriteLine(clientPubkey.EncryptStringWithPublicKey(myChallengeHalves[0]));
            //send first piece of challenge
            _outputStream.Flush();
            var theirChallengeHalf1 = RSAKeyPair.DecryptStringWithPrivateKey(_inputStream.ReadLine());
            //get first piece of their challenge
            _outputStream.WriteLine(clientPubkey.EncryptStringWithPublicKey(myChallengeHalves[1]));
            //send second piece of challenge
            _outputStream.Flush();
            var theirChallengeHalf2 = RSAKeyPair.DecryptStringWithPrivateKey(_inputStream.ReadLine());
            //get second piece of their challenge
            var theirChallenge = theirChallengeHalf1 + theirChallengeHalf2; //Piece together their challenge
            _outputStream.WriteLine(clientPubkey.EncryptStringWithPublicKey(theirChallenge)); //Send back their challenge
            _outputStream.Flush();
            var theChallengeTheySent = RSAKeyPair.DecryptStringWithPrivateKey(_inputStream.ReadLine());
            if (theChallengeTheySent != myChallenge)
            {
                //Challenge validation failed.
                throw new ApplicationException("Challenge validation failed.");
            }
            //Use a hash of the challenge as the session key
            SessionKey = PowerAES.SHA512Hash(theirChallenge);
        }

        public void SetReceiveTimeout(int milliseconds)
        {
            _tcpClient.ReceiveTimeout = milliseconds;
        }

        public void SetSendTimeout(int milliseconds)
        {
            _tcpClient.SendTimeout = milliseconds;
        }

        public void WriteCrypto()
        {
        }

        public void WriteLineCrypto(string data)
        {
            string encryptedData = PowerAES.Encrypt(data, SessionKey);
            _outputStream.WriteLine(encryptedData);
            _outputStream.Flush();
        }
        #endregion Public Methods
    }
}