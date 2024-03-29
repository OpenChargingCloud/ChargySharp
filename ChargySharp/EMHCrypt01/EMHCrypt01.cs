﻿/*
 * Copyright (c) 2018-2019 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of ChargySharp <https://github.com/OpenChargingCloud/ChargySharp>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;
using Org.BouncyCastle.Asn1;

#endregion

namespace cloud.charging.apis.chargy
{

    public class EMHCrypt01 : ACrypt
    {

        #region Data

        public String              CurveName    { get; }
        public X9ECParameters      ECP          { get; }
        public ECDomainParameters  ECSpec       { get; }
        public FpCurve             C            { get; }

        #endregion

        #region Constructor(s)

        public EMHCrypt01(GetMeterDelegate                      GetMeter,
                          CheckMeterPublicKeySignatureDelegate  CheckMeterPublicKeySignature)

            : base("ECC secp192r1",
                   GetMeter,
                   CheckMeterPublicKeySignature)

        {

            this.CurveName  = "P-192";
            this.ECP        = ECNamedCurveTable.GetByName(CurveName);
            this.ECSpec     = new ECDomainParameters(ECP.Curve, ECP.G, ECP.N, ECP.H, ECP.GetSeed());
            this.C          = (FpCurve) ECSpec.Curve;

        }

        #endregion


        #region GenerateKeyPairs()

        public ECKeyPair GenerateKeyPairs()
        {

            var generator = GeneratorUtilities.GetKeyPairGenerator("ECDH");
            generator.Init(new ECKeyGenerationParameters(ECSpec, new SecureRandom()));

            var keyPair = generator.GenerateKeyPair();

            return new ECKeyPair(keyPair.Private as ECPrivateKeyParameters,
                                 keyPair.Public  as ECPublicKeyParameters);

        }

        #endregion

        #region ParsePrivateKey(PrivateKey)

        public ECPrivateKeyParameters ParsePrivateKey(Byte[] PrivateKey)

            => new ECPrivateKeyParameters(new BigInteger(1, PrivateKey), ECSpec);


        public ECPrivateKeyParameters ParsePrivateKey(String PrivateKey)

            => new ECPrivateKeyParameters(new BigInteger(PrivateKey, 16), ECSpec);

        #endregion

        #region ParsePublicKey (PublicKey, PublicKeyFormat = PublicKeyFormats.DER)

        public ECPublicKeyParameters ParsePublicKey(Byte[]            PublicKey,
                                                    PublicKeyFormats  PublicKeyFormat = PublicKeyFormats.DER)
        {

            switch (PublicKeyFormat)
            {

                case PublicKeyFormats.DER:
                    return new ECPublicKeyParameters("ECDSA", ECP.Curve.DecodePoint(PublicKey), ECSpec);

                case PublicKeyFormats.plain:
                    var c           = (FpCurve) ECSpec.Curve;
                    //var x           = c.FromBigInteger(new BigInteger(PublicKey.SubSequence(0, 24)));
                    //var y           = c.FromBigInteger(new BigInteger(PublicKey.SubSequence(   24)));
                    var _PublicKey  = PublicKey.ToHexString();
                    var x           = c.FromBigInteger(new BigInteger(_PublicKey.Substring(0, 48), 16));
                    var y           = c.FromBigInteger(new BigInteger(_PublicKey.Substring(   48), 16));
                    var q           = new FpPoint(ECP.Curve, x, y);
                    var isv         = q.IsValid();
                    return new ECPublicKeyParameters("ECDH", q, SecObjectIdentifiers.SecP192r1);

            }

            return null;

        }

        public ECPublicKeyParameters ParsePublicKey(String            PublicKey,
                                                    PublicKeyFormats  PublicKeyFormat = PublicKeyFormats.DER)
        {

            switch (PublicKeyFormat)
            {

                case PublicKeyFormats.DER:
                    return new ECPublicKeyParameters("ECDSA", ECP.Curve.DecodePoint(PublicKey.HexStringToByteArray()), ECSpec);

                case PublicKeyFormats.plain:
                    var c           = (FpCurve) ECSpec.Curve;
                    var x           = c.FromBigInteger(new BigInteger(PublicKey.Substring(0, 48), 16));
                    var y           = c.FromBigInteger(new BigInteger(PublicKey.Substring(   48), 16));
                    var q           = new FpPoint(ECP.Curve, x, y);
                    var isv         = q.IsValid();
                    return new ECPublicKeyParameters("ECDH", q, SecObjectIdentifiers.SecP192r1);

            }

            return null;

        }

        #endregion

        #region ParsePublicKey (X, Y)

        public ECPublicKeyParameters ParsePublicKey(Byte[] X, Byte[] Y)
        {

            var c = (FpCurve) ECSpec.Curve;
            var q = new FpPoint(ECP.Curve,
                                c.FromBigInteger(new BigInteger(X)),
                                c.FromBigInteger(new BigInteger(Y)));

            return q.IsValid()
                       ? new ECPublicKeyParameters("ECDH", q, SecObjectIdentifiers.SecP192r1)
                       : null;

        }

        public ECPublicKeyParameters ParsePublicKey(String X, String Y)
        {

            var c = (FpCurve) ECSpec.Curve;
            var q = new FpPoint(ECP.Curve,
                                c.FromBigInteger(new BigInteger(X, 16)),
                                c.FromBigInteger(new BigInteger(Y, 16)));

            return q.IsValid()
                       ? new ECPublicKeyParameters("ECDH", q, SecObjectIdentifiers.SecP192r1)
                       : null;

        }

        #endregion


        #region SignChargingSession(ChargingSession,  PrivateKey, SignatureFormat = SignatureFormats.DER)

        public override ISignResult SignChargingSession(IChargingSession  ChargingSession,
                                                        Byte[]            PrivateKey,
                                                        SignatureFormats  SignatureFormat = SignatureFormats.DER)
        {
            return null;
        }

        #endregion

        #region SignMeasurement    (MeasurementValue, PrivateKey, SignatureFormat = SignatureFormats.DER)

        public override ISignResult SignMeasurement(IMeasurementValue2  MeasurementValue,
                                                    Byte[]              PrivateKey,
                                                    SignatureFormats    SignatureFormat = SignatureFormats.DER)
        {

            try
            {

                #region Check MeasurementValue

                if (MeasurementValue == null)
                    return new EMHSignResult(Status: SignResult.InvalidMeasurementValue);

                if (!(MeasurementValue is IEMHMeasurementValue2 EMHMeasurementValue))
                    return new EMHSignResult(Status: SignResult.InvalidMeasurementValue);

                #endregion

                #region Parse PrivateKey

                ECPrivateKeyParameters EMHPrivateKey = null;

                try
                {
                    EMHPrivateKey  = ParsePrivateKey(PrivateKey);
                }
                catch (Exception e)
                {
                    return new EMHSignResult(Status:        SignResult.InvalidPublicKey,
                                             ErrorMessage:  e.Message);
                }

                #endregion


                var cryptoBuffer  = new Byte[320];

                var signResult    = new EMHSignResult(

                    MeterId:                      cryptoBuffer.SetHex        (EMHMeasurementValue.Measurement.MeterId,                                        0),
                    Timestamp:                    cryptoBuffer.SetTimestamp32(EMHMeasurementValue.Timestamp,                                                 10),
                    InfoStatus:                   cryptoBuffer.SetHex        (EMHMeasurementValue.InfoStatus,                                                14, false),
                    SecondsIndex:                 cryptoBuffer.SetUInt32     (EMHMeasurementValue.SecondsIndex,                                              15, true),
                    PaginationId:                 cryptoBuffer.SetHex        (EMHMeasurementValue.PaginationId,                                              19, true),
                    OBIS:                         cryptoBuffer.SetHex        (EMHMeasurementValue.Measurement.OBIS,                                          23, false),
                    UnitEncoded:                  cryptoBuffer.SetInt8       (EMHMeasurementValue.Measurement.UnitEncoded,                                   29),
                    Scale:                        cryptoBuffer.SetInt8       (EMHMeasurementValue.Measurement.Scale,                                         30),
                    Value:                        cryptoBuffer.SetUInt64     (EMHMeasurementValue.Value,                                                     31, true),
                    LogBookIndex:                 cryptoBuffer.SetHex        (EMHMeasurementValue.LogBookIndex,                                              39, false),
                    AuthorizationStart:           cryptoBuffer.SetText       (EMHMeasurementValue.Measurement.ChargingSession.AuthorizationStart.Id,         41),
                    AuthorizationStartTimestamp:  cryptoBuffer.SetTimestamp32(EMHMeasurementValue.Measurement.ChargingSession.AuthorizationStart.Timestamp, 169),

                    Status:                       SignResult.InvalidMeasurementValue);


                var SHA256Hash = new Byte[0];

                // Only the first 24 bytes/192 bits are used!
                using (var SHA256 = new SHA256Managed())
                {

                    SHA256Hash = SHA256.ComputeHash(cryptoBuffer);

                    signResult.SHA256Value = SHA256Hash.ToHexString().
                                                        Substring(0, 48);

                }

                var meter = GetMeter(MeasurementValue.Measurement.MeterId);
                if (meter != null)
                {

                    signResult.SetMeter(meter);

                    var publicKey = meter.PublicKeys.FirstOrDefault();
                    if (publicKey != null && (publicKey.Value?.Trim().IsNotNullOrEmpty() == true))
                    {

                        try
                        {

                            var EMHPublicKey = ParsePublicKey(publicKey.Value?.Trim());

                            //var bVerifier   = SignerUtilities.GetSigner("SHA-256withECDSA");
                            var signer      = SignerUtilities.GetSigner("NONEwithECDSA");
                            signer.Init(true, EMHPrivateKey);
                            signer.BlockUpdate(SHA256Hash, 0, 24);

                            var signature = new EMHSignature(signer.AlgorithmName,
                                                             SignatureFormat, // ToDo: Fix signature format selection!
                                                             signer.GenerateSignature().ToHexString());

                            MeasurementValue.Signatures.Add(signature);
                            signResult.SetSignatureValue(SignResult.OK, signature);

                        }
                        catch (Exception e)
                        {
                            signResult.SetError(Status:        SignResult.InvalidPublicKey,
                                                ErrorMessage:  e.Message);
                        }

                    }
                    else
                        signResult.SetStatus(SignResult.PublicKeyNotFound);

                }
                else
                    signResult.SetStatus(SignResult.MeterNotFound);







                return signResult;

            }
            catch (Exception e)
            {
                return new EMHSignResult(Status:        SignResult.InvalidMeasurementValue,
                                         ErrorMessage:  e.Message);
            }

        }

        #endregion


        #region VerifyChargingSession(ChargingSession)

        public override ISessionCryptoResult VerifyChargingSession(IChargingSession ChargingSession)
        {

            var sessionResult = SessionVerificationResult.UnknownSessionFormat;

            if (ChargingSession.Measurements != null)
            {
                foreach (var measurement in ChargingSession.Measurements)
                {

                    measurement.ChargingSession = ChargingSession;

                    // Must include at least two measurements (start & stop)
                    if (measurement.Values != null && measurement.Values.Count() > 1)
                    {

                        // Validate...
                        foreach (var measurementValue in measurement.Values)
                        {
                            measurementValue.Measurement = measurement;
                            VerifyMeasurement(measurementValue as IEMHMeasurementValue);
                        }


                        // Find an overall result...
                        sessionResult = SessionVerificationResult.ValidSignature;

                        foreach (var measurementValue in measurement.Values)
                        {
                            if (sessionResult == SessionVerificationResult.ValidSignature &&
                                measurementValue.Result.Status != VerificationResult.ValidSignature)
                            {
                                sessionResult = SessionVerificationResult.InvalidSignature;
                            }
                        }

                    }

                    else
                        sessionResult = SessionVerificationResult.AtLeastTwoMeasurementsExpected;

                }
            }

            return new EMHSessionCryptoResult(sessionResult);

        }

        #endregion

        #region VerifyMeasurement    (MeasurementValue)

        public override IVerificationResult VerifyMeasurement(IMeasurementValue MeasurementValue)
        {

            if (MeasurementValue is IEMHMeasurementValue EMHMeasurementValue)
                return VerifyMeasurement(EMHMeasurementValue);

            return new CryptoResult(VerificationResult.UnknownCTRFormat);

        }

        public IVerificationResult VerifyMeasurement(IEMHMeasurementValue MeasurementValue)
        {

            try
            {

                var cryptoBuffer  = new Byte[320];

                var verificationResult  = new EMHVerificationResult(

                    MeterId:                      cryptoBuffer.SetHex        (MeasurementValue.Measurement.MeterId,                                        0),
                    Timestamp:                    cryptoBuffer.SetTimestamp32(MeasurementValue.Timestamp,                                                 10),
                    InfoStatus:                   cryptoBuffer.SetHex        (MeasurementValue.InfoStatus,                                                14, false),
                    SecondsIndex:                 cryptoBuffer.SetUInt32     (MeasurementValue.SecondsIndex,                                              15, true),
                    PaginationId:                 cryptoBuffer.SetHex        (MeasurementValue.PaginationId,                                              19, true),
                    OBIS:                         cryptoBuffer.SetHex        (MeasurementValue.Measurement.OBIS,                                          23, false),
                    UnitEncoded:                  cryptoBuffer.SetInt8       (MeasurementValue.Measurement.UnitEncoded,                                   29),
                    Scale:                        cryptoBuffer.SetInt8       (MeasurementValue.Measurement.Scale,                                         30),
                    Value:                        cryptoBuffer.SetUInt64     (MeasurementValue.Value,                                                     31, true),
                    LogBookIndex:                 cryptoBuffer.SetHex        (MeasurementValue.LogBookIndex,                                              39, false),
                    AuthorizationStart:           cryptoBuffer.SetText       (MeasurementValue.Measurement.ChargingSession.AuthorizationStart.Id,         41),
                    AuthorizationStartTimestamp:  cryptoBuffer.SetTimestamp32(MeasurementValue.Measurement.ChargingSession.AuthorizationStart.Timestamp, 169),

                    Status:                       VerificationResult.InvalidSignature);


                var signatureExpected = MeasurementValue.Signatures.FirstOrDefault();
                if (signatureExpected != null && (signatureExpected.Value?.Trim().IsNotNullOrEmpty() == true))
                {

                    try
                    {

                        //cryptoResult.Signature = {
                        //    algorithm:  MeasurementValue.Measurement.SignatureInfos.Algorithm,
                        //    format:     MeasurementValue.Measurement.SignatureInfos.Format,
                        //    r:          signatureExpected.R,
                        //    s:          signatureExpected.S
                        //};

                        var SHA256Hash = new Byte[0];

                        // Only the first 24 bytes/192 bits are used!
                        using (var SHA256 = new SHA256Managed())
                        {

                            SHA256Hash = SHA256.ComputeHash(cryptoBuffer);

                            verificationResult.SHA256Value = SHA256Hash.ToHexString().
                                                                        Substring(0, 48);

                        }

                        var meter = GetMeter(MeasurementValue.Measurement.MeterId);
                        if (meter != null)
                        {

                            verificationResult.SetMeter(meter);

                            var publicKey = meter.PublicKeys.FirstOrDefault();
                            if (publicKey != null && (publicKey.Value?.Trim().IsNotNullOrEmpty() == true))
                            {

                                try
                                {

                                    var EMHPublicKey = ParsePublicKey(publicKey.Value);

                                //    cryptoResult.publicKey            = publicKey.value.toLowerCase();
                                //    cryptoResult.publicKeyFormat      = publicKey.format;
                                //    cryptoResult.publicKeySignatures  = publicKey.signatures;

                                    try
                                    {

                                        //var bVerifier   = SignerUtilities.GetSigner("SHA-256withECDSA");
                                        var verifier      = SignerUtilities.GetSigner("NONEwithECDSA");
                                        verifier.Init(false, EMHPublicKey);
                                        verifier.BlockUpdate(SHA256Hash, 0, 24);

                                        var SignatureBytes  = signatureExpected.Value.HexStringToByteArray();
                                        var verified        = false;

                                        switch (signatureExpected.Format)
                                        // DER:   3037021900 ab9f84adda460f8410bb26061016d6c8258689caa73b0b fd021a00 fd1eab0aa198b5803358a2a91624dc012d0ef3ee72b3de820a
                                        // plain:            ab9f84adda460f8410bb26061016d6c8258689caa73b0b          fd1eab0aa198b5803358a2a91624dc012d0ef3ee72b3de820a
                                        {

                                            case SignatureFormats.DER:
                                                verified = verifier.VerifySignature(SignatureBytes);
                                                break;

                                            case SignatureFormats.plain: // Shouldn't this be called rs?
                                                verified = verifier.VerifySignature(new DerSequence(
                                                                                        new DerInteger(new BigInteger(SignatureBytes.ToHexString(0, 24), 16)),
                                                                                        new DerInteger(new BigInteger(SignatureBytes.ToHexString(   23), 16))
                                                                                    ).GetDerEncoded());
                                                break;

                                        }


                                        // Success!
                                        if (verified)
                                            verificationResult.SetStatus(VerificationResult.ValidSignature);

                                        else
                                            verificationResult.SetStatus(VerificationResult.InvalidSignature);

                                    }
                                    catch (Exception e)
                                    {
                                        verificationResult.SetError(VerificationResult.InvalidSignature,
                                                                    e.Message);
                                    }

                                }
                                catch (Exception e)
                                {
                                    verificationResult.SetError(VerificationResult.InvalidPublicKey,
                                                                e.Message);
                                }

                            }
                            else
                                verificationResult.SetStatus(VerificationResult.PublicKeyNotFound);

                        }
                        else
                            verificationResult.SetStatus(VerificationResult.MeterNotFound);

                    }
                    catch (Exception e)
                    {
                        verificationResult.SetError(VerificationResult.InvalidSignature,
                                                    e.Message);
                    }

                }

                else
                    verificationResult.SetStatus(VerificationResult.InvalidSignature);

                return MeasurementValue.Result = verificationResult;

            }
            catch (Exception e)
            {
                return MeasurementValue.Result = new EMHVerificationResult(Status:        VerificationResult.UnknownCTRFormat,
                                                                           ErrorMessage:  e.Message);
            }

        }

        #endregion


        // Helper

        #region DecodeStatus(StatusValue)

        public IEnumerable<String> DecodeStatus(String StatusValue)
        {

            var statusArray = new List<String>();

            try
            {

                var status = Int32.Parse(StatusValue);

                if ((status &  1) ==  1)
                    statusArray.Add("Fehler erkannt");

                if ((status &  2) ==  2)
                    statusArray.Add("Synchrone Messwertübermittlung");

                // Bit 3 is reserved!

                if ((status &  8) ==  8)
                    statusArray.Add("System-Uhr ist synchron");
                else
                    statusArray.Add("System-Uhr ist nicht synchron");

                if ((status & 16) == 16)
                    statusArray.Add("Rücklaufsperre aktiv");

                if ((status & 32) == 32)
                    statusArray.Add("Energierichtung -A");

                if ((status & 64) == 64)
                    statusArray.Add("Magnetfeld erkannt");

            }
            catch (Exception e)
            {
                statusArray.Add("Invalid status!");
            }

            return statusArray;

        }

        #endregion


    }

}
