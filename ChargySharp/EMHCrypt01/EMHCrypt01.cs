/*
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.apis.chargy
{

    public class EMHCrypt01 : ACrypt
    {

        #region Constructor(s)

        public EMHCrypt01(GetMeterDelegate                      GetMeter,
                          CheckMeterPublicKeySignatureDelegate  CheckMeterPublicKeySignature)

            : base("ECC secp192r1",
                   GetMeter,
                   CheckMeterPublicKeySignature)

        { }

        #endregion


        public override ICryptoResult SignMeasurement(IMeasurementValue  measurementValue,
                                                      Object             privateKey,
                                                      Object             publicKey)
        {
            return null;
        }


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

        #region VerifyMeasurement(MeasurementValue)

        public override ICryptoResult VerifyMeasurement(IMeasurementValue MeasurementValue)
        {

            if (MeasurementValue is IEMHMeasurementValue EMHMeasurementValue)
                return VerifyMeasurement(EMHMeasurementValue);

            return new CryptoResult(VerificationResult.UnknownCTRFormat);

        }

        public ICryptoResult VerifyMeasurement(IEMHMeasurementValue MeasurementValue)
        {

            try
            {

                //function setResult(verificationResult: VerificationResult)
                //{
                //    cryptoResult.status     = verificationResult;
                //    measurementValue.result = cryptoResult;
                //    return cryptoResult;
                //}

                var cryptoBuffer  = new Byte[320];

                var cryptoResult = new EMHCrypt01Result(

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


                if (MeasurementValue.Signatures.First() is IECCSignature signatureExpected)
                {

                    try
                    {

                        //cryptoResult.Signature = {
                        //    algorithm:  MeasurementValue.Measurement.SignatureInfos.Algorithm,
                        //    format:     MeasurementValue.Measurement.SignatureInfos.Format,
                        //    r:          signatureExpected.R,
                        //    s:          signatureExpected.S
                        //};

                        // Only the first 24 bytes/192 bits are used!
                        using (var sha256 = SHA256.Create())
                        {
                            cryptoResult.SHA256Value = sha256.ComputeHash(cryptoBuffer).
                                                              ToHexString().
                                                              Substring(0, 48);
                        }

                        var meter = GetMeter(MeasurementValue.Measurement.MeterId);
                        if (meter != null)
                        {

                            cryptoResult.Meter = meter;

                            if (meter.PublicKeys.First() is IPublicKey publicKey)
                            {

                                try
                                {

                                //    cryptoResult.publicKey            = publicKey.value.toLowerCase();
                                //    cryptoResult.publicKeyFormat      = publicKey.format;
                                //    cryptoResult.publicKeySignatures  = publicKey.signatures;

                                    try
                                    {

                                        //if (this.curve.keyFromPublic(cryptoResult.publicKey, 'hex').
                                        //               verify       (cryptoResult.sha256value,
                                        //                             cryptoResult.signature))
                                        //{
                                        //    return setResult(VerificationResult.ValidSignature);
                                        //}

                                        //return setResult(VerificationResult.InvalidSignature);

                                    }
                                    catch (Exception e)
                                    {
                                        return new EMHCrypt01Result(Status:        VerificationResult.InvalidSignature,
                                                                    ErrorMessage:  e.Message);
                                    }

                                }
                                catch (Exception e)
                                {
                                    return new EMHCrypt01Result(Status:        VerificationResult.InvalidPublicKey,
                                                                ErrorMessage:  e.Message);
                                }

                            }
                            else
                                return new EMHCrypt01Result(Status:  VerificationResult.PublicKeyNotFound);

                        }
                        else
                            return new EMHCrypt01Result(Status:  VerificationResult.EnergyMeterNotFound);

                    }
                    catch (Exception e)
                    {
                        return new EMHCrypt01Result(Status:        VerificationResult.InvalidSignature,
                                                    ErrorMessage:  e.Message);
                    }

                }

            }
            catch (Exception e)
            {
                return new EMHCrypt01Result(Status:        VerificationResult.UnknownCTRFormat,
                                            ErrorMessage:  e.Message);
            }

            return new EMHCrypt01Result(Status:  VerificationResult.UnknownCTRFormat);

        }

        #endregion


    }

}
