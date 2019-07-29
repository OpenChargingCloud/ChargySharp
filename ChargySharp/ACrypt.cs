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

#endregion

namespace cloud.charging.apis.chargy
{

    public abstract class ACrypt {

        #region Properties

        public String                                Description                     { get; }

        public GetMeterDelegate                      GetMeter                        { get; }

        public CheckMeterPublicKeySignatureDelegate  CheckMeterPublicKeySignature    { get; }

        #endregion

        #region Constructor(s)

        public ACrypt(String                                Description,
                      GetMeterDelegate                      GetMeter,
                      CheckMeterPublicKeySignatureDelegate  CheckMeterPublicKeySignature)
        {

            this.Description                   = Description;
            this.GetMeter                      = GetMeter;
            this.CheckMeterPublicKeySignature  = CheckMeterPublicKeySignature;

        }

        #endregion


        public abstract ISessionCryptoResult VerifyChargingSession(IChargingSession ChargingSession);

        public abstract ISignResult SignMeasurement(IMeasurementValue  MeasurementValue,
                                                    Byte[]             PrivateKey,
                                                    Byte[]             PublicKey);

        public abstract IVerificationResult VerifyMeasurement(IMeasurementValue measurementValue);


    }

}
