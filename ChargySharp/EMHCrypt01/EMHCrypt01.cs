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
using System.Collections.Generic;
using System.Linq;

#endregion

namespace cloud.charging.apis.chargy
{

    public class EMHCrypt01 : ACrypt
    {

        public EMHCrypt01(
                      //GetMeter:                      GetMeterFunc,
                      //CheckMeterPublicKeySignature:  CheckMeterPublicKeySignatureFunc
            )

            : base("ECC secp192r1")

        {

        }


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
            return null;
        }

        public ICryptoResult VerifyMeasurement(IEMHMeasurementValue MeasurementValue)
        {

            //function setResult(verificationResult: VerificationResult)
            //{
            //    cryptoResult.status     = verificationResult;
            //    measurementValue.result = cryptoResult;
            //    return cryptoResult;
            //}

            var cryptoBuffer  = new Byte[320];
            cryptoBuffer.SetHex        (MeasurementValue.Measurement.EnergyMeterId,                                  0);
            cryptoBuffer.SetTimestamp32(MeasurementValue.Timestamp,                                                 10);
            cryptoBuffer.SetHex        (MeasurementValue.InfoStatus,                                                14, false);
            cryptoBuffer.SetUInt32     (MeasurementValue.SecondsIndex,                                              15, true);
            cryptoBuffer.SetHex        (MeasurementValue.PaginationId,                                              19, true);
            cryptoBuffer.SetHex        (MeasurementValue.Measurement.OBIS,                                          23, false);
            cryptoBuffer.SetInt8       (MeasurementValue.Measurement.UnitEncoded,                                   29);
            cryptoBuffer.SetInt8       (MeasurementValue.Measurement.Scale,                                         30);
            cryptoBuffer.SetUInt64     (MeasurementValue.Value,                                                     31, true);
            cryptoBuffer.SetHex        (MeasurementValue.LogBookIndex,                                              39, false);
            cryptoBuffer.SetText       (MeasurementValue.Measurement.ChargingSession.AuthorizationStart.Id,         41);
            cryptoBuffer.SetTimestamp32(MeasurementValue.Measurement.ChargingSession.AuthorizationStart.Timestamp, 169);

            //var cryptoResult = new EMHCrypt01Result(
            //    Status:                       VerificationResult.InvalidSignature,
            //    MeterId:                      SetHex        (cryptoBuffer, MeasurementValue.Measurement.EnergyMeterId,                                  0),
            //    Timestamp:                    SetTimestamp32(cryptoBuffer, MeasurementValue.Timestamp,                                                 10),
            //    InfoStatus:                   SetHex        (cryptoBuffer, MeasurementValue.InfoStatus,                                                14, false),
            //    SecondsIndex:                 SetUInt32     (cryptoBuffer, MeasurementValue.SecondsIndex,                                              15, true),
            //    PaginationId:                 SetHex        (cryptoBuffer, MeasurementValue.PaginationId,                                              19, true),
            //    OBIS:                         SetHex        (cryptoBuffer, MeasurementValue.Measurement.OBIS,                                          23, false),
            //    UnitEncoded:                  SetInt8       (cryptoBuffer, MeasurementValue.Measurement.UnitEncoded,                                   29),
            //    Scale:                        SetInt8       (cryptoBuffer, MeasurementValue.Measurement.Scale,                                         30),
            //    Value:                        SetUInt64     (cryptoBuffer, MeasurementValue.Value,                                                     31, true),
            //    LogBookIndex:                 SetHex        (cryptoBuffer, MeasurementValue.LogBookIndex,                                              39, false),
            //    AuthorizationStart:           SetText       (cryptoBuffer, MeasurementValue.Measurement.ChargingSession.AuthorizationStart.Id,         41),
            //    AuthorizationStartTimestamp:  SetTimestamp32(cryptoBuffer, MeasurementValue.Measurement.ChargingSession.AuthorizationStart.Timestamp, 169)
            //);

            //var signatureExpected = MeasurementValue.Signatures.First() as IECCSignature;
            //if (signatureExpected != null)
            //{

            //    try
            //    {

            //        cryptoResult.signature = {
            //            algorithm:  MeasurementValue.Measurement.SignatureInfos.Algorithm,
            //            format:     MeasurementValue.Measurement.SignatureInfos.Format,
            //            r:          signatureExpected.R,
            //            s:          signatureExpected.S
            //        };

            //        // Only the first 24 bytes/192 bits are used!
            //        cryptoResult.sha256value = this.crypt.createHash('sha256').
            //                                              update(cryptoBuffer).
            //                                              digest('hex').
            //                                              substring(0, 48);


            //        const meter = this.GetMeter(MeasurementValue.Measurement.EnergyMeterId);
            //        if (meter != null)
            //        {

            //            cryptoResult.meter = meter;

            //            var iPublicKey = meter.publicKeys[0] as IPublicKey;
            //            if (iPublicKey != null)
            //            {

            //                try
            //                {

            //                    cryptoResult.publicKey            = iPublicKey.value.toLowerCase();
            //                    cryptoResult.publicKeyFormat      = iPublicKey.format;
            //                    cryptoResult.publicKeySignatures  = iPublicKey.signatures;

            //                    try
            //                    {

            //                        if (this.curve.keyFromPublic(cryptoResult.publicKey, 'hex').
            //                                       verify       (cryptoResult.sha256value,
            //                                                     cryptoResult.signature))
            //                        {
            //                            return setResult(VerificationResult.ValidSignature);
            //                        }

            //                        return setResult(VerificationResult.InvalidSignature);

            //                    }
            //                    catch (Exception e)
            //                    {
            //                        return setResult(VerificationResult.InvalidSignature);
            //                    }

            //                }
            //                catch (Exception e)
            //                {
            //                    return setResult(VerificationResult.InvalidPublicKey);
            //                }

            //            }

            //            else
            //                return setResult(VerificationResult.PublicKeyNotFound);

            //        }

            //        else
            //            return setResult(VerificationResult.EnergyMeterNotFound);

            //    }
            //    catch (Exception e)
            //    {
            //        return setResult(VerificationResult.InvalidSignature);
            //    }

            //}

            return new EMHCrypt01Result(VerificationResult.UnknownCTRFormat);

        }

        #endregion

    }

}
