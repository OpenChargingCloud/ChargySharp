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

#endregion

namespace cloud.charging.apis.chargy
{

    public interface IEMHCrypt01Result : ICryptoResult
    {
        String         SHA256value                      { get; }
        String         MeterId                          { get; }
        IMeter         Meter                            { get; }
        DateTime?      Timestamp                        { get; }
        String         InfoStatus                       { get; }
        String         SecondsIndex                     { get; }
        String         PaginationId                     { get; }
        OBIS?          OBIS                             { get; }
        String         UnitEncoded                      { get; }
        String         Scale                            { get; }
        String         Value                            { get; }
        String         LogBookIndex                     { get; }
        String         AuthorizationStart               { get; }
        String         AuthorizationStop                { get; }
        String         AuthorizationStartTimestamp      { get; }
        String         PublicKey                        { get; }
        String         PublicKeyFormat                  { get; }
        String         PublicKeySignatures              { get; }
        IECCSignature  Signature                        { get; }
    }

    public class EMHCrypt01Result : IEMHCrypt01Result
    {

        public String              SHA256value                    { get; }

        public String              MeterId                        { get; }

        public IMeter              Meter                          { get; }

        public DateTime?           Timestamp                      { get; }

        public String              InfoStatus                     { get; }

        public String              SecondsIndex                   { get; }

        public String              PaginationId                   { get; }

        public OBIS?               OBIS                           { get; }

        public String              UnitEncoded                    { get; }

        public String              Scale                          { get; }

        public String              Value                          { get; }

        public String              LogBookIndex                   { get; }

        public String              AuthorizationStart             { get; }

        public String              AuthorizationStop              { get; }

        public String              AuthorizationStartTimestamp    { get; }

        public String              PublicKey                      { get; }

        public String              PublicKeyFormat                { get; }

        public String              PublicKeySignatures            { get; }

        public IECCSignature       Signature                      { get; }

        public VerificationResult  Status                         { get; }

        public EMHCrypt01Result(VerificationResult  Status,
                                String              SHA256value                         = null,
                                String              MeterId                             = null,
                                IMeter              Meter                               = null,
                                DateTime?           Timestamp                           = null,
                                String              InfoStatus                          = null,
                                String              SecondsIndex                        = null,
                                String              PaginationId                        = null,
                                OBIS?               OBIS                                = null,
                                String              UnitEncoded                         = null,
                                String              Scale                               = null,
                                String              Value                               = null,
                                String              LogBookIndex                        = null,
                                String              AuthorizationStart                  = null,
                                String              AuthorizationStop                   = null,
                                String              AuthorizationStartTimestamp         = null,
                                String              PublicKey                           = null,
                                String              PublicKeyFormat                     = null,
                                String              PublicKeySignatures                 = null,
                                IECCSignature       Signature                           = null)
        {

            this.Status                        = Status;
            this.SHA256value                   = SHA256value;
            this.MeterId                       = MeterId;
            this.Meter                         = Meter;
            this.Timestamp                     = Timestamp;
            this.InfoStatus                    = InfoStatus;
            this.SecondsIndex                  = SecondsIndex;
            this.PaginationId                  = PaginationId;
            this.OBIS                          = OBIS;
            this.UnitEncoded                   = UnitEncoded;
            this.Scale                         = Scale;
            this.Value                         = Value;
            this.LogBookIndex                  = LogBookIndex;
            this.AuthorizationStart            = AuthorizationStart;
            this.AuthorizationStop             = AuthorizationStop;
            this.AuthorizationStartTimestamp   = AuthorizationStartTimestamp;
            this.PublicKey                     = PublicKey;
            this.PublicKeyFormat               = PublicKeyFormat;
            this.PublicKeySignatures           = PublicKeySignatures;
            this.Signature                     = Signature;

        }

    }

}
